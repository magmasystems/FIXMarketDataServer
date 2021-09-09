using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using MagmaTrader.Data;

namespace MagmaTrader.MarketData
{
	[ServiceContract(Name = "EquityLevel2MarketDataBroadcaster")]
	public interface IEquityLevel2MarketDataBroadcaster
	{
		[OperationContract]
		void SendLevel2Book(Level2Book book);
	}

	[ServiceContract]
	public interface INotification
	{
		[OperationContract(IsOneWay = true, AsyncPattern = true, Action = NotificationData.NotificationAction)]
		IAsyncResult BeginNotify(Message message, AsyncCallback callback, object state);
		void EndNotify(IAsyncResult result);
	}

	[ServiceContract(CallbackContract = typeof(INotification))]
	public interface IPubSub
	{
		[OperationContract(IsOneWay = true)]
		void Subscribe(string topic);

		[OperationContract(IsOneWay = true)]
		void Publish(string topic, Level2Book content);
	}

	[MessageContract]
	public class NotificationData
	{
		public const string NotificationAction = "http://microsoft.com/samples/pollingDuplex/notification";

		[MessageBodyMember]
		public Level2Book Content { get; set; }
	}

	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	public class PubSubService : IPubSub
	{
		private readonly object m_syncRoot = new object();
	
		private readonly Dictionary<string, string> m_sessionIdTopic = new Dictionary<string, string>();
		private readonly Dictionary<string, Dictionary<string, INotification>> m_topicSessionIdCallbackChannel = new Dictionary<string, Dictionary<string, INotification>>();

		static readonly TypedMessageConverter m_messageConverter = TypedMessageConverter.Create(typeof(NotificationData), NotificationData.NotificationAction, "http://schemas.datacontract.org/2004/07/MarketData");
		
		static readonly AsyncCallback onNotifyCompleted = NotifyCompleted;

		public void Subscribe(string topic)
		{
			if (topic == null)
			{
				topic = string.Empty;
			}

			string sessionId = OperationContext.Current.Channel.SessionId;
			INotification callbackChannel = OperationContext.Current.GetCallbackChannel<INotification>();

			lock (this.m_syncRoot)
			{
				this.m_sessionIdTopic[sessionId] = topic;
				Dictionary<string, INotification> sessionIdCallbackChannel;
				if (!this.m_topicSessionIdCallbackChannel.TryGetValue(topic, out sessionIdCallbackChannel))
				{
					sessionIdCallbackChannel = new Dictionary<string, INotification>();
					this.m_topicSessionIdCallbackChannel[topic] = sessionIdCallbackChannel;
				}
				sessionIdCallbackChannel[sessionId] = callbackChannel;
			}
			OperationContext.Current.Channel.Faulted += this.Unsubscribe;
			OperationContext.Current.Channel.Closed += this.Unsubscribe;
		}

		public void Publish(string topic, Level2Book content)
		{
			if (topic == null)
			{
				topic = string.Empty;
			}

			List<INotification> clientsToNotify = null;
			lock (this.m_syncRoot)
			{
				Dictionary<string, INotification> sessionIdCallbackChannel;
				if (this.m_topicSessionIdCallbackChannel.TryGetValue(topic, out sessionIdCallbackChannel))
				{
					if (OperationContext.Current == null)
					{
						clientsToNotify = new List<INotification>(sessionIdCallbackChannel.Where(x => true).Select(x => x.Value));
					}
					else
					{
						clientsToNotify = new List<INotification>(sessionIdCallbackChannel.Where(x => x.Key != OperationContext.Current.Channel.SessionId).Select(x => x.Value));
					}
				}
			}

			if (clientsToNotify != null && clientsToNotify.Count > 0)
			{
				MessageBuffer notificationMessageBuffer = m_messageConverter.ToMessage(new NotificationData { Content = content }).CreateBufferedCopy(65536);
				foreach (INotification callbackChannel in clientsToNotify)
				{
					try
					{
						callbackChannel.BeginNotify(notificationMessageBuffer.CreateMessage(), onNotifyCompleted, callbackChannel);
					}
					catch (CommunicationException)
					{
						// empty
					}
				}
			}
		}

		static void NotifyCompleted(IAsyncResult result)
		{
			try
			{
				((INotification)(result.AsyncState)).EndNotify(result);
			}
			catch (CommunicationException)
			{
				// empty
			}
			catch (TimeoutException)
			{
				// empty
			}
		}

		void Unsubscribe(object sender, EventArgs e)
		{
			IContextChannel channel = (IContextChannel)sender;
			if (channel != null && channel.SessionId != null)
			{
				lock (this.m_syncRoot)
				{
					string topic;
					if (this.m_sessionIdTopic.TryGetValue(channel.SessionId, out topic))
					{
						this.m_sessionIdTopic.Remove(channel.SessionId);
						Dictionary<string, INotification> sessionIdCallbackChannel;
						if (this.m_topicSessionIdCallbackChannel.TryGetValue(topic, out sessionIdCallbackChannel))
						{
							sessionIdCallbackChannel.Remove(channel.SessionId);
							if (sessionIdCallbackChannel.Count == 0)
							{
								this.m_topicSessionIdCallbackChannel.Remove(topic);
							}
						}
					}
				}
			}
		}
	}



