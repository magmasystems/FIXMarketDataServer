using FIXMarketDataServer;

namespace MagmaTrader.MarketData
{
	public interface IFXLevel1MarketDataGenerator : IMarketDataGenerator<FXQuote>
	{
		int AskSizeMax { get; set; }
		int AskSizeMin { get; set; }
		int BidSizeMax { get; set; }
		int BidSizeMin { get; set; }
	}
}
