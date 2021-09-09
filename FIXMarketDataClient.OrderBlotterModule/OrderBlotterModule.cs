using MagmaTrader.Interfaces;
using FIXMarketDataClient.OrderBlotterModule.Models;
using FIXMarketDataClient.OrderBlotterModule.ViewModels;
using FIXMarketDataClient.OrderBlotterModule.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;


namespace FIXMarketDataClient.OrderBlotterModule
{
	[Module(ModuleName = "OrderBlotterModule", OnDemand = false)]
	public class OrderBlotterModule : IModule
	{
		public string Name { get; set; }
		private readonly IUnityContainer  m_container;
		private readonly IRegionManager   m_regionManager;
		private IEventAggregator          m_eventAggregator;

		public OrderBlotterModule(IUnityContainer container, IRegionManager regionManager)
		{
			this.Name = "Order Blotter Module";
			this.m_container = container;
			this.m_regionManager = regionManager;
		}

		public void Initialize()
		{
			this.m_container.RegisterType<IOrderBlotterModel,     OrderBlotterModel>();
			this.m_container.RegisterType<IOrderBlotterView,      OrderBlotterView>();
			this.m_container.RegisterType<IOrderBlotterViewModel, OrderBlotterViewModel>();

			IOrderBlotterViewModel presenter = this.m_container.Resolve<IOrderBlotterViewModel>();
			this.m_container.RegisterInstance(presenter);
			this.m_regionManager.AddToRegion("OrderBlotterRegion", presenter.View);

			this.m_eventAggregator = this.m_container.Resolve<IEventAggregator>();
		}
	}
}
