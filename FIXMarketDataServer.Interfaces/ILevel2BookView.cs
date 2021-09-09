using System.ComponentModel;

namespace MagmaTrader.Interfaces
{
	public interface ILevel2BookView : INotifyPropertyChanged
	{
		void SetViewModel(ILevel2BookViewModel viewModel);
	}

}
