using System;
using System.Collections.Generic;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;
using MagmaTrader.Utilities;
using Microsoft.Practices.Prism.Logging;
using QuickFix;
using QuickFix44;
using Message = QuickFix.Message;
using MessageCracker = QuickFix.MessageCracker;
using Quote = FIXMarketDataServer.Quote;
using Side = QuickFix.Side;
using Symbol = QuickFix.Symbol;
using TimeInForce = QuickFix.TimeInForce;

namespace FIXMarketDataClient.FIXClientModule
{
	//  Note - The QuickFIX assembly is built with .NET 2.0, so we need to add useLegacyV2RuntimeActivationPolicy="true" to the app.config file
	//	<configuration>
	//		<startup useLegacyV2RuntimeActivationPolicy="true">
	//			<supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>
	//		</startup>
	//	</configuration>

	public class FIXClient : MessageCracker, Application, IDisposable, IFIXClient
	{
		#region Events
		public event Action<Quote> QuoteReceived = q => { };
		public event Action<Execution> ExecutionReceived = e => { }; 
		#endregion

		#region Variables
		private SocketInitiator m_socketInitiator;
		private FileStoreFactory m_messageStoreFactory;
		private SessionSettings m_settings;
		private FileLogFactory m_logFactory;
		private QuickFix44.MessageFactory m_messageFactory;

		private readonly ILoggerFacade m_logger; // For Microsoft Prism

		private SessionID m_sessionID;  // limit to only 1 session for now

		public string Name { get; private set; }
		public bool IsStarted { get; private set; }
		#endregion

		#region Constructors
		// The logger should be dependency-injected by Prism
		public FIXClient(ILoggerFacade logger)
		{
			this.Name = Guid.NewGuid().ToString();
			this.m_logger = logger;
			
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
				this.m_settings = new SessionSettings(@"..\..\..\ClientModules\FIXClientConfig.txt");
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
				this.m_socketInitiator.stop();
				this.IsStarted = false;
			}
		}
		#endregion

		#region Callbacks
		public void onCreate(SessionID sessionID)
		{
			this.m_logger.Log(string.Format("FIXClient: QuickFIX Initiator created with Session ID {0}", sessionID), Category.Info, Priority.None);
		}

		public void onLogon(SessionID sessionID)
		{
			this.m_logger.Log(string.Format("FIXClient: QuickFIX Initiator logged on with Session ID {0}", sessionID), Category.Info, Priority.None);
			this.m_sessionID = sessionID;
		}

		public void onLogout(SessionID sessionID)
		{
			this.m_logger.Log(string.Format("FIXClient: QuickFIX Initiator logged out with Session ID {0}", sessionID), Category.Info, Priority.None);
			this.m_sessionID = null;
		}

		public void toAdmin(Message message, SessionID sessionID)
		{
			if (FIXHelpers.IsHeartbeat(message))
				return;

			Console.WriteLine("FIXClient: ToAdmin: " + message);
			this.m_logger.Log(string.Format("FIXClient: ToAdmin: {0}", message), Category.Info, Priority.None);
		}

		public void toApp(Message message, SessionID sessionID)
		{
			if (FIXHelpers.IsHeartbeat(message))
				return;
			
			//			Console.WriteLine("ToApp: " + message);
			this.m_logger.Log(string.Format("FIXClient: ToApp: {0}", message), Category.Info, Priority.None);
		}

		public void fromAdmin(Message message, SessionID sessionID)
		{
			if (FIXHelpers.IsHeartbeat(message))
				return;

			Console.WriteLine("FIXClient: FromAdmin: " + message);
			this.m_logger.Log(string.Format("FIXClient: FromAdmin: {0}", message), Category.Info, Priority.None);
		}

		public void fromApp(Message message, SessionID sessionID)
		{
			if (FIXHelpers.IsHeartbeat(message))
				return;

			Console.WriteLine("FIXClient: FromApp: " + message);
			this.m_logger.Log(string.Format("FIXClient: FromApp: {0}", message), Category.Info, Priority.None);
			this.crack(message, sessionID);
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
			//this.m_logger.Log(string.Format("FIXClient: ExecutionReport: {0}", message), Category.Info, Priority.None);
	
			Execution exec = new Execution();
			if (message.isSetClOrdID())
				exec.ClOrderID = message.getClOrdID().getValue();
			if (message.isSetOrderID())
				exec.OrderID = message.getOrderID().getValue();
			if (message.isSetSide())
				exec.Side = FIXExtensions.FromFIX(message.getSide().getValue());
			if (message.isSetAvgPx())
				exec.AveragePrice = message.getAvgPx().getValue();
			if (message.isSetOrderQty())
				exec.Quantity = (int)message.getOrderQty().getValue();
			if (message.isSetCumQty())
				exec.ExecutedQuantity = (int) message.getCumQty().getValue();
			if (message.isSetLeavesQty())
				exec.LeavesQuantity = (int)message.getLeavesQty().getValue();
			if (message.isSetExecID())
				exec.ExecID = message.getExecID().getValue();
			if (message.isSetOrdStatus())
				exec.OrderState = FIXExtensions.FromFIX(message.getOrdStatus());
			if (message.isSetPrice())
				exec.Price = message.getPrice().getValue();
			if (message.isSetTransactTime())
				exec.TransactTime = message.getTransactTime().getValue();

			this.m_logger.Log(string.Format("FIXClient: ExecutionReport event: {0}", exec), Category.Info, Priority.None);
			this.ExecutionReceived(exec);
		}

