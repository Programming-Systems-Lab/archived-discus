using System;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using Microsoft.Data.Odbc;
using PSL.DISCUS.Interfaces.DataAccess;


namespace PSL.DISCUS.Impl.DataAccess
{
	/// <summary>
	/// Service Space Data Access Object
	/// ServiceServiceDAO encapsulates access to database information
	/// of services spaces known to this service space
	/// </summary>
	public class ServiceSpaceDAO:IDataObj
	{
		// Database configuration file
		private string DATACONFIG = DConst.DBASECONFIG_FILE;
		// Source name for event logging
		private const string SOURCENAME = "DataAccess.ServiceSpaceDAO";
		private EventLog m_EvtLog;
		// Database connection string
		private string m_strConnect = "";
		
		/* Constructor */
		public ServiceSpaceDAO()
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

				fs.Close();
			}
			catch( System.Exception e )
			{
				// Report error
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
		}
		
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
		}// End ExecuteReader

		/*	Function registers a GateKeeper in the service space 
		 *  database.
		 *  Input strGateKeeper			- name of GateKeeper
		 *		  strWSDLURL			- location of Gatekeeper
		 *	Return: true if GateKeeper registered successfully, false
		 *			otherwise
		 */ 
		public bool RegisterGateKeeper( string strGateKeeper, string strWSDLURL )
		{
			bool bRetVal = false;
			
			if( strGateKeeper.Length == 0 || strWSDLURL.Length == 0 )
				return false;

			string strSQL = "INSERT INTO ";
			strSQL += DBC.SERVICE_SPACES_TABLE;
			strSQL += "(" + DBC.SS_GATEKEEPER + "," + DBC.SS_WSDL_FILE + ")";
			strSQL += " VALUES(" + "'" + DBUtil.MakeStringSafe( strGateKeeper ) + "'" + "," + "'" + DBUtil.MakeStringSafe( strWSDLURL ) + "'" + ")";

			bRetVal = ExecuteCommandText( strSQL );

			return bRetVal;
		}

		/*	Function unregisters a GateKeeper in the service space 
		 *  database.
		 *  Input strGateKeeper			- name of GateKeeper
		 *	Return: true if GateKeeper unregistered successfully, false
		 *			otherwise
		 */ 
		public bool UnregisterGateKeeper( string strGateKeeper )
		{
			bool bRetVal = false;
			
			string strSQL = "DELETE FROM ";
			strSQL += DBC.SERVICE_SPACES_TABLE;
			strSQL += " WHERE " + DBC.SS_GATEKEEPER + "=" + "'" + DBUtil.MakeStringSafe(strGateKeeper) + "'";

			bRetVal = ExecuteCommandText( strSQL );

			return bRetVal;
		}

		/*	Function updates the locaton of a GateKeeper in the service space 
		 *  database.
		 *  Input strGateKeeper			- name of GateKeeper
		 *		  strNewWSDLURL			- new location of Gatekeeper
		 *	Return: true if GateKeeper location updated successfully, false
		 *			otherwise
		 */ 
		public bool UpdateGateKeeperWSDL( string strGateKeeper, string strNewWSDLURL )
		{
			bool bRetVal = false;
			
			string strSQL = "UPDATE ";
			strSQL += DBC.SERVICE_SPACES_TABLE;
			strSQL += " SET " + DBC.SS_WSDL_FILE + "=" + "'" + DBUtil.MakeStringSafe( strNewWSDLURL ) + "'";
			strSQL += " WHERE " + DBC.SS_GATEKEEPER + "=" + "'" + DBUtil.MakeStringSafe( strGateKeeper ) + "'";

			bRetVal = ExecuteCommandText( strSQL );

			return bRetVal;
		}
		
		/*	Function gets the location of a gatekeeper 
		 *  Input: strGateKeeper - name of GateKeeper
		 *  Return: GateKeeper location if it exists, "" otherwise
		 */
		public string GetGateKeeperWSDLURL( string strGateKeeper )
		{
			string strWSDLURL = "";
			OdbcDataReader dr = null;

			try
			{
				string strSQL = "SELECT " + DBC.SS_WSDL_FILE;
				strSQL += " FROM " + DBC.SERVICE_SPACES_TABLE;
				strSQL += " WHERE " + DBC.SS_GATEKEEPER + "=" + "'" + DBUtil.MakeStringSafe(strGateKeeper) + "'";
			
				dr = ExecuteReader( strSQL );
				if( dr != null )
				{
					dr.Read(); // move reader past BOF to first record
					if( !dr.IsDBNull( 0 ) )
						strWSDLURL = dr.GetString( 0 );
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
			
			return strWSDLURL;
		}
	}
}
