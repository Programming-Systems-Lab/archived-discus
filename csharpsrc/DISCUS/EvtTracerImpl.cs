using System;
using System.Diagnostics;

namespace PSL.DISCUS.Impl.Logging
{
	/// <summary>
	/// Summary description for EvtTracerImpl.
	/// </summary>
	public class EvtTracerImpl:TracerImpl
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
					throw new Exception( "Invalid Tracing Source Specified" );
				m_EvtLogger.Source = m_strSource;			
			}
		}
		
		public EvtTracerImpl( LogTraceContext ctx )
		{
			m_EvtLogger = new EventLog( "Application" );
		}

		public override void TraceError( string strMsg )
		{
			// If no source specified throw exception
			if( m_strSource.Length == 0 )
				throw new Exception( "No Tracing Source Specified" );
			
			// Write to event log
			m_EvtLogger.WriteEntry( strMsg, EventLogEntryType.Error );
		}

		public override void TraceInfo( string strMsg )
		{
			// If no source specified throw exception
			if( m_strSource.Length == 0 )
				throw new Exception( "No Tracing Source Specified" );
			
			// Write to event log
			m_EvtLogger.WriteEntry( strMsg, EventLogEntryType.Information );
		}

		public override void TraceWarning( string strMsg )
		{
			// If no source specified throw exception
			if( m_strSource.Length == 0 )
				throw new Exception( "No Tracing Source Specified" );
			
			// Write to event log
			m_EvtLogger.WriteEntry( strMsg, EventLogEntryType.Warning );
		}

	}
}
