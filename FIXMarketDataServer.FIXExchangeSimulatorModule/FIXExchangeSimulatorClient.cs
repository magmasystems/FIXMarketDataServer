using System;
using System.Collections.Generic;
using MagmaTrader.Interfaces;
using MagmaTrader.Utilities;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Unity;
using QuickFix;
using QuickFix44;
using Message = QuickFix.Message;
using MessageCracker = QuickFix.MessageCracker;

namespace FIXMarketDataServer.FIXExchangeSimulatorModule
{
	//  Note - The QuickFIX assembly is built with .NET 2.0, so we need to add useLegacyV2RuntimeActivationPolicy="true" to the app.config file
	//	<configuration>
	//		<startup useLegacyV2RuntimeActivationPolicy="true">
	//			<supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>
	//		</startup>
	//	</configuration>

	public class FIXExchangeSimulatorClient : MessageCracker, Application, IDisposable, IFIXExchangeSimulatorClient
	{
		#region Events
		public event Action<Quote> QuoteReceived = q => { };
		public event Action<object> ExecutionReportReceived = e => { };
		public event Action<object> PassThruFixMessageReceived = e => { };
		#endregion

		#region Variables
		private SocketInitiator m_socketInitiator;
		private FileStoreFactory m_messageStoreFactory;
		private SessionSettings m_settings;
		private FileLogFactory m_logFactory;
		private QuickFix44.MessageFactory m_messageFactory;

		private readonly ILoggerFacade m_logger; // For Microsoft Prism
		private readonly IEventAggregator m_eventAggregator; // For Microsoft Prism

		private IQuoteCacheViewModel m_quoteCacheViewModel;

		private SessionID m_sessionID;  // limit to only 1 session for now

		public string Name { get; private set; }
		public bool IsStarted { get; private set; }
		#endregion

		#region Constructors
		// The logger should be dependency-injected by Prism
		public FIXExchangeSimulatorClient(ILoggerFacade logger, IEventAggregator eventAggregator)
		{
			this.Name = Guid.NewGuid().ToString();
			this.m_logger = logger;
			this.m_eventAggregator = eventAggregator;
			
			this.Init();
		}
		#endregion

		#region Cleanup
		public void Dispose()
		{
			this.Stop();
			if (this.m_socketInitiator != null)
			{
				this.m_socketInitiator.Dispose();
				this.m_socketInitiator = null;
			}
		}
		#endregion

		#region Control
		private void Init()
		{
			try
			{
				this.m_settings = new SessionSettings(@"..\..\..\ServerModules\FIXExchangeSimulatorClientConfig.txt");
				this.m_messageStoreFactory = new FileStoreFactory(this.m_settings);
				this.m_logFactory = new FileLogFactory(/*this.m_settings*/  @"C:\QuickFixLogs");
				this.m_messageFactory = new QuickFix44.MessageFactory();
				this.m_socketInitiator = new SocketInitiator(this, this.m_messageStoreFactory, this.m_settings, this.m_logFactory, this.m_messageFactory);
			}
			catch (ConfigError exc)
			{
				Console.WriteLine(exc.Message);
			}
		}

		public void Start()
		{
			if (this.m_socketInitiator != null)
			{
				this.m_socketInitiator.start();
				this.IsStarted = true;
			}
		}

		public void Stop()
		{
			if (this.m_socketInitiator != null)
			{
				// Note - This causes the GUI to hang
				//this.m_socketInitiator.stop();
				this.IsStarted = false;
			}
		}
		#endregion

		#region Callbacks
		public void onCreate(SessionID sessionID)
		{
			this.m_logger.Log(string.Format("FIXExchangeSimClient: QuickFIX Initiator created with Session ID {0}", sessionID), Category.Info, Priority.None);
		}

