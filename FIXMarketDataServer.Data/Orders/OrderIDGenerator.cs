using System;

namespace MagmaTrader.Data
{
	static public class OrderIDGenerator
	{
		static public string Generate()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
