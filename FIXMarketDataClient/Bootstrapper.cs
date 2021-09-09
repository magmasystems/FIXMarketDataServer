using System;
using System.Collections.Generic;
using System.Windows;
using MagmaTrader.Interfaces;
using MagmaTrader.Presentation;
using MagmaTrader.Prism;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace FIXMarketDataClient
{
	public class Bootstrapper : UnityBootstrapper
	{
		public event Action<object> ModuleWasLoaded = e => { }; 

		public IShellView ShellView
		{
			get; private set;
		}
	
		protected override IUnityContainer CreateContainer()
		{
			return new UnityContainer();
		}

		protected override void ConfigureContainer()
		{
			// We can override any of the standard managers here.
			// Here is an example of overriding the standard ModuleManager
			this.Container.RegisterType<IModuleManager, MyModuleManager>();

			base.ConfigureContainer();
		}

		protected override IModuleCatalog CreateModuleCatalog()
		{
			DirectoryModuleCatalog catalog = new MultipleDirectoryModuleCatalog(new List<string> { @"..\..\..\ClientModules" });
			return catalog;
		}

		protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
		{
			// We can add our own region mappings here.

			RegionAdapterMappings mappings = ServiceLocator.Current.GetInstance<RegionAdapterMappings>();
			if (mappings != null)
			{
				mappings.RegisterMapping(typeof(MainToolbarView), ServiceLocator.Current.GetInstance<ToolbarAdapter>());
			}
			
			return base.ConfigureRegionAdapterMappings();
		}

#if CANNOT_USE_LOG4NET_CAUSE_IT_USES_SYSTEM_DOT_WEB
		protected override ILoggerFacade CreateLogger()
		{
			return new Log4NetLoggerFacade();
		}
#endif

		protected override ILoggerFacade CreateLogger()
		{
			return new ListViewLogger();
		}

		protected override DependencyObject CreateShell()
		{
			this.Container.RegisterType<IShellView, MainConsoleWindow>();
			IShellView shell = this.Container.Resolve<IShellView>();
			if (shell != null)
				shell.Show();
			this.ShellView = shell;
			return shell as DependencyObject;
		}

		protected override void InitializeShell()
		{
			MainConsoleWindow mainWindow = this.ShellView as MainConsoleWindow;
			
			Application.Current.MainWindow = mainWindow;
			if (mainWindow != null)
			{
				ListViewLogger logger = this.Container.Resolve<ILoggerFacade>() as ListViewLogger;
				if (logger != null)
					logger.Listview = mainWindow.lvFIXLog.ListView;
			}

			// All of the Prism managers have been initialized, but the modules have not been loaded yet.
			// So, create the Equity Container and stick it in a tab
//			this.Container.RegisterType(typeof(EquityContainerView));
//			EquityContainerView view = this.Container.Resolve<EquityContainerView>();
		}

		public override void Run(bool runWithDefaultConfiguration)
		{
			base.Run(runWithDefaultConfiguration);

			object vm;

			vm = this.Container.Resolve<IFXQuoteBlotterViewModel>();
			if (vm != null)
			{
				this.ModuleWasLoaded(vm);
			}

			vm = this.Container.Resolve<IQuoteBlotterViewModel>();
			if (vm != null)
			{
				this.ModuleWasLoaded(vm);

#if NOTYET
				EquityContainerView equityContainerView = this.Container.Resolve<EquityContainerView>();
				if (equityContainerView != null)
				{
					equityContainerView.DataContext = vm;
					IRegionManager regionManager = this.Container.Resolve<IRegionManager>();
					IRegion tabbedRegion = regionManager.Regions["TabbedContentRegion"];
					tabbedRegion.Add(equityContainerView, "EquityQuoteBlotterRegion");
				}
#endif
			}

			vm = this.Container.Resolve<ILevel2BookViewModel>();
			if (vm != null)
			{
				this.ModuleWasLoaded(vm);
			}

			vm = this.Container.Resolve<IOrderBlotterViewModel>();
			if (vm != null)
			{
				this.ModuleWasLoaded(vm);
			}
		}

	}

	class MyModuleManager : ModuleManager
	{
		public MyModuleManager(IModuleInitializer moduleInitializer, IModuleCatalog moduleCatalog, ILoggerFacade loggerFacade)
			: base(moduleInitializer, moduleCatalog, loggerFacade)
		{
			this.LoadModuleCompleted += this.OnLoadModuleCompleted;
		}

		void OnLoadModuleCompleted(object sender, LoadModuleCompletedEventArgs e)
		{
			Console.WriteLine("Module Loaded - " + e.ModuleInfo.ModuleName);
			if (e.Error == null && e.ModuleInfo.State == ModuleState.Initialized)
			{
			}
		}
		
		protected override void HandleModuleTypeLoadingError(ModuleInfo moduleInfo, Exception exception)
		{
			MessageBox.Show(string.Format(exception.Message));
			base.HandleModuleTypeLoadingError(moduleInfo, exception);
		}
	}

	class ToolbarAdapter : RegionAdapterBase<MainToolbarView>
	{
		public ToolbarAdapter(IRegionBehaviorFactory regionBehaviorFactory) : base(regionBehaviorFactory)
		{
		}

		protected override void Adapt(IRegion region, MainToolbarView regionTarget)
		{
		}

		protected override IRegion CreateRegion()
		{
			return new SingleActiveRegion();
		}
	}
}
