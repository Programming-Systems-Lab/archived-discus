using System;

namespace PSL.DISCUS.Interfaces
{
	/// <summary>
	/// IExecuteWorkflow interface implemented by all
	/// entities that need to execute alpha protocols
	/// </summary>
	public interface IExecuteWorkflow
	{
		string [] ExecuteAlphaProtocol( string strXMLProtocol );
	}
}
