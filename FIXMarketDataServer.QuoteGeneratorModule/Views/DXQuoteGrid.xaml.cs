using DevExpress.Xpf.Grid;

namespace FIXMarketDataServer.QuoteGeneratorModule.Views
{
	public partial class DXQuoteGrid
	{
		public DXQuoteGrid()
		{
			InitializeComponent();
		}

		public void RefreshGrid()
		{
			GridControl grid = this.FindName("gridQuoteCache") as GridControl;
			if (grid != null)
			{
				grid.RefreshData();
			}
		}
	}
}
