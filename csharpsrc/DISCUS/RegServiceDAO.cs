using System;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using Microsoft.Data.Odbc;
using PSL.DISCUS.Interfaces.DataAccess;


// DISCUS DataAccess package
namespace PSL.DISCUS.Impl.DataAccess
{
	/// <summary>
	/// Registered Service Data Access Object
	/// RegServiceDAO encapsulates access to database information
	/// of services registered within a service space
	/// </summary>
	public class RegServiceDAO:IDataObj
	{
		// Database configuration file
		private string DATACONFIG = DConst.DBASECONFIG_FILE;
		// Source name used for event logging
		private const string SOURCENAME = "DataAccess.RegServiceDAO";
		private EventLog m_EvtLog;
		// Database connection string
		private string m_strConnect = "";

		/* Constructor */		
		public RegServiceDAO()
		{
			try
			{
				// Initialize event logging facility
				m_EvtLog = new EventLog( "Application" );
				m_EvtLog.Source = SOURCENAME;
				
				// Load config info, dbase, connection info etc.
				FileStream fs = File.Open( DATACONFIG, FileMode.Open );
				TextReader tr = new StreamReader( fs );
				string strConfigFile = tr.ReadToEnd();

				// Load config file
				XmlDocument doc = new XmlDocument();
				doc.LoadXml( strConfigFile );
				
				// Use XPath to extract what info we need
				XmlNode root =  doc.DocumentElement;
				// Get dbase connection info
				m_strConnect = root.SelectSingleNode( "ConnectionString" ).InnerText;

				fs.Close(); // Close file stream
			}
			catch( System.Exception e )
			{
				// Report error
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
		}// End constructor

		/* Implementation IDataObj ExecuteCommandText method
		 * Function executes an SQL command.
		 * Input: strCmd - command to execute (typically an SQL query)
		 * Return: true if command executes, false if errors occur
		 */
		public bool ExecuteCommandText( string strCmd )
		{
			bool bRetVal = false;
		
			try
			{
				// Create new connection
				OdbcConnection Conn = new OdbcConnection( m_strConnect );
				// Create new command
				OdbcCommand Command = new OdbcCommand(strCmd);
				Command.Connection = Conn;
				// Open connection
				Conn.Open();
				// Execute command
				Command.ExecuteReader();
				// Close connection
				Conn.Close();
				// Set return value
				bRetVal = true;
			}
			catch( System.Exception e )
			{
				// Report error
				string strError = e.Message;
				strError += " LAST QUERY: " + strCmd;
				m_EvtLog.WriteEntry( strError, EventLogEntryType.Error );
			}
			return bRetVal;
		}
		
		/*	Function executes a command and returns a DataReader
		 *  useful for retrieving values from the database
		 *  Input: strCmd - command, typically and SQL query
		 *	Return: DataReader resulting from executing command
		 *			DataReader may be null if command contains 
		 *			errors or caused and exception to be raised.
		 */
		public OdbcDataReader ExecuteReader( string strCmd )
		{
			OdbcDataReader dr = null;
			try
			{
				OdbcConnection Conn = new OdbcConnection( m_strConnect );
				OdbcCommand Command = new OdbcCommand(strCmd);
				Command.Connection = Conn;
				Conn.Open();
				dr = Command.ExecuteReader( CommandBehavior.CloseConnection );
			}
			catch( System.Exception e )
			{
				// Report error
				string strError = e.Message;
				strError += " LAST QUERY: " + strCmd;
				m_EvtLog.WriteEntry( strError, EventLogEntryType.Error );
			}
			return dr;
		}

		/*	Function registers a service in the service space 
		 *  database.
		 *  Input strServiceName		- name of service
		 *		  strServiceNameSpace	- namespace of service, may be empty ("")
		 *		  strServiceLoc			- location of service, may be a physical path
		 *								on the local machine or may be a link to
		 *								the services WSDL file
		 *  Return: The ID of the registered service
		 *			if ID == -1 then an error occurred during registration
		 *			and service not registered
		 */ 
		public int RegisterService( string strServiceName, string strServiceNamespace, string strServiceLoc )
		{
			if( strServiceName.Length == 0 || strServiceLoc.Length == 0 )
				return 0;
			
			int nServiceID = -1;
			string strSQL = "INSERT INTO ";
			strSQL += DBC.REGISTERED_SERVICES_TABLE;
			strSQL += " (" + DBC.RS_SERVICE_NAME + ","; 
			// Add namespace if supplied
			if( strServiceNamespace.Length > 0 )
				strSQL += DBC.RS_SERVICENAMESPACE + ",";
			
			strSQL += DBC.RS_SERVICE_LOCATION + ")";
			
			strSQL += " VALUES(" + "'" + DBUtil.MakeStringSafe(strServiceName) + "'" + ","; 
			
			if( strServiceNamespace.Length > 0 )
				strSQL += "'" + DBUtil.MakeStringSafe( strServiceNamespace ) + "'" + ",";
			
			strSQL += "'" + DBUtil.MakeStringSafe(strServiceLoc) + "'" + ")";

			if( ExecuteCommandText( strSQL ) )
				nServiceID = GetServiceID( strServiceName );
			
			return nServiceID;
		}
		
		/*	Function unregisters a service.
		 *  Input: strServiceName - service to unregister
		 *	Return: true if service sucessfully unregistered
		 *			false otherwise
		 */
		public bool UnRegisterService( string strServiceName )
		{
			bool bRetVal = false;
			if( strServiceName.Length == 0 )
				return false;
			
			string strSQL = "DELETE FROM ";
			strSQL += DBC.REGISTERED_SERVICES_TABLE;
			strSQL += " WHERE " + DBUtil.MakeStringSafe(DBC.RS_SERVICE_NAME) + "=" + "'" + strServiceName + "'";
			
			bRetVal = ExecuteCommandText( strSQL );

			return bRetVal;
		}
		
		/*	Function updates a service location. 
		 *	Input: strServiceName - name of service to update
		 *		   strServiceLoc  - location of service
		 *	Return: true if service location sucessfully updated
		 *			false otherwise
		 */
		public bool UpdateServiceLocation( string strServiceName, string strServiceLoc )
		{
			bool bRetVal = false;
			if( strServiceName.Length == 0 || strServiceLoc.Length == 0 )
				return false;
			
			string strSQL = "UPDATE " + DBC.REGISTERED_SERVICES_TABLE;
			strSQL += " SET " + DBC.RS_SERVICE_LOCATION + "=" + "'" + DBUtil.MakeStringSafe(strServiceLoc) + "'";
			strSQL += " WHERE " + DBC.RS_SERVICE_NAME + "=" + "'" + DBUtil.MakeStringSafe(strServiceName) + "'";

			bRetVal = ExecuteCommandText( strSQL );
			
			return bRetVal;
		}

		/*	Function updates a service namespace. 
		 *	Input: strServiceName		- name of service to update
		 *		   strServiceNamespace  - namespace of service
		 *	Return: true if service namespace sucessfully updated
		 *			false otherwise
		 */
		public bool UpdateServiceNamespace( string strServiceName, string strServiceNamespace )
		{
			bool bRetVal = false;
			if( strServiceName.Length == 0 )
				return false;
			
			string strSQL = "UPDATE " + DBC.REGISTERED_SERVICES_TABLE;
			strSQL += " SET " + DBC.RS_SERVICENAMESPACE + "=" + "'" + DBUtil.MakeStringSafe(strServiceNamespace) + "'";
			strSQL += " WHERE " + DBC.RS_SERVICE_NAME + "=" + "'" + DBUtil.MakeStringSafe(strServiceName) + "'";

			bRetVal = ExecuteCommandText( strSQL );
			
			return bRetVal;
		}

		/*	Function registers a service method in the service 
		 *	space database.
		 *	Input: strServiceName - name of service
		 *		   strMethod	  - name of service method
		 *  Return: true if service method registered sucessfully
		 *			false otherwise
		 */
		public bool RegisterServiceMethod( string strServiceName, string strMethod )
		{
			bool bRetVal = false;
	
			if( strServiceName.Length == 0 || strMethod.Length == 0 )
				return false;

			// Get Service ID of service
			int nServiceID = GetServiceID( strServiceName );
			if( nServiceID != -1 )
			{
				string strSQL = "INSERT INTO ";
				strSQL += DBC.SERVICE_METHODS_TABLE;
				strSQL += "(" + DBC.RS_SERVICE_ID + "," + DBC.SM_METHODNAME + ")";
				strSQL += " VALUES(" + nServiceID.ToString() + "," + "'" + DBUtil.MakeStringSafe( strMethod ) + "'" + ")";

				bRetVal = ExecuteCommandText( strSQL );
			}

			return bRetVal;
		}

		/*	Function unregisters a service method in the service 
		 *	space database.
		 *	Input: strServiceName - name of service
		 *		   strMethod	  - name of service method
		 *  Return: true if service method unregistered sucessfully
		 *			false otherwise
		 */
		public bool UnregisterServiceMethod( string strServiceName, string strMethod )
		{
			bool bRetVal = false;

			if( strServiceName.Length == 0 || strMethod.Length == 0 )
				return false;

			int nServiceID = GetServiceID( strServiceName );
			if( nServiceID != -1 )
			{
				string strSQL = "DELETE FROM ";
				strSQL += DBC.SERVICE_METHODS_TABLE;
				strSQL += " WHERE " + DBC.RS_SERVICE_ID + "=" + nServiceID.ToString();
				strSQL += " AND " + DBC.SM_METHODNAME + "=" + "'" + DBUtil.MakeStringSafe( strMethod ) + "'";

				bRetVal = ExecuteCommandText( strSQL );
			}

			return bRetVal;
		}

		/*	Function updates a service method in the service 
		 *	space database.
		 *	Input: strServiceName - name of service
		 *		   strOldMethod	  - name of old service method
		 *		   strNewMethod	  - name of new service method
		 *  Return: true if service method updated sucessfully
		 *			false otherwise
		 */
		public bool UpdateServiceMethod( string strServiceName, string strOldMethod, string strNewMethod )
		{
			bool bRetVal = false;

			if( ( strServiceName.Length == 0 || strOldMethod.Length == 0 ) || strNewMethod.Length == 0 )
				return false;
	
			int nServiceID = GetServiceID( strServiceName );
			if( nServiceID != -1 )
			{
				string strSQL = "UPDATE " + DBC.SERVICE_METHODS_TABLE;
				strSQL += " SET " + DBC.SM_METHODNAME + "=" + "'" + DBUtil.MakeStringSafe( strNewMethod ) + "'";
				strSQL += " WHERE " + DBC.RS_SERVICE_ID + "=" + nServiceID.ToString();
				strSQL += " AND " + DBC.SM_METHODNAME + "=" + "'" + DBUtil.MakeStringSafe( strOldMethod ) + "'";

				bRetVal = ExecuteCommandText( strSQL );
			}

			return bRetVal;
		}

		/*	Function gets the ID of a service 
		 *  Input: strServiceName - name of service
		 *  Return: service ID if it exists, -1 otherwise
		 */
		public int GetServiceID( string strServiceName )
		{
			int nServiceID = -1;
			OdbcDataReader dr = null;

			try
			{
				string strSQL = "SELECT " + DBC.RS_SERVICE_ID;
				strSQL += " FROM " + DBC.REGISTERED_SERVICES_TABLE;
				strSQL += " WHERE " + DBC.RS_SERVICE_NAME + "=" + "'" + DBUtil.MakeStringSafe(strServiceName) + "'";
			
				dr = ExecuteReader( strSQL );
				if( dr != null )
				{
					dr.Read(); // move reader past BOF to first record
					if( !dr.IsDBNull( 0 ) )
						nServiceID = dr.GetInt32( 0 );
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			catch( System.Exception e )
			{
				// Report error
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			finally // cleanup after exception handled
			{
				if( dr != null )
				{
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			return nServiceID;
		}// End GetServiceID

		/*	Function gets the location of a service 
		 *  Input: strServiceName - name of service
		 *  Return: service location if it exists, "" otherwise
		 */
		public string GetServiceLocation( string strServiceName )
		{
			string strServiceLoc = "";
			
			OdbcDataReader dr = null;

			try
			{
				string strSQL = "SELECT " + DBC.RS_SERVICE_LOCATION;
				strSQL += " FROM " + DBC.REGISTERED_SERVICES_TABLE;
				strSQL += " WHERE " + DBC.RS_SERVICE_NAME + "=" + "'" + DBUtil.MakeStringSafe(strServiceName) + "'";
			
				dr = ExecuteReader( strSQL );
				if( dr != null )
				{
					dr.Read(); // move reader past BOF to first record
					if( !dr.IsDBNull( 0 ) )
						strServiceLoc = dr.GetString( 0 );
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			catch( System.Exception e )
			{
				// Report error
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			finally // cleanup after exception handled
			{
				if( dr != null )
				{
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			
			return strServiceLoc;
		}// End GetServiceLocation

		
		/*	Function gets the namespace of a service 
		 *  Input: strServiceName - name of service
		 *  Return: service namespace if it exists, "" otherwise
		 */
		public string GetServiceNamespace( string strServiceName )
		{
			string strServiceNamespace = "";
			
			OdbcDataReader dr = null;

			try
			{
				string strSQL = "SELECT " + DBC.RS_SERVICENAMESPACE;
				strSQL += " FROM " + DBC.REGISTERED_SERVICES_TABLE;
				strSQL += " WHERE " + DBC.RS_SERVICE_NAME + "=" + "'" + DBUtil.MakeStringSafe(strServiceName) + "'";
			
				dr = ExecuteReader( strSQL );
				if( dr != null )
				{
					dr.Read(); // move reader past BOF to first record
					if( !dr.IsDBNull( 0 ) )
						strServiceNamespace = dr.GetString( 0 );
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			catch( System.Exception e )
			{
				// Report error
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			finally // cleanup after exception handled
			{
				if( dr != null )
				{
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			
			return strServiceNamespace;
		}// End GetServiceNamespace

		
		/*	Function determines whether a service has a specific
		 *	method registered.
		 *	Input: strServiceName - name of service
		 *		   strMethod	  - name of method
		 *	Return: true if service has registered method, false
		 *			otherwise.
		 */
		public bool MethodExists( string strServiceName, string strMethod )
		{
			bool bRetVal = false;
			OdbcDataReader dr = null;
			int nCount = 0;
			
			try
			{
				int nServiceID = GetServiceID( strServiceName );
				if( nServiceID == -1 )
					return false;

				string strSQL = "SELECT COUNT(*) FROM ";
				strSQL += DBC.SERVICE_METHODS_TABLE;
				strSQL += " WHERE " + DBC.RS_SERVICE_ID + "=" + nServiceID.ToString();
				strSQL += " AND " + DBC.SM_METHODNAME + "=" + "'" + strMethod + "'";

				dr = ExecuteReader( strSQL );
				if( dr != null )
				{
					dr.Read();
					if( !dr.IsDBNull( 0 ) )
						nCount = dr.GetInt32( 0 );

					if( nCount > 0 )
						bRetVal = true;

					if( !dr.IsClosed )
						dr.Close();
				}
			}
			catch( System.Exception e )
			{
				// Report error
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			finally // cleanup after exception handled
			{
				if( dr != null )
				{
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			return bRetVal;
		}// End MethodExists
	}//End RegServiceDAO
}
