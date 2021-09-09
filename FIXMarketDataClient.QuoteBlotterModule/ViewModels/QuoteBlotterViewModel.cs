using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using FIXMarketDataClient.EquityQuoteBlotterModule.Views;
using FIXMarketDataServer;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;
using MagmaTrader.Presentation;
using MagmaTrader.Presentation.ValueConverters;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;

namespace FIXMarketDataClient.EquityQuoteBlotterModule.ViewModels
{
	public class QuoteBlotterViewModel : DependencyObject, INotifyPropertyChanged, IQuoteBlotterViewModel
	{
		// The ViewModel should know NOTHING about the View. Everything is communicated through data binding.
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Variables
		public IQuoteCacheModel Model { get; set; }
		public IQuoteCacheView View { get; set; }
		private IFIXClient FIXClient { get; set; }

		private readonly IUnityContainer m_unityContainer;
		private readonly IEventAggregator m_eventAggregator;
		#endregion

		#region Constructors
		public QuoteBlotterViewModel(IUnityContainer unityContainer, IQuoteCacheView view, IQuoteCacheModel model, IFIXClient fixClient)
		{
			this.m_unityContainer = unityContainer;
			this.m_eventAggregator = this.m_unityContainer.Resolve<IEventAggregator>();

			this.View = view;
			this.Model = model;
			this.View.SetViewModel(this);
			this.View.PropertyChanged += this.OnPropertyChanged;

			this.FIXClient = fixClient;
			if (this.FIXClient != null)
			{
				this.FIXClient.QuoteReceived += this.ProcessQuote;
			}

			((QuoteBlotterView) view).BlotterQuoteDoubleClicked += this.OnBlotterQuoteDoubleClicked;
			((QuoteBlotterView) view).BlotterNewQuoteSymbolEntered += this.OnBlotterNewQuoteSymbolEntered;

			this.UpColor = PriceChangeToColorValueConverter.UpColor;
			this.DownColor = PriceChangeToColorValueConverter.DownColor;
			this.NoChangeColor = PriceChangeToColorValueConverter.NoChangeColor;
		}

		void OnBlotterNewQuoteSymbolEntered(object sender, RoutedEventArgs e)
		{
			e.Handled = true;

			BlotterNewQuoteSymbolEnteredEventArgs args = e as BlotterNewQuoteSymbolEnteredEventArgs;
			if (args == null)
				return;

			if (!this.SubscribeToQuote(args.Symbol))
				return;

			if (this.m_eventAggregator != null)
				this.m_eventAggregator.GetEvent<BlotterNewQuoteSymbolEnteredEvent>().Publish(args);
		}


		bool SubscribeToQuote(Symbol symbol)
		{
			if (symbol == null || string.IsNullOrEmpty(symbol.Name))
				return false;
	
			// See if we have this quote already
			if (this.Model.Contains(symbol.Name))
				return false;

			this.Model.AddQuote(new Quote { Symbol = symbol.Name });

			if (this.FIXClient != null)
				this.FIXClient.RequestQuoteStream(symbol);

			return true;
		}

		void OnBlotterQuoteDoubleClicked(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			if (this.m_eventAggregator == null)
				return;
			this.m_eventAggregator.GetEvent<BlotterQuoteDoubleClickedEvent>().Publish(e as BlotterQuoteDoubleClickedEventArgs);
		}
		#endregion

		#region Properties
		public string Title
		{
			get { return ((QuoteBlotterView)this.View).Title; }
			set { ((QuoteBlotterView)this.View).Title = value; }
		}
		#endregion

		#region Change Colors on the View
		public static readonly DependencyProperty UpColorProperty = DependencyProperty.Register("UpColor", typeof (Color), typeof (QuoteBlotterViewModel));
		public static readonly DependencyProperty DownColorProperty = DependencyProperty.Register("DownColor", typeof(Color), typeof(QuoteBlotterViewModel));
		public static readonly DependencyProperty NoChangeColorProperty = DependencyProperty.Register("NoChangeColor", typeof(Color), typeof(QuoteBlotterViewModel));

		public Color UpColor
		{
			get
			{
				return (Color) GetValue(UpColorProperty);
			}
			set
			{
				SetValue(UpColorProperty, value);
				this.View.UpColor = value;
				this.NotifyPropertyChanged("UpColor");
			}
		}
		public Color DownColor
		{
			get
			{
				return (Color)GetValue(DownColorProperty);
			}
			set
			{
				SetValue(DownColorProperty, value);
				this.View.DownColor = value;
				this.NotifyPropertyChanged("DownColor");
			}
		}
		public Color NoChangeColor
		{
			get
			{
				return (Color)GetValue(NoChangeColorProperty);
			}
			set
			{
				SetValue(NoChangeColorProperty, value);
				this.View.NoChangeColor = value;
				this.NotifyPropertyChanged("NoChangeColor");
			}
		}
		#endregion

		#region Interface to the QuoteCache
		public QuoteCache QuoteCache
		{
			get
			{
				return this.Model.QuoteCache;
			}
			set
			{
				this.QuoteCache = value;
				this.NotifyPropertyChanged("QuoteCache");
			}
		}

		// DataBinding for the list of quotes to the grids
		public ObservableCollection<Quote> TheQuotes
		{
			get { return this.QuoteCache.Cache; }
		}

		public void ProcessQuote(Quote quote)
		{
			if (quote == null)
				return;

			if (Dispatcher.CheckAccess())
			{
				this.QuoteCache.Process(quote);
			}
			else
			{
				Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() => this.QuoteCache.Process(quote)));
			}
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

		#region FIX Generator Control
		public void OnFIXClientActionReceived(FIXClientControlEventArgs e)
		{
			switch (e.Action)
			{
				case FIXGeneratorAction.Start:
					if (this.FIXClient != null)
					{
						this.FIXClient.Start();
					}
					break;

				case FIXGeneratorAction.Stop:
					if (this.FIXClient != null)
					{
						this.FIXClient.Stop();
					}
					break;
			}
		}
		#endregion
	}
}
