using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using FIXMarketDataServer;

namespace MagmaTrader.MarketData
{
	public class FXLevel1MarketDataGenerator : MarketDataGenerator<FXQuote>, IFXLevel1MarketDataGenerator
	{
		public int BidSizeMin { get; set; }
		public int BidSizeMax { get; set; }
		public int AskSizeMin { get; set; }
		public int AskSizeMax { get; set; }


		public FXLevel1MarketDataGenerator(ObservableCollection<FXQuote> quoteCache, int interval = 500) : base(quoteCache, interval)
		{
			this.BidSizeMin = 100;
			this.BidSizeMax = 200;
			this.AskSizeMin = 100;
			this.AskSizeMax = 200;
		}

		static public List<FXQuote> CreateSampleQuoteList()
		{
			/*
				CHF/JPY 		Swiss Franc / Japanese Yen 	

				EUR/JPY 		Euro / Japanese Yen 	
				EUR/CHF 		Euro / Swiss Franc 	
				EUR/GBP 		Euro / British Pound
				
				GBP/CHF 		British Pound / Swiss Franc 	
				GBP/JPY 		British Pound / Japanese Yen 	

				USD/AUD 		US Dollar / Australian Dollar
				USD/CHF 		US Dollar / Swiss Franc 	
				USD/CAD 		US Dollar / Canadian Dollar 	
				USD/EUR 		US Dollar / Euro
				USD/GBP 	  	US Dollar / British Pound
				USD/JPY 		US Dollar / Japanese Yen 	
				USD/ZAR 		US Dollar / South African Rand 	
				USD/GRD 		US Dollar / Greek Drachma 	
				USD/SEK 		US Dollar / Swedish Kroner 	
				USD/NOK 		US Dollar / Norwegian Kroner 	
				USD/DKK 		US Dollar / Danish Kroner 	
				USD/FIM 		US Dollar / Finnish Markka 	
				USD/NLG 		US Dollar / Dutch Guilder 	
				USD/MXN 		US Dollar / Mexican Peso 	
				USD/BRL 		US Dollar / Brazilian Real 	
				USD/IDR 		US Dollar / Indonesian Rupiah 	
				USD/HKD 		US Dollar / Hong Kong Dollar 	
				USD/SGD 		US Dollar / Singapore Dollar 	
				USD/CZK 		US Dollar / Czech Kroner
				USD/NZD 		US Dollar / New Zealand Dollar 	
			 
				GLD				Gold
				SLV				Silver
			*/

			List<FXQuote> quotes = new List<FXQuote>
			{
				new FXQuote {Symbol = "USDEUR", Bid = 380.00, Ask = 380.05 },
				new FXQuote {Symbol = "EURJPY", Bid = 32.50,  Ask = 32.51 },
				new FXQuote {Symbol = "USDGBP", Bid = 10.23,  Ask = 10.24 },
				new FXQuote {Symbol = "USDJPY", Bid = 590.45, Ask = 590.55 },
				new FXQuote {Symbol = "USDAUD", Bid = 36.75,  Ask = 36.76 },
			};

			return quotes;
		}

		public override void GenerateTick()
		{
			// Pick out one of the quotes in the cache
			int idxQuote = this.m_chooser.NextIndex;
			Debug.Assert(idxQuote >= 0 && idxQuote < this.m_quoteCache.Count);
			FXQuote quote = this.m_quoteCache[idxQuote];

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
			FXQuote newQuote = new FXQuote { Symbol = quote.Symbol, Bid = bid, Ask = ask, BidSize = bidSize, AskSize = askSize };
			this.FireQuoteGeneratedEvent(newQuote, idxQuote);
		}

	}
}
