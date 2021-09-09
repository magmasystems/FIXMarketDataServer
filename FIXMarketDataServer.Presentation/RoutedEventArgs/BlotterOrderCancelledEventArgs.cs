using System.Windows;
using MagmaTrader.Data;
using Microsoft.Practices.Prism.Events;

namespace MagmaTrader.Presentation
{
	public class BlotterOrderCancelledEventArgs : RoutedEventArgs
	{
		public Order Order { get; set; }

		public BlotterOrderCancelledEventArgs(Order order, RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{
			this.Order = order;
		}
	}

	public class BlotterOrderCancelledEvent : CompositePresentationEvent<BlotterOrderCancelledEventArgs>
	{
	}
}
