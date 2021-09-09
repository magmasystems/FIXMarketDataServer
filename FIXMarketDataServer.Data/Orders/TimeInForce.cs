namespace MagmaTrader.Data
{
	// http://www.ibkr.us/en/software/tws/usersguidebook/ordertypes/time_in_force_for_orders.htm
	public enum TimeInForce
	{
		Undefined = -1,
		Day,  // Day
		GTC,  // Good til Canceled
		FOK,  // fill or kill
		IOC,  // Immediate or Cancel
		OPG,  // LOO or MOO or LOC or MOC (limit-on-open, market-on-open, limit-on-close, market-on-close)
		CLOSE, // At the close
		GTD,  // Good til date
	}
}
