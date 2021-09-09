using System;
using Microsoft.Practices.Prism.Events;

namespace MagmaTrader.Interfaces
{
	public enum Level2QuoteServiceAction
	{
		Subscribe,
		Unsubscribe,
	}
	
	public class Level2QuoteServiceControlEventArgs : EventArgs
	{
		public Level2QuoteServiceAction Action;

		public Level2QuoteServiceControlEventArgs(Level2QuoteServiceAction action)
		{
			this.Action = action;
		}
	}

	public class Level2QuoteServiceControlEvent : CompositePresentationEvent<Level2QuoteServiceControlEventArgs>
	{
	}
}
