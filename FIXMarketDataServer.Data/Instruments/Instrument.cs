namespace MagmaTrader.Data
{
	abstract public class Instrument
	{
		public Symbol Symbol { get; set; }
		public FinancialInstrumentType FinancialInstrumentType { get; set; }
	}
}
