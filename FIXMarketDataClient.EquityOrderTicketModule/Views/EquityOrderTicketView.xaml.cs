using System.ComponentModel;
using System.Windows;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;

namespace FIXMarketDataClient.EquityOrderTicketModule.Views
{
	public partial class EquityOrderTicketView : IEquityOrderTicketView
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private EquityOrder m_order;
		public EquityOrder Order
		{
			get { return this.m_order; }
			set
			{
				this.m_order = value; 
				if (this.m_order != null)
					this.Title = string.Format("{0}", this.m_order.ClOrderID);
			}
		}
	
		public EquityOrderTicketView()
		{
			InitializeComponent();
		}

		public void SetViewModel(IEquityOrderTicketViewModel viewModel)
		{
			this.DataContext = viewModel;
			this.Order = viewModel.Order;
		}

		private void OnBuy(object sender, RoutedEventArgs e)
		{
			if (this.m_order != null)
			{
				this.m_order.Side = Side.Buy;
				this.m_order.LeavesQuantity = this.m_order.Quantity;
			}
			this.DialogResult = true;
		}

		private void OnSell(object sender, RoutedEventArgs e)
		{
			if (this.m_order != null)
			{
				this.m_order.Side = Side.Sell;
				this.m_order.LeavesQuantity = this.m_order.Quantity;
			}
			this.DialogResult = true;
		}
	}
}
