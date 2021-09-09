using System;
using System.Collections.Generic;

namespace MagmaTrader.Interfaces
{
	public interface IFIXExchangeSimulatorClient
	{
		void Start();
		void Stop();
		bool IsStarted { get; }

		void Publish(object message);
		void RequestQuoteStream(List<Data.Symbol> symbols);

		event Action<object> ExecutionReportReceived;
		event Action<object> PassThruFixMessageReceived;
	}
}
