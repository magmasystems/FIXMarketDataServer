namespace MagmaTrader.Data
{
	public enum OrderType
	{
		Undefined = -1,
		Limit,
		Market,
		Stop,
		StopLimit,
		LimitOnClose,
		MarketOnClose,
		PeggedToStock,
		PeggedToMarket,
		PeggedToMidpoint,
		TrailingStop,
		TrailingStopLimit,
		VWAP,
	}
}
