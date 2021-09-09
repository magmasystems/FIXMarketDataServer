using MagmaTrader.Data;

namespace MagmaTrader.MarketData
{
	public interface IEquityLevel2MarketDataGenerator : IMarketDataGenerator<Level2Book>
	{
		int AskSizeMax { get; set; }
		int AskSizeMin { get; set; }
		int BidSizeMax { get; set; }
		int BidSizeMin { get; set; }
	}
}
