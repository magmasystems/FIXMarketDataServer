using MagmaTrader.Interfaces;
using MagmaTrader.OrderManagerModule.Models;
using MagmaTrader.OrderManagerModule.Services;
using MagmaTrader.OrderManagerModule.ViewModels;
using MagmaTrader.OrderManagerModule.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;


namespace MagmaTrader.OrderManagerModule
{
	[Module(ModuleName = "OrderManagerModule", OnDemand = false)]
	[ModuleDependency("FIXServerModule")]
	[ModuleDependency("FIXExchangeSimulatorModule")]
	public class Module : IModule
	{
		public string Name { get; set; }
		private readonly IUnityContainer  m_container;
		private readonly IRegionManager   m_regionManager;
		private IEventAggregator          m_eventAggregator;

		public Module(IUnityContainer container, IRegionManager regionManager)
		{
			this.Name = "Order Manager Module";
			this.m_container = container;
			this.m_regionManager = regionManager;
		}

		public void Initialize()
		{
			this.m_container.RegisterType<IFIXServer, IFIXServer>();
			this.m_container.RegisterType<IFIXExchangeSimulatorClient, IFIXExchangeSimulatorClient>();

			this.m_container.RegisterType<IOrderManagerModel, OrderManagerModel>();
			this.m_container.RegisterType<IOrderManagerView, OrderManagerView>();
			this.m_container.RegisterType<IOrderManagerViewModel, OrderManagerViewModel>();
			this.m_container.RegisterType<IOrderManagerService, OrderManagerService>();

			IOrderManagerViewModel vm = this.m_container.Resolve<IOrderManagerViewModel>();
			this.m_container.RegisterInstance(vm);
#if NOTYET
			this.m_regionManager.AddToRegion("Main", vm.View);
#endif
			this.m_eventAggregator = this.m_container.Resolve<IEventAggregator>();
			this.m_eventAggregator.GetEvent<OrderMessageReceivedEvent>().Subscribe(vm.OnOrderMessageReceived);
			this.m_eventAggregator.GetEvent<MarketDataRequestReceivedEvent>().Subscribe(vm.OnMarketDataRequestReceived);
			this.m_eventAggregator.GetEvent<FIXGeneratorControlEvent>().Subscribe(vm.OnFIXGeneratorActionReceived);
		}
	}
}
