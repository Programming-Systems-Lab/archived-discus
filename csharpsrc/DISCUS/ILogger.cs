using System;

namespace PSL.DISCUS.Interfaces
{
	/// <summary>
	/// Summary description for ILogger.
	/// </summary>
	public interface ILogger
	{
		void LogError( string strMsg );
		void LogInfo( string strMsg );
		void LogWarning( string strMsg );
	}
}
