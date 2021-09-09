using MagmaTrader.Interfaces;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace FIXMarketDataServer.FIXExchangeSimulatorModule
{
	[Module(ModuleName = "FIXExchangeSimulatorModule", OnDemand = false)]
	public class FIXExchangeSimulatorModule : IModule
	{
		public string Name { get; set; }
		public IFIXExchangeSimulatorClient FIXClient { get; set; }
		static public IUnityContainer Container { get; private set; }

		// ReSharper disable UnusedParameter.Local
		public FIXExchangeSimulatorModule(IUnityContainer container, IRegionManager regionManager)
		{
			this.Name = "FIX Exchange Simulator Module";
			Container = container;
		}
		// ReSharper restore UnusedParameter.Local

		public void Initialize()
		{
			// We want a singleton of the FIX Client across all modules
			ContainerControlledLifetimeManager lifetimeManager = new ContainerControlledLifetimeManager();
			Container.RegisterType<IFIXExchangeSimulatorClient, FIXExchangeSimulatorClient>(lifetimeManager);

			// Create the singleton FIX Client and hold on to the instance.
			// Note that if we wanted multiple FIX clients, then we can't do this.
			this.FIXClient = Container.Resolve<IFIXExchangeSimulatorClient>();
			Container.RegisterInstance(this.FIXClient);
		}
	}
}

