namespace MagmaTrader.Data
{
	// http://www.ibkr.us/en/software/tws/twsguide.htm#usersguidebook/ordertypes/order_attributes.htm
	public enum OrderAttributes
	{
		Undefined = -1,
	    Hidden,
	    Discretionary,
	    Iceberg,
		AllOrNone,
		MinimumQuantity,
		Block,
		SweepToFill,
		OneCancelsAll,
	}
}
