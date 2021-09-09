using System;

namespace MagmaTrader.MarketData
{
	public interface IMarketDataGenerator<TQuote> : IDisposable
	{
		event Action<TQuote, int> QuoteGenerated;
		
		void Start();
		void Stop();
		void IncrementChooserSize();

		int Interval { get; set; }
	}
}
