using System;

namespace PSL.DISCUS.Impl.DataAccess
{
	/// <summary>
	/// Summary description for DBUtil.
	/// </summary>
	public abstract class DBUtil
	{
		public static string MakeStringSafe( string strString )
		{
			// Make string quote safe for db
			string strTemp = strString.Replace("'", "''");
			return strTemp.Replace( "\\", "\\\\" );
		}
	}
}
