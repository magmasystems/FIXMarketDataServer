using System;
using System.Globalization;
using System.Windows.Controls;
using FIXMarketDataServer;

namespace FIXMarketDataClient
{
	public class EquityOrderTicketQuantityValidationRule : ValidationRule
	{
		#region Overrides of ValidationRule
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			int quantity = Convert.ToInt32(value);

			if (quantity <= 0)
				return new ValidationResult(false, "Quantity must be greater than 0");

			if (quantity % 10 != 0)
				return new ValidationResult(false, "Quantity must be a mulitple of 10");

			return ValidationResult.ValidResult;
		}
		#endregion
	}

	public class EquityOrderTicketPriceValidationRule : ValidationRule
	{
		public double Bid { get; set; }
		public double Ask { get; set; }
	
		#region Overrides of ValidationRule
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			double price = Convert.ToDouble(value);

			if (price <= 0)
				return new ValidationResult(false, "Price must be greater than 0");

			if (price >= 1000)
				return new ValidationResult(false, "The price must be less that $1000");

			// We probably want to make sure that the price is within a certain percentage of the market bid and ask

			return ValidationResult.ValidResult;
		}
		#endregion
	}
}
