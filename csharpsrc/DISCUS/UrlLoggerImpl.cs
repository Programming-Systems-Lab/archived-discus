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
		// In the event that something goes wrong sending data over the network
		// record error in local event log
		private EventLog m_EvtLogger;
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

		public override string Source
		{
			get
			{ return m_strSource; }

			set
			{ 
				m_strSource = value; 
				
				if( m_strSource.Length == 0 )
					throw new Exception( "Invalid Logging Source" );
			}
		}
		
		public UrlLoggerImpl( LogTraceContext logCtx )
		{
			m_EvtLogger = new EventLog( "Application" );
			m_EvtLogger.Source = "PSL.DISCUS.Impl.Logging.UrlLoggerImpl";

			m_strHostname = logCtx.Hostname;
			m_nPort = logCtx.Port;

			if( m_strHostname.Length == 0 || m_nPort <= 0 )
				throw new Exception( "Invalid Logging Hostname and/or Port" );
		}

		public override void LogError( string strMsg )
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
				strXmlMsg += "LOG_" + m_strSource + " " + strMsg;

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
				m_EvtLogger.WriteEntry( "Error: " + e.Message + " occurred while sending data: " + strXmlMsg, EventLogEntryType.Error );
			}
		}

		public override void LogInfo( string strMsg )
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
				strXmlMsg += "LOG_" + m_strSource + " " + strMsg;

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
				m_EvtLogger.WriteEntry( e.Message, EventLogEntryType.Error );
			}
		}

		public override void LogWarning( string strMsg )
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
				strXmlMsg += "LOG_" + m_strSource + " " + strMsg;

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
				m_EvtLogger.WriteEntry( e.Message, EventLogEntryType.Error );
			}
		}
	}
}
