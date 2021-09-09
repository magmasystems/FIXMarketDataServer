using MagmaTrader.Data;
using Microsoft.Practices.Prism.Events;

namespace MagmaTrader.Presentation
{
	public class NewOrderEventArgs
	{
		public Order Order { get; set; }

		public NewOrderEventArgs(Order order)
		{
			this.Order = order;
		}
	}

	public class NewOrderEvent : CompositePresentationEvent<NewOrderEventArgs>
	{
	}

}
