using System;

namespace PSL.DISCUS.Interfaces
{
	/// <summary>
	/// Summary description for ILogger.
	/// </summary>
	public interface ILogger
	{
		void LogError( string strSource, string strMsg );
		void LogInfo( string strSource, string strMsg );
		void LogWarning( string strSource, string strMsg );
	}
}
