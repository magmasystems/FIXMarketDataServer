namespace MagmaTrader.Data
{
	public class EquityOptionInstrument : Instrument
	{
		public OptionStyle OptionStyle { get; set; }
		public PutCall PutCall         { get; set; }
	
		public EquityOptionInstrument()
		{
			this.FinancialInstrumentType = FinancialInstrumentType.EquityOption;
			this.PutCall = PutCall.Undefined;
			this.OptionStyle = OptionStyle.Undefined;
		}
	}
}
