using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Collections;
using System.Security.Permissions;
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

		// Inner ExecRequext class
		class ExecRequest
		{
			public string m_strGKName;
			public string m_strServiceName;
			public string m_strServiceMethod;
			public ArrayList m_XMLParameters;
			public int m_nTreatyID;
		
			public ExecRequest()
			{
				m_XMLParameters = new ArrayList();
				m_nTreatyID = -1; // TreatyID exec request assoc with
			}

			public string ToXML()
			{
				string strXMLReq = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
				strXMLReq += "<ExecServiceMethodRequest>";
				strXMLReq += "<TreatyID>";
				strXMLReq += m_nTreatyID.ToString();	
				strXMLReq += "</TreatyID>";
				strXMLReq += "<ServiceName>";
				strXMLReq += m_strServiceName;
				strXMLReq += "</ServiceName>";
				strXMLReq += "<MethodName>";
				strXMLReq += m_strServiceMethod;
				strXMLReq += "</MethodName>";
						
				if( m_XMLParameters.Count > 0 )
				{
					Object[] arrParams = m_XMLParameters.ToArray();

					for( int i = 0; i < arrParams.Length; i++ )
					{
						strXMLReq += "<Parameter>";
						strXMLReq += "<![CDATA[";
						strXMLReq += (string) arrParams[i];
						strXMLReq += "]]>";
						strXMLReq += "</Parameter>";
					}
				}
				strXMLReq += "</ExecServiceMethodRequest>";

				return strXMLReq;
			}
		};
	
		/* Constructor */
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
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
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
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
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public string ExecuteServiceMethod( string strXMLExecRequest )
		{
			string strRetVal = "";
			string strError = "";

			try
			{
				// 7 stages for execution
				// 1) Validate schema of strXMLExecRequest
				// 2) Do Security check
				// 3) Extraction of parameters
				// 4) Resolve Web Service Proxy Location (Proxy may have to be generated at runtime)
				// 5) Runtime Deserializtion of parameters
				// 6) Proxy Method Invocation
				// 7) Serialize and return results
			
				// Stage 1

				// Stage 2

				// Stage 3 - Extraction of parameters
				InternalRegistry ireg = new InternalRegistry();
				// Load Execution request
				XmlDocument doc = new XmlDocument();
				doc.LoadXml( strXMLExecRequest );
			
				XmlNode root = doc.DocumentElement;
				// Get TreatyID
				XmlNode treatyID = root.SelectSingleNode( "/ExecServiceMethodRequest/TreatyID" );
				// Get Service Name
				XmlNode serviceName = root.SelectSingleNode( "/ExecServiceMethodRequest/ServiceName" );
				// Get MethodName
				XmlNode serviceMethod = root.SelectSingleNode( "/ExecServiceMethodRequest/MethodName" );
				
				string strServiceName = serviceName.InnerText;
				string strServiceMethod = serviceMethod.InnerText;
				string strServiceLocation = ireg.GetServiceLocation( strServiceName );
				string strServiceNamespace = ireg.GetServiceNamespace( strServiceName );
				string strServiceAccessPoint = ireg.GetServiceAccessPoint( strServiceName );

				// Quick checks for valid data returned from database
				if( strServiceLocation.Length == 0 )
					return "<GKError>Service Not Found</GKError>";
				// Verify that service supports method
				if( !ireg.MethodExists( strServiceName, serviceMethod.InnerText ) )
					return "<GKError>Service Method Not Found</GKError>";

				// Get Paramters
				// Parameters must be passed in correct order
				XmlNodeList lstParamNodes = root.SelectNodes( "/ExecServiceMethodRequest/Parameter" );
				// Create array of XML docs representing parameters
				string[] arrParams = new string[lstParamNodes.Count];
				int y = 0;
				// Extract each XML document
				foreach( XmlNode n in lstParamNodes )
				{
					// If inner text is <string></string>
					// force all <string>s to be interpreted
					// literally
					string strTemp = n.InnerText;
					int nFirstIndex = strTemp.IndexOf( "<string>" );
					int nLastIndex = strTemp.LastIndexOf( "</string>" );
					if( nFirstIndex != -1 && nLastIndex != -1 )
					{
						int nStartIndex = nFirstIndex + 8;
						int nLength = nLastIndex - nStartIndex;
						string strNewString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><string><![CDATA[";
							strNewString += strTemp.Substring( nStartIndex, nLength );
						strNewString += "]]></string>";
						arrParams[y] = strNewString;
					}
					else arrParams[y] = n.InnerText;
					
					y++;
				}
				
				// Stage 4 - Resolve Web Service Proxy Location
				// Do Runtime ProxyGeneration if necessary				
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
					req.serviceName = strServiceName;
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
						strError = "Error generating proxy to WSDL ref: ";
						strError += strServiceLocation;
						m_EvtLog.WriteEntry( strError, EventLogEntryType.Error );
						return "<GKError>Proxy Generation Failed</GKError>";
					}
				
					// Update database location of service, point to
					// dynamic proxy, change namespace the dynamic proxy namespace
					ireg.UpdateServiceLocation( strServiceName, strAssembly );
					ireg.UpdateServiceNamespace( strServiceName, req.dynNamespace );
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
				
				// Stage 5 - Runtime Deserializtion of parameters
				// Load Assembly containing web service proxy
				bool bProxyMethodHasParameters = false;
				Assembly a = Assembly.LoadFrom( strAssembly );
				// Get the correct type (ProxyClass)
				Type ProxyType = a.GetType( strServiceName );
				// Create an instance of the Proxy Class
				Object objProxy = a.CreateInstance( strServiceName );
				if( objProxy == null || ProxyType == null )
				{
					strError = "Cannot create type/proxy instance ";
					strError += strServiceName;
					strError += " in assembly ";
					strError += strAssembly;
					m_EvtLog.WriteEntry( strError, EventLogEntryType.Error );
					return "<GKError>Reflection Failed</GKError>";
				}
				
				// Check whether Proxy class has a property
				// called Url, valid for generated
				// proxies, if it does check its value
				// if value is empty fill in proxy access point
				PropertyInfo Url = ProxyType.GetProperty( "Url" );
				if( Url != null )
				{
					string strUrl = (string) Url.GetValue( objProxy, null );
					if( strUrl.Length == 0 )
						Url.SetValue( objProxy, strServiceAccessPoint, null );
				}

				// Once we have a Proxy Object and a Type instance
				// use reflection to get info on method to be
				// executed.
				MethodInfo mInfo = ProxyType.GetMethod( strServiceMethod );
				if( mInfo == null )
				{
					strError = "Cannot find method ";
					strError += strServiceMethod;
					strError += " of Proxy ";
					strError += strServiceName;
					strError += " loaded from assembly ";
					strError += strAssembly;
					m_EvtLog.WriteEntry( strError, EventLogEntryType.Error );
					return "<GKError>Service Method Not Found</GKError>";
				}
				
				// Get info on parameters expected by Proxy method
				ParameterInfo[] arrParamInfo = mInfo.GetParameters();
				if( arrParamInfo.Length > 0 )
					bProxyMethodHasParameters = true;

				Object[] param = null;

				if( bProxyMethodHasParameters )
				{
					// Parameters passed not equal to
					// parameters expected
					// (is method overloading an issue?? - probably not with dynamically generated proxies)
					if( arrParams.Length != arrParamInfo.Length )
						return "<GKError>Wrong Number of Arguments Passed</GKError>";
					
					// Create array to hold parameters
					param = new Object[arrParamInfo.Length];

					// Try deserialization
					for( int i = 0; i < arrParamInfo.Length; i++ )
					{
						// Get the expected type
						Type paramType = arrParamInfo[i].ParameterType;
						// Create XmlSerializer
						XmlSerializer xs = new XmlSerializer( paramType );
						// Read in Xml doc representing parameter
						System.Xml.XmlReader xt = new XmlTextReader( arrParams[i], XmlNodeType.Document, null );
						xt.Read(); 
						// Deserialize
						Object paramInst = xs.Deserialize( xt );
						// Store in parameter array
						param[i] = (Object) paramInst;
					}
				}// End if bProxyMethodHasParameters
				
				// Stage 6 - Proxy Method Invocation
				Object objInvokeResult = null;
				if( bProxyMethodHasParameters )
					objInvokeResult = ProxyType.InvokeMember( strServiceMethod,
						BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
						null,
						objProxy,
						param );
 
				else objInvokeResult = ProxyType.InvokeMember( strServiceMethod,
						 BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
						 null,
						 objProxy,
						 null );

				// Stage 7 - Serialize and return results
				if( objInvokeResult == null )
					return "";
				
				// Otherwise serialize results to XML
				// Get returned type
				Type returnType = objInvokeResult.GetType();
				// Create XmlSerializer
				XmlSerializer ser = new XmlSerializer( returnType );
				// Create a memory stream
				MemoryStream ms = new MemoryStream();
				// Serialize to stream ms
				ser.Serialize( ms, objInvokeResult );
				// Goto start of stream
				ms.Seek( 0, System.IO.SeekOrigin.Begin );
				// Create a stream reader
				TextReader reader = new StreamReader( ms );
				// Read entire stream, this is our return value
				strRetVal = reader.ReadToEnd();

				// Close reader
				reader.Close();
				// Close stream
				ms.Close();
			}
			catch( System.Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}
		
		/* Function executes a service method.
		 * Input: nTreatyID			- ID of the treaty to verify service use against
		 *		  strServiceName	- service we wish to use
		 *		  strServiceMethod	- service method we wisk to use
		 *		  paramaeters 		- array of parameters to pass to service method	
		 */
		/*[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
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
			InternalRegistry ireg = new InternalRegistry();
			object objResult = null;
			// Get service location
			string strServiceLocation = ireg.GetServiceLocation( strServiceName );
			string strServiceAccessPoint = ireg.GetServiceAccessPoint( strServiceName );
			// Get Service namespace
			string strServiceNamespace = ireg.GetServiceNamespace( strServiceName );
            if( strServiceLocation.Length == 0 )
				return null;
			// Verify that service supports method
			if( !ireg.MethodExists( strServiceName, strServiceMethod ) )
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
				ireg.UpdateServiceLocation( strServiceName, strAssembly );
				ireg.UpdateServiceNamespace( strServiceName, req.dynNamespace );
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
		
		*/

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public string[] ExecuteAlphaProtocol( string strAlphaProtocol )
		{
			if( strAlphaProtocol.Length == 0 )
				return null;

			// 6 Stages to Alpha Protocol Execution
			// 1) Verify that alpha-protocol is valid
			// 2) Get the list of actions to be performed
			// 3) Create the Execution requests
			// 4) Create nec treaties with other GKs - Resource Acquire stage
			// 5) Execution stage
			// 6) Fill in array of returned values (if any)

			string [] arrRetVal = null; // Holds returned values
			
			// Execution queue
			Queue execQ = new Queue();

			try
			{
				// Stage 1 - Verify alpha-protocol is valid
				// Create XML text reader
				XmlTextReader xt = new XmlTextReader( strAlphaProtocol, XmlNodeType.Document, null );
				// Add namespaces expected in alpha protocol so our xpath queries work
				XmlNamespaceManager mgr = new XmlNamespaceManager( xt.NameTable );
				mgr.AddNamespace( "xlang", "http://schemas.microsoft.com/bixtalk/xlang" );
				mgr.AddNamespace( "xs", "http://www.w3.org/2001/XMLSchema" );
				// Load document					
				XmlDocument doc = new XmlDocument();
				doc.Load( xt );
				// Get the sequence of steps to be executed		
				XmlNode root = doc.DocumentElement;
				XmlNode seq = root.SelectSingleNode( "/definitions/xlang:behavior/xlang:body/xlang:sequence", mgr );
				
				if( seq == null || !seq.HasChildNodes )
					throw new Exception( "Nothing to do" );

				// Stage 2 - Get list of actions to be performed
				// Every sequence should have a list of actions to be performed
				// The first action must be an activation action
				// otherwise the alpha-protocol is invalid
				XmlNode startNode = seq.FirstChild;
				XmlAttributeCollection attCol = startNode.Attributes;
				XmlNode activation = attCol.GetNamedItem( "activation" );
				if( activation == null || activation.Value.ToLower().CompareTo( "true" ) != 0 )
					throw new Exception( "Invalid Alpha-Protocol...No Activation Action" );

				// Stage 3 - Create Execution requests
				XmlNodeList lstActions = seq.ChildNodes;
				// For each action create an execution request
				// and add to a queue, an execution request
				// is simply an xml document
				foreach( XmlNode action in lstActions )
				{
					// Create an attribute collection
					XmlAttributeCollection att = action.Attributes;
					// An action must have attributes
					if( att.Count == 0 )
						throw new Exception( "Invalid Alpha-Protocol - no attributes specified" );

					ExecRequest e = new ExecRequest();
					// Get the Gatekeeper that will service request
					XmlNode val = att.GetNamedItem( "gatekeeper" );
					if( val == null ) // May mean use local only...handle this later
						throw new Exception( "No Provider GateKeeper" );
					e.m_strGKName = val.Value;
					
					// Get the name of the service we want to use
					val = att.GetNamedItem( "servicename" );
					if( val == null )
						throw new Exception( "No Service To Use" );
					e.m_strServiceName = val.Value;

					// Get the operation/method we are supposed to request
					val = att.GetNamedItem( "operation" );
					if( val == null )
						throw new Exception( "No Service Method Specified" );
					e.m_strServiceMethod = val.Value;
					
					// Get any parameters the action may have
					if( action.HasChildNodes )
					{
						XmlNodeList lstParams = action.ChildNodes;
						// Parameters should be well formed
						// XML documents (surrounded by
						// CDATA modifiers to prevent parsing)
						foreach( XmlNode param in lstParams )
						{
							// Just add XML document to list
							e.m_XMLParameters.Add( param.InnerText );
						}
					}// End if action has child nodes
					// Add request to execution queue
					execQ.Enqueue( e );
				}// End for each action in lstActions
				
				// Stage 4 - Create nec treaties with other GKs
				// Resource Acquire stage
				// Determine whether requests can be serviced locally
				// Need to create treaty/treaties with all
				// GateKeepers that must be contacted
				// Need to copy execution Q to a list, sort by GKName
				// & create a consolidated treaty to be passed to
				// any external GK requesting use of its services
				
				// Stage 5 - Execution stage
				IEnumerator it = execQ.GetEnumerator();
				InternalRegistry ireg = new InternalRegistry();
				string strGKLocation = "";
				string strGKAccessPoint = "";
				string strGKNamespace = "";
				string strAssembly = "";
				
				// Create appropriate sized array to hold results
				arrRetVal = new string[execQ.Count];
				int nIndex = 0;
				// Go thru execution queue servicing each request
				while( it.MoveNext() )
				{
					ExecRequest toExec = (ExecRequest) it.Current;
						
					strGKLocation = ireg.GetGateKeeperLocation( toExec.m_strGKName );
					strGKAccessPoint = ireg.GetGateKeeperLocation( toExec.m_strGKName );
					strGKNamespace = ireg.GetGateKeeperNamespace( toExec.m_strGKName );
					
					if( strGKLocation.Length == 0 )
						continue; // should throw exception??
					// May have to go find gatekeeper

					if( strGKLocation.ToLower().StartsWith( "http://" ) )
					{
						// Create a dynamic request
						DynamicRequest req = new DynamicRequest();	
						req.serviceName = toExec.m_strGKName;
						// Set properties of the Dynamic request
						req.filenameSource = req.serviceName;
						// Location of wsdl file to use for proxy generation
						req.wsdlFile = strGKLocation;
						// Service Access Point
						req.baseURL = strGKAccessPoint;
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
							strError += strGKLocation;
							m_EvtLog.WriteEntry( strError, EventLogEntryType.Error );
							throw new Exception( strError );
						}
				
						// Update database location of service, point to
						// dynamic proxy, change namespace the dynamic proxy namespace
						ireg.UpdateGateKeeperLocation( toExec.m_strGKName, strAssembly );
						ireg.UpdateGateKeeperNamespace( toExec.m_strGKName, req.dynNamespace );
						// Set Service name to Fully qualified 
						// name i.e <Namespace>.<servicename>
						// necessary for reflection
						toExec.m_strGKName = req.dynNamespace + "." + req.serviceName;
					}
					else
					{
						if( strGKNamespace.Length > 0 )
							toExec.m_strGKName = strGKNamespace + "." + toExec.m_strGKName;
						strAssembly = strGKLocation;
					}
		
					// Load assembly from file
					Assembly a = Assembly.LoadFrom( strAssembly );
					// Get the correct type (typename must be fully qualified with namespace)
					Type GKProxyType = a.GetType( toExec.m_strGKName );
					// Create an instance of the type
					Object objGKProxy = a.CreateInstance( toExec.m_strGKName );
					PropertyInfo Url = GKProxyType.GetProperty( "Url" );
					// Check whether object has a property
					// called Url, valid for generated
					// proxies, if it does check its value
					// if value is empty fill in proxy access point
					if( Url != null )
					{
						string strUrl = (string) Url.GetValue( objGKProxy, null );
						if( strUrl.Length == 0 )
							Url.SetValue( objGKProxy, strGKAccessPoint, null );
					}
					
					// Trace
					string strTrace = "Executing method: ";
					strTrace += toExec.m_strServiceMethod;
					strTrace += " against GK: ";
					strTrace += toExec.m_strGKName;
					m_EvtLog.WriteEntry( strTrace, EventLogEntryType.Information );
					
					// GK expects one parameter - an XML document
					Object[] parameters = new Object[1];
					
					// Create XML document (execute service method request)
					string strXMLReq = toExec.ToXML();
					// Set parameter to pass to GK proxy
					parameters[0] = strXMLReq;
	
					// Invoke method
					Object objResult = GKProxyType.InvokeMember( "ExecuteServiceMethod", 
							BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
							null,
							objGKProxy,
							parameters ); 
						
					if( objResult != null )
					{
						// save string
						arrRetVal[nIndex] = (string) objResult;
					}
					nIndex++;
				} // End while it.MoveNext
			}
			catch( System.Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			return arrRetVal;
		}
		
		/*[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public int ExecuteAlphaProtocol( string strAlphaProtocol )
		{
			if( strAlphaProtocol.Length == 0 )
				return 0;

			try
			{
				Queue execQ = new Queue();
				XmlTextReader xt = new XmlTextReader( strAlphaProtocol, XmlNodeType.Document, null );

				// Execute an alpha protocol from file
				// XmlTextReader xt = new XmlTextReader( "DemoAlpha.xml" );
				// Add namespaces expected in alpha protocol so our xpath queries work
				XmlNamespaceManager mgr = new XmlNamespaceManager( xt.NameTable );
				mgr.AddNamespace( "xlang", "http://schemas.microsoft.com/bixtalk/xlang" );
				mgr.AddNamespace( "xs", "http://www.w3.org/2001/XMLSchema" );
									
				XmlDocument doc = new XmlDocument();
				doc.Load( xt );
					
				XmlNode root = doc.DocumentElement;
				XmlNode seq = root.SelectSingleNode( "//xlang:sequence", mgr );
					
				// Check whether first child in sequence starts an activation
				// if not sequence is invalid
					
				// Get all children of sequence, these are the actions to be carried out
				if( seq.HasChildNodes )
				{
					XmlNode startNode = seq.FirstChild;
					XmlAttributeCollection attCol = startNode.Attributes;
					XmlNode activation = attCol.GetNamedItem( "activation" );
					if( activation == null || activation.Value.ToLower().CompareTo( "true" ) != 0 )
						return 0;  //invalid sequence
						
					// Create list of actions
					XmlNodeList lstActions = seq.ChildNodes;
					// For each action create an execution request
					// and add to a queue
					foreach( XmlNode n in lstActions )
					{
						XmlAttributeCollection att = n.Attributes;
						if( att.Count > 0 )
						{
							ExecutionRequest e = new ExecutionRequest();
							XmlNode val = att.GetNamedItem( "gatekeeper" );
							if( val != null )
								e.m_strGKName = val.Value;
							val = att.GetNamedItem( "servicename" );
							if( val != null )
								e.m_strServiceName = val.Value;
							val = att.GetNamedItem( "operation" );
							if( val != null )
								e.m_strMethod = val.Value;
							if( n.HasChildNodes )
							{
								XmlNodeList lstParams = n.ChildNodes;
								foreach( XmlNode p in lstParams )
								{
									XmlAttributeCollection paramAtt = p.Attributes;
									XmlNode type = paramAtt.GetNamedItem( "type" );
									if( type == null )
										return 0; //all specified params must have a (simeple xsd) type 
									string strType = type.Value;

									// Do mapping from (xsd type) xs:xyz to system type
									if( strType.ToLower().IndexOf( "xs:string" ) != -1 )
									{
										Object obj = p.InnerText;
										e.m_Params.Add( obj );
									}
									if( strType.ToLower().IndexOf( "xs:int" ) != -1 )
									{
										Object obj = Int32.Parse( p.InnerText );
										e.m_Params.Add( obj );
									}
								}
							}
							// Add request to execution queue
							execQ.Enqueue( e );
						}
					}

					// Determine whether requests can be serviced locally
					// Need to create treaty/treaties with all
					// GateKeepers that must be contacted
					// Need to copy execution Q to a list, sort by GKName
					// & create a consolidated treaty to be passed to
					// any external GK requesting use of its services
											
					// Service each execution request
					IEnumerator it = execQ.GetEnumerator();
					InternalRegistry ireg = new InternalRegistry();
					string strGKLocation = "";
					string strGKAccessPoint = "";
					string strGKNamespace = "";
					string strAssembly = "";

					while( it.MoveNext() )
					{
						ExecutionRequest exec = (ExecutionRequest) it.Current;
						
						strGKLocation = ireg.GetGateKeeperLocation( exec.m_strGKName );
						strGKAccessPoint = ireg.GetGateKeeperLocation( exec.m_strGKName );
						strGKNamespace = ireg.GetGateKeeperNamespace( exec.m_strGKName );
						
						if( strGKLocation.Length == 0 )
							continue;

						if( strGKLocation.ToLower().StartsWith( "http://" ) )
						{
							// Create a dynamic request
							DynamicRequest req = new DynamicRequest();	
							req.serviceName = exec.m_strGKName;
							// Set properties of the Dynamic request
							req.filenameSource = req.serviceName;
							// Location of wsdl file to use for proxy generation
							req.wsdlFile = strGKLocation;
							// Service Access Point
							req.baseURL = strGKAccessPoint;
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
								strError += strGKLocation;
								m_EvtLog.WriteEntry( strError, EventLogEntryType.Error );
								return 0;
							}
				
							// Update database location of service, point to
							// dynamic proxy, change namespace the dynamic proxy namespace
							ireg.UpdateGateKeeperLocation( exec.m_strGKName, strAssembly );
							ireg.UpdateGateKeeperNamespace( exec.m_strGKName, req.dynNamespace );
							// Set Service name to Fully qualified 
							// name i.e <Namespace>.<servicename>
							// necessary for reflection
							exec.m_strGKName = req.dynNamespace + "." + req.serviceName;
						}
						else
						{
							if( strGKNamespace.Length > 0 )
								exec.m_strGKName = strGKNamespace + "." + exec.m_strGKName;
							strAssembly = strGKLocation;
						}
						
						
						// Load assembly from file
						Assembly a = Assembly.LoadFrom( strAssembly );
						// Get the correct type (typename must be fully qualified with namespace)
						Type tDyn = a.GetType( exec.m_strGKName );
				
						// Create an instance of the type
						Object obj = a.CreateInstance( exec.m_strGKName );
						PropertyInfo Url = tDyn.GetProperty( "Url" );
						// Check whether object has a property
						// called Url, valid for generated
						// proxies, if it does check its value
						// if value is empty fill in proxy access point
						if( Url != null )
						{
							string strUrl = (string) Url.GetValue( obj, null );
							if( strUrl.Length == 0 )
								Url.SetValue( obj, strGKAccessPoint, null );
						}
						
						// Trace
						string strTrace = "Executing method: ";
						strTrace += exec.m_strMethod;
						strTrace += " against GK: ";
						strTrace += exec.m_strGKName;
						m_EvtLog.WriteEntry( strTrace, EventLogEntryType.Information );
						
						Object[] parameters = new Object[4];
						object objResult = null;
						parameters[0] = exec.m_nTreatyID;
						parameters[1] = exec.m_strServiceName;
						parameters[2] = exec.m_strMethod;
						if( exec.m_Params.Count > 0 )
							parameters[3] = exec.m_Params.ToArray();
						else parameters[3] = null;

						// Invoke method
						objResult = tDyn.InvokeMember( "ExecuteServiceMethod", 
							BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
							null,
							obj,
							parameters ); 
						
						if( objResult != null )
						{
							strTrace = "Exec succeeded - results returned";
							m_EvtLog.WriteEntry( strTrace, EventLogEntryType.Information );
						}
					}
				}
			}
			catch( System.Exception e )
			{
				string strError = "Error executing Alpha-protocol : ";
				strError += e.Message;
				m_EvtLog.WriteEntry( strError, EventLogEntryType.Error );
				return 0;
			}
			return 1;
		}*/
	}// End GateKeeper
}
