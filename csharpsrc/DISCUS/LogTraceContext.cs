using System;

namespace PSL.DISCUS.Impl.Logging
{
	/// <summary>
	/// Summary description for LogTraceContext.
	/// </summary>
	public class LogTraceContext
	{
		// All contextual info needed by logging and tracing implementations
		// Event Logging info = none

		// UrlLogging info = hostname and port
		private string m_strHostname = "";
		public string Hostname
		{
			get
			{ return m_strHostname; }
			set
			{ m_strHostname = value; }
		}

		private int m_nPort = -1;
		public int Port
		{
			get
			{ return m_nPort; }
			set
			{ m_nPort = value; }
		}
		
		// WebServiceLogging info = WSDL file location of web service and access point
		// useful for using a pub/sub event system via a web sevice interface to
		// facilitate publishing on the event system
		private string m_strWebServiceWSDL = "";
		public string WebServiceWSDL
		{
			get
			{ return m_strWebServiceWSDL; }
			set
			{ m_strWebServiceWSDL = value; }
		}
		
		private string m_strWebServiceAccessPoint = "";
		public string WebServiceAccessPoint
		{
			get
			{ return m_strWebServiceAccessPoint; }
			set
			{ m_strWebServiceAccessPoint = value; }
		}
			
		public LogTraceContext()
		{
		}
	}
}
