using System;
using System.Diagnostics;

namespace PSL.DISCUS.Logging
{
	/// <summary>
	/// Summary description for EvtLoggerImpl.
	/// </summary>
	public class EvtLoggerImpl:LoggerImpl
	{
		public EvtLoggerImpl( LogTraceContext ctx )
		{
		}

		public override void LogError( string strSource, string strMsg )
		{
			// If no source specified throw exception
			if( strSource.Length == 0 )
				throw new Exception( "No Logging Source Specified" );
			
			// Write to event log
			EventLog.WriteEntry( strSource, strMsg, EventLogEntryType.Error );
		}

		public override void LogInfo( string strSource, string strMsg )
		{
			// If no source specified throw exception
			if( strSource.Length == 0 )
				throw new Exception( "No Logging Source Specified" );
			
			// Write to event log
			EventLog.WriteEntry( strSource, strMsg, EventLogEntryType.Information );
		}

		public override void LogWarning( string strSource, string strMsg )
		{
			// If no source specified throw exception
			if( strSource.Length == 0 )
				throw new Exception( "No Logging Source Specified" );
			
			// Write to event log
			EventLog.WriteEntry( strSource, strMsg, EventLogEntryType.Warning );
		}
	}
}
