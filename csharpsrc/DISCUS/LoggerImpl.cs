using System;
using PSL.DISCUS.Interfaces;

namespace PSL.DISCUS.Logging
{
	/// <summary>
	/// Summary description for LoggerImpl.
	/// </summary>
	public abstract class LoggerImpl:ILogger
	{
		public LoggerImpl()
		{}

		public virtual void LogError( string strSource, string strMsg )
		{}
		
		public virtual void LogInfo( string strSource, string strMsg )
		{}

		public virtual void LogWarning( string strSource, string strMsg )
		{}
	}
}
