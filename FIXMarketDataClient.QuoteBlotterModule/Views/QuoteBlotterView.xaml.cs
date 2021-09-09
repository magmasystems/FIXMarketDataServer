using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using MagmaTrader.Interfaces;
using MagmaTrader.Presentation.ValueConverters;

namespace FIXMarketDataClient.EquityQuoteBlotterModule.Views
{
	public partial class QuoteBlotterView : IQuoteCacheView
	{
		private WPFQuoteGrid m_wpfQuoteGrid;

		#region Routed Event infrastructure for double-clicking a quote in the quote blotter
		public static RoutedEvent BlotterQuoteDoubleClickedEvent = 
			EventManager.RegisterRoutedEvent("EquityQuoteDoubleClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(QuoteBlotterView));
		
		public event RoutedEventHandler BlotterQuoteDoubleClicked
		{
			add    { AddHandler(BlotterQuoteDoubleClickedEvent, value);    }
			remove { RemoveHandler(BlotterQuoteDoubleClickedEvent, value); }
		}
		#endregion

		#region Routed Event infrastructure for entering a new symbol in the quote blotter
		public static RoutedEvent BlotterNewQuoteSymbolEnteredEvent =
			EventManager.RegisterRoutedEvent("BlotterNewQuoteSymbolEnteredEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(QuoteBlotterView));

		public event RoutedEventHandler BlotterNewQuoteSymbolEntered
		{
			add { AddHandler(BlotterNewQuoteSymbolEnteredEvent, value); }
			remove { RemoveHandler(BlotterNewQuoteSymbolEnteredEvent, value); }
		}
		#endregion

		static public DependencyProperty TitleProperty = DependencyProperty.RegisterAttached("Title", typeof(string), typeof(QuoteBlotterView), new PropertyMetadata("View"));
		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public QuoteBlotterView()
		{
			this.Title = "Equities";
			InitializeComponent();
			this.Loaded += this.OnLoaded;
			//RegionManager.SetRegionName(this, "EquityQuoteBlotterRegion");
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
