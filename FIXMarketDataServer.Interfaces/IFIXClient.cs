using System;
using FIXMarketDataServer;
using MagmaTrader.Data;

namespace MagmaTrader.Interfaces
{
	public interface IFIXClient
	{
		void Start();
		void Stop();
		bool IsStarted { get; }

		event Action<Quote>     QuoteReceived;
		event Action<Execution> ExecutionReceived;

		/// <summary>
		/// Sends a NewOrderSingle out to the FIX-based OMS
		/// </summary>
		void Publish(Order order);

		/// <summary>
		/// Sends a NewOrderSingle out to the FIX-based OMS
		/// </summary>
		string Cancel(Order order);

		/// <summary>
		/// Sends a RequestQuoteStream to the FIX-based OMS
		/// </summary>
		void RequestQuoteStream(Symbol symbol);
	}
}
