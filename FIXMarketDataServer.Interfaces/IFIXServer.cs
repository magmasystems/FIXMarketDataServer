using FIXMarketDataServer;

namespace MagmaTrader.Interfaces
{
	public interface IFIXServer
	{
		void Start();
		void Stop();
		bool IsStarted { get; }

		void Publish(Quote quote);
		void SendMessage(object message);
	}
}
