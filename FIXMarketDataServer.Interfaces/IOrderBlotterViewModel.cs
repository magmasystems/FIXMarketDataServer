using MagmaTrader.Data;

namespace MagmaTrader.Interfaces
{
	public interface IOrderBlotterViewModel
	{
		IOrderBlotterModel Model { get; set; }
		IOrderBlotterView View   { get; set; }

		void ProcessOrder(Order order);
	}
}
