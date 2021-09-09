using System.Collections.Generic;
using System.ComponentModel;
using FIXMarketDataServer;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;

namespace FIXMarketDataClient.Level2BookModule.Models
{
	public class Level2BookModel : INotifyPropertyChanged, ILevel2BookModel
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private Level2BookCache m_bookCache;
		public Level2BookCache Level2BookCache
		{
			get
			{
				return this.m_bookCache;
			}
			set
			{
				this.m_bookCache = value;
				this.NotifyPropertyChanged("Level2BookCache");
			}
		}

		public Level2BookModel()
		{
			this.Level2BookCache = new Level2BookCache();
		}

		public Level2Book this[string symbol]
		{
			get { return this.m_bookCache[symbol]; }
		}

		public void Add(List<Level2Book> books)
		{
			this.m_bookCache.Add(books);			
		}

		public void Add(Level2Book book)
		{
			this.m_bookCache.Add(book);
		}

		public Level2Book Current { get; set; }

		private void NotifyPropertyChanged(string prop)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}
	}
}
