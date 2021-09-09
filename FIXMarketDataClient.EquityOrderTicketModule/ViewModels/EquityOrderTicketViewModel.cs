using System.ComponentModel;
using System.Windows;
using FIXMarketDataClient.EquityOrderTicketModule.Views;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;
using MagmaTrader.Presentation;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;

namespace FIXMarketDataClient.EquityOrderTicketModule.ViewModels
{
	public class EquityOrderTicketViewModel : DependencyObject, INotifyPropertyChanged, IEquityOrderTicketViewModel
	{
		// The ViewModel should know NOTHING about the View. Everything is communicated through data binding.
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Variables
		public IEquityOrderTicketModel Model { get; set; }
		public IEquityOrderTicketView  View  { get; set; }
		private IFIXClient FIXClient { get; set; }

		private readonly IUnityContainer m_unityContainer;
		private readonly IEventAggregator m_eventAggregator;
		#endregion

		#region Constructors
		public EquityOrderTicketViewModel(IUnityContainer unityContainer, IEquityOrderTicketView view, IEquityOrderTicketModel model, IFIXClient fixClient)
		{
			this.m_unityContainer = unityContainer;
			this.m_eventAggregator = this.m_unityContainer.Resolve<IEventAggregator>();

			this.Model = model;

			this.View = view;
			this.View.SetViewModel(this);
			this.View.PropertyChanged += this.OnPropertyChanged;

			// The FIX client is used to send FIX Order messages out to the OMS
			this.FIXClient = fixClient;
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

		#region Interface to the Model
		public EquityOrder Order
		{
			get
			{
				return this.Model.Order;
			}
			set
			{
				this.Model.Order = value;
				((EquityOrderTicketView) this.View).Order = value;
				this.NotifyPropertyChanged("Order");
			}
		}
		#endregion

		#region Dialog Box
		public bool? Show(Window owner)
		{
			Window view = this.View as Window;
			if (view == null)
				return false;
			
			view.Owner = owner;
			view.ShowDialog();
			bool? rc = view.DialogResult;

			if (rc.HasValue && rc == true)
			{
				// Send the order to the OMS
				this.SendOrderToOMS(this.Order);
			}

			return rc;
		}
		#endregion

		#region OMS Interface
		protected void SendOrderToOMS(Order order)
		{
			// Sanity checks
			if (this.FIXClient == null)
				return;
			if (order == null)
				return;
			if (!order.IsValid)
				return;

			// Kludge - until we have the OrderManagement infra set up, just publish this to the FIX Client
			this.FIXClient.Publish(order);

			if (this.m_eventAggregator != null)
				this.m_eventAggregator.GetEvent<NewOrderEvent>().Publish(new NewOrderEventArgs(order));
		}
		#endregion
	}
}
