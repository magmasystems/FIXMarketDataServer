using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using FIXMarketDataClient.FXQuoteBlotterModule.Views;
using FIXMarketDataServer;
using MagmaTrader.Interfaces;
using MagmaTrader.Presentation.ValueConverters;

namespace FIXMarketDataClient.FXQuoteBlotterModule.ViewModels
{
	public class FXQuoteBlotterViewModel : DependencyObject, INotifyPropertyChanged, IFXQuoteBlotterViewModel
	{
		// The ViewModel should know NOTHING about the View. Everything is communicated through data binding.
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Variables
		public IQuoteCacheModel Model { get; set; }
		public IQuoteCacheView View { get; set; }
		private IFIXClient FIXClient { get; set; }
		#endregion

		#region Constructors
		public FXQuoteBlotterViewModel(IQuoteCacheView view, IQuoteCacheModel model, IFIXClient fixClient)
		{
			this.View = view;
			this.Model = model;
			this.View.SetViewModel(this);
			this.View.PropertyChanged += this.OnPropertyChanged;

			this.FIXClient = fixClient;
			if (this.FIXClient != null)
			{
				this.FIXClient.QuoteReceived += this.ProcessQuote;
			}

			this.UpColor = PriceChangeToColorValueConverter.UpColor;
			this.DownColor = PriceChangeToColorValueConverter.DownColor;
			this.NoChangeColor = PriceChangeToColorValueConverter.NoChangeColor;
		}
		#endregion

		#region Properties
		public string Title
		{
			get { return ((FXQuoteBlotterView) this.View).Title; }
			set { ((FXQuoteBlotterView) this.View).Title = value; }
		}
		#endregion

		#region Change Colors on the View
		public static readonly DependencyProperty UpColorProperty = DependencyProperty.Register("UpColor", typeof (Color), typeof (FXQuoteBlotterViewModel));
		public static readonly DependencyProperty DownColorProperty = DependencyProperty.Register("DownColor", typeof(Color), typeof(FXQuoteBlotterViewModel));
		public static readonly DependencyProperty NoChangeColorProperty = DependencyProperty.Register("NoChangeColor", typeof(Color), typeof(FXQuoteBlotterViewModel));

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