		public void onLogon(SessionID sessionID)
		{
			this.m_logger.Log(string.Format("FIXExchangeSimClient: QuickFIX Initiator logged on with Session ID {0}", sessionID), Category.Info, Priority.None);
			this.m_sessionID = sessionID;

			if (this.m_eventAggregator != null)
				this.m_eventAggregator.GetEvent<FIXExchangeSimulatorControlEvent>().Publish(new FIXExchangeSimulatorControlEventArgs(this, FIXExchangeSimulatorAction.LoggedIn));
		}

		public void onLogout(SessionID sessionID)
		{
			this.m_logger.Log(string.Format("FIXExchangeSimClient: QuickFIX Initiator logged out with Session ID {0}", sessionID), Category.Info, Priority.None);
			this.m_sessionID = null;

			if (this.m_eventAggregator != null)
				this.m_eventAggregator.GetEvent<FIXExchangeSimulatorControlEvent>().Publish(new FIXExchangeSimulatorControlEventArgs(this, FIXExchangeSimulatorAction.LoggedOut));
		}

		public void toAdmin(Message message, SessionID sessionID)
		{
			if (FIXHelpers.IsHeartbeat(message))
				return;

			Console.WriteLine("FIXExchangeSimClient: ToAdmin: " + message);
			this.m_logger.Log(string.Format("FIXExchangeSimClient: ToAdmin: {0}", message), Category.Info, Priority.None);
		}

		public void toApp(Message message, SessionID sessionID)
		{
			if (FIXHelpers.IsHeartbeat(message))
				return;
			
			//			Console.WriteLine("ToApp: " + message);
			this.m_logger.Log(string.Format("FIXExchangeSimClient: ToApp: {0}", message), Category.Info, Priority.None);
		}

		public void fromAdmin(Message message, SessionID sessionID)
		{
			if (FIXHelpers.IsHeartbeat(message))
				return;

			Console.WriteLine("FIXExchangeSimClient: FromAdmin: " + message);
			this.m_logger.Log(string.Format("FIXExchangeSimClient: FromAdmin: {0}", message), Category.Info, Priority.None);
		}

		public void fromApp(Message message, SessionID sessionID)
		{
			if (FIXHelpers.IsHeartbeat(message))
				return;

			Console.WriteLine("FIXExchangeSimClient: FromApp: " + message);
			this.m_logger.Log(string.Format("FIXExchangeSimClient: FromApp: {0}", message), Category.Info, Priority.None);

			try
			{
				this.crack(message, sessionID);
			}
			catch (UnsupportedMessageType)
			{
				this.m_logger.Log(string.Format("FIXExchangeSimClient: UnsupportedMessageType exception"), Category.Exception, Priority.None);
			}
		}

		public override void onMessage(QuickFix44.Quote message, SessionID session)
		{
			// It may be easier to dispatch this to another thread
	
			Quote quote = new Quote {Symbol = message.getSymbol().getValue()};
			if (message.isSetBidPx())
				quote.Bid = message.getBidPx().getValue();
			if (message.isSetOfferPx())
				quote.Ask = message.getOfferPx().getValue();
			if (message.isSetBidSize())
				quote.BidSize = (int) message.getBidSize().getValue();
			if (message.isSetOfferSize())
				quote.AskSize = (int) message.getOfferSize().getValue();
			if (message.isSetQuoteID())
				quote.QuoteID = message.getQuoteID().getValue();

			this.QuoteReceived(quote);
		}

		public override void onMessage(ExecutionReport message, SessionID session)
		{
			// We need to send the execution report message to the GUI
			this.m_logger.Log(string.Format("FIXExchangeSimClient: ExecutionReport Message: {0}", message), Category.Info, Priority.None);
			this.ExecutionReportReceived(message);
		}

