using System;

namespace PSL.DISCUS.DataAccess
{
	/// <summary>
	/// Abstract class defines database constants,
	/// database table names and column names
	/// </summary>
	public abstract class DBC
	{
		// Dbase Table names
		public static string REGISTERED_SERVICES_TABLE	= "REGISTERED_SERVICES";
		public static string SERVICE_METHODS_TABLE		= "SERVICE_METHODS";
		public static string SERVICE_SPACES_TABLE		= "SERVICE_SPACES";
		public static string TREATY_DATA_TABLE			= "TREATY_DATA";
	
		// Dbase Column names
		// Registered Services Table
		public static string RS_SERVICE_ID			= "RS_SERVICE_ID";
		public static string RS_SERVICE_NAME		= "RS_SERVICENAME";
		public static string RS_SERVICENAMESPACE	= "RS_SERVICENAMESPACE";
		public static string RS_SERVICE_LOCATION	= "RS_SERVICE_LOCATION";
		public static string RS_SERVICE_ACCESSPOINT	= "RS_SERVICE_ACCESSPOINT";

		// Service Methods Table
		public static string SM_METHODNAME			= "SM_METHODNAME";
		
		// Service Spaces Table
		public static string SS_GK_NAME				= "SS_GK_NAME";
		public static string SS_GK_NAMESPACE		= "SS_GK_NAMESPACE";
		public static string SS_GK_LOCATION			= "SS_GK_LOCATION";
		public static string SS_GK_ACCESSPOINT		= "SS_GK_ACCESSPOINT";
	
	}// End DBC
}
