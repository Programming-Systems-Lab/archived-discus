using System;

namespace PSL.DISCUS.Interfaces
{
	/// <summary>
	/// Interface for all execution proxies/brokers
	/// </summary>
	public interface IExecuteService
	{
		string ExecuteServiceMethod( string strXMLExecRequest );
	}
}
