using System;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Diagnostics;
using Microsoft.Data.Odbc;
using PSL.DISCUS.Interfaces.DataAccess;
using PSL.DISCUS.DynamicProxy;

namespace PSL.DISCUS.DataAccess
{
	/// <summary>
	/// Service Space Data Access Object
	/// ServiceServiceDAO encapsulates access to database information
	/// of services spaces known to this service space
	/// </summary>
	public class ServiceSpaceDAO:IDataObj
	{
		// Source name for event logging
		private const string SOURCENAME = "DataAccess.ServiceSpaceDAO";
		// Database connection string
		private string m_strConnect = "";
		
		/* Constructor */
		public ServiceSpaceDAO( string strDBConnect )
		{
				m_strConnect = strDBConnect;
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
				EventLog.WriteEntry( SOURCENAME, strError, EventLogEntryType.Error );
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
				EventLog.WriteEntry( SOURCENAME, strError, EventLogEntryType.Error );
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
		//public bool RegisterGateKeeper( string strGateKeeper, string strWSDLURL )
		//string strServiceNamespace, , string strServiceAccessPoint )
		public bool RegisterGateKeeper( string strGKName, string strGKNamespace, string strGKLocation, string strGKAccessPoint ) 
		{
			bool bRetVal = false;
			if( strGKName.Length == 0 || strGKLocation.Length == 0 )
				return false;

			StringBuilder strSQL = new StringBuilder();
			strSQL.Append( "INSERT INTO " );
			strSQL.Append( DBC.SERVICE_SPACES_TABLE );
			
			strSQL.Append( "(" + DBC.SS_GK_NAME + "," );

			if( strGKNamespace.Length > 0 )
				strSQL.Append( DBC.SS_GK_NAMESPACE + "," );
			
			if( strGKAccessPoint.Length > 0 )
				strSQL.Append( DBC.SS_GK_ACCESSPOINT + "," );

			strSQL.Append( DBC.SS_GK_LOCATION + ")" );
			
			strSQL.Append( " VALUES(" );
			strSQL.Append( "'" + DBUtil.MakeStringSafe( strGKName ) + "'" + "," );

			if( strGKNamespace.Length > 0 )
				strSQL.Append( "'" + DBUtil.MakeStringSafe( strGKNamespace ) + "'" + "," );

			if( strGKAccessPoint.Length > 0 )
				strSQL.Append( "'" + DBUtil.MakeStringSafe( strGKAccessPoint ) + "'" + "," );

			strSQL.Append( "'" + DBUtil.MakeStringSafe( strGKLocation ) + "'" + ")" );

			bRetVal = ExecuteCommandText( strSQL.ToString() );

			return bRetVal;
		}
				
		/*	Function unregisters a GateKeeper in the service space 
		 *  database.
		 *  Input strGateKeeper			- name of GateKeeper
		 *	Return: true if GateKeeper unregistered successfully, false
		 *			otherwise
		 */ 
		public bool UnregisterGateKeeper( string strGKName )
		{
			bool bRetVal = false;
			
			StringBuilder strSQL = new StringBuilder();
			strSQL.Append( "DELETE FROM " );
			strSQL.Append( DBC.SERVICE_SPACES_TABLE );
			strSQL.Append( " WHERE " + DBC.SS_GK_NAME + "=" + "'" + DBUtil.MakeStringSafe( strGKName ) + "'" );

			bRetVal = ExecuteCommandText( strSQL.ToString() );

			return bRetVal;
		}

		public bool UpdateGateKeeperAccessPoint( string strGKName, string strGKAccessPoint )
		{
			bool bRetVal = false;
			
			if( strGKAccessPoint.Length == 0 )
				return false;
			
			StringBuilder strSQL = new StringBuilder();
			strSQL.Append( "UPDATE " );
			strSQL.Append( DBC.SERVICE_SPACES_TABLE );
			strSQL.Append( " SET " + DBC.SS_GK_ACCESSPOINT + "=" + "'" + DBUtil.MakeStringSafe( strGKAccessPoint ) + "'" );
			strSQL.Append( " WHERE " + DBC.SS_GK_NAME + "=" + "'" + DBUtil.MakeStringSafe( strGKName ) + "'" );

			bRetVal = ExecuteCommandText( strSQL.ToString() );

			return bRetVal;
		}

		/*	Function updates the locaton of a GateKeeper in the service space 
		 *  database.
		 *  Input strGateKeeper			- name of GateKeeper
		 *		  strNewWSDLURL			- new location of Gatekeeper
		 *	Return: true if GateKeeper location updated successfully, false
		 *			otherwise
		 */ 
		public bool UpdateGateKeeperLocation( string strGKName, string strGKLocation )
		{
			bool bRetVal = false;
			
			if( strGKLocation.Length == 0 )
				return false;
			
			StringBuilder strSQL = new StringBuilder();
			strSQL.Append( "UPDATE " );
			strSQL.Append( DBC.SERVICE_SPACES_TABLE );
			strSQL.Append( " SET " + DBC.SS_GK_LOCATION + "=" + "'" + DBUtil.MakeStringSafe( strGKLocation ) + "'" );
			strSQL.Append( " WHERE " + DBC.SS_GK_NAME + "=" + "'" + DBUtil.MakeStringSafe( strGKName ) + "'" );

			bRetVal = ExecuteCommandText( strSQL.ToString() );

			return bRetVal;
		}
		
		public bool UpdateGateKeeperNamespace( string strGKName, string strGKNamespace )
		{
			bool bRetVal = false;

			if( strGKNamespace.Length == 0 )
				return false;
			
			StringBuilder strSQL = new StringBuilder();
			strSQL.Append( "UPDATE " );
			strSQL.Append( DBC.SERVICE_SPACES_TABLE );
			strSQL.Append( " SET " + DBC.SS_GK_NAMESPACE + "=" + "'" + DBUtil.MakeStringSafe( strGKNamespace ) + "'" );
			strSQL.Append( " WHERE " + DBC.SS_GK_NAME + "=" + "'" + DBUtil.MakeStringSafe( strGKName ) + "'" );

			bRetVal = ExecuteCommandText( strSQL.ToString() );

			return bRetVal;
		}
		
		public string GetGateKeeperAccessPoint( string strGKName )
		{
			string strGKAccessPoint = "";
			OdbcDataReader dr = null;

			try
			{
				StringBuilder strSQL = new StringBuilder();
				strSQL.Append( "SELECT " + DBC.SS_GK_ACCESSPOINT );
				strSQL.Append( " FROM " + DBC.SERVICE_SPACES_TABLE );
				strSQL.Append( " WHERE " + DBC.SS_GK_NAME + "=" + "'" + DBUtil.MakeStringSafe( strGKName ) + "'" );
			
				dr = ExecuteReader( strSQL.ToString() );
				if( dr != null )
				{
					dr.Read(); // move reader past BOF to first record
					if( !dr.IsDBNull( 0 ) )
						strGKAccessPoint = dr.GetString( 0 );
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			catch( System.Exception e )
			{
				// Report error
				EventLog.WriteEntry( SOURCENAME, e.Message, EventLogEntryType.Error );
			}
			finally // cleanup after exception handled
			{
				if( dr != null )
				{
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			
			return strGKAccessPoint;
		}

		/*	Function gets the location of a gatekeeper 
		 *  Input: strGateKeeper - name of GateKeeper
		 *  Return: GateKeeper location if it exists, "" otherwise
		 */
		public string GetGateKeeperLocation( string strGKName )
		{
			string strGKLocation = "";
			OdbcDataReader dr = null;

			try
			{
				StringBuilder strSQL = new StringBuilder();
				strSQL.Append( "SELECT " + DBC.SS_GK_LOCATION );
				strSQL.Append( " FROM " + DBC.SERVICE_SPACES_TABLE );
				strSQL.Append( " WHERE " + DBC.SS_GK_NAME + "=" + "'" + DBUtil.MakeStringSafe( strGKName ) + "'" );
			
				dr = ExecuteReader( strSQL.ToString() );
				if( dr != null )
				{
					dr.Read(); // move reader past BOF to first record
					if( !dr.IsDBNull( 0 ) )
						strGKLocation = dr.GetString( 0 );
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			catch( System.Exception e )
			{
				// Report error
				EventLog.WriteEntry( SOURCENAME, e.Message, EventLogEntryType.Error );
			}
			finally // cleanup after exception handled
			{
				if( dr != null )
				{
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			
			return strGKLocation;
		}

		public string GetGateKeeperNamespace( string strGKName )
		{
			string strGKNamespace = "";
			OdbcDataReader dr = null;

			try
			{
				StringBuilder strSQL = new StringBuilder();
				strSQL.Append( "SELECT " + DBC.SS_GK_NAMESPACE ); 
				strSQL.Append( " FROM " + DBC.SERVICE_SPACES_TABLE );
				strSQL.Append( " WHERE " + DBC.SS_GK_NAME + "=" + "'" + DBUtil.MakeStringSafe( strGKName ) + "'" );
			
				dr = ExecuteReader( strSQL.ToString() );
				if( dr != null )
				{
					dr.Read(); // move reader past BOF to first record
					if( !dr.IsDBNull( 0 ) )
						strGKNamespace = dr.GetString( 0 );
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			catch( System.Exception e )
			{
				// Report error
				EventLog.WriteEntry( SOURCENAME, e.Message, EventLogEntryType.Error );
			}
			finally // cleanup after exception handled
			{
				if( dr != null )
				{
					if( !dr.IsClosed )
						dr.Close();
				}
			}
			
			return strGKNamespace;
		}
	}
}
