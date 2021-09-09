using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace MagmaTrader.Data
{
	[DataContract]
	public class Level2Quote
	{
		[DataMember]
		public double Price  { get; set; }

		[DataMember]
		public int Quantity  { get; set; }

		[DataMember]
		public string Broker { get; set; }

		[DataMember]
		public DateTime Time { get; set; }

		public Level2Quote()
		{
			this.Time = DateTime.Now;
		}
	
		public Level2Quote(double price, int quantity, string broker) :
			this(price, quantity, broker, DateTime.Now)
		{
		}

		public Level2Quote(double price, int quantity, string broker, DateTime dt)
		{
			this.Broker   = broker;
			this.Price    = price;
			this.Quantity = quantity;
			this.Time     = dt;
		}
	}

	/// <summary>
	/// Adds a few more fields to the raw Level2 Quote. These are things that are useful in building the book for visual display.
	/// </summary>
	[DataContract]
	public class Level2DisplayQuote : Level2Quote
	{
		/// <summary>
		/// Cumulative quantity
		/// </summary>
		[DataMember]
		public int CumQuantity { get; set; }

		/// <summary>
		/// 0-based pricing level - different color banding per level
		/// </summary>
		[DataMember]
		public int PriceLevel { get; set; }

		/// <summary>
		/// For aggregated mode - number of brokers at this price level
		/// </summary>
		[DataMember]
		public int NumBrokers { get; set; }

		public Level2DisplayQuote(double price, int quantity, string broker) :
			base(price, quantity, broker, DateTime.Now)
		{
			this.NumBrokers = 0;
		}

		public Level2DisplayQuote(double price, int quantity, string broker, DateTime dt) : 
			base(price, quantity, broker, dt)
		{
			this.NumBrokers = 0;
		}

		public Level2DisplayQuote(Level2Quote quote) : this(quote.Price, quote.Quantity, quote.Broker, quote.Time)
		{
		}
	}

	[DataContract]
	public class Level2QuoteSide : INotifyPropertyChanged
	{
		[DataMember]
		public Side Side { get; set; }
		
		/// <summary>
		/// This is the big decision. Do we use a preallocated array of quotes, or do we use a linked list?
		/// </summary>
		[DataMember]
		public ObservableCollection<Level2DisplayQuote> BookForOneSide { get; set; }

		protected Level2QuoteSide()
		{
			this.BookForOneSide = new ObservableCollection<Level2DisplayQuote>();
		}

		public Level2QuoteSide(Side side) : this()
		{
			this.Side = side;
		}

		public void Clear()
		{
			this.BookForOneSide.Clear();
			this.NotifyPropertyChanged("Book");
		}
	
		public void Copy(Level2QuoteSide bookSide)
		{
			this.BookForOneSide.Clear();
			foreach (Level2DisplayQuote quote in bookSide.BookForOneSide)
			{
				this.Insert(quote);				
			}
		}

		public void Insert(Level2DisplayQuote quote)
		{
			// We need to insert the quote in the proper place in the side of the book.
			// We may need a "time" field, so that older quotes are ahead in the book.

			// For finding the proper place to insert, we might want to do a binary search.

			int nQuotes = this.BookForOneSide.Count;
			int idx;

			for (idx = 0;  idx < nQuotes && this.BookForOneSide[idx].Price <= quote.Price;  idx++)
			{
			}

			if (idx == nQuotes)
				this.BookForOneSide.Add(quote);
			else
				this.BookForOneSide.Insert(idx, quote);

			// Now, fix up the book. We can start fixing from the new record onwards, but let's just do it
			// for the entire side to make it easier.
			this.RecomputePriceLevelsAndCumQuantity();

			this.NotifyPropertyChanged("Book");
		}

		public void RecomputePriceLevelsAndCumQuantity()
		{
			double lastPrice = 0;
			int priceLevel   = -1;
			int cumQuantity  = 0;
			int numBrokers   = 1;

			// Note - we need to lock somewheree. Try locking at the outer-most level


			for (int idx = 0;  idx < this.BookForOneSide.Count;  idx++)
			{
				Level2DisplayQuote quote = this.BookForOneSide[idx];
				if (quote.Price != lastPrice)
				{
					cumQuantity = quote.Quantity;
					priceLevel++;
					lastPrice = quote.Price;
					if (idx == 0)
					{
						quote.NumBrokers = 1;
					}
					else
					{
						quote.NumBrokers = numBrokers;
					}
				}
				else
				{
					cumQuantity += quote.Quantity;
					numBrokers++;
				}

				quote.CumQuantity = cumQuantity;
				quote.PriceLevel = priceLevel;
			}

			this.BookForOneSide[this.BookForOneSide.Count - 1].NumBrokers = numBrokers;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string prop)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}

	[DataContract]
	public class Level2Book
	{
		[DataMember]
		public string Symbol           { get; set; }

		[DataMember]
		public Level2QuoteSide BidBook { get; set; }

		[DataMember]
		public Level2QuoteSide AskBook { get; set; }

		public Level2Book(string symbol)
		{
			this.Symbol  = symbol;
			this.BidBook = new Level2QuoteSide(Side.Buy);
			this.AskBook = new Level2QuoteSide(Side.Sell);
		}

		public Level2QuoteSide this[Side side]
		{
			get { return (side == Side.Buy) ? this.BidBook : this.AskBook; }
		}

		public void Copy(Level2Book book)
		{
			this.Clear();
			this.BidBook.Copy(book.BidBook);
			this.AskBook.Copy(book.AskBook);
		}

		public void Insert(Side side, Level2Quote quote)
		{
			this.Insert(side, new Level2DisplayQuote(quote));
		}

		public void Insert(Side side, Level2DisplayQuote quote)
		{
			if (side == Side.Buy)
			{
				this.BidBook.Insert(quote);
			}
			else
			{
				this.AskBook.Insert(quote);
			}
		}

		public void Clear()
		{
			this.Clear(Side.Buy);
			this.Clear(Side.Sell);
		}

		public void Clear(Side side)
		{
			if (side == Side.Buy)
				this.BidBook.Clear();
			else
				this.AskBook.Clear();
		}
	}
}
