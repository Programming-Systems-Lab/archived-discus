using System;
using System.Diagnostics;

namespace PSL.DISCUS.Logging
{
	/// <summary>
	/// Summary description for EvtTracerImpl.
	/// </summary>
	public class EvtTracerImpl:TracerImpl
	{
		public EvtTracerImpl( LogTraceContext ctx )
		{
		}

		public override void TraceError( string strSource, string strMsg )
		{
			// If no source specified throw exception
			if( strSource.Length == 0 )
				throw new Exception( "No Tracing Source Specified" );
			
			// Write to event log
			EventLog.WriteEntry( strSource, strMsg, EventLogEntryType.Error );
		}

		public override void TraceInfo( string strSource, string strMsg )
		{
			// If no source specified throw exception
			if( strSource.Length == 0 )
				throw new Exception( "No Tracing Source Specified" );
			
			// Write to event log
			EventLog.WriteEntry( strSource, strMsg, EventLogEntryType.Information );
		}

		public override void TraceWarning( string strSource, string strMsg )
		{
			// If no source specified throw exception
			if( strSource.Length == 0 )
				throw new Exception( "No Tracing Source Specified" );
			
			// Write to event log
			EventLog.WriteEntry( strSource, strMsg, EventLogEntryType.Warning );
		}

	}
}
