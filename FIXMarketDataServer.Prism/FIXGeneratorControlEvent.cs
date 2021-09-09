using Microsoft.Practices.Prism.Events;

namespace FIXMarketDataServer.Prism
{
	public enum FIXGeneratorAction
	{
		Start,
		Stop,
		Pause,
	}
	
	public class FIXGeneratorControlEvent : CompositePresentationEvent<FIXGeneratorAction>
	{
	}
}
