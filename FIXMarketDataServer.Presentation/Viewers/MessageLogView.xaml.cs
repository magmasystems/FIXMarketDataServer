using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;

namespace MagmaTrader.Presentation
{
	public class Commands
	{
		static public RoutedUICommand CopyRowCommandParam2 = new RoutedUICommand("RoutedCmd2", "CopyRowCommandParam2", typeof(MessageLogView));
	}

	public partial class MessageLogView
	{
		public bool IsLoggerStarted { get; set; }

		public DelegateCommand ToggleLoggerCommand { get; private set; }
		public DelegateCommand CopyRowCommand { get; private set; }
		public DelegateCommand<object> CopyRowCommandParam { get; set; }

		public ListView ListView { get { return this.Listview; } }

		public MessageLogView()
		{
			InitializeComponent();

			this.IsLoggerStarted = true;
			ToggleLoggerCommand = new DelegateCommand(this.ToggleLoggerStartStop);
			CopyRowCommand = new DelegateCommand(this.CopyRow, this.IsRowSelected);
			CopyRowCommandParam = new DelegateCommand<object>(this.CopyRowParam, this.IsRowSelectedParam);
		}

		public void ToggleLoggerStartStop()
		{
			ListViewLogger logger = ServiceLocator.Current.GetInstance<ILoggerFacade>() as ListViewLogger;
			if (logger == null)
				return;

			if (this.IsLoggerStarted)
			{
				logger.Stop();
				this.miToggleLogger.Header = "Start";
			}
			else
			{
				logger.Start();
				this.miToggleLogger.Header = "Stop";
			}

			this.IsLoggerStarted = !this.IsLoggerStarted;
		}

		public void CopyRow()
		{
			int idx = this.Listview.SelectedIndex;
			if (idx < 0)
				return;
	
			LogMessage logMessage = this.Listview.Items[idx] as LogMessage;
			if (logMessage == null)
				return;

			Clipboard.Clear();
			Clipboard.SetText(logMessage.Message, TextDataFormat.Text);
		}

		public bool IsRowSelected()
		{
			int idx = this.Listview.SelectedIndex;
			return idx >= 0;
		}

		public void CopyRowParam(object obj)
		{
			if (obj == null)
				return;

			LogMessage logMessage = obj as LogMessage;
			if (logMessage == null)
				return;

			Clipboard.Clear();
			Clipboard.SetText(logMessage.Message, TextDataFormat.Text);
		}

		public bool IsRowSelectedParam(object obj)
		{
			return (obj != null);
		}

		private void CopyRowCommandParam2_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (e.Parameter != null);
		}

		private void CopyRowCommandParam2_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter == null)
				return;

			LogMessage logMessage = e.Parameter as LogMessage;
			if (logMessage == null)
				return;

			Clipboard.Clear();
			Clipboard.SetText(logMessage.Message, TextDataFormat.Text);
		}
	
	}
}
