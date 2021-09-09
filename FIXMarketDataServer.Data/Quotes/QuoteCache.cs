using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FIXMarketDataServer
{
	public class QuoteCache
	{
		public ObservableCollection<Quote> Cache { get; set; }
		private readonly HashSet<string> MapCacheContainsSymbol = new HashSet<string>();

		public QuoteCache()
		{
			this.Cache = new ObservableCollection<Quote>();
		}

		public QuoteCache(IEnumerable<Quote> quotes)
		{
			this.Cache = new ObservableCollection<Quote>(quotes);
		}

		public void Add(Quote quote)
		{
			if (!this.MapCacheContainsSymbol.Contains(quote.Symbol))
			{
				this.MapCacheContainsSymbol.Add(quote.Symbol);
				this.Cache.Add(quote);
			}
			else
			{
				this.Update(quote);
			}
		}

		public void Add(List<Quote> quotes)
		{
			quotes.ForEach(this.Add);
		}

		public bool Contains(string symbol)
		{
			return this.MapCacheContainsSymbol.Contains(symbol);
		}

		public void Process(Quote quote)
		{
			if (!this.MapCacheContainsSymbol.Contains(quote.Symbol))
			{
				this.Add(quote);
			}
			else
			{
				this.Update(quote);
			}
		}

		private void Update(Quote quote)
		{
			this.Cache.Where(q => q.Symbol == quote.Symbol).First().Copy(quote);
		}
	}
}
