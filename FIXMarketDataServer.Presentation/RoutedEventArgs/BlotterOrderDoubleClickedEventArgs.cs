using System.Windows;
using MagmaTrader.Data;
using Microsoft.Practices.Prism.Events;

namespace MagmaTrader.Presentation
{
	public class BlotterOrderDoubleClickedEventArgs : RoutedEventArgs
	{
		public Order Order { get; set; }
		
		public BlotterOrderDoubleClickedEventArgs(Order order, RoutedEvent routedEvent, object source) : base(routedEvent, source)
		{
			this.Order = order;
		}
	}

	public class BlotterOrderDoubleClickedEvent : CompositePresentationEvent<BlotterOrderDoubleClickedEventArgs>
	{
	}

}
