using System;
using System.Threading;
using System.Reflection;
using System.IO;
using PSL.DISCUS.Logging;

namespace PSL.DISCUS
{
	/// <summary>
	/// Summary description for GatekeeperSettings.
	/// </summary>
	public class GatekeeperSettings
	{
		// Settings with acceptable defaults
		private string m_strDatabaseConnectionString = Constants.DEFAULT_DBASE_CONNECT;
		private string m_strGatekeeperName = Constants.DEFAULT_GATEKEEPER_NAME;
		private string m_strProxyCache = Constants.DEFAULT_PROXY_CACHE;
		private enuLoggingType m_loggingType = Constants.DEFAULT_LOGGING_TYPE;
		private enuTracingType m_tracingType = Constants.DEFAULT_TRACING_TYPE;
		private bool m_bTraceOn = Constants.DEFAULT_TRACE_ON;
		
		// Settings with no acceptable defaults
		private string m_strUrlLoggingHostname = "";
		private int m_nUrlLoggingPort = -1;
		private string m_strUrlTracingHostname = "";
		private int m_nUrlTracingPort = -1;

		public GatekeeperSettings()
		{
			// Set Fully Qualified name of ProxyCache
			string strLocalPath = new Uri( Assembly.GetExecutingAssembly().CodeBase ).LocalPath;
			this.m_strProxyCache = new FileInfo( strLocalPath ).Directory.FullName + "\\" + Constants.DEFAULT_PROXY_CACHE;				
		}

		// Properties
		public string DatabaseConnectString
		{
			get
			{ return this.m_strDatabaseConnectionString; }
			set
			{ 
				// Make no changes
				if( value == null || value.Length == 0 )
					return;
				
				lock( m_strDatabaseConnectionString )
				{
					this.m_strDatabaseConnectionString = value;
				}
			}
		}

		public string GatekeeperName
		{
			get
			{ return this.m_strGatekeeperName; }
			set
			{ 
				// Make no changes
				if( value == null || value.Length == 0 )
					return;

				lock( m_strGatekeeperName )
				{
					this.m_strGatekeeperName = value; 
				}
			}
		}
		public string ProxyCache
		{
			get
			{ return this.m_strProxyCache; }
			set
			{ 
				// Make no changes
				if( value == null || value.Length == 0 )
					return;

				lock( m_strProxyCache )
				{
					this.m_strProxyCache = value; 
				}
			}
		}
		public enuLoggingType LoggingType
		{
			get
			{ return this.m_loggingType; }
			set
			{	
				lock( (object) m_loggingType )
				{
					this.m_loggingType = value; 
				}
			}
		}

		public enuTracingType TracingType
		{
			get
			{ return this.m_tracingType; }
			set
			{ 
				lock( (object) m_tracingType )
				{
					this.m_tracingType = value; 
				}
			}
		}
		
		public bool TraceOn
		{
			get
			{ return this.m_bTraceOn; }
			set
			{ 
				lock( (object) m_bTraceOn )
				{
					this.m_bTraceOn = value; 
				}
			}
		}

		public string UrlLoggingHostname
		{
			get
			{ return this.m_strUrlLoggingHostname; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;

				lock( m_strUrlLoggingHostname )
				{
					this.m_strUrlLoggingHostname = value; 
				}
			}
		}

		public int UrlLoggingPort
		{
			get
			{ return this.m_nUrlLoggingPort; }
			set
			{ 
				lock( (object) m_nUrlLoggingPort )
				{
					this.m_nUrlLoggingPort = value; 
				}
			}
		}

		public string UrlTracingHostname
		{
			get
			{ return this.m_strUrlTracingHostname; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;

				lock( m_strUrlTracingHostname )
				{
					this.m_strUrlTracingHostname = value; 
				}
			}
		}

		public int UrlTracingPort
		{
			get
			{ return this.m_nUrlTracingPort; }
			set
			{ 
				lock( (object) m_nUrlTracingPort )
				{
					this.m_nUrlTracingPort = value; 
				}
			}
		}
	}
}
