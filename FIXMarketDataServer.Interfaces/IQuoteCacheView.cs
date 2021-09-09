using System.ComponentModel;
using System.Windows.Media;

namespace MagmaTrader.Interfaces
{
	public interface IQuoteCacheView : INotifyPropertyChanged
	{
		void SetViewModel(IQuoteBlotterViewModelBase viewModel);

		Color UpColor   { get; set; }
		Color DownColor { get; set; }
		Color NoChangeColor { get; set; }
	}

}
