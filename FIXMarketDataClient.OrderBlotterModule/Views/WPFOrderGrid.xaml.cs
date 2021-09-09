using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MagmaTrader.Data;
using MagmaTrader.Presentation;

namespace FIXMarketDataClient.OrderBlotterModule.Views
{
	public class Commands
	{
		static public RoutedUICommand CancelOrderCommand = new RoutedUICommand("CancelOrderCommand", "CancelOrderCommand", typeof(WPFOrderGrid));
	}

	public partial class WPFOrderGrid
	{
		public WPFOrderGrid()
		{
			InitializeComponent();
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			switch (e.Property.Name)
			{
				default:
					break;
			}
		}

		private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DependencyObject obj = e.OriginalSource as DependencyObject;
			if (obj == null)
				return;

			// Check if the user double-clicked a grid row and not something else
			DataGridRow row = ItemsControl.ContainerFromElement((DataGrid)sender, obj) as DataGridRow;
			if (row == null)
				return;

			// We want to get the Order object associated with the row
			Order order = row.Item as Order;

			// Kludge - for testing, we don't want to always have the server running, so let's pass a fake quote up
			if (order == null)
				order = new EquityOrder
				        	{
				        		Symbol = new Symbol("AAPL"),
				        		Price = 289.00,
				        		Quantity = 250,
				        		Side = Side.Buy,
				        		TIF = TimeInForce.Day,
				        		Type = OrderType.Limit
				        	};

			if (order != null)
			{
				RoutedEventArgs args = new BlotterOrderDoubleClickedEventArgs(order, OrderBlotterView.BlotterOrderDoubleClickedEvent, this);
				this.RaiseEvent(args);
			}
		}

		public Order SelectedOrder
		{
			get
			{
				return this.gridOrderCacheWPF.SelectedItem as Order;
			}
		}

		private void CancelOrderCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = false;

			Order order = e.Parameter as Order;
			if (order == null)
				return;

			if (!order.IsValid)
				return;

			if (order.IsTerminal)
				return;

			e.CanExecute = true;
		}

		private void CancelOrderCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Order order = e.Parameter as Order;
			if (order == null)
				return;

			RoutedEventArgs args = new BlotterOrderCancelledEventArgs(order, OrderBlotterView.BlotterOrderCancelledEvent, this);
			this.RaiseEvent(args);
		}

	}
}
