using log4net;
using Microsoft.Practices.Prism.Logging;

namespace Magma.Prism
{
	public class Log4NetLoggerFacade : ILoggerFacade
	{
		#region Variables
		public static readonly ILog m_logger = LogManager.GetLogger(typeof(Log4NetLoggerFacade));
		#endregion

		#region Constructors
		public Log4NetLoggerFacade()
		{
			log4net.Config.XmlConfigurator.Configure();
		}
		#endregion

		#region Implementation of ILoggerFacade
		public void Log(string message, Category category, Priority priority)
		{
			if (m_logger == null)
				return;

			switch (category)
			{
				case Category.Debug:
					m_logger.Debug(message);
					break;
				case Category.Exception:
					m_logger.Error(message);
					break;
				case Category.Info:
					m_logger.Info(message);
					break;
				case Category.Warn:
					m_logger.Info(message);
					break;
			}
		}
		#endregion
	}
}
