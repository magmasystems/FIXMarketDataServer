using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MagmaTrader.Data;

namespace MagmaTrader.Presentation.ValueConverters
{
	[ValueConversion(typeof(PriceChange), typeof(Color))]
	public class PriceChangeToColorValueConverter : IValueConverter
	{
		static SolidColorBrush UpBrush       = new SolidColorBrush(Colors.LightGreen);
		static SolidColorBrush DownBrush     = new SolidColorBrush(Colors.LightPink);
		static SolidColorBrush NoChangeBrush = new SolidColorBrush(Colors.White);
	
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((PriceChange)value == PriceChange.UP)
				return UpBrush;
			if ((PriceChange) value == PriceChange.DOWN)
				return DownBrush;
			return NoChangeBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		static public Color UpColor
		{
			get { return UpBrush.Color;   }
			set { UpBrush = new SolidColorBrush(value); }
		}

		static public Color DownColor
		{
			get { return DownBrush.Color;     }
			set { DownBrush = new SolidColorBrush(value);   }
		}

		static public Color NoChangeColor
		{
			get { return NoChangeBrush.Color; }
			set { NoChangeBrush = new SolidColorBrush(value); }
		}
	}
}
