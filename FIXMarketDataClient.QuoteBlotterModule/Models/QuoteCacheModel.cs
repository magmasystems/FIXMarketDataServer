using System;
using System.Collections.Generic;
using System.ComponentModel;
using FIXMarketDataServer;
using MagmaTrader.Interfaces;

namespace FIXMarketDataClient.EquityQuoteBlotterModule.Models
{
	public class QuoteCacheModel : INotifyPropertyChanged, IQuoteCacheModel
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private QuoteCache m_quoteCache;
		public QuoteCache QuoteCache
		{
			get
			{
				return this.m_quoteCache;
			}
			set
			{
				this.m_quoteCache = value;
				this.NotifyPropertyChanged("QuoteCache");
			}
		}

		public QuoteCacheModel()
		{
			this.QuoteCache = new QuoteCache();
		}

		public void AddQuotes(List<Quote> quotes)
		{
			this.m_quoteCache.Add(quotes);			
		}

		public void AddQuote(Quote quote)
		{
			this.m_quoteCache.Add(quote);
		}

		public bool Contains(string symbol)
		{
			return this.m_quoteCache.Contains(symbol);
		}

		private void NotifyPropertyChanged(string prop)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}
	}
}
