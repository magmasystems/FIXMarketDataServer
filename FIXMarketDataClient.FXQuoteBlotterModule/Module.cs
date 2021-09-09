using MagmaTrader.Interfaces;
using FIXMarketDataClient.FXQuoteBlotterModule.Models;
using FIXMarketDataClient.FXQuoteBlotterModule.ViewModels;
using FIXMarketDataClient.FXQuoteBlotterModule.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;


namespace FIXMarketDataClient.FXQuoteBlotterModule
{
	[Module(ModuleName = "FXQuoteBlotterModule", OnDemand = false)]
	[ModuleDependency("FIXClientModule")]
	public class Module : IModule
	{
		public string Name { get; set; }
		private readonly IUnityContainer  m_container;
		private readonly IRegionManager   m_regionManager;
		private IEventAggregator          m_eventAggregator;

		public Module(IUnityContainer container, IRegionManager regionManager)
		{
			this.Name = "FX Quote Blotter Module";
			this.m_container = container;
			this.m_regionManager = regionManager;
		}

		public void Initialize()
		{
			this.Name = "FX Quote Blotter Module";

			this.m_container.RegisterType<IQuoteCacheModel, FXQuoteCacheModel>();
			this.m_container.RegisterType<IQuoteCacheView, FXQuoteBlotterView>();
			this.m_container.RegisterType<IFXQuoteBlotterViewModel, FXQuoteBlotterViewModel>();
			this.m_container.RegisterType<IFIXClient, IFIXClient>();

			IRegion tabbedRegion = this.m_regionManager.Regions["TabbedContentRegion"];

			IFXQuoteBlotterViewModel fxPresenter = this.m_container.Resolve<IFXQuoteBlotterViewModel>();
			this.m_container.RegisterInstance(fxPresenter);
			tabbedRegion.Add(fxPresenter.View, "FX");

//			this.m_eventAggregator = this.m_container.Resolve<IEventAggregator>();
//			this.m_eventAggregator.GetEvent<FIXClientControlEvent>().Subscribe(fxPresenter.OnFIXClientActionReceived);
		}
	}
}
