using FIXMarketDataServer;

namespace MagmaTrader.Interfaces
{
	public interface IFXQuoteBlotterViewModel : IQuoteBlotterViewModelBase
	{
		void ProcessQuote(Quote quote);
	}

}
