using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using FIXMarketDataClient.Level2QuoteServiceReference;
using FIXMarketDataServer;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;
using MagmaTrader.Presentation;
using MagmaTrader.MarketData;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;

namespace FIXMarketDataClient
{
	public partial class MainConsoleWindow : IShellView, IPubSubCallback
	{
		private IEventAggregator m_eventAggregator;
		private PubSubClient m_level2QuoteServiceClient;
		private ILevel2BookViewModel m_level2BookViewModel;
	
		public MainConsoleWindow()
		{
			InitializeComponent();
			App.Bootstrapper.ModuleWasLoaded += this.OnModuleLoaded;
		}

		private void OnModuleLoaded(object viewModel)
		{
			IUnityContainer container = App.Bootstrapper.Container;

			if (this.m_eventAggregator == null)
			{
				if (container != null)
				{
					this.m_eventAggregator = container.Resolve<IEventAggregator>();
					if (this.m_eventAggregator != null)
					{
						this.m_eventAggregator.GetEvent<Level2QuoteServiceControlEvent>().Subscribe(this.OnLevel2QuoteServiceSubscribe);
					}
				}
			}

			if (viewModel is IQuoteBlotterViewModel)
			{
				this.MainToolbar.toolBarFIXControl.DataContext = viewModel;
				if (this.m_eventAggregator != null)
					this.m_eventAggregator.GetEvent<BlotterQuoteDoubleClickedEvent>().Subscribe(this.OnBlotterQuoteDoubleClicked);
			}

			else if (viewModel is IFXQuoteBlotterViewModel)
			{
			}

			else if (viewModel is ILevel2BookViewModel)
			{
				this.m_level2BookViewModel = (ILevel2BookViewModel) viewModel;
				this.MainToolbar.toolBarLevel2Control.DataContext = viewModel;
			}

			else if (viewModel is IOrderBlotterViewModel)
			{
				if (this.m_eventAggregator != null)
					this.m_eventAggregator.GetEvent<BlotterOrderDoubleClickedEvent>().Subscribe(this.OnBlotterOrderDoubleClicked);
			}
		}

		void OnBlotterQuoteDoubleClicked(BlotterQuoteDoubleClickedEventArgs e)
		{
			Quote quote = e.Quote;
			if (quote == null)
				return;

			// We want to put up an order ticker
			IEquityOrderTicketViewModel vmOrderTicket = App.Bootstrapper.Container.Resolve<IEquityOrderTicketViewModel>(
				new ParameterOverrides
				{
				    { "quote", quote },
				    { "order", new Order()  },
				});
			if (vmOrderTicket == null)
			{
				MessageBox.Show("Cannot create the equity order ticket");
				return;
			}
			
			bool? rc = vmOrderTicket.Show(this);
			if (rc.GetValueOrDefault(true))
			{
			}
		}

		void OnBlotterOrderDoubleClicked(BlotterOrderDoubleClickedEventArgs e)
		{
			Order order = e.Order;
			if (order == null)
				return;

			// We want to put up an order ticker
			IEquityOrderTicketViewModel vmOrderTicket = App.Bootstrapper.Container.Resolve<IEquityOrderTicketViewModel>(
				new ParameterOverrides
				{
				    { "quote", new Quote()  },
				    { "order", order },
				});
			if (vmOrderTicket == null)
			{
				MessageBox.Show("Cannot create the equity order ticket");
				return;
			}
			vmOrderTicket.Order = order as EquityOrder;

			bool? rc = vmOrderTicket.Show(this);
			if (rc.GetValueOrDefault(true))
			{
			}
		}

		void OnLevel2QuoteServiceSubscribe(Level2QuoteServiceControlEventArgs e)
		{
			if (this.m_level2BookViewModel.IsGeneratorRunning)
			{
				this.m_level2BookViewModel.IsGeneratorRunning = false;
				return;
			}

			if (e.Action == Level2QuoteServiceAction.Subscribe)
			{
				IPubSubCallback objCallback = this;
				InstanceContext objContext = new InstanceContext(objCallback); 
				this.m_level2QuoteServiceClient = new PubSubClient(objContext, "NetTcpBinding_IPubSub");
				this.m_level2QuoteServiceClient.Subscribe("Level2Quotes.Book");
				this.m_level2BookViewModel.IsGeneratorRunning = true;
			}
		}

		#region Implementation of IPubSubCallback
		// This is called by the WCF Service whenever a Level2 Quote is published by the Server
		public void Notify(Message request)
		{
			if (request == null)
				return;

			if (this.m_level2BookViewModel == null)
				return;

			NotificationData notificationData = request.GetBody<NotificationData>();
			if (notificationData == null)
				return;

			Level2Book book = notificationData.Content;
			this.m_level2BookViewModel.ProcessBook(book);
		}

		public IAsyncResult BeginNotify(Message request, AsyncCallback callback, object asyncState)
		{
			return null;
		}

		public void EndNotify(IAsyncResult result)
		{
		}
		#endregion
	}
}
