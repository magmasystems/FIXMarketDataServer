using System;
using Microsoft.Practices.Prism.Events;

namespace MagmaTrader.Interfaces
{
	public enum FIXExchangeSimulatorAction
	{
		Start,
		Stop,
		Pause,
		LoggedIn,
		LoggedOut,
	}
	
	public class FIXExchangeSimulatorControlEventArgs : EventArgs
	{
		public IFIXExchangeSimulatorClient Client;
		public FIXExchangeSimulatorAction  Action;

		public FIXExchangeSimulatorControlEventArgs(IFIXExchangeSimulatorClient sim, FIXExchangeSimulatorAction action)
		{
			this.Client = sim;
			this.Action = action;
		}
	}

	public class FIXExchangeSimulatorControlEvent : CompositePresentationEvent<FIXExchangeSimulatorControlEventArgs>
	{
	}
}
