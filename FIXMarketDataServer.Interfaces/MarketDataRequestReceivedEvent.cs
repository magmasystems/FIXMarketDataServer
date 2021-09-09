using System;
using MagmaTrader.Data;
using Microsoft.Practices.Prism.Events;

namespace MagmaTrader.Interfaces
{
	public class MarketDataRequestReceivedEventArgs : EventArgs
	{
		public string Symbol;
		public MarketDataRequestAction Action;
		public object UserData;

		public MarketDataRequestReceivedEventArgs(string symbol, MarketDataRequestAction action, object userData = null)
		{
			this.Symbol = symbol;
			this.Action = action;
			this.UserData = userData;
		}
	}

	public class MarketDataRequestReceivedEvent : CompositePresentationEvent<MarketDataRequestReceivedEventArgs>
	{
	}
}
