using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;
using MagmaTrader.MarketData;
using MagmaTrader.Presentation.ValueConverters;
using Microsoft.Practices.Prism.Events;

namespace FIXMarketDataServer.QuoteGeneratorModule.ViewModels
{
	public class QuoteCacheViewModel : DependencyObject, INotifyPropertyChanged, IQuoteCacheViewModel
	{
		// The ViewModel should know NOTHING about the View. Everything is communicated through data binding.
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Variables
		public IQuoteCacheModel Model { get; set; }
		public IQuoteCacheView View { get; set; }
		private readonly IEquityLevel1MarketDataGenerator m_quoteGenerator;
		private IFIXServer FIXServer { get; set; }
		private IFIXExchangeSimulatorClient FIXExchangeSimulator { get; set; }

		// Prism stuff
		private readonly IEventAggregator m_eventAggregator;
		#endregion

		#region Constructors
		public QuoteCacheViewModel(IQuoteCacheView view, IQuoteCacheModel model, IFIXServer fixServer, IFIXExchangeSimulatorClient fixExchangeSimulatorClient, IEventAggregator eventAgregator)
		{
			this.View = view;
			this.Model = model;
			this.View.SetViewModel(this);
			this.View.PropertyChanged += this.OnPropertyChanged;

			this.m_eventAggregator = eventAgregator;
			if (this.m_eventAggregator != null)
			{
				this.m_eventAggregator.GetEvent<FIXExchangeSimulatorControlEvent>().Subscribe(this.OnFIXExchangeSimulatorActionReceived);
			}

			this.FIXServer = fixServer;
			this.FIXExchangeSimulator = fixExchangeSimulatorClient;

			this.UpColor = PriceChangeToColorValueConverter.UpColor;
			this.DownColor = PriceChangeToColorValueConverter.DownColor;
			this.NoChangeColor = PriceChangeToColorValueConverter.NoChangeColor;

			this.AddQuotes(EquityLevel1MarketDataGenerator.CreateSampleQuoteList());

			this.m_quoteGenerator = new EquityLevel1MarketDataGenerator(this.QuoteCache.Cache);
			this.m_quoteGenerator.QuoteGenerated += this.OnQuoteGenerated;
		}
		#endregion

		#region Change Colors on the View
		public static readonly DependencyProperty UpColorProperty = DependencyProperty.Register("UpColor", typeof (Color), typeof (QuoteCacheViewModel));
		public static readonly DependencyProperty DownColorProperty = DependencyProperty.Register("DownColor", typeof(Color), typeof(QuoteCacheViewModel));
		public static readonly DependencyProperty NoChangeColorProperty = DependencyProperty.Register("NoChangeColor", typeof(Color), typeof(QuoteCacheViewModel));

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

		public void AddQuotes(List<Quote> quotes)
		{
			this.QuoteCache.Add(quotes);
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

		#region Quote Generator Control
		// DataBinding for the quote generator interval
		public int Interval
		{
			get { return this.m_quoteGenerator.Interval;  }
			set { this.m_quoteGenerator.Interval = value; }
		}

		public bool IsGeneratorRunning { get; set; }

		public void ToggleQuoteGenerationTimer()
		{
			if (this.IsGeneratorRunning)
			{
				this.m_quoteGenerator.Stop();
				this.IsGeneratorRunning = false;
			}
			else
			{
				this.m_quoteGenerator.Start();
				this.IsGeneratorRunning = true;
			}
			this.NotifyPropertyChanged("IsGeneratorRunning");
		}

		private void OnQuoteGenerated(Quote quote, int idxQuote)
		{
			if (quote == null)
				return;

			Quote oldQuote = this.QuoteCache.Cache[idxQuote];
			oldQuote.Copy(quote);

			if (this.FIXServer != null && this.FIXServer.IsStarted)
			{
				this.FIXServer.Publish(oldQuote);
			}
		}

		/// <summary>
		/// When the GUI sends a MarketDataRequest FIX message, the Event Generator will publish an event for all to listen to.
		/// </summary>
		public void OnMarketDataRequestReceived(MarketDataRequestReceivedEventArgs e)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke((Action) (() => this.OnMarketDataRequestReceived(e)));
				return;
			}

			if (string.IsNullOrEmpty(e.Symbol))
				return;

			if (this.Model.Contains(e.Symbol))
				return;
	
			Quote quote = new Quote {Symbol = e.Symbol, Bid = 30.00, Ask = 30.03 };
			this.Model.AddQuote(quote);
			this.m_quoteGenerator.IncrementChooserSize();
		}
		#endregion

		#region FIX Generator Control
		public void OnFIXGeneratorActionReceived(FIXGeneratorControlEventArgs e)
		{
			switch (e.Action)
			{
				case FIXGeneratorAction.Start:
					if (this.FIXServer != null)
					{
						this.FIXServer.Start();
					}
					break;
				case FIXGeneratorAction.Stop:
					if (this.FIXServer != null)
					{
						this.FIXServer.Stop();
					}
					break;
			}
		}
		#endregion

		#region FIX Exchange Simulator Control
		public void OnFIXExchangeSimulatorActionReceived(FIXExchangeSimulatorControlEventArgs e)
		{
			switch (e.Action)
			{
				case FIXExchangeSimulatorAction.LoggedIn:
					List<Symbol> symbols = new List<Symbol>(this.QuoteCache.Cache.Count);
					symbols.AddRange(this.QuoteCache.Cache.Select(quote => new Symbol(quote.Symbol)));
					this.FIXExchangeSimulator.RequestQuoteStream(symbols);
					break;
			}
		}
		#endregion

	}
}
