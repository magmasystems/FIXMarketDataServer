using MagmaTrader.Interfaces;
using FIXMarketDataServer.Level2BookModule.Models;
using FIXMarketDataServer.Level2BookModule.ViewModels;
using FIXMarketDataServer.Level2BookModule.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;


namespace FIXMarketDataServer.Level2BookModule
{
	[Module(ModuleName = "Level2BookModule", OnDemand = false)]
	public class Module : IModule
	{
		public string Name { get; set; }
		private readonly IUnityContainer  m_container;
		private readonly IRegionManager   m_regionManager;

		public Module(IUnityContainer container, IRegionManager regionManager)
		{
			this.Name = "Level 2 Book Module";
			this.m_container = container;
			this.m_regionManager = regionManager;
		}

		public void Initialize()
		{
			this.Name = "Level 2 Book Module";

			this.m_container.RegisterType<ILevel2BookModel, Level2BookModel>();
			this.m_container.RegisterType<ILevel2BookView, Level2BookView>();
			this.m_container.RegisterType<ILevel2BookViewModel, Level2BookViewModel>();

			ILevel2BookViewModel presenter = this.m_container.Resolve<ILevel2BookViewModel>();
			this.m_container.RegisterInstance(presenter);
			this.m_regionManager.AddToRegion("OrderBooks", presenter.View);
		}
	}
}
