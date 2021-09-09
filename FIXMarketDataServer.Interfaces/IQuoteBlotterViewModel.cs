using FIXMarketDataServer;

namespace MagmaTrader.Interfaces
{
	public interface IQuoteBlotterViewModel : IQuoteBlotterViewModelBase
	{
		void OnFIXClientActionReceived(FIXClientControlEventArgs e);
		void ProcessQuote(Quote quote);
	}

}
