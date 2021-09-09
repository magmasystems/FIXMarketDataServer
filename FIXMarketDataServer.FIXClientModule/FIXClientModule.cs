using MagmaTrader.Interfaces;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace FIXMarketDataClient.FIXClientModule
{
	[Module(ModuleName = "FIXClientModule", OnDemand = false)]
	public class FIXClientModule : IModule
	{
		public string Name { get; set; }
		public IFIXClient FIXClient { get; set; }
		private readonly IUnityContainer m_container;

		// ReSharper disable UnusedParameter.Local
		public FIXClientModule(IUnityContainer container, IRegionManager regionManager)
		{
			this.Name = "FIX Client Module";
			this.m_container = container;
		}
		// ReSharper restore UnusedParameter.Local

		public void Initialize()
		{
			this.Name = "FIX Client Module";

			// We want a singleton of the FIX Client across all modules
			ContainerControlledLifetimeManager lifetimeManager = new ContainerControlledLifetimeManager();
			this.m_container.RegisterType<IFIXClient, FIXClient>(lifetimeManager);

			// Create the singleton FIX Client and hold on to the instance.
			// Note that if we wanted multiple FIX clients, then we can't do this.
			this.FIXClient = this.m_container.Resolve<IFIXClient>();
			this.m_container.RegisterInstance(this.FIXClient);
		}
	}
}

