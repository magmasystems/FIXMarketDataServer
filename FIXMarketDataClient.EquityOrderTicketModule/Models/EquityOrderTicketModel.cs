using System.ComponentModel;
using FIXMarketDataServer;
using MagmaTrader.Data;
using MagmaTrader.Interfaces;

namespace FIXMarketDataClient.EquityOrderTicketModule.Models
{
	public class EquityOrderTicketModel : IEquityOrderTicketModel
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public string Name
		{
			get { return this.Order.ClOrderID; }
		}

		private EquityOrder m_order;
		public EquityOrder Order
		{
			get
			{
				return this.m_order;
			}
			set
			{
				this.m_order = value;
				this.NotifyPropertyChanged("Order");
			}
		}

		public Quote Quote
		{
			set
			{
				this.Order = new EquityOrder
				{
					Symbol = new Symbol(value.Symbol),
					Quantity = 0,
					Price = (value.Bid + value.Ask) / 2,
					Side = Side.Undefined,
					TIF = TimeInForce.Day,
				};
			}
		}

		public EquityOrderTicketModel(Quote quote, Order order)
		{
			if (quote != null && !string.IsNullOrEmpty(quote.Symbol))
				this.Quote = quote;
			if (order != null && order.Symbol != null)
				this.Order = order as EquityOrder;
		}

		private void NotifyPropertyChanged(string prop)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}
	}
}
