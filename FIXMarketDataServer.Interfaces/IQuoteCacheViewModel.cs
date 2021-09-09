using System.Collections.Generic;
using FIXMarketDataServer;

namespace MagmaTrader.Interfaces
{
	public interface IQuoteCacheViewModel : IQuoteBlotterViewModelBase
	{
		void ToggleQuoteGenerationTimer();
		bool IsGeneratorRunning { get; set; }

		void AddQuotes(List<Quote> quotes);
		
		void OnFIXGeneratorActionReceived(FIXGeneratorControlEventArgs e);
		void OnMarketDataRequestReceived(MarketDataRequestReceivedEventArgs e);
	}

}
