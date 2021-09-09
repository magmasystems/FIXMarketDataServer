using System.ComponentModel;
using MagmaTrader.Interfaces;

namespace FIXMarketDataServer.Level2BookModule.Views
{
	public partial class Level2BookView : ILevel2BookView
	{
		public Level2BookView()
		{
			InitializeComponent();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void SetViewModel(ILevel2BookViewModel viewModel)
		{
			this.DataContext = viewModel;
		}
	}
}
