using System.ComponentModel;
using System.Windows;
using FIXMarketDataServer;
using MagmaTrader.Data;

namespace MagmaTrader.Interfaces
{
	public interface IEquityOrderTicketModel : INotifyPropertyChanged
	{
		EquityOrder Order { get; set; }
		string Name { get; }
	}

	public interface IEquityOrderTicketView : INotifyPropertyChanged
	{
		void SetViewModel(IEquityOrderTicketViewModel viewModel);
	}

	public interface IEquityOrderTicketViewModel
	{
		IEquityOrderTicketModel Model { get; set; }
		IEquityOrderTicketView View { get; set; }
		EquityOrder Order { get; set; }

		bool? Show(Window owner);
	}

}
