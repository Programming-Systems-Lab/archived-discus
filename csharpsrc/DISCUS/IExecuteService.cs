using System;

namespace PSL.DISCUS.Interfaces
{
	/// <summary>
	/// Summary description for IExecuteService.
	/// </summary>
	public interface IExecuteService
	{
		Object ExecuteServiceMethod( int nTreatyID, string strServiceName, string strServiceMethod, Object[] parameters );
	}
}
