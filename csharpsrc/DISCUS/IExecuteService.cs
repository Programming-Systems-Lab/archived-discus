using System;

namespace PSL.DISCUS.Interfaces
{
	/// <summary>
	/// Interface for all execution proxies/brokers
	/// </summary>
	public interface IExecuteService
	{
		// Object ExecuteServiceMethod( int nTreatyID, string strServiceName, string strServiceMethod, Object[] parameters );
		string ExecuteServiceMethod( string strXMLExecRequest );
	}
}
