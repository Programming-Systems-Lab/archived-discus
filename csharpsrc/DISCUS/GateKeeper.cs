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
		// SecurityManagerService
		private string SECURITY_MANAGER = "SecurityManagerService";
		// SecurityManagerServiceMethods
		// DoRequestCheck
		private string REQUEST_CHECK_METHOD = "doRequestCheck";
		private string SIGN_DOCUMENT_METHOD = "signDocument";
		private string VERIFY_DOCUMENT_METHOD = "verifyDocument";
		private string VERIFY_TREATY_METHOD = "verifyTreaty";
		private int SECURITY_MANAGER_STATUS_FIELD = 0;
		private string SECURITY_MANAGER_ERROR_CODE = "-1";
 
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
		
		public string EnlistServicesByName( string strXMLTreatyReq )
		{
			string strRetVal = "";
			object[] objParams = new object[2];
			objParams[0] = strXMLTreatyReq;
			objParams[1] = false;
			object objRes = null;

			objRes = Execute( SECURITY_MANAGER, VERIFY_TREATY_METHOD, objParams );

			return strRetVal;
		}

		public bool RequestServiceByName( string strServiceName )
		{
			// Validate Credentials
			
			// Do lookup in dbase for service

			return false;
		}

		public void RevokeTreaty( int nTreatyID )
		{
		
		}
