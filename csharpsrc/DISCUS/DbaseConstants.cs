using System;

namespace PSL.DISCUS.Impl.DataAccess
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
		//public static string RS_SERVICE_WSDL		= "RS_SERVICE_WSDL";
		public static string RS_SERVICE_ACCESSPOINT	= "RS_SERVICE_ACCESSPOINT";

		// Service Methods Table
		public static string SM_METHODNAME			= "SM_METHODNAME";
		
		// Service Spaces Table
		public static string SS_GATEKEEPER			= "SS_GATEKEEPER";
		public static string SS_WSDL_FILE			= "SS_WSDL_FILE";

		// Treaty Data Table
		public static string TD_TREATY_ID			= "TD_TREATY_ID";
		public static string TD_XML_TREATY			= "TD_XML_TREATY";
		public static string TD_TREATY_STATUS		= "TD_TREATY_STATUS";
	}// End DBC
}
