using System;
using PSL.DISCUS.Interfaces;

namespace PSL.DISCUS.Logging
{
	/// <summary>
	/// Summary description for TracerImpl.
	/// </summary>
	public abstract class TracerImpl:ITracer
	{
		protected string m_strSource = "";
		public virtual string Source
		{
			get{ return m_strSource; }
			set{ m_strSource = value; }
		}

		public TracerImpl()
		{}

		public virtual void TraceError( string strMsg )
		{}
		
		public virtual void TraceInfo( string strMsg )
		{}

		public virtual void TraceWarning( string strMsg )
		{}
	}
}
