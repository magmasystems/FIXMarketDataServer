using MagmaTrader.Interfaces;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace FIXMarketDataServer.FIXServerModule
{
	[Module(ModuleName = "FIXServerModule", OnDemand = false)]
	public class FixServerModule : IModule
	{
		public string Name { get; set; }
		public IFIXServer FIXServer { get; set; }
		private readonly IUnityContainer m_container;

		// ReSharper disable UnusedParameter.Local
		public FixServerModule(IUnityContainer container, IRegionManager regionManager)
		{
			this.Name = "FIX Server Module";
			this.m_container = container;
		}
		// ReSharper restore UnusedParameter.Local

		public void Initialize()
		{
			this.Name = "FIX Server Module";

			this.m_container.RegisterType<IFIXServer, FIXServer>();
			this.FIXServer = this.m_container.Resolve<IFIXServer>();
			this.m_container.RegisterInstance(this.FIXServer);
		}
	}
}

