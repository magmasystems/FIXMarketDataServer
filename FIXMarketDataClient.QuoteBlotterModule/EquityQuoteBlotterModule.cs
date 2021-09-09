using FIXMarketDataClient.FIXClientModule;
using MagmaTrader.Interfaces;
using FIXMarketDataClient.EquityQuoteBlotterModule.Models;
using FIXMarketDataClient.EquityQuoteBlotterModule.ViewModels;
using FIXMarketDataClient.EquityQuoteBlotterModule.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;


namespace FIXMarketDataClient.EquityQuoteBlotterModule
{
	[Module(ModuleName = "QuoteBlotterModule", OnDemand = false)]
	[ModuleDependency("FIXClientModule")]
	public class EquityQuoteBlotterModule : IModule
	{
		public string Name { get; set; }
		private readonly IUnityContainer  m_container;
		private readonly IRegionManager   m_regionManager;
		private IEventAggregator          m_eventAggregator;

		public EquityQuoteBlotterModule(IUnityContainer container, IRegionManager regionManager)
		{
			this.Name = "Equities Quote Blotter Module";
			this.m_container = container;
			this.m_regionManager = regionManager;
		}

		public void Initialize()
		{
			this.Name = "Equities Quote Blotter Module";

			this.m_container.RegisterType<IQuoteCacheModel, QuoteCacheModel>();
			this.m_container.RegisterType<IQuoteCacheView, QuoteBlotterView>();
			this.m_container.RegisterType<IQuoteBlotterViewModel, QuoteBlotterViewModel>();

			// We want a singleton of the FIX Client across all modules
//			ContainerControlledLifetimeManager lifetimeManager = new ContainerControlledLifetimeManager();
//			this.m_container.RegisterType<IFIXClient, FIXClient>(lifetimeManager);

			IQuoteBlotterViewModel presenter = this.m_container.Resolve<IQuoteBlotterViewModel>();
			this.m_container.RegisterInstance(presenter);
			this.m_regionManager.AddToRegion("EquityQuoteBlotterRegion", presenter.View);

			this.m_eventAggregator = this.m_container.Resolve<IEventAggregator>();
			this.m_eventAggregator.GetEvent<FIXClientControlEvent>().Subscribe(presenter.OnFIXClientActionReceived);
		}
	}
}
