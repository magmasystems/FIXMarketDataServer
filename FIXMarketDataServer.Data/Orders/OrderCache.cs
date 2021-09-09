using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Caching;

namespace MagmaTrader.Data
{
	public class OrderCache : IDisposable
	{
		#region Variables
		private readonly MemoryCache m_cache;
		private readonly ObservableCollection<Order> m_orders;
		private readonly HashSet<string> MapCacheContainsOrder = new HashSet<string>();
		#endregion

		#region Constructors
		public OrderCache()
		{
			this.m_cache = new MemoryCache("OrderCache", null);
			this.m_orders = new ObservableCollection<Order>();
		}
		#endregion

		#region Cleanup
		public void Dispose()
		{
			this.m_cache.Dispose();
			this.m_orders.Clear();
		}
		#endregion

		#region Methods
		public bool Contains(string orderId)
		{
			return this.m_cache.Contains(orderId);
		}

		public Order Get(string orderId)
		{
			if (!this.m_cache.Contains(orderId))
				return null;
	
			return this.m_cache.Get(orderId) as Order;
		}

		public void Process(Order order)
		{
			if (!this.MapCacheContainsOrder.Contains(order.ClOrderID))
			{
				this.Add(order);
				return;
			}

			this.Cache.Where(q => q.ClOrderID == order.ClOrderID).First().Copy(order);
		}

		public void Add(List<Order> orders)
		{
			orders.ForEach(this.Add);
		}

		public void Add(Order order)
		{
			if (order == null)
				return;

			if (this.Contains(order.ClOrderID))
				return;

			if (!this.MapCacheContainsOrder.Contains(order.ClOrderID))
				this.MapCacheContainsOrder.Add(order.ClOrderID);
			
			this.m_cache.Add(order.ClOrderID, order, new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration });
			this.m_orders.Add(order);
		}

		public void Remove(Order order)
		{
			if (order == null)
				return;

			this.Remove(order.ClOrderID);
			this.m_orders.Remove(order);
		}

		public void Remove(string orderId)
		{
			this.m_cache.Remove(orderId);
		}

		public ObservableCollection<Order> Cache
		{
			get { return this.m_orders; }
		}
		#endregion
	}
}
