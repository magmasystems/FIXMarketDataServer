using System.Windows;

namespace FIXMarketDataServer
{
	public partial class App
	{
		static public Bootstrapper Bootstrapper { get; private set; }
	
		protected override void OnStartup(StartupEventArgs e)
		{
			Bootstrapper = new Bootstrapper();
			Bootstrapper.Run();
			
			base.OnStartup(e);
		}
	}
}
