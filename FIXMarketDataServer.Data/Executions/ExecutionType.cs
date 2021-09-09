namespace MagmaTrader.Data
{
	public enum ExecutionType
	{
		NEW = '0',
		PARTIAL_FILL = '1',
		FILL = '2',
		DONE = '3',
		CANCELLED = '4',
		REPLACED = '5',
		PENDING_CANCEL = '6',
		STOPPED = '7',
		REJECTED = '8',
		SUSPENDED = '9',

		PENDINGNEW = 'A',
		CALCULATED = 'B',
		EXPIRED = 'C',
		RESTATED = 'D',
		PENDINGREPLACE = 'E',
		TRADE = 'F',
		TRADECORRECT = 'G',
		TRADECANCEL = 'H',
		ORDERSTATUS = 'I',
		TRADE_IN_A_CLEARING_HOLD = 'J',
		TRADE_HAS_BEEN_RELEASED_TO_CLEARING = 'K',
		TRIGGERED_OR_ACTIVATED_BY_SYSTEM = 'L',
	}
}