		public override void onMessage(MarketDataSnapshotFullRefresh message, SessionID session)
		{
			this.m_logger.Log(string.Format("FIXServer: MarketDataSnapshotFullRefresh Message: Symbol {0}",
				message.getSymbol().getValue()),
				Category.Info, Priority.None);

			NoMDEntries noMdEntries = new NoMDEntries();
			message.get(noMdEntries);
			int nEntries = noMdEntries.getValue();
			if (nEntries == 0)
				return;

			Quote quote = new Quote { Symbol = message.getSymbol().getValue() };
			List<Quote> quotes = new List<Quote> {quote};

			MDEntryType type = new MDEntryType();
			MDEntryPx px = new MDEntryPx();
			MarketDataSnapshotFullRefresh.NoMDEntries group = new MarketDataSnapshotFullRefresh.NoMDEntries();
			for (uint i = 1; i <= nEntries; i++)
			{
				message.getGroup(i, group);
				group.get(type);
				group.get(px);

				switch (type.getValue())
				{
					case MDEntryType.BID:
						quote.Bid = px.getValue();
						break;
					case MDEntryType.OFFER:
						quote.Ask = px.getValue();
						break;
				}
				this.m_logger.Log(string.Format("    Message: Symbol {0}, Type {1}, Px {2}", message.getSymbol().getValue(), type, px), Category.Info, Priority.None);
			}

			// Send the quotes to the GUI
			this.PassThruFixMessageReceived(message);

			// We also need to send the quotes to our quote cache
			if (this.m_quoteCacheViewModel == null)
				this.m_quoteCacheViewModel = FIXExchangeSimulatorModule.Container.Resolve<IQuoteCacheViewModel>();
			if (this.m_quoteCacheViewModel != null)
			{
				this.m_quoteCacheViewModel.AddQuotes(quotes);
			}
		}

		public override void onMessage(MarketDataIncrementalRefresh message, SessionID session)
		{
			base.onMessage(message, session);
		}

		public override void onMessage(Reject message, SessionID session)
		{
			Console.WriteLine("FIXExchangeSimClient: Reject Message: " + message);
			this.m_logger.Log(string.Format("FIXExchangeSimClient: Reject Message: {0}", message), Category.Info, Priority.None);
		}
		public override void onMessage(BusinessMessageReject message, SessionID session)
		{
			Console.WriteLine("FIXExchangeSimClient: BusinessMessageReject Message: " + message);
			this.m_logger.Log(string.Format("FIXExchangeSimClient: BusinessMessageReject Message: {0}", message), Category.Info, Priority.None);
		}
		public override void onMessage(MarketDataRequestReject message, SessionID session)
		{
			Console.WriteLine("FIXExchangeSimClient: MarketDataRequestReject Message: " + message);
			this.m_logger.Log(string.Format("FIXExchangeSimClient: MarketDataRequestReject Message: {0}", message), Category.Info, Priority.None);
		}
		public override void onMessage(QuoteRequestReject message, SessionID session)
		{
			Console.WriteLine("FIXExchangeSimClient: QuoteRequestReject Message: " + message);
			this.m_logger.Log(string.Format("FIXExchangeSimClient: QuoteRequestReject Message: {0}", message), Category.Info, Priority.None);
		}
		public override void onMessage(OrderCancelReject message, SessionID session)
		{
			Console.WriteLine("FIXExchangeSimClient: OrderCancelReject Message: " + message);
			this.m_logger.Log(string.Format("FIXExchangeSimClient: OrderCancelReject Message: {0}", message), Category.Info, Priority.None);
		}
		#endregion

		#region Send FIX Messages to the Server
		public void Publish(object message)
		{
			if (this.m_sessionID == null)
				return;

			Message fixMessage = message as Message;
			if (fixMessage == null)
				return;

			this.m_logger.Log(string.Format("FIXExchangeSimClient: sending message to Sim {0}", fixMessage), Category.Info, Priority.None);

			Session.sendToTarget(fixMessage, this.m_sessionID);
		}

		public void RequestQuoteStream(List<MagmaTrader.Data.Symbol> symbols)
		{
			if (this.m_sessionID == null)
				return;

			this.m_logger.Log(string.Format("FIXExchangeSimClient.RequestQuoteStream - symbols {0}", symbols), Category.Info, Priority.None);
			MarketDataRequest request = FIXHelpers.CreateMarketDataRequestMessage(symbols);
			Session.sendToTarget(request, this.m_sessionID);
		}

		#endregion
	}
}
