
using System;
// DISCUS system constants
namespace PSL.DISCUS.Impl
{
	/// <summary>
	/// DISCUS Constants (system wide constants)
	/// </summary>
	public abstract class DConst
	{
		// Contains system wide settings e.g location of
		// Dynamic Proxy Cache
		public static string DISCUSCONFIG_FILE = "DiscusConf.xml";
		public static string DEFAULT_PXY_DIR = "PxyCache";
		// Database configuration file, contains dbase connection
		// string etc.
		public static string DBASECONFIG_FILE = "DataConfig.xml"; 

		// Logging Constants
		public static string EVENT_LOGGING = "EventLog";
		public static string URL_LOGGING = "UrlLog";
		public static string WEB_SERVICE_LOGGING = "WebServiceLog";

		// Tracing Constants
		public static string EVENT_TRACING = "EventTrace";
		public static string URL_TRACING = "UrlTrace";
		public static string WEB_SERVICE_TRACING = "WebServiceTrace";

		// Logging/Tracing Message constants
		public static string ERROR_MSG_TYPE = "Error";
		public static string INFO_MSG_TYPE = "Info";
		public static string WARNING_MSG_TYPE = "Warning";

	}
}
