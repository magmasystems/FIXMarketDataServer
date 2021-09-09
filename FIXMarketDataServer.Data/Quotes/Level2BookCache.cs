using System.Collections.Generic;
using MagmaTrader.Data;

namespace FIXMarketDataServer
{
	public class Level2BookCache
	{
		public Dictionary<string, Level2Book> Cache { get; set; }

		public Level2BookCache()
		{
			this.Cache = new Dictionary<string, Level2Book>();
		}

		public Level2BookCache(IEnumerable<Level2Book> books) : this()
		{
			foreach (var book in books)
			{
				this.Cache.Add(book.Symbol, book);
			}
		}

		public Level2Book this[string symbol]
		{
			get { return this.Cache.ContainsKey(symbol) ? this.Cache[symbol] : null; }
		}

		public void Add(Level2Book book)
		{
			this.Cache.Add(book.Symbol, book);
		}

		public void Add(List<Level2Book> books)
		{
			foreach (var book in books)
			{
				this.Cache.Add(book.Symbol, book);
			}
		}

		public void Process(Level2Book book)
		{
			Level2Book oldBook;
			if (!this.Cache.TryGetValue(book.Symbol, out oldBook))
			{
				this.Add(book);
				return;
			}

			oldBook.Copy(book);
			//this.Cache[book.Symbol] = book;
		}
	}
}
