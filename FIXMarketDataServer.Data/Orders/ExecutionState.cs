namespace MagmaTrader.Data
{
	public enum ExecutionState
	{
		Undefined = 1,
		NotFilled,
		PartiallyFilled,
		Filled,
		Rejected,
		Stopped,
		Cancelled,
	}
}
