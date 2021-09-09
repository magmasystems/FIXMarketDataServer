using System;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;
using MagmaTrader.OrderManagerModule.ViewModels;
using MagmaTrader.Threading;

namespace MagmaTrader.OrderManagerModule.Services
{
	public interface IOrderManagerService : IDisposable
	{
		void SetViewModel(IOrderManagerViewModel vm);
	}

	public class OrderManagerService : IOrderManagerService, IThreadQueueCallback<OrderMessageReceivedEventArgs>
	{
		#region Variables
		private IOrderManagerViewModel ViewModel { get; set; }
		private readonly IThreadQueue<OrderMessageReceivedEventArgs> m_threadQueueOrders;
		#endregion


		#region Constructors
		public OrderManagerService()
		{
//			this.m_threadQueueOrders = new OneThreadQueue<OrderMessageReceivedEventArgs>(this);
			if (this.m_threadQueueOrders != null)
				this.m_threadQueueOrders.Start();
		}
		#endregion

		#region Cleanup
		public void Dispose()
		{
			if (this.m_threadQueueOrders != null)
				this.m_threadQueueOrders.Stop();
		}
		#endregion

		#region Setting the ViewModel
		public void SetViewModel(IOrderManagerViewModel vm)
		{
			this.ViewModel = vm;
			this.ViewModel.OrderMessageReceived += this.OnOrderMessageReceived;
		}
		#endregion

		#region Order Message Handlers
		void OnOrderMessageReceived(OrderMessageReceivedEventArgs e)
		{
			if (this.m_threadQueueOrders != null)
				this.m_threadQueueOrders.QueueObject(e);
			else
				this.OnThreadCallback(e);
		}

		public void OnThreadCallback(OrderMessageReceivedEventArgs e)
		{
			switch (e.Action)
			{
				case OrderAction.New:
					this.OnNewOrder(e.Order);
					break;
				case OrderAction.Reject:
					break;
				case OrderAction.Cancel:
					break;
			}
		}

		protected void OnNewOrder(Order order)
		{
			if (order == null)
				return;

			// Enrich the order with state information
			order.OrderState = OrderState.PendingNew;  // first go into PendingNew, then into New.
			order.ExecutionState = ExecutionState.NotFilled;

			// Cache the order and let the simulator do fills on it
			this.ViewModel.Model.AddOrder(order);

			//
		}
		#endregion
	}
}