	public class EquityLevel2MarketDataBroadcaster : IDisposable
	{
		public ServiceHost Level2TCPHost;
		public ServiceHost Level2MexHost;
		public IPubSub     Publisher;

		public EquityLevel2MarketDataBroadcaster()
		{
			this.Publisher = new PubSubService();
			this.InitWCF();
		}

		public void Dispose()
		{
			this.CloseWCF();
		}

		protected void InitWCF()
		{
			/*
				 <netTcpBinding>
					<binding name="PubSub">
						<security mode="None"/>
					</binding>
				 </netTcpBinding>
			  
				<endpoint address="" binding="netTcpBinding" bindingConfiguration="PubSub" contract="MarketData.IEquityLevel2MarketDataBroadcaster"/>
                <endpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange"/>
			 */

			try
			{
				// We can use this binding for higher performance
#if HIGH_PERF_BINDING
				NetTcpBinding tcpBinding = new NetTcpBinding
				{
				    TransferMode = TransferMode.Streamed,
				    ReceiveTimeout = TimeSpan.MaxValue,
				    MaxReceivedMessageSize = long.MaxValue
				};
#else
				NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
#endif
				Uri uriTcp = new Uri("net.tcp://" + Dns.GetHostName() + ":8989/Level2QuoteService");
				this.Level2TCPHost = new ServiceHost(this.Publisher);
				this.Level2TCPHost.AddServiceEndpoint(typeof(IPubSub), tcpBinding, uriTcp);

				// Add a MEX binding so that Visual Studio can discover this service
				this.Level2TCPHost.Description.Behaviors.Add(new ServiceMetadataBehavior());
				Binding mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
				Uri uriMex = new Uri("net.tcp://" + Dns.GetHostName() + ":8989/Level2QuoteService/mex");
				this.Level2TCPHost.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, uriMex);

				this.Level2TCPHost.Open();

	
				// A client can be written like this
				/*
					EndpointAddress address = new EndpointAddress("net.tcp://localhost:8989/Level2QuoteService");
					NetTcpBinding binding = new NetTcpBinding();
					ChannelFactory<IEquityLevel2MarketDataBroadcaster> channel = new ChannelFactory<IEquityLevel2MarketDataBroadcaster>(binding, address);
			        IEquityLevel2MarketDataBroadcaster quoteService = channel.CreateChannel();				 
				    .....
					((IClientChannel) quoteService).Close();
				 */
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
			}
		}

		protected void CloseWCF()
		{
			if (this.Level2TCPHost != null)
			{
				this.Level2TCPHost.Close();
			}
		}

		public void SendLevel2Book(Level2Book book)
		{
			if (this.Level2TCPHost != null)
			{
				var publisher = this.Level2TCPHost.SingletonInstance;
				if (publisher is IPubSub)
				{
					((IPubSub) publisher).Publish("Level2Quotes.Book", book);
				}
			}
		}
	}
}
