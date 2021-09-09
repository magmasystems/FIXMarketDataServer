namespace FIXMarketDataServer
{
	public class FXQuote : Quote
	{
		public FXQuote()
		{
			this.SymbolPair = new string[2];
		}
		
		public string[] SymbolPair { get; set; }
	
		// We override the Symbol property so that we can break apart the symbol into the pair
		public override string Symbol
		{
			get
			{
				return base.Symbol;
			}
			set
			{
				base.Symbol = value;
				this.SymbolPair[0] = value.Substring(0, 3);
				this.SymbolPair[1] = value.Substring(3, 3);
			}
		}
	}
}
