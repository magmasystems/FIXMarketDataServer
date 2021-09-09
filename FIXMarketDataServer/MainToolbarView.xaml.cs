using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MagmaTrader.Interfaces;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;

namespace FIXMarketDataServer
{
	public partial class MainToolbarView
	{
		private readonly IEventAggregator m_eventPublisher;
		private bool m_isFIXStarted;
		
		public MainToolbarView()
		{
			InitializeComponent();

			IUnityContainer container = App.Bootstrapper.Container;
			this.m_eventPublisher = container.Resolve<IEventAggregator>();
		}

		private void OnFixStartClicked(object sender, RoutedEventArgs e)
		{
			this.m_eventPublisher.GetEvent<FIXGeneratorControlEvent>().Publish(new FIXGeneratorControlEventArgs(null, FIXGeneratorAction.Start));
			this.m_isFIXStarted = true;
		}

		private void OnFixStopClicked(object sender, RoutedEventArgs e)
		{
			this.m_eventPublisher.GetEvent<FIXGeneratorControlEvent>().Publish(new FIXGeneratorControlEventArgs(null, FIXGeneratorAction.Stop));
			this.m_isFIXStarted = false;
		}

		private void OnTimerToggleClicked(object sender, RoutedEventArgs e)
		{
			IQuoteCacheViewModel vm = this.DataContext as IQuoteCacheViewModel;
			if (vm != null)
				vm.ToggleQuoteGenerationTimer();
		}

		private void UpColorPicker_SelectedColorChanged(Color clr)
		{
			IQuoteCacheViewModel vm = this.DataContext as IQuoteCacheViewModel;
			if (vm != null)
				vm.UpColor = clr;
		}

		private void DownColorPicker_SelectedColorChanged(Color clr)
		{
			IQuoteCacheViewModel vm = this.DataContext as IQuoteCacheViewModel;
			if (vm != null)
				vm.DownColor = clr;
		}

		private void NoChangeColorPicker_SelectedColorChanged(Color clr)
		{
			IQuoteCacheViewModel vm = this.DataContext as IQuoteCacheViewModel;
			if (vm != null)
				vm.NoChangeColor = clr;
		}

		#region Custom WPF Commands
		private void StartFIXCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !this.m_isFIXStarted;
		}

		private void StartFIXCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.OnFixStartClicked(sender, e);
		}

		private void StopFIXCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			//e.CanExecute = this.m_isFIXStarted;
			e.CanExecute = true;
		}

		private void StopFIXCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.OnFixStopClicked(sender, e);
		}
		#endregion

		#region Commands for the Level2 Book toolbar
		private void OnLevel2BookTimerToggleClicked(object sender, RoutedEventArgs e)
		{
			IUnityContainer container = App.Bootstrapper.Container;
			ILevel2BookViewModel vm = container.Resolve<ILevel2BookViewModel>();
			if (vm != null)
				vm.ToggleQuoteGenerationTimer();
		}

		private void cbxLevel2BookList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{

		}
		#endregion
	}
}
