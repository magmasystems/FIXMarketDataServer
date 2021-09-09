namespace MagmaTrader.Data
{
	public enum OrderState
	{
		Undefined = -1,
		PendingNew,
		New,
		PartiallyFilled,
		Filled,
		DoneForDay,
		PendingCancel,
		Cancelled,
		Rejected,
		Stopped,
	}
}
