using System;
using Microsoft.Practices.Prism.Events;

namespace MagmaTrader.Interfaces
{
	public enum FIXGeneratorAction
	{
		Start,
		Stop,
		Pause,
	}
	
	public class FIXGeneratorControlEventArgs : EventArgs
	{
		public IFIXServer FIXServer;
		public FIXGeneratorAction Action;

		public FIXGeneratorControlEventArgs(IFIXServer server, FIXGeneratorAction action)
		{
			this.FIXServer = server;
			this.Action = action;
		}
	}

	public class FIXClientControlEventArgs : EventArgs
	{
		public IFIXClient FIXClient;
		public FIXGeneratorAction Action;

		public FIXClientControlEventArgs(IFIXClient client, FIXGeneratorAction action)
		{
			this.FIXClient = client;
			this.Action = action;
		}
	}

	
	public class FIXGeneratorControlEvent : CompositePresentationEvent<FIXGeneratorControlEventArgs>
	{
	}

	public class FIXClientControlEvent : CompositePresentationEvent<FIXClientControlEventArgs>
	{
	}
}
