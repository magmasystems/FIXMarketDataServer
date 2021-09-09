using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MagmaTrader.Interfaces;
using MagmaTrader.Presentation.ValueConverters;

namespace FIXMarketDataClient.FXQuoteBlotterModule.Views
{
	public partial class FXQuoteBlotterView : IQuoteCacheView
	{
		private WPFQuoteGrid m_wpfQuoteGrid;

		static public DependencyProperty TitleProperty = DependencyProperty.RegisterAttached("Title", typeof(string), typeof(FXQuoteBlotterView), new PropertyMetadata("View"));
		public string Title
		{
			get { return (string) GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public FXQuoteBlotterView()
		{
			this.Title = "FX";
			InitializeComponent();
			this.Loaded += this.OnLoaded;
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			this.m_wpfQuoteGrid = this.FindName("WPFQuoteGridControl") as WPFQuoteGrid;
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
			}
		}
		#endregion
	}
}
