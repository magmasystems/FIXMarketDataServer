using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using FIXMarketDataServer;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;
using MagmaTrader.MarketData;

namespace FIXMarketDataClient.Level2BookModule.ViewModels
{
	public class Level2BookViewModel : DependencyObject, INotifyPropertyChanged, ILevel2BookViewModel, IDisposable
	{
		// The ViewModel should know NOTHING about the View. Everything is communicated through data binding.
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Variables
		public ILevel2BookModel Model { get; set; }
		public ILevel2BookView View { get; set; }
		#endregion

		#region Constructors
		// This is automatically constructed by the Unity Container
		public Level2BookViewModel(ILevel2BookView view, ILevel2BookModel model)
		{
			this.View = view;
			this.Model = model;
			this.View.SetViewModel(this);
			this.View.PropertyChanged += this.OnPropertyChanged;

			this.PopulateCache(EquityLevel2MarketDataGenerator.CreateSampleQuoteList());
			this.Current = this.BookCache["AAPL"];
		}
		#endregion

		#region Cleanup
		public void Dispose()
		{
		}
		#endregion

		#region Interface to the QuoteCache
		public Level2BookCache BookCache
		{
			get { return this.Model.Level2BookCache; }
			set
			{
				this.Model.Level2BookCache = value;
				this.NotifyPropertyChanged("Level2BookCache");
			}
		}

		public void PopulateCache(List<Level2Book> books)
		{
			this.BookCache.Add(books);
		}

		// DataBinding for the list of quotes to the grids
		public Level2Book this[string symbol]
		{
			get { return this.BookCache[symbol]; }
		}

		public Level2Book Current
		{
			get { return this.Model.Current;  }
			set { this.Model.Current = value; }
		}

		public void ProcessBook(Level2Book book)
		{
			if (book == null)
				return;

			if (Dispatcher.CheckAccess())
			{
				this.BookCache.Process(book);
			}
			else
			{
				Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() => this.BookCache.Process(book)));
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

		#region Quote Generator Control
		// DataBinding for the quote generator interval
		public int Interval
		{
			get { return 0; }
			set { }
		}

		public bool IsGeneratorRunning
		{
			get; set;
		}

		public void ToggleQuoteGenerationTimer()
		{
		}
		#endregion
	}
}
