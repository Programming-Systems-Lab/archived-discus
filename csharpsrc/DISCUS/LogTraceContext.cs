using System;
using System.Threading;

namespace PSL.DISCUS.Logging
{
	public enum enuLoggingType
	{
		None = 0,
		EventLog,
		UrlLog
	}

	public enum enuTracingType
	{
		None = 0,
		EventTrace,
		UrlTrace,
	}
	
	/// <summary>
	/// Summary description for LogTraceContext.
	/// </summary>
	public class LogTraceContext
	{
		private string m_strHostname = "";
		private int m_nPort = -1;

		public string Hostname
		{
			get
			{ return m_strHostname; }
			set
			{ 
				lock( m_strHostname )
				{
					m_strHostname = value; 
				}
			}
		}
		
		public int Port
		{
			get
			{ return m_nPort; }
			set
			{ 
				lock( (object) m_nPort )
				{
					m_nPort = value; 
				}
			}
		}
					
		public LogTraceContext()
		{
		}
	}
}
