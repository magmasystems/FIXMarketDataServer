using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using FIXMarketDataClient.OrderBlotterModule.Views;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;
using MagmaTrader.Presentation;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Unity;

namespace FIXMarketDataClient.OrderBlotterModule.ViewModels
{
	public class OrderBlotterViewModel : DependencyObject, INotifyPropertyChanged, IOrderBlotterViewModel
	{
		// The ViewModel should know NOTHING about the View. Everything is communicated through data binding.
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Variables
		public IOrderBlotterModel Model { get; set; }
		public IOrderBlotterView View   { get; set; }
		private IFIXClient FIXClient { get; set; }

		private readonly IUnityContainer m_unityContainer;
		private readonly IEventAggregator m_eventAggregator;
		private readonly ILoggerFacade m_logger;
		#endregion

		#region Constructors
		public OrderBlotterViewModel(IUnityContainer unityContainer, IOrderBlotterView view, IOrderBlotterModel model, IFIXClient fixClient)
		{
			this.m_unityContainer = unityContainer;
			this.m_eventAggregator = this.m_unityContainer.Resolve<IEventAggregator>();
			this.m_logger = this.m_unityContainer.Resolve<ILoggerFacade>();

			this.View = view;
			this.Model = model;
			this.View.SetViewModel(this);
			this.View.PropertyChanged += this.OnPropertyChanged;

			this.FIXClient = fixClient;
			if (this.FIXClient != null)
			{
				this.FIXClient.ExecutionReceived += this.ProcessOrder;
			}

			((OrderBlotterView) view).BlotterOrderDoubleClicked += this.OnBlotterQuoteDoubleClicked;
			((OrderBlotterView) view).BlotterOrderCancelled += this.OnBlotterOrderCancelled;

			// When the user presses Buy/Sell on the order ticket, a new order is sent to the OMS. We need to
			// hook into this event and populate the order blotter with the new order.
			if (this.m_eventAggregator != null)
				this.m_eventAggregator.GetEvent<NewOrderEvent>().Subscribe(this.OnNewOrderSubmitted);
		}
		#endregion

		#region UI Event Handlers
		// ReSharper disable UnusedParameter.Local
		void OnBlotterQuoteDoubleClicked(object sender, RoutedEventArgs e)
		{
			if (this.m_eventAggregator == null)
				return;
			this.m_eventAggregator.GetEvent<BlotterOrderDoubleClickedEvent>().Publish(e as BlotterOrderDoubleClickedEventArgs);
		}

		void OnBlotterOrderCancelled(object sender, RoutedEventArgs e)
		{
			if (this.m_eventAggregator == null)
				return;

			BlotterOrderCancelledEventArgs args = e as BlotterOrderCancelledEventArgs;
			if (args == null)
				return;

			this.m_eventAggregator.GetEvent<BlotterOrderCancelledEvent>().Publish(e as BlotterOrderCancelledEventArgs);
			this.SendCancelOrderToOMS(args.Order);
		}
		// ReSharper restore UnusedParameter.Local
		#endregion

		#region Interface to the OrderCache
		public OrderCache OrderCache
		{
			get
			{
				return this.Model.OrderCache;
			}
			set
			{
				this.OrderCache = value;
				this.NotifyPropertyChanged("OrderCache");
			}
		}

		// DataBinding for the list of quotes to the grids
		public ObservableCollection<Order> TheOrders
		{
			get { return this.OrderCache.Cache; }
		}

		public void ProcessOrder(Order order)
		{
			if (order == null)
				return;

			if (Dispatcher.CheckAccess())
			{
				this.OrderCache.Process(order);
			}
			else
			{
				Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() => this.OrderCache.Process(order)));
			}
		}
		#endregion

		#region Interface to the Order Model
		private void OnNewOrderSubmitted(NewOrderEventArgs e)
		{
			if (e == null || e.Order == null)
				return;

			if (Dispatcher.CheckAccess())
			{
				this.Model.AddOrder(e.Order);
			}
			else
			{
				Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() => this.Model.AddOrder(e.Order)));
			}
		}
		#endregion

		#region OMS Interface
		protected void SendCancelOrderToOMS(Order order)
		{
			// Sanity checks
			if (this.FIXClient == null)
				return;
			if (order == null)
				return;
			if (!order.IsValid)
				return;

			string cancelRequestID = this.FIXClient.Cancel(order);
			if (!string.IsNullOrEmpty(cancelRequestID))
			{
				// TODO - put this request into a list of pending cancel requests
			}
		}
		#endregion


		#region Property Change and Notification
		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}

		private void NotifyPropertyChanged(string prop)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}
		#endregion
	}
}
