using System;
using PSL.DISCUS.Interfaces;

namespace PSL.DISCUS.Logging
{
	/// <summary>
	/// Summary description for TracerImpl.
	/// </summary>
	public abstract class TracerImpl:ITracer
	{
		public TracerImpl()
		{}

		public virtual void TraceError( string strSource, string strMsg )
		{}
		
		public virtual void TraceInfo( string strSource, string strMsg )
		{}

		public virtual void TraceWarning( string strSource, string strMsg )
		{}
	}
}
