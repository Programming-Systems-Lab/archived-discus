using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using PSL.DISCUS.Interfaces;
using PSL.DISCUS.Impl.DataAccess;

namespace PSL.DISCUS.Impl.GateKeeper
{
	/// <summary>
	/// Summary description for GateKeeper.
	/// </summary>
	public class GateKeeper:IGateKeeper
	{		
		private string CONFIG = DConst.DISCUSCONFIG_FILE;
		private EventLog m_EvtLog;
		private string m_strName = "";
		private string m_strPxyCacheDir = "";
		
		public GateKeeper()
		{
			try
			{
				m_EvtLog = new EventLog( "Application" );
				m_EvtLog.Source = "GateKeeper"; //Generic source name

				// Read DISCUS config file
				FileStream fs = File.Open( CONFIG, FileMode.Open );
				TextReader tr = new StreamReader( fs );
				string strConfigFile = tr.ReadToEnd();

				// Load config file into XML document
				XmlDocument doc = new XmlDocument();
				doc.LoadXml( strConfigFile );
				
				// Use XPath to extract what info we need
				XmlNode root =  doc.DocumentElement;
				m_strName = root.SelectSingleNode( "GateKeeperName" ).InnerText;

				if( m_strName.Length > 0 )
					m_EvtLog.Source = m_strName; // Specific sourcename
				
				m_strPxyCacheDir = root.SelectSingleNode( "ProxyCache" ).InnerText;
				if( m_strPxyCacheDir.Length == 0 )
					m_strPxyCacheDir = DConst.DEFAULT_PXY_DIR;

				// Determine whether ProxyCache Directory exists
				// If it does not exist then we try to create it

				fs.Close();
			}
			catch( System.Exception e )
			{
				// Report error
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
		}

/*************************************************************/
//IResourceAcquire Impl	
		public void DissolveTreaty( int nTreatyID )
		{
		
		}
		
		public string EnlistServicesByName( string strXMLTreatyReq, string strXMLCredentials )
		{
			string strRetVal = "";
			try
			{
				
			}
			catch( System.Xml.XmlException e )
			{
				string strMessage = e.Message;
			}
			catch( System.Exception e )
			{
				string strMessage = e.Message;
			}

			return strRetVal;
		}

		public bool RequestServiceByName( string strServiceName, string strXMLCredentials )
		{
			// Validate Credentials
			
			// Do lookup in dbase for service

			return false;
		}

		public void RevokeTreaty( int nTreatyID )
		{
		
		}
/**************************************************************/
//IExecuteService Impl		
		public Object ExecuteServiceMethod( int nTreatyID, string strServiceName, string strServiceMethod, Object[] parameters )
		{
			// Verify treatyID, method name and params

			// Runtime resolution of web service references stored in dbase
			// lookup service name and method
			// if servicelocation contains "http://" then
			// service is a web service and a proxy may have to be 
			// generated @ runtime, the generated proxy is stored
			// locally, the local proxy location may replace the web ref
			// Gatekeepers need to have a local dir/cache where runtime
			// generated proxy dlls are stored
			
			RegServiceDAO servInfo = new RegServiceDAO();
			object objResult = null;
			// Get assembly info (proxy dll loc)
			string strAssembly = servInfo.GetServiceLocation( strServiceName );
            if( strAssembly.Length == 0 )
				return null;
			
			// Verify that service supports method
			if( !servInfo.MethodExists( strServiceName, strServiceMethod ) )
				return null;
			
			// Use reflection to execute method
			try
			{
				// load assembly from file
				Assembly a = Assembly.LoadFrom( strAssembly );
				// Get the correct type (typename must be fully qualified with namespace)
				Type tDyn = a.GetType( strServiceName );
				// Create an instance of the type
				Object obj = Activator.CreateInstance( tDyn );
				// Invoke method
				objResult = tDyn.InvokeMember( strServiceMethod, 
											   BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
											   null,
											   obj,
											   parameters ); 
			}
			catch( System.Exception e )
			{
				string strError = e.Message;
			}

			// return object
			return objResult;
		}
	}
}
