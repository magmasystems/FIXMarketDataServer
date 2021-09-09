using System;
using MagmaTrader.Data;
using Microsoft.Practices.Prism.Events;

namespace MagmaTrader.Interfaces
{
	public class OrderMessageReceivedEventArgs : EventArgs
	{
		public Order Order;
		public OrderAction Action;
		public object UserData;

		public OrderMessageReceivedEventArgs(Order order, OrderAction action, object userData = null)
		{
			this.Order = order;
			this.Action = action;
			this.UserData = userData;
		}
	}

	public class OrderMessageReceivedEvent : CompositePresentationEvent<OrderMessageReceivedEventArgs>
	{
	}
}
