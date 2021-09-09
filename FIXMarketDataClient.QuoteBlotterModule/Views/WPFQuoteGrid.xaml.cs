using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FIXMarketDataServer;
using MagmaTrader.Data;
using MagmaTrader.Presentation;

namespace FIXMarketDataClient.EquityQuoteBlotterModule.Views
{
	public partial class WPFQuoteGrid
	{
		public WPFQuoteGrid()
		{
			InitializeComponent();
		}

		protected void OnSymbolCellLostFocus(object sender, RoutedEventArgs e)
		{
			DataGridCell cell = sender as DataGridCell;
			if (cell == null)
				return;
			
			TextBox tbx = cell.Content as TextBox;
			if (tbx == null)
				return;
			
			if (string.IsNullOrEmpty(tbx.Text))
				return;

			RoutedEventArgs args = new BlotterNewQuoteSymbolEnteredEventArgs(new Symbol(tbx.Text), QuoteBlotterView.BlotterNewQuoteSymbolEnteredEvent, this);
			this.RaiseEvent(args);
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			switch (e.Property.Name)
			{
				case "UpColor":
					this.Foreground = new SolidColorBrush((Color)e.NewValue);
					break;
			}
		}

		public void RefreshGrid()
		{
			DataGrid grid = this.FindName("gridQuoteCacheWPF") as DataGrid;
			if (grid != null)
			{
				grid.InvalidateProperty(ForegroundProperty);
				grid.Items.Refresh();
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

			// We want to get the Quote object associated with the row
			Quote quote = row.Item as Quote;
			
			// Kludge - for testing, we don't want to always have the server running, so let's pass a fake quote up
			if (quote == null)
				quote = new Quote {Symbol = "YHOO", Ask = 10.03, Bid = 10.02, AskSize = 100, BidSize = 100};

			if (quote != null)
			{
				RoutedEventArgs args = new BlotterQuoteDoubleClickedEventArgs(quote, QuoteBlotterView.BlotterQuoteDoubleClickedEvent, this);
				this.RaiseEvent(args);
			}
		}
	}
}
