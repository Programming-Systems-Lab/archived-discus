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
	/// Summary description for ServiceDAO.
	/// </summary>
	public class RegServiceDAO:IDataObj
	{
		// Database configuration file
		private string DATACONFIG = DConst.DBASECONFIG_FILE;
		private const string SOURCENAME = "DataAccess.RegServiceDAO";
		private EventLog m_EvtLog;
		//private bool m_bInit; 
		private string m_strConnect = "";
				
		public RegServiceDAO()
		{
			try
			{
				m_EvtLog = new EventLog( "Application" );
				m_EvtLog.Source = SOURCENAME;
				
				// load config info, dbase, connection info etc.
				FileStream fs = File.Open( DATACONFIG, FileMode.Open );
				TextReader tr = new StreamReader( fs );
				string strConfigFile = tr.ReadToEnd();

				// Load config file
				XmlDocument doc = new XmlDocument();
				doc.LoadXml( strConfigFile );
				
				// Use XPath to extract what info we need
				XmlNode root =  doc.DocumentElement;
				m_strConnect = root.SelectSingleNode( "ConnectionString" ).InnerText;

				fs.Close();
			}
			catch( System.Exception e )
			{
				// Report error
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
		}

		public bool ExecuteCommandText( string strCmd )
		{
			bool bRetVal = false;
		
			try
			{
				OdbcConnection Conn = new OdbcConnection( m_strConnect );
				OdbcCommand Command = new OdbcCommand(strCmd);
				Command.Connection = Conn;
				Conn.Open();
				Command.ExecuteReader();
				Conn.Close();
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

		/* Add service to dbase, return service ID */
		public int RegisterService( string strServiceName, string strServiceLoc )
		{
			if( strServiceName.Length == 0 || strServiceLoc.Length == 0 )
				return 0;
			
			int nServiceID = -1;
			string strSQL = "INSERT INTO ";
			strSQL += DBC.REGISTERED_SERVICES_TABLE;
			strSQL += " (" + DBC.RS_SERVICE_NAME + "," + DBC.RS_SERVICE_LOCATION + ")";
			strSQL += " VALUES(" + "'" + DBUtil.MakeStringSafe(strServiceName) + "'" + "," + "'" + DBUtil.MakeStringSafe(strServiceLoc) + "'" + ")";

			if( ExecuteCommandText( strSQL ) )
				nServiceID = GetServiceID( strServiceName );
			
			return nServiceID;
		}
		
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

		// Service must be in REGISTERED_SERVICES table before methods can be called
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
		}
	}
}
