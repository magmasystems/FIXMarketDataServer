using FIXMarketDataServer;

namespace MagmaTrader.MarketData
{
	public interface IEquityLevel1MarketDataGenerator : IMarketDataGenerator<Quote>
	{
		int AskSizeMax { get; set; }
		int AskSizeMin { get; set; }
		int BidSizeMax { get; set; }
		int BidSizeMin { get; set; }
	}
}
