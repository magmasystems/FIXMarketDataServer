using System;
using System.Collections.Generic;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;
using MagmaTrader.Utilities;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using QuickFix;
using QuickFix44;
using Message = QuickFix.Message;
using MessageCracker = QuickFix.MessageCracker;
using Symbol = QuickFix.Symbol;

namespace FIXMarketDataServer.FIXServerModule
{
	//  Note - The QuickFIX assembly is built with .NET 2.0, so we need to add useLegacyV2RuntimeActivationPolicy="true" to the app.config file
	//	<configuration>
	//		<startup useLegacyV2RuntimeActivationPolicy="true">
	//			<supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>
	//		</startup>
	//	</configuration>


	public class FIXServer : MessageCracker, Application, IDisposable, IFIXServer
	{
		#region Variables
		private SocketAcceptor   m_socketAcceptor;
		private FileStoreFactory m_messageStoreFactory;
		private SessionSettings  m_settings;
		private FileLogFactory   m_logFactory;
		private QuickFix44.MessageFactory m_messageFactory;
		private readonly ILoggerFacade m_logger;  // For Microsoft Prism
		private readonly IEventAggregator m_eventAggregator;  // For Microsoft Prism
		private SessionID        m_sessionID;  // limit to only 1 session for now
		#endregion

		public FIXServer(ILoggerFacade logger, IEventAggregator eventAggregator)
		{
			this.m_logger = logger;
			this.m_eventAggregator = eventAggregator;
			this.Init();
		}

		public void Dispose()
		{
			this.Stop();
			if (this.m_socketAcceptor != null)
			{
				this.m_socketAcceptor.Dispose();
				this.m_socketAcceptor = null;
			}
		}

		private void Init()
		{
			try
			{
				this.m_settings            = new SessionSettings(@"..\..\..\ServerModules\FIXServerConfig.txt");
				this.m_messageStoreFactory = new FileStoreFactory(this.m_settings);
				this.m_logFactory          = new FileLogFactory(/*this.m_settings*/@"C:\QuickFixLogs");
				this.m_messageFactory      = new QuickFix44.MessageFactory();
				this.m_socketAcceptor      = new SocketAcceptor(this, this.m_messageStoreFactory, this.m_settings, this.m_logFactory, this.m_messageFactory);
			}
			catch (ConfigError exc)
			{
				Console.WriteLine(exc.Message);
			}
		}

		public void Start()
		{
			if (this.m_socketAcceptor != null)
			{
				this.m_socketAcceptor.start();
				this.IsStarted = true;
				this.m_logger.Log(string.Format("FIXServer: Started"), Category.Info, Priority.None);
			}
		}

		public void Stop()
		{
			if (this.m_socketAcceptor != null && this.m_socketAcceptor.isLoggedOn())
			{
				// NOTE - we seem to get stuck when we call stop()
				//this.m_socketAcceptor.stop();
				this.IsStarted = false;
				this.m_logger.Log(string.Format("FIXServer: Stopped"), Category.Info, Priority.None);
			}
		}

		public bool IsStarted { get; private set; }

		public void onCreate(SessionID sessionID)
		{
			this.m_logger.Log(string.Format("FIXServer: QuickFIX Acceptor created with Session ID {0}", sessionID), Category.Info, Priority.None);
		}

		public void onLogon(SessionID sessionID)
		{
			this.m_logger.Log(string.Format("FIXServer: QuickFIX Acceptor logged on with Session ID {0}", sessionID), Category.Info, Priority.None);
			this.m_sessionID = sessionID;
		}

		public void onLogout(SessionID sessionID)
		{
			this.m_logger.Log(string.Format("FIXServer: QuickFIX Acceptor logged out with Session ID {0}", sessionID), Category.Info, Priority.None);
			this.m_sessionID = null;
		}

		public void toAdmin(Message message, SessionID sessionID)
		{
			if (FIXHelpers.IsHeartbeat(message))
				return;

			Console.WriteLine("ToAdmin: " + message);
			this.m_logger.Log(string.Format("FIXServer: ToAdmin: {0}", message), Category.Info, Priority.None);
		}

		public void toApp(Message message, SessionID sessionID)
		{
			if (FIXHelpers.IsHeartbeat(message))
				return;

			Console.WriteLine("FIXServer ToApp: " + message);
			this.m_logger.Log(string.Format("FIXServer: ToApp: {0}", message), Category.Info, Priority.None);
		}

		public void fromAdmin(Message message, SessionID sessionID)
		{
			if (FIXHelpers.IsHeartbeat(message))
				return;

			Console.WriteLine("FromAdmin: " + message);
			this.m_logger.Log(string.Format("FIXServer: FromAdmin: {0}", message), Category.Info, Priority.None);
		}

		public void fromApp(Message message, SessionID sessionID)
		{
			if (FIXHelpers.IsHeartbeat(message))
				return;

			Console.WriteLine("FromApp: " + message);
			this.m_logger.Log(string.Format("FIXServer: FromApp: {0}", message), Category.Info, Priority.None);

			this.crack(message, sessionID);
		}


