using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;

namespace MagmaTrader.Presentation
{
	public class LogMessage
	{
		public DateTime Time { get; set; }
		public string Message { get; set; }
	
		public LogMessage()
		{
			this.Message = string.Empty;
			this.Time = DateTime.Now;
		}

		public LogMessage(string s) : this()
		{
			this.Message = s;
		}
	}

	public class ListViewLogger : DependencyObject, ILoggerFacade
	{
#if NOTYET
		public static DependencyProperty IsStoppedProperty = DependencyProperty.Register("IsStopped", typeof (bool), typeof (ListViewLogger), new PropertyMetadata(false));
		public bool IsStopped
		{
			get { return (bool) GetValue(IsStoppedProperty); }
			set { SetValue(IsStoppedProperty, value); }
		}
#else
		public bool IsStopped
		{
			get; set;
		}
#endif
	
		public ListView Listview { get; set; }
		private static ListViewLogger s_logger;
		
		public ListViewLogger()
		{
			this.Listview = null;
			s_logger = this;
		}

		#region Implementation of ILoggerFacade
		public void Log(string message, Category category, Priority priority)
		{
			if (this.Listview == null)
				return;

			LogMessage logMessage = new LogMessage(message);

			if (this.Listview.CheckAccess())
				this.Listview.Items.Add(logMessage);
			else
				this.Listview.Dispatcher.Invoke((Action) (() => this.Listview.Items.Add(logMessage)) );
		}
		#endregion

		#region Methods to control the logging
		public void Start()
		{
			if (!this.IsStopped)
				return;

			this.IsStopped = false;
		}

		public void Stop()
		{
			if (this.IsStopped)
				return;

			this.IsStopped = true;
		}
		#endregion

		#region Logging Helpers
		static private bool InsureLogger()
		{
			if (s_logger == null)
			{
				ILoggerFacade baseLogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
				if (baseLogger == null)
					return false;

				ListViewLogger logger = baseLogger as ListViewLogger;
				if (logger == null)
					return false;

				s_logger = logger;
			}

			return true;
		}
	
		static public void Log(string message)
		{
			if (InsureLogger())
			{
				s_logger.Log(message, Category.Info, Priority.None);
			}
		}

		static public void Log(object caller, string message)
		{
			if (InsureLogger())
			{
				message = string.Format("{0}: {1}", caller.GetType().Name, message);
				s_logger.Log(message, Category.Info, Priority.None);
			}
		}
		#endregion
	}
}
