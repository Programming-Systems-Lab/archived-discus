using System;

// DISCUS DataAccess package
namespace PSL.DISCUS.Impl.DataAccess
{
	/// <summary>
	/// Abstract Utility class used for database access
	/// </summary>
	public abstract class DBUtil
	{
		/* Function takes an input string and returns a 
		 * database "safe" string by making the string
		 * quote "'" safe and backslash safe "\\" 
		 * Input: strString - input string
		 * Return: database "safe" string
		 */
		public static string MakeStringSafe( string strString )
		{
			// Make string quote and backslash safe for dbase
			string strTemp = strString.Replace("'", "''");
			return strTemp.Replace( "\\", "\\\\" );
		}
	}// End DBUtil
}
