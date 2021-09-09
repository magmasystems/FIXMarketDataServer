using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MagmaTrader.Presentation.ValueConverters
{
	[ValueConversion(typeof(int), typeof(Brush))]
	public class DOBPriceLevelToBrushValueConverter : IValueConverter
	{
		private static readonly SolidColorBrush NoBrush = new SolidColorBrush(Colors.Transparent);
		private static readonly List<SolidColorBrush> PriceLevelBrushes = new List<SolidColorBrush>
		{
			new SolidColorBrush(Color.FromRgb(128, 128, 255)),
			new SolidColorBrush(Color.FromRgb(128, 160, 224)),
			new SolidColorBrush(Color.FromRgb(128, 192, 192)),
			new SolidColorBrush(Color.FromRgb(128, 224, 160)),
			new SolidColorBrush(Color.FromRgb(128, 255, 128)),
			new SolidColorBrush(Color.FromRgb(255, 128, 255)),
			new SolidColorBrush(Color.FromRgb(255, 160, 224)),
			new SolidColorBrush(Color.FromRgb(255, 192, 192)),
			new SolidColorBrush(Color.FromRgb(255, 224, 160)),
			new SolidColorBrush(Color.FromRgb(255, 255, 128)),
		};
	
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int priceLevel = (int) value;
			if (priceLevel < 0 || priceLevel >= PriceLevelBrushes.Count)
				return NoBrush;

			return PriceLevelBrushes[priceLevel];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