/**************************************************************/
		private string SignDocument( string strXmlDoc )
		{
			string strRetVal = "";
	
			try
			{
				string[] arrSecurityManagerResponse = null;
				// Interact with SecurityManagerService via reflection
				InternalRegistry ireg = new InternalRegistry();
				string strLocation = ireg.GetServiceLocation( SECURITY_MANAGER );
				if( strLocation.Length == 0 )
					throw new System.Exception( "Cannot Find Security Manager" );
				
				object[] objParams = new Object[1];
				objParams[0] = strXmlDoc;
				object objExecResult = Execute( SECURITY_MANAGER, SIGN_DOCUMENT_METHOD, objParams );
				if( objExecResult != null )
				{
					arrSecurityManagerResponse = objExecResult as string[];
					strRetVal = arrSecurityManagerResponse[1];
				}
			}
			catch( System.Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );		
			}
			
			return strRetVal;
		}
		
		private string VerifyDocument( string strXmlDoc )
		{
			string strRetVal = "";
	
			try
			{
				string[] arrSecurityManagerResponse = null;
				// Interact with SecurityManagerService via reflection
				InternalRegistry ireg = new InternalRegistry();
				string strLocation = ireg.GetServiceLocation( SECURITY_MANAGER );
				if( strLocation.Length == 0 )
					throw new System.Exception( "Cannot Find Security Manager" );
				
				object[] objParams = new Object[1];
				objParams[0] = strXmlDoc;
				object objExecResult = Execute( SECURITY_MANAGER, VERIFY_DOCUMENT_METHOD, objParams );
				if( objExecResult != null )
				{
					arrSecurityManagerResponse = objExecResult as string[];
						strRetVal = arrSecurityManagerResponse[1];
				}
			}
			catch( System.Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );		
			}
			
			return strRetVal;
		}
		
		
		private string[] DoRequestCheck( string strXMLExecRequest, bool bSigned )
		{
			string[] arrRetVal = null;
			
			// Workaround Security Manager
			//*****************************************************************************
			/*arrRetVal = new string[2];
			arrRetVal[0] = "1";
			arrRetVal[1] = strXMLExecRequest;
			int c = 9;
			if( c == 9 )
				return arrRetVal;*/
			//*****************************************************************************

			try
			{
				// Interact with SecurityManagerService via reflection
				InternalRegistry ireg = new InternalRegistry();
				string strLocation = ireg.GetServiceLocation( SECURITY_MANAGER );
				if( strLocation.Length == 0 )
					throw new System.Exception( "Cannot Find Security Manager" );
				
				object[] objParams = new Object[2];
				objParams[0] = strXMLExecRequest;
				objParams[1] = false;
				object objExecResult = Execute( SECURITY_MANAGER, REQUEST_CHECK_METHOD, objParams );
				if( objExecResult != null )
					arrRetVal = objExecResult as string[];
			}
			catch( System.Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );		
			}

			return arrRetVal;
		}
		
		private string GenerateProxy( string strName, string strLocation, string strAccessPoint )
		{
			string strRetVal = "";

			try
			{
				// Create a dynamic request
				DynamicRequest req = new DynamicRequest();	
				// Resolve service name
				req.serviceName = strName;
				// Set properties of the Dynamic request
				req.filenameSource = req.serviceName;
				// Location of wsdl file to use for proxy generation
				req.wsdlFile = strLocation;
				// Service Access Point
				req.baseURL = strAccessPoint;
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
				strRetVal = m_pxyGen.GenerateAssembly( req );	
			}
			catch( System.Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );		
			}

			return strRetVal;
		}

		private string GenerateServiceProxy( string strServiceName )
		{
			string strRetVal = "";
			if( strServiceName.Length == 0 )
				return strRetVal;
			
			InternalRegistry ireg = new InternalRegistry();
			string strServiceLocation = ireg.GetServiceLocation( strServiceName );
			string strServiceAccessPoint =  ireg.GetServiceAccessPoint( strServiceName );
			
			strRetVal = GenerateProxy( strServiceName, strServiceLocation, strServiceAccessPoint );

			return strRetVal;
		}

		public object Execute( string strServiceName, string strServiceMethod, object[] objParams )
		{
			object objInvokeResult = null;
			try
			{
				if( strServiceName.Length == 0 || strServiceMethod.Length == 0 )
					throw new System.Exception( "Invalid Arguments Passed to Execute" );
				
				InternalRegistry ireg = new InternalRegistry();
				string strServiceLocation = ireg.GetServiceLocation( strServiceName );
				string strServiceNamespace = ireg.GetServiceNamespace( strServiceName );
				string strServiceAccessPoint = ireg.GetServiceAccessPoint( strServiceName );
				
				// Quick checks for valid data returned from database
				if( strServiceLocation.Length == 0 )
					throw new Exception( "Cannot Find Location of Service: " + strServiceName );
				// Verify that service supports method
				if( !ireg.MethodExists( strServiceName, strServiceMethod ) )
					throw new Exception( "Service: " + strServiceName + " does not support method: " + strServiceMethod );
	
				// Resolve Web Service Proxy Location
				// Do Runtime ProxyGeneration if necessary				
				// If service location is a link to a WSDL file
				// then we must generate a proxy to the webservice,
				// store the proxy in the proxy cache, update our
				// database and set strServiceLocation to the generated
				// proxy. Must also change service namespace in dbase
				string strAssembly = "";
				if( strServiceLocation.ToLower().StartsWith( "http://" ) )
				{
					strAssembly = GenerateProxy( strServiceName, strServiceLocation, strServiceAccessPoint );
					// If no assembly generated, report error and exit
					if( strAssembly.Length == 0 )
						throw new System.Exception( "Error generating proxy to " + strServiceName + " using WSDL ref: " + strServiceLocation );
								
					// Update database location of service, point to
					// dynamic proxy, change namespace the dynamic proxy namespace
					ireg.UpdateServiceLocation( strServiceName, strAssembly );
					ireg.UpdateServiceNamespace( strServiceName, DynamicRequest.DEFAULT_PROXY_NAMESPACE );
					// Set Service name to Fully qualified 
					// name i.e <Namespace>.<servicename>
					// necessary for reflection
					strServiceName = DynamicRequest.DEFAULT_PROXY_NAMESPACE + "." + strServiceName;
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
				
				// Load Assembly containing web service proxy
				Assembly a = Assembly.LoadFrom( strAssembly );
				// Get the correct type (ProxyClass)
				Type ProxyType = a.GetType( strServiceName );
				// Create an instance of the Proxy Class
				Object objProxy = a.CreateInstance( strServiceName );
				if( objProxy == null || ProxyType == null )
				{
					string strError = "Cannot create type/proxy instance ";
					strError += strServiceName;
					strError += " in assembly ";
					strError += strAssembly;
					throw new System.Exception( strError );
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

				// Proxy Method Invocation
				objInvokeResult = ProxyType.InvokeMember( strServiceMethod,
								  BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
								  null,
								  objProxy,
								  objParams );

			}
			catch( System.Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );		
			}

			return objInvokeResult;
		}

//IExecuteService Impl		
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public string ExecuteServiceMethod( string strXMLExecRequest )
		{
			string strRetVal = "";
			string strError = "";
            ExecServiceMethodRequestType execReq = null;
			InternalRegistry ireg = new InternalRegistry();

			try
			{
				// 6 stages for execution
				// 1) Do Security check
				// 2) Extraction of parameters
				// 3) Resolve Web Service Proxy Location (Proxy may have to be generated at runtime)
				// 4) Runtime Deserializtion of parameters
				// 5) Proxy Method Invocation
				// 6) Serialize and return results
							
				// Stage 1 - Do Security Check
				// Contact SecurityManageService
				// Pass request, get back string []
				// Status code and actual XML request
				string[] arrSecurityManagerResponse = DoRequestCheck( strXMLExecRequest, false );
				if( arrSecurityManagerResponse == null || arrSecurityManagerResponse[SECURITY_MANAGER_STATUS_FIELD].CompareTo( SECURITY_MANAGER_ERROR_CODE ) == 0 )
					throw new System.Exception( "Security Exception: Request Not Verified" );
				
				try
				{
					// Deserialize Execution request
					execReq = new ExecServiceMethodRequestType();
					XmlSerializer ser = new XmlSerializer( execReq.GetType() );
					XmlReader xt = new XmlTextReader( strXMLExecRequest, XmlNodeType.Document, null );
					xt.Read();
					object objInst = ser.Deserialize( xt );
					execReq = objInst as ExecServiceMethodRequestType;
				}
				catch( System.Exception e )
				{
					throw new Exception( "Error Deserializing Service Execution Request " + e.Message ); 
				}

				// Stage 2 - Cleaning of parameters
				/*for( int x = 0; x < execReq.m_Parameter.Count; x++ )
				{
					string strParamData = (string) execReq.m_Parameter[i];
					
					int nFirstIndex = strParamData.IndexOf( "<string>" );
					int nLastIndex = strParamData.IndexOf( "</string>" );
					if( nFirstIndex != -1 && nLastIndex != -1 )
					{
						int nStartIndex = nFirstIndex + 8;
						int nLength = nLastIndex - nStartIndex;
					}
				}*/
				
				// Stage 3 - Resolve Web Service Proxy Location
				// Do Runtime ProxyGeneration if necessary				
				// If service location is a link to a WSDL file
				// then we must generate a proxy to the webservice,
				// store the proxy in the proxy cache, update our
				// database and set strServiceLocation to the generated
				// proxy. Must also change service namespace in dbase
				string strServiceLocation = ireg.GetServiceLocation( execReq.ServiceName );
				string strServiceNamespace = ireg.GetServiceNamespace( execReq.ServiceName );
				string strServiceAccessPoint = ireg.GetServiceAccessPoint( execReq.ServiceName );
				string strAssembly = "";
				if( strServiceLocation.ToLower().StartsWith( "http://" ) )
				{
					strAssembly = GenerateProxy( execReq.ServiceName, strServiceLocation, strServiceAccessPoint );
					// If no assembly generated, report error and exit
					if( strAssembly.Length == 0 )
						throw new System.Exception( "Error generating proxy to " + execReq.ServiceName + " using WSDL ref: " + strServiceLocation );
								
					// Update database location of service, point to
					// dynamic proxy, change namespace the dynamic proxy namespace
					ireg.UpdateServiceLocation( execReq.ServiceName, strAssembly );
					ireg.UpdateServiceNamespace( execReq.ServiceName, DynamicRequest.DEFAULT_PROXY_NAMESPACE );
					// Set Service name to Fully qualified 
					// name i.e <Namespace>.<servicename>
					// necessary for reflection
					execReq.ServiceName = DynamicRequest.DEFAULT_PROXY_NAMESPACE + "." + execReq.ServiceName;
				}
				else
				{
					// Fully qualify service name with namespace
					// necesary for reflection
					if( strServiceNamespace.Length > 0 )
						execReq.ServiceName = strServiceNamespace + "." + execReq.ServiceName;
					// Set assembly location
					strAssembly = strServiceLocation;
				}
			
				// Load Assembly containing web service proxy
				bool bProxyMethodHasParameters = false;
				Assembly a = Assembly.LoadFrom( strAssembly );
				// Get the correct type (ProxyClass)
				Type ProxyType = a.GetType( execReq.ServiceName );
				// Create an instance of the Proxy Class
				Object objProxy = a.CreateInstance( execReq.ServiceName );
				if( objProxy == null || ProxyType == null )
				{
					strError = "Cannot create type/proxy instance ";
					strError += execReq.ServiceName;
					strError += " in assembly ";
					strError += strAssembly;
					throw new System.Exception( strError );
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
				MethodInfo mInfo = ProxyType.GetMethod( execReq.MethodName );
				if( mInfo == null )
				{
					strError = "Cannot find method ";
					strError += execReq.MethodName;
					strError += " of Proxy ";
					strError += execReq.ServiceName;
					strError += " loaded from assembly ";
					strError += strAssembly;
					throw new System.Exception( strError );
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
					if( execReq.m_Parameter.Count != arrParamInfo.Length )
						throw new System.Exception( "Wrong Number of Arguments Passed" );
					
					// Create array to hold parameters
					param = new Object[arrParamInfo.Length];

					// Try deserialization
					for( int i = 0; i < execReq.m_Parameter.Count; i++ )
					{
						// Get the expected type
						Type paramType = arrParamInfo[i].ParameterType;
						// Create XmlSerializer
						XmlSerializer xs = new XmlSerializer( paramType );
						// Read in Xml doc representing parameter
						System.Xml.XmlReader xt = new XmlTextReader( (string) execReq.m_Parameter[i], XmlNodeType.Document, null );
						xt.Read(); 
						// Deserialize
						Object paramInst = xs.Deserialize( xt );
						// Store in parameter array
						param[i] = (Object) paramInst;
					}
				}// End if bProxyMethodHasParameters
				
				// Stage 5 - Proxy Method Invocation
				Object objInvokeResult = null;
				if( bProxyMethodHasParameters )
					objInvokeResult = ProxyType.InvokeMember( execReq.MethodName,
						BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
						null,
						objProxy,
						param );
 
				else objInvokeResult = ProxyType.InvokeMember( execReq.MethodName,
						 BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
						 null,
						 objProxy,
						 null );

				// Stage 6 - Serialize and return results
				if( objInvokeResult != null )
				{
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

					
					// SignDocument strips off <?xml version="1.0"?>\n\n
					string strSigned = SignDocument( strRetVal );
					string strVerified = VerifyDocument( strSigned );
					
					// Close reader
					reader.Close();
					// Close stream
					ms.Close();
				}
				
				
				
				
				
				
				
				
				
				/*InternalRegistry ireg = new InternalRegistry();
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
				
				// Stage 4 - 
				
				
				
				
				
				
					return "";
				
				*/
			}
			catch( System.Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

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
		
		
	}// End GateKeeper
}
