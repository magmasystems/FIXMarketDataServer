using System.Windows;
using FIXMarketDataServer;
using FIXMarketDataServer.Data;

namespace FIXMarketDataClient
{
	public partial class EquityOrderTicket
	{
		public EquityOrder Order { get; set; }
		public Side Side { get; set; }
		public int Quantity { get; set; }

		public EquityOrderTicket(Quote quote)
		{
			this.Order = new EquityOrder
			             	{
			             		Symbol = new Symbol(quote.Symbol),
			             		Quantity = 0,
			             		Price = (quote.Bid + quote.Ask)/2,
			             		Side = Side.Undefined,
								TIF = TimeInForce.Day,
			             	};
			
			this.DataContext = this.Order;
		
			InitializeComponent();
		}

		private void OnBuy(object sender, RoutedEventArgs e)
		{
			this.Side = Side.Buy;
			this.DialogResult = true;
		}

		private void OnSell(object sender, RoutedEventArgs e)
		{
			this.Side = Side.Sell;
			this.DialogResult = true;
		}
	}
}
