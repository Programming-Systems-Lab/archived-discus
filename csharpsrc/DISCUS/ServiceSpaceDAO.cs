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
	/// Summary description for ServiceSpaceDAO.
	/// </summary>
	public class ServiceSpaceDAO:IDataObj
	{
		// Database configuration file
		private string DATACONFIG = DConst.DBASECONFIG_FILE;
		private const string SOURCENAME = "DataAccess.ServiceSpaceDAO";
		private EventLog m_EvtLog;
		private string m_strConnect = "";
		
		public ServiceSpaceDAO()
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

		public bool RegisterGateKeeper( string strGateKeeper, string strWSDLURL )
		{
			bool bRetVal = false;
			
			// strWSDLURL NOT URL encoded string

			if( strGateKeeper.Length == 0 || strWSDLURL.Length == 0 )
				return false;

			string strSQL = "INSERT INTO ";
			strSQL += DBC.SERVICE_SPACES_TABLE;
			strSQL += "(" + DBC.SS_GATEKEEPER + "," + DBC.SS_WSDL_FILE + ")";
			strSQL += " VALUES(" + "'" + DBUtil.MakeStringSafe( strGateKeeper ) + "'" + "," + "'" + DBUtil.MakeStringSafe( strWSDLURL ) + "'" + ")";

			bRetVal = ExecuteCommandText( strSQL );

			return bRetVal;
		}

		public bool UnregisterGateKeeper( string strGateKeeper )
		{
			bool bRetVal = false;
			
			string strSQL = "DELETE FROM ";
			strSQL += DBC.SERVICE_SPACES_TABLE;
			strSQL += " WHERE " + DBC.SS_GATEKEEPER + "=" + "'" + DBUtil.MakeStringSafe(strGateKeeper) + "'";

			bRetVal = ExecuteCommandText( strSQL );

			return bRetVal;
		}

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
