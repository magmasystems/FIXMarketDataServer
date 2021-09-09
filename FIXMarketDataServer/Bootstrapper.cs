using System;
using System.Collections.Generic;
using System.Windows;
using MagmaTrader.Interfaces;
using MagmaTrader.Presentation;
using MagmaTrader.Prism;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

namespace FIXMarketDataServer
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
			ModuleCatalog catalog = new MultipleDirectoryModuleCatalog(new List<string> { @"..\..\..\ServerModules" });
			return catalog;
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
					logger.Listview = mainWindow.FIXLogger.ListView;
			}

			// All of the Prism managers have been initialized, but the modules have not been loaded yet.
			// So, create the Equity Container and stick it in a tab
			//			this.Container.RegisterType(typeof(EquityContainerView));
			//			EquityContainerView view = this.Container.Resolve<EquityContainerView>();
		}

		public override void Run(bool runWithDefaultConfiguration)
		{
			base.Run(runWithDefaultConfiguration);

			object vm = this.Container.Resolve<IQuoteCacheViewModel>();
			if (vm != null)
			{
				this.ModuleWasLoaded(vm);
			}

			vm = this.Container.Resolve<ILevel2BookViewModel>();
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

}
