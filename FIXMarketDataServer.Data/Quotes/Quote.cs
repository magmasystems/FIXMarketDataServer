using System.ComponentModel;
using MagmaTrader.Data;

namespace FIXMarketDataServer
{
	public class Quote : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Quote()
		{
			this.QuoteID = (++m_nextID).ToString();
		}
	
		public void Copy(Quote q)
		{
			this.Bid = q.Bid;
			this.Ask = q.Ask;
			this.BidSize = q.BidSize;
			this.AskSize = q.AskSize;
		}

		protected void NotifyPropertyChanged(string prop)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}

		public static int m_nextID;


		public string QuoteID { get; set; }
		
		protected string m_symbol;
		public virtual string Symbol
		{
			get { return this.m_symbol; }
			set
			{
				this.m_symbol = value;
				this.NotifyPropertyChanged("Symbol");
			}
		}

		protected double m_bid;
		public double Bid
		{
			get { return this.m_bid; }
			set
			{
				this.PrevBid = this.m_bid;
				if (this.m_bid == value)
					return;
				this.m_bid = value;
				this.NotifyPropertyChanged("Bid");
				this.NotifyPropertyChanged("BidChange");
			}
		}

		public double PrevBid { get; set; }

		public PriceChange BidChange
		{
			get
			{
				double diff = this.m_bid - this.PrevBid;
				if (diff > 0)
					return PriceChange.UP;
				if (diff < 0)
					return PriceChange.DOWN;
				return PriceChange.NOCHANGE;
			}
		}

		protected double m_ask;
		public double Ask
		{
			get { return this.m_ask; }
			set
			{
				this.PrevAsk = this.m_ask;
				if (this.m_ask == value)
					return;
				this.m_ask = value;
				this.NotifyPropertyChanged("Ask");
				this.NotifyPropertyChanged("AskChange");
			}
		}

		public double PrevAsk { get; set; }

		public PriceChange AskChange
		{
			get
			{
				double diff = this.m_ask - this.PrevAsk;
				if (diff > 0)
					return PriceChange.UP;
				if (diff < 0)
					return PriceChange.DOWN;
				return PriceChange.NOCHANGE;
			}
		}

		protected int m_bidSize;
		public int BidSize
		{
			get { return this.m_bidSize; }
			set
			{
				if (this.m_bidSize == value)
					return;
				this.m_bidSize = value;
				this.NotifyPropertyChanged("BidSize");
			}
		}

		protected int m_askSize;
		public int AskSize
		{
			get { return this.m_askSize; }
			set
			{
				if (this.m_askSize == value)
					return;
				this.m_askSize = value;
				this.NotifyPropertyChanged("AskSize");
			}
		}

		// Daily High and Low
		protected double m_low;
		public double Low
		{
			get { return this.m_low; }
			set
			{
				if (this.m_low == value)
					return;
				this.m_low = value;
				this.NotifyPropertyChanged("Low");
			}
		}

		protected double m_high;
		public double High
		{
			get { return this.m_high; }
			set
			{
				if (this.m_high == value)
					return;
				this.m_high = value;
				this.NotifyPropertyChanged("High");
			}
		}
	}
}
