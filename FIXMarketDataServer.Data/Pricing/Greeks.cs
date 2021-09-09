namespace MagmaTrader.Data
{
	public class Greeks
	{
		// The Theoretical Value of the option - this is what we really want
		public double TV            { get; set; }
	
		// These can be outputs from the pricer, although the user can tweak these in the model
		public double Delta         { get; set; }
		public double Gamma         { get; set; }
		public double Theta         { get; set; }
		public double Vega          { get; set; }
		public double Rho           { get; set; }

		// These are usually inputs to the model
		public double? InterestRate { get; set; }
		public double? Dividend     { get; set; }
		public double? Borrow       { get; set; }
		public double? Volatility   { get; set; }
	}
}
