using System.Windows;
using FIXMarketDataServer;
using Microsoft.Practices.Prism.Events;

namespace MagmaTrader.Presentation
{
	public class BlotterQuoteDoubleClickedEventArgs : RoutedEventArgs
	{
		public Quote Quote { get; set; }
		
		public BlotterQuoteDoubleClickedEventArgs(Quote quote, RoutedEvent routedEvent, object source) : base(routedEvent, source)
		{
			this.Quote = quote;
		}
	}

	public class BlotterQuoteDoubleClickedEvent : CompositePresentationEvent<BlotterQuoteDoubleClickedEventArgs>
	{
	}

}
