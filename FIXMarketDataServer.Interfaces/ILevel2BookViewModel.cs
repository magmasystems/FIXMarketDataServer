using MagmaTrader.Data;

namespace MagmaTrader.Interfaces
{
	public interface ILevel2BookViewModel
	{
		ILevel2BookModel Model { get; set; }
		ILevel2BookView  View  { get; set; }

		Level2Book Current { get; set; }

		void ProcessBook(Level2Book book);

		void ToggleQuoteGenerationTimer();
		bool IsGeneratorRunning { get; set; }
	}
}
