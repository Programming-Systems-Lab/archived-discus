using System;

namespace PSL.DISCUS.Interfaces.DataAccess
{
	/// <summary>
	/// Summary description for IDataObj.
	/// </summary>
	public interface IDataObj
	{
		bool ExecuteCommandText( string strCmd );
	}
}