		public override void onMessage(QuickFix44.NewOrderSingle message, SessionID session)
		{
			this.m_logger.Log(string.Format("FIXServer: NewOrderSingle Message: Symbol {0}, Qty {1}, Price {2:nn.00}", 
				message.getSymbol().getValue(), message.getOrderQty().getValue(), message.getPrice().getValue()), 
				Category.Info, Priority.None);

			// Hand this over to the order manager
			Order order = FIXSerializers.ToOrder(message);
			this.m_eventAggregator.GetEvent<OrderMessageReceivedEvent>().Publish(new OrderMessageReceivedEventArgs(order, OrderAction.New, message));
		}

		public override void onMessage(QuickFix44.OrderCancelRequest message, SessionID session)
		{
			this.m_logger.Log(string.Format("FIXServer: OrderCancelRequest Message: Symbol {0}",
				message.getSymbol().getValue()),
				Category.Info, Priority.None);

			// Hand this over to the order manager
			Order order = FIXSerializers.ToOrder(message);
			this.m_eventAggregator.GetEvent<OrderMessageReceivedEvent>().Publish(new OrderMessageReceivedEventArgs(order, OrderAction.Cancel, message));
		}

		public override void onMessage(QuickFix44.MarketDataRequest message, SessionID session)
		{
			this.m_logger.Log(string.Format("FIXServer: MarketDataRequest Message: NoRelatedSymbol {0}",
				message.getNoRelatedSym().getValue()),
				Category.Info, Priority.None);

			Symbol symbol = new Symbol();

			NoRelatedSym noRelatedSym = new NoRelatedSym();
			message.get(noRelatedSym);
			int nSymbols = noRelatedSym.getValue();
			if (nSymbols == 0)
				return;

			QuickFix44.MarketDataRequest.NoRelatedSym group = new QuickFix44.MarketDataRequest.NoRelatedSym();
			for (uint i = 1; i <= nSymbols; i++)
			{
				message.getGroup(i, group);
				group.get(symbol);

				// Hand this over to the order manager
				this.m_eventAggregator.GetEvent<MarketDataRequestReceivedEvent>().Publish(new MarketDataRequestReceivedEventArgs(symbol.getValue(), MarketDataRequestAction.Subscribe, message));
			}
		}

		public override void onMessage(QuickFix44.Reject message, SessionID session)
		{
			Console.WriteLine("FIXServer: Reject Message: " + message);
			this.m_logger.Log(string.Format("FIXServer: Reject Message: {0}", message), Category.Info, Priority.None);
		}
		public override void onMessage(QuickFix44.BusinessMessageReject message, SessionID session)
		{
			Console.WriteLine("FIXServer: BusinessMessageReject Message: " + message);
			this.m_logger.Log(string.Format("FIXServer: BusinessMessageReject Message: {0}", message), Category.Info, Priority.None);
		}
		public override void onMessage(QuickFix44.MarketDataRequestReject message, SessionID session)
		{
			Console.WriteLine("FIXServer: MarketDataRequestReject Message: " + message);
			this.m_logger.Log(string.Format("FIXServer: MarketDataRequestReject Message: {0}", message), Category.Info, Priority.None);
		}
		public override void onMessage(QuickFix44.QuoteRequestReject message, SessionID session)
		{
			Console.WriteLine("FIXServer: QuoteRequestReject Message: " + message);
			this.m_logger.Log(string.Format("FIXServer: QuoteRequestReject Message: {0}", message), Category.Info, Priority.None);
		}
		public override void onMessage(QuickFix44.OrderCancelReject message, SessionID session)
		{
			Console.WriteLine("FIXServer: OrderCancelReject Message: " + message);
			this.m_logger.Log(string.Format("FIXServer: OrderCancelReject Message: {0}", message), Category.Info, Priority.None);
		}

		public void Publish(Quote quote)
		{
			QuickFix44.Quote fixQuote = new QuickFix44.Quote();
			fixQuote.setString(Symbol.FIELD,    quote.Symbol);
			fixQuote.setDouble(BidPx.FIELD,     quote.Bid);
			fixQuote.setDouble(OfferPx.FIELD,   quote.Ask);
			fixQuote.setDouble(BidSize.FIELD,   quote.BidSize);
			fixQuote.setDouble(OfferSize.FIELD, quote.AskSize);
			fixQuote.setString(QuoteID.FIELD,   quote.QuoteID);

			if (this.m_sessionID != null)
			{
				Session.sendToTarget(fixQuote, this.m_sessionID);
			}
		}

		/// <summary>
		/// Relays a FIX message to the GUI
		/// </summary>
		public void SendMessage(object message)
		{
			Message fixMessage = message as Message;
			if (message == null)
				return;

			if (this.m_sessionID != null)
			{
				Session.sendToTarget(fixMessage, this.m_sessionID);
			}
		}
	}
}
