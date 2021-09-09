using System.Collections.Generic;
using System.ComponentModel;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;

namespace FIXMarketDataClient.OrderBlotterModule.Models
{
	public class OrderBlotterModel : INotifyPropertyChanged, IOrderBlotterModel
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private OrderCache m_orderCache;
		public OrderCache OrderCache
		{
			get
			{
				return this.m_orderCache;
			}
			set
			{
				this.m_orderCache = value;
				this.NotifyPropertyChanged("OrderCache");
			}
		}

		public OrderBlotterModel()
		{
			this.OrderCache = new OrderCache();
		}

		public void AddOrders(List<Order> orders)
		{
			this.OrderCache.Add(orders);
		}

		public void AddOrder(Order order)
		{
			this.OrderCache.Add(order);
		}

		private void NotifyPropertyChanged(string prop)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}
	}
}
