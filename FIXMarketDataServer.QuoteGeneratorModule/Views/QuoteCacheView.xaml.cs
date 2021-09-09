using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using MagmaTrader.Interfaces;
using MagmaTrader.Presentation.ValueConverters;

namespace FIXMarketDataServer.QuoteGeneratorModule.Views
{
	public partial class QuoteCacheView : IQuoteCacheView
	{
		private WPFQuoteGrid m_wpfQuoteGrid;
		private DXQuoteGrid  m_dxQuoteGrid;
		
		public QuoteCacheView()
		{
			InitializeComponent();
			this.Loaded += this.QuoteCacheView_Loaded;
		}

		void QuoteCacheView_Loaded(object sender, RoutedEventArgs e)
		{
			this.m_wpfQuoteGrid = this.FindName("WPFQuoteGridControl") as WPFQuoteGrid;
			this.m_dxQuoteGrid  = this.FindName("DXQuoteGridControl") as DXQuoteGrid;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void SetViewModel(IQuoteBlotterViewModelBase viewModel)
		{
			this.DataContext = viewModel;
		}

		#region Dependency Properties
		public Color UpColor
		{
			get { return PriceChangeToColorValueConverter.UpColor; }
			set
			{
				PriceChangeToColorValueConverter.UpColor = value;
				if (this.m_wpfQuoteGrid != null)
					this.m_wpfQuoteGrid.RefreshGrid();
				if (this.m_dxQuoteGrid != null)
					this.m_dxQuoteGrid.RefreshGrid();
			}
		}
		public Color DownColor
		{
			get { return PriceChangeToColorValueConverter.DownColor; }
			set
			{
				PriceChangeToColorValueConverter.DownColor = value;
				if (this.m_wpfQuoteGrid != null)
					this.m_wpfQuoteGrid.RefreshGrid();
				if (this.m_dxQuoteGrid != null)
					this.m_dxQuoteGrid.RefreshGrid();
			}
		}
		public Color NoChangeColor
		{
			get { return PriceChangeToColorValueConverter.NoChangeColor; }
			set
			{
				PriceChangeToColorValueConverter.NoChangeColor = value;
				if (this.m_wpfQuoteGrid != null)
					this.m_wpfQuoteGrid.RefreshGrid();
				if (this.m_dxQuoteGrid != null)
					this.m_dxQuoteGrid.RefreshGrid();
			}
		}
		#endregion

	}
}
