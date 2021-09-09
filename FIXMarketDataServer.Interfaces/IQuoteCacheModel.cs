using System.Collections.Generic;
using FIXMarketDataServer;

namespace MagmaTrader.Interfaces
{
	public interface IQuoteCacheModel
	{
		QuoteCache QuoteCache { get; set; }

		void AddQuotes(List<Quote> quotes);
		void AddQuote(Quote quote);
		bool Contains(string symbol);
	}
}
