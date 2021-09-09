using System.Windows;
using MagmaTrader.Data;
using Microsoft.Practices.Prism.Events;

namespace MagmaTrader.Presentation
{
	public class BlotterNewQuoteSymbolEnteredEventArgs : RoutedEventArgs
	{
		public Symbol Symbol { get; set; }

		public BlotterNewQuoteSymbolEnteredEventArgs(Symbol symbol, RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{
			this.Symbol = symbol;
		}
	}

	public class BlotterNewQuoteSymbolEnteredEvent : CompositePresentationEvent<BlotterNewQuoteSymbolEnteredEventArgs>
	{
	}
}
