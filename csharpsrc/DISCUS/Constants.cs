
using System;
using PSL.DISCUS.Logging;

// DISCUS system constants
namespace PSL.DISCUS
{
	/// <summary>
	/// DISCUS Constants (system wide constants)
	/// </summary>
	public abstract class Constants
	{
		// Set System Defaults

		public const string DEFAULT_GATEKEEPER_NAME = "Gatekeeper";
		public const string DEFAULT_PROXY_CACHE = "PxyCache";
		public const string DEFAULT_DBASE_CONNECT = "DSN=DISCUS";
		public const bool DEFAULT_TRACE_ON = false;
		public const enuLoggingType DEFAULT_LOGGING_TYPE = enuLoggingType.EventLog;
		public const enuTracingType DEFAULT_TRACING_TYPE = enuTracingType.None;
				
		// Logging Constants
		public const string EVENT_LOGGING = "EventLog";
		public const string URL_LOGGING = "UrlLog";
		public const string WEB_SERVICE_LOGGING = "WebServiceLog";

		// Tracing Constants
		public const string EVENT_TRACING = "EventTrace";
		public const string URL_TRACING = "UrlTrace";
		public const string WEB_SERVICE_TRACING = "WebServiceTrace";

		// Logging/Tracing Message constants
		public const string ERROR_MSG_TYPE = "Error";
		public const string INFO_MSG_TYPE = "Info";
		public const string WARNING_MSG_TYPE = "Warning";
	}
}
