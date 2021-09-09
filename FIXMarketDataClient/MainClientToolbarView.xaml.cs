using System.Windows;
using System.Windows.Input;
using MagmaTrader.Interfaces;
using MagmaTrader.Presentation;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;

namespace FIXMarketDataClient
{
	public partial class MainToolbarView
	{
		private readonly IEventAggregator m_eventPublisher;
		private bool m_isFIXStarted;    // for the Command interface for the toolbar buttons
	
		public MainToolbarView()
		{
			InitializeComponent();

			if (App.Bootstrapper != null)
			{
				IUnityContainer container = App.Bootstrapper.Container;
				if (container != null)
				{
					this.m_eventPublisher = container.Resolve<IEventAggregator>();
				}
			}
		}

		private void OnFixStartClicked(object sender, RoutedEventArgs e)
		{
			this.m_eventPublisher.GetEvent<FIXClientControlEvent>().Publish(new FIXClientControlEventArgs(null, FIXGeneratorAction.Start));
			this.m_isFIXStarted = true;
			ListViewLogger.Log(this, "IsFixStarted became true");
		}

		private void OnFixStopClicked(object sender, RoutedEventArgs e)
		{
			this.m_eventPublisher.GetEvent<FIXClientControlEvent>().Publish(new FIXClientControlEventArgs(null, FIXGeneratorAction.Stop));
			this.m_isFIXStarted = false;
			ListViewLogger.Log(this, "IsFixStarted became false");
		}

		private void OnLevel2SubscribeClicked(object sender, RoutedEventArgs e)
		{
			this.m_eventPublisher.GetEvent<Level2QuoteServiceControlEvent>().Publish(new Level2QuoteServiceControlEventArgs(Level2QuoteServiceAction.Subscribe));
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
	}
}
