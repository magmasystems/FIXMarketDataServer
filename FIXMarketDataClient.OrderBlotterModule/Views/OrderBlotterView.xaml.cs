using System.ComponentModel;
using System.Windows;
using MagmaTrader.Interfaces;

namespace FIXMarketDataClient.OrderBlotterModule.Views
{
	public partial class OrderBlotterView : IOrderBlotterView
	{
		#region Routed Event infrastructure for double-clicking an order in the order blotter
		public static RoutedEvent BlotterOrderDoubleClickedEvent =
			EventManager.RegisterRoutedEvent("OrderDoubleClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OrderBlotterView));

		public event RoutedEventHandler BlotterOrderDoubleClicked
		{
			add { AddHandler(BlotterOrderDoubleClickedEvent, value); }
			remove { RemoveHandler(BlotterOrderDoubleClickedEvent, value); }
		}
		#endregion

		#region Routed Event infrastructure for cancelling an order in the order blotter
		public static RoutedEvent BlotterOrderCancelledEvent =
			EventManager.RegisterRoutedEvent("OrderCancelled", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OrderBlotterView));

		public event RoutedEventHandler BlotterOrderCancelled
		{
			add { AddHandler(BlotterOrderCancelledEvent, value); }
			remove { RemoveHandler(BlotterOrderCancelledEvent, value); }
		}
		#endregion

	
		public OrderBlotterView()
		{
			InitializeComponent();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void SetViewModel(IOrderBlotterViewModel viewModel)
		{
			this.DataContext = viewModel;
		}
	}
}
