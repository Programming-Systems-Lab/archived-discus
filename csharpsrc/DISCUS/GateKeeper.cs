using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using PSL.DISCUS.Interfaces;
using PSL.DISCUS.Impl.DataAccess;
using PSL.DISCUS.Impl.DynamicProxy;

// DISCUS GateKeeper package
namespace PSL.DISCUS.Impl.GateKeeper
{
	/// <summary>
	/// Sample implementation of a GateKeeper
	/// </summary>
	public class GateKeeper:IGateKeeper
	{		
		// DISCUD system config file
		private string CONFIG = DConst.DISCUSCONFIG_FILE;
		// Event log instance
		private EventLog m_EvtLog;
		// GateKeeper name
		private string m_strName = "";
		// Location where dynamically generated proxies are stored
		private string m_strPxyCacheDir = "";
		// Proxy Generator
		private ProxyGen m_pxyGen;

		/* Constructor */
		public GateKeeper()
		{
			try
			{
				m_pxyGen = new ProxyGen();
				// Initialize error logging facility
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
				// Get specific GateKeeper name
				m_strName = root.SelectSingleNode( "GateKeeperName" ).InnerText;

				// Specific sourcename
				if( m_strName.Length > 0 )
					m_EvtLog.Source = m_strName; 
				
				// Find out where we store dynamic proxies
				// if no dir specified create default 
				// proxy cache directory under the current directory
				m_strPxyCacheDir = root.SelectSingleNode( "ProxyCache" ).InnerText;
				if( m_strPxyCacheDir.Length == 0 )
					m_strPxyCacheDir = DConst.DEFAULT_PXY_DIR;

				fs.Close(); // Close file stream
				
				// Determine whether ProxyCache Directory exists
				// If it does not exist then we try to create it
				InitializeProxyCacheDir();
			}
			catch( System.Exception e ) // Catch exception
			{
				// Report error
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
		}

		/*	Function tests whether the proxy cache directory
		 *  exists, if it does then ok, otherwise we try to
		 *  create it.
		 *  Input: none
		 *  Return: true if we successfully set up cache dir 
		 *			otherwise false if we can't
		 */
		private bool InitializeProxyCacheDir()
		{
			bool bRetVal = false;
			
			try
			{
				// Determine if directory name of the proxy cache
				// read from the system config file is NOT Fully 
				// Quallified i.e <drive letter>:\<path>
				// If not Fully Qualified then we must prefix
				// with Current Directory
				if( m_strPxyCacheDir.IndexOf( ":" ) == -1 )
				{
					// Link to current dir
					DirectoryInfo temp = new DirectoryInfo( "." );
					// Get FullQName
					string strPath = temp.FullName;
					// Append proxy cache sub dir
					strPath += "\\";
					strPath += m_strPxyCacheDir;
					m_strPxyCacheDir = strPath;
					temp = null;
				}
				
				// Try to access DirectoryInfo of ProxyCache
				DirectoryInfo dirInfo = new DirectoryInfo( m_strPxyCacheDir );
				// If the directory does not exist then try creating it
				if( !dirInfo.Exists )
					Directory.CreateDirectory( dirInfo.FullName );
				
				bRetVal = true;
			}
			catch( System.IO.DirectoryNotFoundException e )
			{
				// If ProxyCache Directory does not exist then we
				// must create it
				try
				{
					string strTemp = e.Message;
					Directory.CreateDirectory( m_strPxyCacheDir ); 
					bRetVal = true;
				}
				catch( System.Exception sysE )
				{
					// Report Error
					string strError = "Error Creating ProxyCache ";
					strError += sysE.Message;
					m_EvtLog.WriteEntry( strError, EventLogEntryType.Error );
				}
			}
			return bRetVal;
		}
		
/*************************************************************/
//IResourceAcquire Impl	
		public void DissolveTreaty( int nTreatyID )
		{
		
		}
		
		public string EnlistServicesByName( string strXMLTreatyReq, string strXMLCredentials )
		{
			string strRetVal = "";
			
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
		/* Function executes a service method.
		 * Input: nTreatyID			- ID of the treaty to verify service use against
		 *		  strServiceName	- service we wish to use
		 *		  strServiceMethod	- service method we wisk to use
		 *		  paramaeters 		- array of parameters to pass to service method	
		 */
		public Object ExecuteServiceMethod( int nTreatyID, string strServiceName, string strServiceMethod, Object[] parameters )
		{
			// Verify treatyID, method name and params

			// Runtime resolution of web service references stored in dbase
			// lookup service name and method
			// if servicelocation contains "http://" then
			// service is a web service and a proxy may have to be 
			// generated @ runtime, the generated proxy is stored
			// locally, the local proxy location may replace the web ref.
			
			// Check registered service information
			RegServiceDAO servInfo = new RegServiceDAO();
			object objResult = null;
			// Get service location
			string strServiceLocation = servInfo.GetServiceLocation( strServiceName );
			string strServiceAccessPoint = servInfo.GetServiceAccessPoint( strServiceName );
			// Get Service namespace
			string strServiceNamespace = servInfo.GetServiceNamespace( strServiceName );
            if( strServiceLocation.Length == 0 )
				return null;
			// Verify that service supports method
			if( !servInfo.MethodExists( strServiceName, strServiceMethod ) )
				return null;
			
			// If service location is a link to a WSDL file
			// then we must generate a proxy to the webservice,
			// store the proxy in the proxy cache, update our
			// database and set strServiceLocation to the generated
			// proxy. Must also change service namespace in dbase
			string strAssembly = "";
			
			if( strServiceLocation.ToLower().StartsWith( "http://" ) )
			{
				// Create a dynamic request
				DynamicRequest req = new DynamicRequest();	
				// Resolve service name
				int nIndex = strServiceName.LastIndexOf( "." );
				if( nIndex == -1 )
					req.serviceName = strServiceName;
				else req.serviceName = strServiceName.Substring( nIndex + 1 );

				// Set properties of the Dynamic request
				req.filenameSource = req.serviceName;
				// Location of wsdl file to use for proxy generation
				req.wsdlFile = strServiceLocation;
				// Service Access Point
				req.baseURL = strServiceAccessPoint;
				if( req.baseURL.Length == 0 )
				{
					// Issue warning, Proxy will ONLY function as expected if
					// WSDL file contains the service access point
					string strMsg = "Service did not provide a Base URL (Service Access Point), generated proxy may not function as expected";
					m_EvtLog.WriteEntry( strMsg, EventLogEntryType.Warning );
				}
				// Where to store generated proxy
				req.proxyPath = m_strPxyCacheDir;
				// Pass Dynamic request to proxy generator
				strAssembly = m_pxyGen.GenerateAssembly( req );
				// If no assembly generated, report error and exit
				if( strAssembly.Length == 0 )
				{
					// Report error generating proxy
					string strError = "Error generating proxy to WSDL ref: ";
					strError += strServiceLocation;
					m_EvtLog.WriteEntry( strError, EventLogEntryType.Error );
					return null;
				}
				
				// Update database location of service, point to
				// dynamic proxy, change namespace the dynamic proxy namespace
				servInfo.UpdateServiceLocation( strServiceName, strAssembly );
				servInfo.UpdateServiceNamespace( strServiceName, req.dynNamespace );
				// Set Service name to Fully qualified 
				// name i.e <Namespace>.<servicename>
				// necessary for reflection
				strServiceName = req.dynNamespace + "." + req.serviceName;
			}
			else
			{
				// Fully qualify service name with namespace
				// necesary for reflection
				if( strServiceNamespace.Length > 0 )
					strServiceName = strServiceNamespace + "." + strServiceName;
				// Set assembly location
				strAssembly = strServiceLocation;
			}
 						
			// Use reflection to execute method
			try
			{
				// Load assembly from file
				Assembly a = Assembly.LoadFrom( strAssembly );
				// Get the correct type (typename must be fully qualified with namespace)
				Type tDyn = a.GetType( strServiceName );
				
				// Create an instance of the type
				Object obj = Activator.CreateInstance( tDyn );
				try
				{
					PropertyInfo Url = tDyn.GetProperty( "Url" );
					// Check whether object has a property
					// called Url, valid for generated
					// proxies, if it does check its value
					// if value is empty fill in proxy access point
					if( Url != null )
					{
						string strUrl = (string) Url.GetValue( obj, null );
						if( strUrl.Length == 0 )
							Url.SetValue( obj, strServiceAccessPoint, null );
					}
				}
				catch( System.Exception e )
				{
					string strE = e.Message;
				}
				
				// Invoke method
				objResult = tDyn.InvokeMember( strServiceMethod, 
											   BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
											   null,
											   obj,
											   parameters ); 
			}
			catch( System.Exception e )
			{
				string strError = "Error Executing Method: ";
				strError += strServiceMethod + " of service: ";
				strError += strServiceName + " errmsg-> ";
				strError += e.Message;
				m_EvtLog.WriteEntry( strError, EventLogEntryType.Error );
			}

			// return result
			return objResult;
		}// End ExecuteServiceMethod
	}// End GateKeeper
}
