using System;
// DISCUS DataAccess package
namespace PSL.DISCUS.Interfaces.DataAccess
{
	/// <summary>
	/// Interface that all data objects must support
	/// used mainly for executing simple SQL queries
	/// </summary>
	public interface IDataObj
	{
		bool ExecuteCommandText( string strCmd );
	}
}
