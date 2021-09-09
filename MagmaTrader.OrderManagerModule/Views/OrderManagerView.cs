using System.ComponentModel;
using MagmaTrader.OrderManagerModule.ViewModels;

namespace MagmaTrader.OrderManagerModule.Views
{
	public interface IOrderManagerView : INotifyPropertyChanged
	{
		void SetViewModel(IOrderManagerViewModel viewModel);
	}

	public class OrderManagerView : IOrderManagerView
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void SetViewModel(IOrderManagerViewModel viewModel)
		{
			//this.DataContext = viewModel;
		}
	}
}
