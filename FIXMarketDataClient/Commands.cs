﻿using System.Windows.Input;

namespace FIXMarketDataClient
{
	public class Commands
	{
		static public RoutedUICommand StartFIXCommand = new RoutedUICommand("Start FIX", "StartFIXCommand", typeof(MainToolbarView));
		static public RoutedUICommand StopFIXCommand  = new RoutedUICommand("Stop FIX",  "StopFIXCommand",  typeof(MainToolbarView));
	}
}
