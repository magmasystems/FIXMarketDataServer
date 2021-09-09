using System.Collections.Generic;
using MagmaTrader.Data;

namespace MagmaTrader.Interfaces
{
	public interface IOrderBlotterModel
	{
		OrderCache OrderCache { get; set; }
		void AddOrders(List<Order> orders);
		void AddOrder(Order order);
	}
}
