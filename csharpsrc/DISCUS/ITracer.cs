using System;

namespace PSL.DISCUS.Interfaces
{
	/// <summary>
	/// Summary description for ITracer.
	/// </summary>
	public interface ITracer
	{
		void TraceError( string strSource, string strMsg );
		void TraceInfo( string strSource, string strMsg );
		void TraceWarning( string strSource, string strMsg );
	}
}
