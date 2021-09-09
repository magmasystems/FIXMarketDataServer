using FIXMarketDataClient.EquityOrderTicketModule.Models;
using FIXMarketDataClient.EquityOrderTicketModule.ViewModels;
using FIXMarketDataClient.EquityOrderTicketModule.Views;
using FIXMarketDataClient.FIXClientModule;
using FIXMarketDataServer;
using MagmaTrader.Interfaces;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;


namespace FIXMarketDataClient.EquityOrderTickerModule
{
	[Module(ModuleName = "EquityOrderTicketModule", OnDemand = false)]
	[ModuleDependency("FIXClientModule")]
	public class Module : IModule
	{
		public string Name { get; set; }
		private readonly IUnityContainer  m_container;
		private IEventAggregator          m_eventAggregator;

		public Module(IUnityContainer container)
		{
			this.Name = "Equities Order Ticket Module";
			this.m_container = container;
		}

		public void Initialize()
		{
			this.m_container.RegisterType(typeof(Quote), "Quote");
			this.m_container.RegisterType<IEquityOrderTicketModel, EquityOrderTicketModel>();
			this.m_container.RegisterType<IEquityOrderTicketView, EquityOrderTicketView>();
			this.m_container.RegisterType<IEquityOrderTicketViewModel, EquityOrderTicketViewModel>();

			// We want a singleton of the FIX Client across all modules
//			ContainerControlledLifetimeManager lifetimeManager = new ContainerControlledLifetimeManager();
//			this.m_container.RegisterType<IFIXClient, FIXClient>(lifetimeManager);

			this.m_eventAggregator = this.m_container.Resolve<IEventAggregator>();
		}
	}
}
