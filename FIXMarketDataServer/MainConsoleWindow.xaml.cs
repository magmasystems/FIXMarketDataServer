using MagmaTrader.Interfaces;

namespace FIXMarketDataServer
{
	public partial class MainConsoleWindow : IShellView
	{
		public MainConsoleWindow()
		{
			InitializeComponent();
			App.Bootstrapper.ModuleWasLoaded += this.OnModuleLoaded;
			//RegionManager.SetRegionName(this, "MainWindow");
		}

		private void OnModuleLoaded(object viewModel)
		{
			if (viewModel is IQuoteCacheViewModel)
			{
				var toolbar = this.FindName("MainToolbar");
				if (toolbar != null && toolbar is MainToolbarView)
				{
					((MainToolbarView) toolbar).DataContext = viewModel;
				}
			}

			else if (viewModel is ILevel2BookViewModel)
			{
				this.MainToolbar.toolBarLevel2.DataContext = viewModel;
			}
		}
	}
}
