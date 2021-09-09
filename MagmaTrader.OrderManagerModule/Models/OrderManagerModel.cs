using System;
using System.ComponentModel;
using MagmaTrader.Data;

namespace MagmaTrader.OrderManagerModule.Models
{
	public interface IOrderManagerModel : IDisposable
	{
		OrderCache OrderCache { get; }

		void  AddOrder(Order order);
		Order GetOrder(string orderId);
		bool  ContainsOrder(string orderId);
	}

	public class OrderManagerModel : IOrderManagerModel
	{
		public OrderCache OrderCache { get; private set; }

		public OrderManagerModel()
		{
			this.OrderCache = new OrderCache();
		}

		public void Dispose()
		{
			this.OrderCache.Dispose();
		}

		public void AddOrder(Order order)
		{
			this.OrderCache.Add(order);
			order.PropertyChanged += this.OnOrderPropertyChanged;
		}

		public Order GetOrder(string orderId)
		{
			return this.OrderCache.Get(orderId);
		}

		public bool ContainsOrder(string orderId)
		{
			return this.OrderCache.Contains(orderId);
		}

		void OnOrderPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// Trap changes that are sent by the exchange simulator.
			// We need to send state changes back to the client.
		}
	}
}
