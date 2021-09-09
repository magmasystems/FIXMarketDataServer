using MagmaTrader.Interfaces;
using FIXMarketDataServer.QuoteGeneratorModule.Models;
using FIXMarketDataServer.QuoteGeneratorModule.ViewModels;
using FIXMarketDataServer.QuoteGeneratorModule.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;


namespace FIXMarketDataServer.QuoteGeneratorModule
{
	[Module(ModuleName = "QuoteGeneratorModule", OnDemand = false)]
	[ModuleDependency("FIXServerModule")]
	[ModuleDependency("FIXExchangeSimulatorModule")]
	public class QuoteGeneratorModule : IModule
	{
		public string Name { get; set; }
		private readonly IUnityContainer  m_container;
		private readonly IRegionManager   m_regionManager;
		private IEventAggregator          m_eventAggregator;

		public QuoteGeneratorModule(IUnityContainer container, IRegionManager regionManager)
		{
			this.Name = "Quote Generator Module";
			this.m_container = container;
			this.m_regionManager = regionManager;
		}

		public void Initialize()
		{
			this.Name = "Quote Generator Module";

			this.m_container.RegisterType<IFIXServer, IFIXServer>();
			this.m_container.RegisterType<IFIXExchangeSimulatorClient, IFIXExchangeSimulatorClient>();

			this.m_container.RegisterType<IQuoteCacheModel, QuoteCacheModel>();
			this.m_container.RegisterType<IQuoteCacheView, QuoteCacheView>();
			this.m_container.RegisterType<IQuoteCacheViewModel, QuoteCacheViewModel>();

			IQuoteCacheViewModel vm = this.m_container.Resolve<IQuoteCacheViewModel>();
			this.m_container.RegisterInstance(vm);
			this.m_regionManager.AddToRegion("Main", vm.View);

			this.m_eventAggregator = this.m_container.Resolve<IEventAggregator>();
			this.m_eventAggregator.GetEvent<FIXGeneratorControlEvent>().Subscribe(vm.OnFIXGeneratorActionReceived);
			this.m_eventAggregator.GetEvent<MarketDataRequestReceivedEvent>().Subscribe(vm.OnMarketDataRequestReceived);
		}
	}
}
