using System;
using System.ComponentModel;
using System.Windows;
using MagmaTrader.Interfaces;
using MagmaTrader.OrderManagerModule.Models;
using MagmaTrader.OrderManagerModule.Services;
using MagmaTrader.OrderManagerModule.Views;

namespace MagmaTrader.OrderManagerModule.ViewModels
{
	public interface IOrderManagerViewModel : IObservable<OrderMessageReceivedEventArgs>, IDisposable
	{
		event Action<OrderMessageReceivedEventArgs> OrderMessageReceived;

		void OnOrderMessageReceived(OrderMessageReceivedEventArgs e);
		void OnMarketDataRequestReceived(MarketDataRequestReceivedEventArgs e);
		void OnFIXGeneratorActionReceived(FIXGeneratorControlEventArgs e);

		IOrderManagerModel Model { get; set; }
		IOrderManagerView View { get; set; }
	}

	public class OrderManagerViewModel : DependencyObject, INotifyPropertyChanged, IOrderManagerViewModel
	{
		// The ViewModel should know NOTHING about the View. Everything is communicated through data binding.
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		public event Action<OrderMessageReceivedEventArgs> OrderMessageReceived = e => { };
		#endregion

		#region Variables
		public IOrderManagerModel Model { get; set; }
		public IOrderManagerView  View  { get; set; }
		private IFIXServer FIXServer    { get; set; }
		private IFIXExchangeSimulatorClient FIXExchangeSimulator { get; set; }
		public IOrderManagerService OrderManager { get; private set; }
		#endregion

		#region Constructors
		public OrderManagerViewModel(IOrderManagerView view, IOrderManagerModel model, IOrderManagerService orderManagerService, IFIXServer fixServer, IFIXExchangeSimulatorClient fixExchangeSimulatorClient)
		{
			this.View = view;
			this.Model = model;
			this.View.SetViewModel(this);
			this.View.PropertyChanged += this.OnPropertyChanged;

			this.FIXServer = fixServer;
			this.FIXExchangeSimulator = fixExchangeSimulatorClient;
			this.FIXExchangeSimulator.ExecutionReportReceived += e => this.FIXServer.SendMessage(e);
			this.FIXExchangeSimulator.PassThruFixMessageReceived += e => this.FIXServer.SendMessage(e);

			this.OrderManager = orderManagerService;
			this.OrderManager.SetViewModel(this);
		}
		#endregion

		#region Cleanup
		public void Dispose()
		{
			this.OrderManager.Dispose();
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

		# region Handlers and Dispatchers for order messages
		public void OnOrderMessageReceived(OrderMessageReceivedEventArgs e)
		{
			if (e == null || e.Order == null)
				return;

			// We want to send out the new order to any RX Subscribers
			// But for now, let's just use a simple event
			this.OrderMessageReceived(e);

			// Send the FIX message to the exchange simulator
			if (e.UserData != null && this.FIXExchangeSimulator != null)
			{
				this.FIXExchangeSimulator.Publish(e.UserData);
			}
		}

		public void OnMarketDataRequestReceived(MarketDataRequestReceivedEventArgs e)
		{
			if (e == null || string.IsNullOrEmpty(e.Symbol))
				return;

			// Send the FIX message to the exchange simulator
			if (e.UserData != null && this.FIXExchangeSimulator != null)
			{
				this.FIXExchangeSimulator.Publish(e.UserData);
			}
		}
		#endregion

		#region Implementation of IObservable<out OrderMessageReceivedEventArgs>
		public IDisposable Subscribe(IObserver<OrderMessageReceivedEventArgs> observer)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region FIX Generator Control
		public void OnFIXGeneratorActionReceived(FIXGeneratorControlEventArgs e)
		{
			switch (e.Action)
			{
				case FIXGeneratorAction.Start:
					if (this.FIXExchangeSimulator != null)
					{
						this.FIXExchangeSimulator.Start();
					}
					break;
				case FIXGeneratorAction.Stop:
					if (this.FIXExchangeSimulator != null)
					{
						this.FIXExchangeSimulator.Stop();
					}
					break;
			}
		}
		#endregion
	}
}
