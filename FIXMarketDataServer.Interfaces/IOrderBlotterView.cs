using System.ComponentModel;

namespace MagmaTrader.Interfaces
{
	public interface IOrderBlotterView : INotifyPropertyChanged
	{
		void SetViewModel(IOrderBlotterViewModel viewModel);
	}

}
