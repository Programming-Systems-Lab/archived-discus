using System;

namespace PSL.DISCUS.Interfaces
{
	/// <summary>
	/// Summary description for ITracer.
	/// </summary>
	public interface ITracer
	{
		void TraceError( string strMsg );
		void TraceInfo( string strMsg );
		void TraceWarning( string strMsg );
	}
}
