using System.Windows.Controls;
using System.Windows.Media;

namespace FIXMarketDataClient.FXQuoteBlotterModule.Views
{
	public partial class WPFQuoteGrid
	{
		public WPFQuoteGrid()
		{
			InitializeComponent();
		}

		protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
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
	}
}
