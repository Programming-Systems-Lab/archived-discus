using System;
using PSL.DISCUS.Interfaces;

namespace PSL.DISCUS.Impl.Logging
{
	/// <summary>
	/// Summary description for LoggerImpl.
	/// </summary>
	public abstract class LoggerImpl:ILogger
	{
		protected string m_strSource = "";
		public virtual string Source
		{
			get{ return m_strSource; }
			set{ m_strSource = value; }
		}

		
		public LoggerImpl()
		{}

		public virtual void LogError( string strMsg )
		{}
		
		public virtual void LogInfo( string strMsg )
		{}

		public virtual void LogWarning( string strMsg )
		{}
	}
}
