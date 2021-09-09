using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using FIXMarketDataServer;

namespace MagmaTrader.MarketData
{
	public class EquityLevel1MarketDataGenerator : MarketDataGenerator<Quote>, IEquityLevel1MarketDataGenerator
	{
		public int BidSizeMin { get; set; }
		public int BidSizeMax { get; set; }
		public int AskSizeMin { get; set; }
		public int AskSizeMax { get; set; }


		public EquityLevel1MarketDataGenerator(ObservableCollection<Quote> quoteCache, int interval = 500) : base(quoteCache, interval)
		{
			this.BidSizeMin = 100;
			this.BidSizeMax = 200;
			this.AskSizeMin = 100;
			this.AskSizeMax = 200;
		}

		static public List<Quote> CreateSampleQuoteList()
		{
			List<Quote> quotes = new List<Quote>
			{
				new Quote {Symbol = "AAPL", Bid = 380.00, Ask = 380.05 },
				new Quote {Symbol = "C", Bid = 32.50, Ask = 32.51 },
				new Quote {Symbol = "DAL", Bid = 10.23, Ask = 10.24 },
				new Quote {Symbol = "GOOG", Bid = 590.45, Ask = 590.55 },
				new Quote {Symbol = "HPQ", Bid = 36.75, Ask = 36.76 },
			};

			return quotes;
		}

		public override void GenerateTick()
		{
			// Pick out one of the quotes in the cache
			int idxQuote = this.m_chooser.NextIndex;
			Debug.Assert(idxQuote >= 0 && idxQuote < this.m_quoteCache.Count);
			Quote quote = this.m_quoteCache[idxQuote];

			// Ticks should move by a penny. If the random number is between 0 and 0.3, move down. If it's between 0.7 and 1, then move up. Else, stay the same.

			// Adjust the bid
			double r = this.m_rnd.NextDouble();
			int sign = (r > 0.7) ? 1 : (r < 0.3) ? -1 : 0;
			double increment = 0.01 * sign;
			double bid = quote.Bid + increment;

			// Adjust the ask
			r = this.m_rnd.NextDouble();
			sign = (r > 0.7) ? 1 : (r < 0.3) ? -1 : 0;
			increment = 0.01 * sign;
			double ask = quote.Ask + increment;

			// Sanity check
			if (ask <= bid)
				ask = bid + 0.01;

			// Adjust the bid and ask sizes
			int bidSize = this.m_rnd.Next(this.BidSizeMin, this.BidSizeMax);
			int askSize = this.m_rnd.Next(this.AskSizeMin, this.AskSizeMax);

			// Put the new values back into the quote
			Quote newQuote = new Quote { Symbol = quote.Symbol, Bid = bid, Ask = ask, BidSize = bidSize, AskSize = askSize };
			this.FireQuoteGeneratedEvent(newQuote, idxQuote);
		}

	}
}
