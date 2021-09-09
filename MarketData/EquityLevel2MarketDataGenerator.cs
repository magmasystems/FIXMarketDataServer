using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MagmaTrader.Data;

namespace MagmaTrader.MarketData
{
	public class EquityLevel2MarketDataGenerator : MarketDataGenerator<Level2Book>, IEquityLevel2MarketDataGenerator
	{
		public int BidSizeMin { get; set; }
		public int BidSizeMax { get; set; }
		public int AskSizeMin { get; set; }
		public int AskSizeMax { get; set; }

		protected List<string> BrokerList = new List<string> {"ARCA", "BAC", "C", "INET", "JPM"};
		protected EquityLevel2MarketDataBroadcaster m_wcfBroadcaster;

		public EquityLevel2MarketDataGenerator(ObservableCollection<Level2Book> quoteCache, int interval = 500)
			: base(quoteCache, interval)
		{
			this.BidSizeMin = 100;
			this.BidSizeMax = 200;
			this.AskSizeMin = 100;
			this.AskSizeMax = 200;

			this.Interval = 2000;

			this.m_wcfBroadcaster = new EquityLevel2MarketDataBroadcaster();
		}

		public override void Dispose()
		{
			if (this.m_wcfBroadcaster != null)
				this.m_wcfBroadcaster.Dispose();
			
			base.Dispose();
		}


		static public List<Level2Book> CreateSampleQuoteList()
		{
			List<Level2Book> quotes = new List<Level2Book>
			{
				new Level2Book("AAPL"),
			};

			return quotes;
		}

		public override void GenerateTick()
		{
			// Pick out one of the quotes in the cache
			int idxBook = this.m_chooser.NextIndex;
			Debug.Assert(idxBook >= 0 && idxBook < this.m_quoteCache.Count);

			Level2Book book = this.m_quoteCache[idxBook];

			// Wipe out the previous book
			book.Clear();

			// First build the bid side of the book, then the offer side);
			for (int idxSide = 0;  idxSide < 2;  idxSide++)
			{
				Side side = (idxSide == 0) ? Side.Buy : Side.Sell;

				// Establish a base price for the stock
				double incrementValue = (side == Side.Buy) ? -0.01 : 0.01;
				double basePrice = 25.00 + incrementValue;

				int numQuotesToGenerateForSide = this.m_rnd.Next(5, 10);
				while (--numQuotesToGenerateForSide >= 0)
				{
					int quantity = (side == Side.Buy) ? this.m_rnd.Next(this.BidSizeMin, this.BidSizeMax) : this.m_rnd.Next(this.AskSizeMin, this.AskSizeMax);
					string broker = this.BrokerList[this.m_rnd.Next(0, this.BrokerList.Count)];
					Level2DisplayQuote quote = new Level2DisplayQuote(basePrice, quantity, broker);
					book.Insert(side, quote);

					// Should we increment/decrement. Roll a 3-sides dice. If it's 0, then increment, else stay the same.
					bool shouldIncrement = this.m_rnd.Next(0, 3) == 0;
					if (shouldIncrement)
						basePrice += incrementValue;
				}
			}

			// this.FireQuoteGeneratedEvent(newQuote, idxQuote);

			if (this.m_wcfBroadcaster != null)
			{
				this.m_wcfBroadcaster.SendLevel2Book(book);
			}
		}
	}
}
