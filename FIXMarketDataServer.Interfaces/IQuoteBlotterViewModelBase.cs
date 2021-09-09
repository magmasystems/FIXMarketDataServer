using System.Windows.Media;

namespace MagmaTrader.Interfaces
{
	public interface IQuoteBlotterViewModelBase
	{
		IQuoteCacheModel Model { get; set; }
		IQuoteCacheView  View  { get; set; }

		Color UpColor { get; set; }
		Color DownColor { get; set; }
		Color NoChangeColor { get; set; }
	}

}
