using System;
using System.Diagnostics;

namespace PSL.DISCUS.Impl.Logging
{
	/// <summary>
	/// Summary description for EvtLoggerImpl.
	/// </summary>
	public class EvtLoggerImpl:LoggerImpl
	{
		private EventLog m_EvtLogger;

		public override string Source
		{
			get
			{ return m_strSource; }

			set
			{ 
				m_strSource = value;
				if( m_strSource.Length == 0 )
					throw new Exception( "Invalid Logging Source Specified" );
				m_EvtLogger.Source = m_strSource;			
			}
		}
		
		public EvtLoggerImpl()
		{
			// Create new Application Event Log instance
			m_EvtLogger = new EventLog( "Application" );
		}

		public override void LogError( string strMsg )
		{
			// If no source specified throw exception
			if( m_strSource.Length == 0 )
				throw new Exception( "No Logging Source Specified" );
			
			// Write to event log
			m_EvtLogger.WriteEntry( strMsg, EventLogEntryType.Error );
		}

		public override void LogInfo( string strMsg )
		{
			// If no source specified throw exception
			if( m_strSource.Length == 0 )
				throw new Exception( "No Logging Source Specified" );
			
			// Write to event log
			m_EvtLogger.WriteEntry( strMsg, EventLogEntryType.Information );
		}

		public override void LogWarning( string strMsg )
		{
			// If no source specified throw exception
			if( m_strSource.Length == 0 )
				throw new Exception( "No Logging Source Specified" );
			
			// Write to event log
			m_EvtLogger.WriteEntry( strMsg, EventLogEntryType.Warning );
		}
	}
}
