using System;
using System.IO;
using System.Net.Sockets;
using System.Diagnostics;

namespace PSL.DISCUS.Logging
{
	/// <summary>
	/// Summary description for UrlLoggerImpl.
	/// </summary>
	public class UrlLoggerImpl:LoggerImpl
	{
		private const string SOURCENAME = "PSL.DISCUS.Impl.Logging.UrlLoggerImpl";
		private string m_strHostname = "";
		public string Hostname
		{
			get
			{ return m_strHostname; }
			set
			{ 
				m_strHostname = value; 
				
				if( m_strHostname.Length == 0 )
					throw new Exception( "Invalid Logging Hostname" );
			}
		}

		private int m_nPort = -1;
		public int Port
		{
			get
			{ return m_nPort; }
			set
			{ 
				m_nPort = value;
 
				if( m_nPort <= 0 )
					throw new Exception( "Invalid Logging Port" );
			}
		}
		
		public UrlLoggerImpl( LogTraceContext logCtx )
		{
			if( logCtx.Hostname.Length == 0 || logCtx.Port <= 0 )
				throw new Exception( "Invalid Logging Hostname and/or Port" );

			m_strHostname = logCtx.Hostname;
			m_nPort = logCtx.Port;
		}

		public override void LogError( string strSource, string strMsg )
		{
			string strXmlMsg = "";
			try
			{
				// Format strMsg as XML
				// strXmlMsg = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n";
				// strXmlMsg += "<Source>" + m_strSource + "</Source>\n"; // Set source
				// strXmlMsg += "<MessageType>" + DConst.ERROR_MSG_TYPE + "</MessageType>\n"; // Set msg type
				// strXmlMsg += "<Message><![CDATA[" + strMsg + "]]></Message>\n"; // Set message

				// TODO: Remove later...For Demo only
				strXmlMsg += "LOG_" + strSource + " " + strMsg;

				// Send message (using TCP)
				TcpClient clientSocket = new TcpClient( m_strHostname, m_nPort );
				StreamWriter output = new StreamWriter( clientSocket.GetStream() );
				output.Write( strXmlMsg );
				// Flush stream before closing
				output.Flush();
				output.Close();
			}
			catch( Exception e )
			{
				EventLog.WriteEntry( strSource, "Error: " + e.Message + " occurred while sending data: " + strXmlMsg, EventLogEntryType.Error );
			}
		}

		public override void LogInfo( string strSource, string strMsg )
		{
			string strXmlMsg = "";
			try
			{
				// Format strMsg as XML
				// strXmlMsg = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n";
				// strXmlMsg += "<Source>" + m_strSource + "</Source>\n"; // Set source
				// strXmlMsg += "<MessageType>" + DConst.INFO_MSG_TYPE + "</MessageType>\n"; // Set msg type
				// strXmlMsg += "<Message><![CDATA[" + strMsg + "]]></Message>\n"; // Set message

				// TODO: Remove later...For Demo only
				strXmlMsg += "LOG_" + strSource + " " + strMsg;

				// Send message (using TCP)
				TcpClient clientSocket = new TcpClient( m_strHostname, m_nPort );
				StreamWriter output = new StreamWriter( clientSocket.GetStream() );
				output.Write( strXmlMsg );
				// Flush stream before closing
				output.Flush();
				output.Close();
			}
			catch( Exception e )
			{
				EventLog.WriteEntry( strSource, e.Message, EventLogEntryType.Error );
			}
		}

		public override void LogWarning( string strSource, string strMsg )
		{
			string strXmlMsg = "";
			try
			{
				// Format strMsg as XML
				// strXmlMsg = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n";
				// strXmlMsg += "<Source>" + m_strSource + "</Source>\n"; // Set source
				// strXmlMsg += "<MessageType>" + DConst.WARNING_MSG_TYPE + "</MessageType>\n"; // Set msg type
				// strXmlMsg += "<Message><![CDATA[" + strMsg + "]]></Message>\n"; // Set message

				// TODO: Remove later...For Demo only
				strXmlMsg += "LOG_" + strSource + " " + strMsg;

				// Send message (using TCP)
				TcpClient clientSocket = new TcpClient( m_strHostname, m_nPort );
				StreamWriter output = new StreamWriter( clientSocket.GetStream() );
				output.Write( strXmlMsg );
				// Flush stream before closing
				output.Flush();
				output.Close();
			}
			catch( Exception e )
			{
				EventLog.WriteEntry( strSource, e.Message, EventLogEntryType.Error );
			}
		}
	}
}
