using System.Collections.Generic;
using FIXMarketDataServer;
using MagmaTrader.Data;

namespace MagmaTrader.Interfaces
{
	public interface ILevel2BookModel
	{
		Level2BookCache Level2BookCache { get; set; }
		Level2Book this[string symbol] { get; }
		Level2Book Current { get; set; }
		void Add(Level2Book book);
		void Add(List<Level2Book> books);
	}
}