		public override void onMessage(MarketDataSnapshotFullRefresh message, SessionID session)
		{
			this.m_logger.Log(string.Format("FIXClient: MarketDataSnapshotFullRefresh Message: Symbol {0}",
				message.getSymbol().getValue()),
				Category.Info, Priority.None);

			Quote quote = new Quote { Symbol = message.getSymbol().getValue() };

			NoMDEntries noMdEntries = new NoMDEntries();
			message.get(noMdEntries);
			int nEntries = noMdEntries.getValue();
			if (nEntries == 0)
				return;

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
			}

			this.QuoteReceived(quote);
		}

		public override void onMessage(MarketDataIncrementalRefresh message, SessionID session)
		{
			base.onMessage(message, session);
		}

		public override void onMessage(Reject message, SessionID session)
		{
			Console.WriteLine("FIXClient: Reject Message: " + message);
			this.m_logger.Log(string.Format("FIXClient: Reject Message: {0}", message), Category.Info, Priority.None);
		}
		public override void onMessage(BusinessMessageReject message, SessionID session)
		{
			Console.WriteLine("FIXClient: BusinessMessageReject Message: " + message);
			this.m_logger.Log(string.Format("FIXClient: BusinessMessageReject Message: {0}", message), Category.Info, Priority.None);
		}
		public override void onMessage(MarketDataRequestReject message, SessionID session)
		{
			Console.WriteLine("FIXClient: MarketDataRequestReject Message: " + message);
			this.m_logger.Log(string.Format("FIXClient: MarketDataRequestReject Message: {0}", message), Category.Info, Priority.None);
		}
		public override void onMessage(QuoteRequestReject message, SessionID session)
		{
			Console.WriteLine("FIXClient: QuoteRequestReject Message: " + message);
			this.m_logger.Log(string.Format("FIXClient: QuoteRequestReject Message: {0}", message), Category.Info, Priority.None);
		}
		public override void onMessage(OrderCancelReject message, SessionID session)
		{
			Console.WriteLine("FIXClient: OrderCancelReject Message: " + message);
			this.m_logger.Log(string.Format("FIXClient: OrderCancelReject Message: {0}", message), Category.Info, Priority.None);
		}
		#endregion

		#region Send FIX Messages to the Server
		public void Publish(Order order)
		{
			if (this.m_sessionID == null)
				return;

			this.m_logger.Log(string.Format("FIXClient.Publish - sending an order {0}", order), Category.Info, Priority.None);

			NewOrderSingle fixOrder = new NewOrderSingle();

			fixOrder.set(new ClOrdID(order.ClOrderID));
			fixOrder.set(new Symbol(order.Symbol.Name));
			fixOrder.set(new Side(FIXExtensions.ToFIX(order.Side)));
			fixOrder.set(new OrderQty(order.Quantity));
			fixOrder.set(new Price(order.Type == OrderType.Market ? 0 : order.Price));
			fixOrder.set(new TimeInForce(FIXExtensions.ToFIX(order.TIF)));
			fixOrder.set(new OrdType(FIXExtensions.ToFIX(order.Type)));
			fixOrder.set(new TransactTime(DateTime.Now));

			Session.sendToTarget(fixOrder, this.m_sessionID);
		}

		public string Cancel(Order order)
		{
			if (this.m_sessionID == null)
				return null;

			this.m_logger.Log(string.Format("FIXClient.Cancel - canceling an order {0}", order), Category.Info, Priority.None);

			OrderCancelRequest cancelRequest = new OrderCancelRequest(
				new OrigClOrdID(order.ClOrderID),
				new ClOrdID(order.ClOrderID),
				new Side(FIXExtensions.ToFIX(order.Side)),
				new TransactTime(DateTime.Now));
			cancelRequest.set(new Symbol(order.Symbol.Name));
			cancelRequest.set(new OrderQty(order.ExecutedQuantity + order.LeavesQuantity));
			cancelRequest.set(new OrderID(order.OrderID));

			Session.sendToTarget(cancelRequest, this.m_sessionID);

			return order.OrderID;
		}

		public void RequestQuoteStream(List<MagmaTrader.Data.Symbol> symbols)
		{
			if (this.m_sessionID == null)
				return;

			this.m_logger.Log(string.Format("FIXClient.RequestQuoteStream - symbols {0}", symbols), Category.Info, Priority.None);
			MarketDataRequest request = FIXHelpers.CreateMarketDataRequestMessage(symbols);
			Session.sendToTarget(request, this.m_sessionID);
		}

		public void RequestQuoteStream(MagmaTrader.Data.Symbol symbol)
		{
			if (this.m_sessionID == null)
				return;

			this.m_logger.Log(string.Format("FIXClient.RequestQuoteStream - symbol {0}", symbol.Name), Category.Info, Priority.None);
			MarketDataRequest request = FIXHelpers.CreateMarketDataRequestMessage(new List<MagmaTrader.Data.Symbol> { symbol });
			Session.sendToTarget(request, this.m_sessionID);
		}
		#endregion
	}
}
