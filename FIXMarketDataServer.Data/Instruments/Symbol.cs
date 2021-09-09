namespace MagmaTrader.Data
{
	public enum SymbolType
	{
		Undefined = -1,
		Ticker,
		RIC,
		CUSIP,
		Bloomberg,
		Custom,
	}
	
	public class Symbol
	{
		public string Name { get; set; }
		public SymbolType SymbolType { get; set; }

		public Symbol(string name) : this(name, SymbolType.Ticker)
		{
		}

		public Symbol(string name, SymbolType type)
		{
			this.Name = name;
			this.SymbolType = type;
		}
	}
}
