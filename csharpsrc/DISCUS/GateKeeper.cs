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
using PSL.DISCUS.Impl.Logging;

// DISCUS GateKeeper package
namespace PSL.DISCUS.Impl.GateKeeper
{
	/// <summary>
	/// Sample implementation of a GateKeeper
	/// </summary>
	public class GateKeeper:IGateKeeper
	{		
		// GateKeeper name
		private string m_strName = "";
		public string Name
		{
			get
			{ return m_strName; }
			set
			{ 
				// Set GK name
				m_strName = value; 
				// Update logging & tracing facilities
				m_objLogger.Source = m_strName;
				m_objTracer.Source = m_strName;
			}
		}
		// Location where dynamically generated proxies are stored
		private string m_strPxyCacheDir = "";
		public string ProxyCache
		{
			get
			{ return m_strPxyCacheDir; }
			set
			{ m_strPxyCacheDir = value; }
		}
		// DISCUS system config file
		private string CONFIG = DConst.DISCUSCONFIG_FILE;
		public string ConfigFile
		{
			get
			{ return CONFIG; }
			set
			{ CONFIG = value; }
		}

		// Turn on tracing
		private bool m_bTraceOn = false;
		public bool TraceOn
		{
			get
			{ return m_bTraceOn; }
			set
			{ m_bTraceOn = value; }
		}

		LogTraceContext m_logCtx = new LogTraceContext();
		LogTraceContext m_traceCtx = new LogTraceContext();

		// Set up logging and tracing facilities
		private LoggerImpl m_objLogger; 
		private TracerImpl m_objTracer; 
		
		// Proxy Generator
		private ProxyGen m_pxyGen;
		// SecurityManagerService
		private string SECURITY_MANAGER = "SecurityManagerService";
		// SecurityManagerServiceMethods
		private string REQUEST_CHECK_METHOD = "doRequestCheck";
		private string SIGN_DOCUMENT_METHOD = "signDocument";
		private string VERIFY_DOCUMENT_METHOD = "verifyDocument";
		private string VERIFY_TREATY_METHOD = "verifyTreaty";
		// Security Manager constants
		private int SECURITY_MANAGER_STATUS_FIELD = 0;
		private string SECURITY_MANAGER_ERROR_CODE = "-1";
		private int SECURITY_MANAGER_RETURNED_TREATY_INDEX = 1;
		private int SECURITY_MANAGER_RETURNED_SIGNATURE_INDEX = 1;
		private int SECURITY_MANAGER_RETURNED_VERIFICATION_INDEX = 1;
 		// GateKeeper Methods
		private string GK_EXECUTE_SERVICE_METHOD = "ExecuteServiceMethod";
		private string GK_ENLIST_SERVICES_BY_NAME_METHOD = "EnlistServicesByName";
		private string DEFAULT_CLIENT_SERVICE_SPACE = "100";
		
		// Private inner class representing an AlphaProtocol execution 
		// step
		private class AlphaRequest:IComparable
		{
			private string m_strProvider;
			public string Provider
			{
				get
				{ return m_strProvider; }
				set
				{ m_strProvider = value; }
			}
			public ExecServiceMethodRequestType m_Req;

			public AlphaRequest()
			{
				m_strProvider = "";
				m_Req = new ExecServiceMethodRequestType();
			}
			
			public int CompareTo( Object obj )
			{
				AlphaRequest temp = obj as AlphaRequest;
				// Case insensitive comparison of providernames
				return m_strProvider.ToLower().CompareTo( temp.m_strProvider.ToLower() );
			}
		}

		/* GK Constructor */
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public GateKeeper()
		{
			try
			{
				// Create ProxyGenerator instance
				m_pxyGen = new ProxyGen();

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
				XmlNode currentNode = root.SelectSingleNode( "GateKeeperName" );
				
				if( currentNode != null )
					m_strName = currentNode.InnerText;

				// Set logging and tracing contexts

				// Logging context info
				currentNode = root.SelectSingleNode( "Logging/UrlLogging/Hostname" );
				if( currentNode != null )
					m_logCtx.Hostname = currentNode.InnerText;

				currentNode = root.SelectSingleNode( "Logging/UrlLogging/Port" );
				if( currentNode != null )
					m_logCtx.Port = Int32.Parse( currentNode.InnerText );

				currentNode = root.SelectSingleNode( "Logging/WebServiceLogging/WSDL" );
				if( currentNode != null )
					m_logCtx.WebServiceWSDL = currentNode.InnerText;

				currentNode = root.SelectSingleNode( "Logging/WebServiceLogging/AccessPoint" );
				if( currentNode != null )
					m_logCtx.WebServiceAccessPoint = currentNode.InnerText;

				// Tracing context info
				currentNode = root.SelectSingleNode( "Tracing/UrlTracing/Hostname" );
				if( currentNode != null )
					m_traceCtx.Hostname = currentNode.InnerText;

				currentNode = root.SelectSingleNode( "Tracing/UrlTracing/Port" );
				if( currentNode != null )
					m_traceCtx.Port = Int32.Parse( currentNode.InnerText );

				currentNode = root.SelectSingleNode( "Tracing/WebServiceTracing/WSDL" );
				if( currentNode != null )
					m_traceCtx.WebServiceWSDL = currentNode.InnerText;

				currentNode = root.SelectSingleNode( "Tracing/WebServiceTracing/AccessPoint" );
				if( currentNode != null )
					m_traceCtx.WebServiceAccessPoint = currentNode.InnerText;

				
				// Determine the type of logging anf tracing facilities to use
                string strLoggingType = "";
				currentNode = root.SelectSingleNode( "Logging/LoggingType" );
				if( currentNode == null || currentNode.InnerText.Length == 0 )
					strLoggingType = DConst.EVENT_LOGGING;
				else strLoggingType = currentNode.InnerText;

				string strTracingType = "";
				currentNode = root.SelectSingleNode( "Tracing/TracingType" );
				if( currentNode == null || currentNode.InnerText.Length == 0 )
					strTracingType = DConst.EVENT_TRACING;
				else strTracingType = currentNode.InnerText;
				
				// Gaurantees that some logging and tracing facility is available
				m_objLogger = CreateLoggingInstance( strLoggingType, m_logCtx );
				m_objTracer = CreateTracingInstance( strTracingType, m_traceCtx );
				
				// Specific sourcename
				if( m_strName.Length > 0 )
				{
					m_objLogger.Source = m_strName;
					m_objTracer.Source = m_strName;
				}
				else
				{
					m_objLogger.Source = "Gatekeeper";
					m_objTracer.Source = "Gatekeeper";
				}
				
				// Find out where we store dynamic proxies
				// if no dir specified create default 
				// proxy cache directory under the current directory
				currentNode = root.SelectSingleNode( "ProxyCache" );
				
				if( currentNode != null )
					m_strPxyCacheDir = currentNode.InnerText;
				
				if( m_strPxyCacheDir.Length == 0 )
					m_strPxyCacheDir = DConst.DEFAULT_PXY_DIR;

				fs.Close(); // Close file stream
				
				// Determine whether ProxyCache Directory exists
				// If it does not exist then we try to create it
				bool bStatus = InitializeProxyCacheDir();
				if( m_bTraceOn )
				{
					if( bStatus )
						TraceInfo( "ProxyCache Initialized" );
					else TraceError( "ProxyCache NOT Initialized" );
				}
			}
			catch( System.Exception e ) // Catch exception
			{
				// Report error
				LogError( e.Message );
			}
		}

		/* Function builds an Execution Queue (sequence of steps)
		 * from an Alpha protocol
		 * Input: strAlphaProtocol - sequence of actions to be
		 *		  performed (using a simple subset of xlang)
		 * Return values: A Queue of steps to execute
		 */
		private Queue BuildAlphaProtocolExecutionQ( string strAlphaProtocol )
		{
			Queue execQ = new Queue();
			
			try
			{
				if( m_bTraceOn )
					TraceInfo( "About to build execQ from Alpha-Protocol: " + strAlphaProtocol );
				
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
				// Every sequence should have a list of actions to be performed.
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
				// and add to a queue
				foreach( XmlNode action in lstActions )
				{
					// Create an attribute collection
					XmlAttributeCollection att = action.Attributes;
					// An action must have attributes
					if( att.Count == 0 )
						throw new Exception( "Invalid Alpha-Protocol - no attributes specified" );
					
					AlphaRequest actionReq = new AlphaRequest();
					ExecServiceMethodRequestType methodReq = new ExecServiceMethodRequestType();
					// Get the Gatekeeper that will service request
					XmlNode val = att.GetNamedItem( "gatekeeper" );
					if( val == null ) // May mean use local only...handle this later
						throw new Exception( "No Provider GateKeeper" );
					actionReq.Provider = val.Value;
					
					// Get the name of the service we want to use
					val = att.GetNamedItem( "servicename" );
					if( val == null )
						throw new Exception( "No Service To Use" );
					methodReq.ServiceName = val.Value;

					// Get the operation/method we are supposed to request
					val = att.GetNamedItem( "operation" );
					if( val == null )
						throw new Exception( "No Service Method Specified" );
					methodReq.MethodName = val.Value;
					
					// Get any parameters the action may have
					if( action.HasChildNodes )
					{
						XmlNodeList lstParams = action.ChildNodes;
						// Parameters should be well formed
						// XML documents 
						foreach( XmlNode param in lstParams )
						{
							// Create an attribute collection
							XmlAttributeCollection paramAtt = param.Attributes;
							// Any param must have at least one attribute (a parameter name)
							if( paramAtt.Count == 0 )
								throw new Exception( "Invalid Alpha-Protocol - no attributes specified" );
							// Get the name of the parameter
							XmlNode paramNameVal = paramAtt.GetNamedItem( "name" );
							if( paramNameVal == null ) 
								throw new Exception( "No Parameter Name Specified" );
							// Add parameter name to list
							methodReq.m_ParamName.Add( paramNameVal.Value );
							// Add parameter value to list
							methodReq.m_ParamValue.Add( param.InnerText );
						}
					}// End if action has child nodes
					// Set the method request of the action request
					actionReq.m_Req = methodReq;
					// Add request to execution queue
					execQ.Enqueue( actionReq );
				}// End for each action in lstActions
			}
			catch( Exception e )
			{
				LogError( e.Message );				
			}
			
			if( m_bTraceOn )
				TraceInfo( "Built execQ: " + execQ.Count + " actions to perform" );
				
			return execQ; // return Execution Queue
		}// End BuildAlphaProtocolExecutionQ

		/* Function Builds a Treaty based on a stack of
		 * services requested from the same provider 
		 * service space 
		 * Input: stkProvidiers - a stack of requests to a 
		 *		  provider
		 * Return Values: A treaty reperesenting a request
		 */
		TreatyType BuildTreaty( Stack stkProviders )
		{
			// If Stack is empty, nothing to do so exit
			if( stkProviders.Count == 0 )
				return null;
			
			if( m_bTraceOn )
				TraceInfo( "About to build treaty" );
			
			// Treaty to be returned
			TreatyType treaty = null;
			// Debugging to verify serialization to Xml
			string strCheck = "";
			try
			{
				treaty = new TreatyType();
				// Get Client Service Space name
				treaty.ClientServiceSpace = DEFAULT_CLIENT_SERVICE_SPACE;
				// Get Provider Service Space name
				treaty.ProviderServiceSpace = ( (AlphaRequest) stkProviders.Peek() ).Provider; //DEFAULT_PROVIDER_SERVICE_SPACE;
				// Count the number of services requested
				int nServiceCount = 0;
				// Create entries for each service to be requested
				treaty.m_ServiceInfo = new ServiceDataType[stkProviders.Count];
				// While there are requests to be processed...
				while( stkProviders.Count > 0 )
				{
					// Get request at top of stack
					AlphaRequest req = (AlphaRequest) stkProviders.Pop();
					// Create objects to store Service Data and Method Data
					ServiceDataType serviceData = new ServiceDataType();
					ServiceMethodDataType methodData = new ServiceMethodDataType();
					// Get ServiceName
					serviceData.ServiceName = req.m_Req.ServiceName;
					serviceData.m_ServiceMethod = new ServiceMethodDataType[1];
					// Get MethodName
					methodData.MethodName = req.m_Req.MethodName;
					// Add Parameters to methodData
					methodData.m_Parameter = new string[req.m_Req.m_ParamValue.Count];
					
					for( int nParamCount = 0; nParamCount < req.m_Req.m_ParamValue.Count; nParamCount++ )
					{
						methodData.m_Parameter[nParamCount] = (string) req.m_Req.m_ParamName[nParamCount];
					}
					// Add method data to ServiceData
					serviceData.m_ServiceMethod[0] = methodData;
					treaty.m_ServiceInfo[nServiceCount] = serviceData;
					nServiceCount++;
				}
				// Debugging to verify serialization to Xml
				strCheck = treaty.ToXml();
				
				if( m_bTraceOn )
					TraceInfo( "Built treaty: " + treaty.ToXml() );
			}
			catch( Exception e )
			{
				LogError( e.Message );				
			}
			return treaty; // return the Treaty we create from the stack
		}// End BuildTreaty
		
		/* Function analyzes all Treaties created and returns 
		 * true if for each treaty the requested methods have
		 * been authorized, i.e. permission has been granted
		 * to use all the methods.
		 * Inputs: mapping - the mapping between external GKs
		 *					 and the treaties created
		 * Return values: true if for all treaties all methods
		 *				  autorized, otherwise returns false 
		 */
		private bool CanExecuteAllSteps( Hashtable mapping )
		{
			if( mapping == null || mapping.Count == 0 )
				return false;

			bool bRetVal = false;

			if( m_bTraceOn )
				TraceInfo( "Checking whether we can execute all requested service methods" );
			
			try
			{
				// Get a Dictionary Enumerator
				IDictionaryEnumerator it = mapping.GetEnumerator();

				// Go thru each entry in the hashtable
				while( it.MoveNext() )
				{
					// Get the current treaty
					TreatyType currTreaty = it.Value as TreatyType;
					// Check whether all methods in the treaty authorized
					for( int i = 0; i < currTreaty.m_ServiceInfo.Length; i++ )
					{
						if( !currTreaty.m_ServiceInfo[i].AllServiceMethodsAuthorized() )
							return false;
					}
				}
				bRetVal = true;
			}
			catch( Exception e )
			{
				LogError( e.Message );				
			}
			return bRetVal;
		}

		/* Function creates a logging instance. 
		 * Input: strLoggingType - the type of logging instance to create
		 *						   EvtLoggerImpl, UrlLoggerImpl or WebServiceLoggerImpl
		 *						   based on the names: "EventLog", "UrlLog", "WebServiceLog"
		 * Return values: an instance of the named logging implementation OR
		 *				  an instance of the default logging implementation, EvtLoggerImpl
		 */
		private LoggerImpl CreateLoggingInstance( string strLoggingType, LogTraceContext logCtx )
		{
			LoggerImpl objLoggingInst = null;
			
			try
			{
				if( strLoggingType.ToLower().CompareTo( DConst.URL_LOGGING.ToLower() ) == 0 )
				{
					// UrlLoggerImpl
					objLoggingInst = new UrlLoggerImpl( logCtx );
				}
				else if( strLoggingType.ToLower().CompareTo( DConst.WEB_SERVICE_LOGGING.ToLower() ) == 0 )
				{
					// WebServiceLoggerImpl
					objLoggingInst = new EvtLoggerImpl( logCtx );
				}
				else objLoggingInst = new EvtLoggerImpl( logCtx );
			}
			catch( Exception e )
			{
				objLoggingInst = new EvtLoggerImpl( logCtx );
				objLoggingInst.LogError( e.Message );
			}

			return objLoggingInst;
		}
		
		/* Function creates a tracing instance. 
		 * Input: strTracingType - the type of logging instance to create
		 *						   EvtTracerImpl, UrlTracerImpl or WebServiceTracerImpl
		 *						   based on the names: "EventTrace", "UrlTrace", "WebServiceTrace"
		 * Return values: an instance of the named tracing implementation OR
		 *				  an instance of the default tracing implementation, EvtTracerImpl
		 */
		private TracerImpl CreateTracingInstance( string strTracingType, LogTraceContext traceCtx )
		{
			TracerImpl objTracingInst = null;
			
			try
			{
				if( strTracingType.ToLower().CompareTo( DConst.URL_TRACING.ToLower() ) == 0 )
				{
					// UrlTracerImpl
					objTracingInst = new UrlTracerImpl( traceCtx );
				}
				else if( strTracingType.ToLower().CompareTo( DConst.WEB_SERVICE_TRACING.ToLower() ) == 0 )
				{
					// WebServiceTracerImpl
					objTracingInst = new EvtTracerImpl( traceCtx );
				}
				else objTracingInst = new EvtTracerImpl( traceCtx );
			}
			catch( Exception e )
			{
				objTracingInst = new EvtTracerImpl( traceCtx );
				objTracingInst.TraceError( e.Message );
			}

			return objTracingInst;
		}

		/* Procedure dissolves a Treaty with a given TreatyID
		 * Input: nTreatyID - the treaty to be dissolved
		 */
		public void DissolveTreaty( int nTreatyID )
		{
			if( m_bTraceOn )
				TraceInfo( "Dissolving Treaty " + nTreatyID );
			
		}// End DissolveTreaty

		/* Function uses the SecurityManagerService to check whether
		 * a request for service use is allowed or not.
		 * Input: strXmlExecRequest
		 * Return values: an array of strings containing the 
		 *				  status code and response from the
		 *				  SecurityManagerService
		 */
		private string[] DoRequestCheck( string strXmlExecRequest, bool bSigned )
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
				if( m_bTraceOn )
					TraceInfo( "Doing request check on: " + strXmlExecRequest );
			
				// Interact with SecurityManagerService via reflection
				InternalRegistry ireg = new InternalRegistry();
				string strLocation = ireg.GetServiceLocation( SECURITY_MANAGER );
				if( strLocation.Length == 0 )
					throw new Exception( "Cannot Find Security Manager" );
				
				// Create array of parameters to pass to SecurityManagerService
				object[] objParams = new Object[2];
				objParams[0] = strXmlExecRequest;
				objParams[1] = false;//bSigned; //Request not signed (yet)
				// Execute Request Check method of security manager
				object objExecResult = ExecuteService( SECURITY_MANAGER, REQUEST_CHECK_METHOD, objParams );
				// Convert result to string[] 
				if( objExecResult != null )
					arrRetVal = objExecResult as string[];
			}
			catch( System.Exception e )
			{
				LogError( e.Message );		
			}
			return arrRetVal; // return SecurityManagerService response
		}//End DoRequestCheck

		/* Function responds to a treaty request for services
		 * Input: strXmlTreatyReq - treaty request
		 * Return Values: treaty returned from the SecurityManagerService
		 */
		public string EnlistServicesByName( string strXmlTreatyReq )
		{
			string strRetVal = "";
			try
			{
				if( m_bTraceOn )
					TraceInfo( "Received treaty request, passing to SecurityManager to verify: " + strXmlTreatyReq );
			
				// Create array of params to pass to VerifyTreaty
				// of SecurityManagerService
				object[] objParams = new object[2];
				objParams[0] = strXmlTreatyReq;
				objParams[1] = false; // request not signed (yet)
				object objRes = null; // return value
				// Execute method
				objRes = ExecuteService( SECURITY_MANAGER, VERIFY_TREATY_METHOD, objParams );
				// Check whether execution returned something
				if( objRes != null )
				{
					// Convert returned object to object array
					object[] temp = (object[]) objRes;
					// Check contents of returned array and extract the copy of the 
					// treaty returned by the SecurityManagerService
					if( temp[SECURITY_MANAGER_RETURNED_TREATY_INDEX] != null )
						strRetVal = (string) temp[SECURITY_MANAGER_RETURNED_TREATY_INDEX];

					if( strRetVal.Length > 0 )
					{
						if( m_bTraceOn )
							TraceInfo( "SecurityManager verification returned: " + strRetVal );
					}
				}
			}
			catch( Exception e )
			{
				LogError( e.Message );		
			}
			return strRetVal;
		}// End EnlistServicesByName
		
		/* Function executes an Alpha-protocol. 
		 * Inputs: strAlphaProtocol - Alpha-protocol to execute
		 * Return Values - a string array of the returned results for each action
		 */
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public string[] ExecuteAlphaProtocol( string strAlphaProtocol )
		{
			// If Alpha-protocol empty then exit
			if( strAlphaProtocol.Length == 0 )
				return null;

			
			// 4 stage execution			
			// 1) Build the execution queue (steps to perform)
			// 2) Create nec treaties with other GKs - Resource Acquire stage
			// 3) Ensure that all requested service authorized
			// 4) Execution stage
			string [] arrRetVal = null; // Holds returned values
			try
			{
				// Stage 1 - Build Execution queue (steps to execute)
				if( m_bTraceOn )
					TraceInfo( "Executing Alpha Protocol Stage 1 - Building ExecutionQ" );
			
				Queue execQ = BuildAlphaProtocolExecutionQ( strAlphaProtocol );
				if( execQ == null || execQ.Count == 0 )
					throw new Exception( "Error obtaining sequence of actions to perform" );

				// Create appropriate sized array to hold results
				arrRetVal = new string[execQ.Count];
				// Create an array of requests
				AlphaRequest[] arrReqs = new AlphaRequest[execQ.Count];
				// Copy values from execQ
				object[] objTemp =  execQ.ToArray();
				for( int i = 0; i < objTemp.Length; i++ )
				{
					arrReqs[i] = (AlphaRequest) objTemp[i];
				}
				// Sort ActionRequests by GateKeeper name, we want
				// one consolidated treaty per GateKeeper so we 
				// request all the services we need from a GK
				// in one go.
				Array.Sort( arrReqs );
				
				
				// Stage 2 - Create the necessary treaties and return a hashtable
				// of treatyIDs to be used with each GK.
				if( m_bTraceOn )
					TraceInfo( "Executing Alpha Protocol Stage 2 - Creating Treaties" );
			
				Hashtable mapping = FormTreaties( arrReqs );
				if( mapping == null || mapping.Count == 0 )
					throw new Exception( "Resource Acquire Failed, Error Creating Treaties" );

				if( m_bTraceOn )
					TraceInfo( "Executing Alpha Protocol Stage 3 - Analyzing Treaty Responses" );
				// Stage 3 - Analyze mapping to make sure all requested methods authorized
				// if not then we report the failure and exit
				if( !CanExecuteAllSteps( mapping ) )
				{
					if( m_bTraceOn )
						TraceError( "All Necessary Resources NOT Acquired" );
					// Write Treaties to event log for analysis later
					throw new Exception( "Resource Acquire Failed, All Requests NOT Authorized" );
				}

				// Stage 4 - Execution stage
				if( m_bTraceOn )
					TraceInfo( "Executing Alpha Protocol Stage 4 - Execution" );
				
				IEnumerator it = execQ.GetEnumerator();
				InternalRegistry ireg = new InternalRegistry();
				int nIndex = 0;	
			
				while( it.MoveNext() )
				{
					AlphaRequest actionReq = (AlphaRequest) it.Current;
					// GK expects one parameter - an XML document
					// representing a request
					object[] objParams = new object[1];
					// Retrieve current treaty (applicable for provider)
					TreatyType currentTreaty = (TreatyType) mapping[actionReq.Provider];
					if( currentTreaty == null )
						throw new Exception( "Error retrieving treaty for " + actionReq.Provider );

					// Set TreatyID
					actionReq.m_Req.TreatyID = currentTreaty.TreatyID;
					// Resolve method implementation as specified by treaty
					actionReq.m_Req.MethodName = ResolveMethodImplementation( actionReq, currentTreaty );
					
					// Ideally request should be signed first
					objParams[0] = actionReq.m_Req.ToXml();
					// Execute against provider GK
					object objRes = ExecuteGateKeeper( actionReq.Provider, GK_EXECUTE_SERVICE_METHOD , objParams );
					if( objRes != null )
					{
						// save results returned
						string strTemp = (string) objRes;
						string strVerify = VerifyDocument( strTemp );
						if( strVerify.Length > 0 )
							arrRetVal[nIndex] = strVerify;
						else arrRetVal[nIndex] = strTemp;
					}
					nIndex++;
				}// End while it.MoveNext()
			}
			catch( System.Exception e )
			{
				LogError( e.Message );
			}
			
			if( m_bTraceOn )
				TraceInfo( "Finished Executing Alpha Protocol" );
				
			return arrRetVal;
		}//End ExecuteAlphaProtocol
		
		/* Function executes a method against a Gatekeeper proxy via reflection.
		 * Input: strGKName - name of GK to execute against
		 *		  strGKMethod - name of method to execute
		 *		  objParams - array of parameters to pass to proxy method
		 * Return Values: the results of execution against the GK
		 */
		private object ExecuteGateKeeper( string strGKName, string strGKMethod, object[] objParams )
		{
			object objInvokeResult = null;
			try
			{
				if( m_bTraceOn )
					TraceInfo( "Executing method " + strGKMethod + " against GK " + strGKName  );
							
				// Check that we have been given a GK name and a method name to execute
				if( strGKName.Length == 0 || strGKMethod.Length == 0 )
					throw new System.Exception( "Invalid Arguments Passed to ExecuteGateKeeper" );
				
				// Lookup GK location, namespace and acessppoint
				InternalRegistry ireg = new InternalRegistry();
				string strGKLocation = ireg.GetGateKeeperLocation( strGKName );
				string strGKNamespace = ireg.GetGateKeeperNamespace( strGKName );
				string strGKAccessPoint = ireg.GetGateKeeperAccessPoint( strGKName );
				
				// Quick checks for valid data returned from database
				if( strGKLocation.Length == 0 )
					throw new Exception( "Cannot Find Location of GateKeeper: " + strGKName );
				
				// Resolve Web Service Proxy Location
				// Do Runtime ProxyGeneration if necessary				
				// If GK location is a link to a WSDL file
				// then we must generate a proxy to the GK,
				// store the proxy in the proxy cache, update our
				// database and set strGKLocation to the generated
				// proxy. Must also change GK namespace in dbase
				string strAssembly = "";
				if( strGKLocation.ToLower().StartsWith( "http://" ) )
				{
					if( m_bTraceOn )
						TraceInfo( "Generating Proxy to " + strGKName + " located at " + strGKLocation + " AccessPoint: " + strGKAccessPoint );
			
					strAssembly = GenerateProxy( strGKName, strGKLocation, strGKAccessPoint );
					// If no assembly generated, report error and exit
					if( strAssembly.Length == 0 )
						throw new System.Exception( "Error generating proxy to " + strGKName + " using WSDL ref: " + strGKLocation );
								
					// Update database location of service, point to
					// dynamic proxy, change namespace the dynamic proxy namespace
					ireg.UpdateGateKeeperLocation( strGKName, strAssembly );
					ireg.UpdateGateKeeperNamespace( strGKName, DynamicRequest.DEFAULT_PROXY_NAMESPACE );
					// Set Service name to Fully qualified 
					// name i.e <Namespace>.<servicename>
					// necessary for reflection
					strGKName = DynamicRequest.DEFAULT_PROXY_NAMESPACE + "." + strGKName;
				}
				else
				{
					// Fully qualify service name with namespace
					// necesary for reflection
					if( strGKNamespace.Length > 0 )
						strGKName = strGKNamespace + "." + strGKName;
					// Set assembly location
					strAssembly = strGKLocation;
				}
				
				// Load Assembly containing web service proxy
				Assembly a = Assembly.LoadFrom( strAssembly );
				// Get the correct type (ProxyClass)
				Type ProxyType = a.GetType( strGKName );
				// Create an instance of the Proxy Class
				Object objProxy = a.CreateInstance( strGKName );
				if( objProxy == null || ProxyType == null )
				{
					string strError = "Cannot create type/proxy instance ";
					strError += strGKName;
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
						Url.SetValue( objProxy, strGKAccessPoint, null );
				}

				// Proxy Method Invocation
				objInvokeResult = ProxyType.InvokeMember( strGKMethod,
					BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
					null,
					objProxy,
					objParams );

			}
			catch( System.Exception e )
			{
				LogError( e.Message );		
			}

			return objInvokeResult;		
		}//End ExecuteGatekeeper

		/* Function executes a method against a web service proxy via reflection.
		 * Input: strGKName - name of web service proxy to execute against
		 *		  strGKMethod - name of method to execute
		 *		  objParams - array of parameters to pass to proxy method
		 * Return Values: the results of execution against the web service
		 */
		private object ExecuteService( string strServiceName, string strServiceMethod, object[] objParams )
		{
			object objInvokeResult = null;
			try
			{
				if( m_bTraceOn )
					TraceInfo( "Executing " + strServiceMethod + " against service " + strServiceName );
			
				if( strServiceName.Length == 0 || strServiceMethod.Length == 0 )
					throw new System.Exception( "Invalid Arguments Passed to Execute" );
				
				InternalRegistry ireg = new InternalRegistry();
				// Lookup service location, namespace and accespoint
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
					if( m_bTraceOn )
						TraceInfo( "Generating Proxy to " + strServiceName + " located at " + strServiceLocation + " AccessPoint: " + strServiceAccessPoint );		
			
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
				LogError( e.Message );		
			}
			return objInvokeResult;
		}//End ExecuteService

		/* Function executes a service method based on a request.
		 * Input: strXmlExecRequest - the Xml representation of a request
		 * Return values: the results of executing the requested method
		 *				  serialized to xml if the request is sllowed by the 
		 *				  SecurityManagerService.
		 */
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public string ExecuteServiceMethod( string strXmlExecRequest )
		{
			string strRetVal = "";
			string strError = "";
			ExecServiceMethodRequestType execReq = null;
			InternalRegistry ireg = new InternalRegistry();

			try
			{
				if( m_bTraceOn )
					TraceInfo( "Received ExecServiceMethodRequest : " + strXmlExecRequest );
				
				// 6 stages for execution
				// 1) Do Security check
				// 2) Deserialize Execution request
				// 3) Resolve Web Service Proxy Location (Proxy may have to be generated at runtime)
				// 4) Runtime Deserializtion of parameters
				// 5) Proxy Method Invocation
				// 6) Serialize and return results
							
				// Stage 1 - Do Security Check
				// Contact SecurityManageService
				// Pass request, get back string []
				// Status code and actual XML request
				if( m_bTraceOn )
					TraceInfo( "Stage 1 - Do Security Check" );
				
				string[] arrSecurityManagerResponse = DoRequestCheck( strXmlExecRequest, false );
				if( arrSecurityManagerResponse == null || arrSecurityManagerResponse[SECURITY_MANAGER_STATUS_FIELD].CompareTo( SECURITY_MANAGER_ERROR_CODE ) == 0 )
					throw new System.Exception( "Security Exception: Request Not Verified: " + strXmlExecRequest + "Reason: " + arrSecurityManagerResponse[1] );
				
				if( m_bTraceOn )
					TraceInfo( "Passed Security Check" );
				
				try
				{
					// Stage 2 - Deserialize Execution request
					if( m_bTraceOn )
						TraceInfo( "Stage 2 - Deserialize Execution Request" );
				
					execReq = new ExecServiceMethodRequestType();
					XmlSerializer ser = new XmlSerializer( execReq.GetType() );
					XmlReader xt = new XmlTextReader( strXmlExecRequest, XmlNodeType.Document, null );
					xt.Read();
					object objInst = ser.Deserialize( xt );
					execReq = objInst as ExecServiceMethodRequestType;
				}
				catch( System.Exception e )
				{
					throw new Exception( "Error Deserializing Service Execution Request " + e.Message ); 
				}

				// Stage 3 - Resolve Web Service Proxy Location
				if( m_bTraceOn )
					TraceInfo( "Stage 3 - Resolve Web Service Proxy Location" );
				
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

				// Stage 4 - runtime Deserialization of parameters
				if( m_bTraceOn )
					TraceInfo( "Stage 4 - Derialize Parameters (if any)" );
				
				if( bProxyMethodHasParameters )
				{
					// Parameters passed not equal to
					// parameters expected
					// (is method overloading an issue?? - probably not with dynamically generated proxies)
					if( execReq.m_ParamValue.Count != arrParamInfo.Length )
						throw new System.Exception( "Wrong Number of Arguments Passed" );
					
					// Create array to hold parameters
					param = new Object[arrParamInfo.Length];

					// Try deserialization
					for( int i = 0; i < execReq.m_ParamValue.Count; i++ )
					{
						// Get the expected type
						Type paramType = arrParamInfo[i].ParameterType;
						// Create XmlSerializer
						XmlSerializer xs = new XmlSerializer( paramType );
						// Read in Xml doc representing parameter
						System.Xml.XmlReader xt = new XmlTextReader( (string) execReq.m_ParamValue[i], XmlNodeType.Document, null );
						xt.Read(); 
						// Deserialize
						Object paramInst = xs.Deserialize( xt );
						// Store in parameter array
						param[i] = (Object) paramInst;
					}
				}// End if bProxyMethodHasParameters
				
				// Stage 5 - Proxy Method Invocation
				if( m_bTraceOn )
					TraceInfo( "Stage 5 - Invoke Proxy Method" );
				
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
					if( m_bTraceOn )
						TraceInfo( "Stage 6 - Serialize, Sign and Return Parameters" );
				
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
					
					// SignDocument strips off "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n\n" 
					if( strRetVal.Length > 0 )
					{
						// Sign document
						string strSigned = SignDocument( strRetVal );
						if( strSigned.Length > 0 )
							strRetVal = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n\n" + strSigned;
						// Debugging, checking verification
						string strVerified = VerifyDocument( strSigned );
					}
					// Close reader
					reader.Close();
					// Close stream
					ms.Close();
				}
			}
			catch( System.Exception e )
			{
				LogError( e.Message );
			}
			
			if( m_bTraceOn )
				TraceInfo( "Finished Executing Service Method Request" );
			
			
			return strRetVal;
		}//End ExecuteServiceMethod

		/* Function takes an array of AlphaRequests and attempts
		 * to create the necessary Treaties that must go out
		 * to the external Gatekeepers from who we request
		 * use of service. Returns a mapping between the 
		 * Gatekeeper (provider) and the Treaty sent back as
		 * a response to a request for service use.
		 * Inputs: arrReqs the array of AlphaRequests sorted by Gatekeeper
		 *		   name
		 * Return Values: returns a mapping between each external
		 *				  gatekeeper and the treaty created with
		 *				  with them.
		 */
		private Hashtable FormTreaties( AlphaRequest[] arrReqs )
		{
			if( arrReqs.Length == 0 )
				return null;
			Hashtable mapping = null;
			try
			{
				if( m_bTraceOn )
					TraceInfo( "Forming Treaties for " + arrReqs.Length + " requests" );
				
				mapping = new Hashtable();
				IEnumerator it = arrReqs.GetEnumerator();
				Stack stkProviders = new Stack();
				AlphaRequest currReq = null;
				// Get first request
				if( it.MoveNext() )
					currReq = (AlphaRequest) it.Current;
				else return null;
				// Add request to stack
				stkProviders.Push( currReq );
				
				// Create a stack that contains all the requests
				// going to one Gatekeeper so that we can create
				// a consolidated treaty to request all the services
				// we want to use from this service space in one go.
				while( true )
				{
					// Push all requests for a single GK onto stack
					while( it.MoveNext() && ( (AlphaRequest) it.Current ).Provider == currReq.Provider )
					{
						stkProviders.Push( (AlphaRequest) it.Current );
					}
					
					// Build a treaty from the stack of requests
					if( stkProviders.Count > 0 )
					{
						// Build treaty
						TreatyType treaty = BuildTreaty( stkProviders );
						// Pass Treaty to remote GK to have it 
						// verified so we can get a TreatyID to use
						object[] objParams = new object[1];
						objParams[0] = treaty.ToXml();
						object objRes = ExecuteGateKeeper( treaty.ProviderServiceSpace, GK_ENLIST_SERVICES_BY_NAME_METHOD, objParams );

						if( objRes != null || ((string) objRes).Length > 0 )
						{
							//Deserialize into TreatyType
							TreatyType returnedTreaty = null;
							string strTreaty = (string) objRes;
							XmlSerializer ser = new XmlSerializer( typeof(TreatyType) );
							
							XmlReader xt = new XmlTextReader( strTreaty, XmlNodeType.Document, null );
							xt.Read();
							object objInst = ser.Deserialize( xt );
							returnedTreaty = objInst as TreatyType;
							
							// Store the Treaty assoc with the ProviderServiceSpace
							mapping.Add( returnedTreaty.ProviderServiceSpace, returnedTreaty );
						}
						else throw new Exception( "Error Creating Treaty with " + treaty.ProviderServiceSpace );
						
						// Clear stack	
						stkProviders.Clear();
						// If no nore requests then exit otherwise get the next one
						if( !it.MoveNext() )
							break;
                        else it.MoveNext();
					}
					
					stkProviders.Push( (AlphaRequest) it.Current );
					currReq = (AlphaRequest) it.Current;
				}
			}
			catch( Exception e )
			{
				LogError( e.Message );				
			}
			return mapping;
		}// End FormTreaties

		/* Function generates a proxy to a web service 
		 * Input: strName - name of web service to generate proxy to
		 *		  strLocation - location of web service WSDL file
		 *		  strAccessPoint - URL of web service
		 *  Return value: path to generated Assembly containing proxy
		 */
		public string GenerateProxy( string strName, string strLocation, string strAccessPoint )
		{
			string strRetVal = "";
				
			try
			{
				if( m_bTraceOn )
					TraceInfo( "Generating Proxy" );
				
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
					LogWarning( strMsg );
				}
				// Where to store generated proxy
				req.proxyPath = m_strPxyCacheDir;
				// Pass Dynamic request to proxy generator
				strRetVal = m_pxyGen.GenerateAssembly( req );	
			}
			catch( System.Exception e )
			{
				LogError( e.Message );		
			}
			return strRetVal;
		}//End GenerateProxy

		/* Function generates a proxy to a web service
		 * Input: strServiceName - name of service to generate proxy for
		 * Return Values: path to generated Assembly containing proxy
		 */
		private string GenerateServiceProxy( string strServiceName )
		{
			string strRetVal = "";
			if( strServiceName.Length == 0 )
				return strRetVal;
			// Lookup service location and accesspoint
			InternalRegistry ireg = new InternalRegistry();
			string strServiceLocation = ireg.GetServiceLocation( strServiceName );
			string strServiceAccessPoint =  ireg.GetServiceAccessPoint( strServiceName );
			// Generate proxy
			strRetVal = GenerateProxy( strServiceName, strServiceLocation, strServiceAccessPoint );
			return strRetVal; //return Assembly generated
		}//End GenerateServiceProxy

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
				if( m_bTraceOn )
					TraceInfo( "Initizlize Proxy Cache Dir" );
				
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
					LogError( strError );
				}
			}
			return bRetVal;
		}//End InitializeProxyCacheDir
			
		public void LogError( string strMsg )
		{
			m_objLogger.LogError( strMsg );
		}

		public void LogInfo( string strMsg )
		{
			m_objLogger.LogInfo( strMsg );
		}

		public void LogWarning( string strMsg )
		{
			m_objLogger.LogWarning( strMsg );
		}
		
		private string ResolveMethodImplementation( AlphaRequest actionReq, TreatyType currentTreaty )
		{
			string strRetVal = "";
			try
			{
				for( int i = 0; i < currentTreaty.m_ServiceInfo.Length; i++ )
				{
					// Find service name entry of AlphaRequest for service named in current treaty
					if( currentTreaty.m_ServiceInfo[i].ServiceName.CompareTo( actionReq.m_Req.ServiceName ) == 0 )
					{
						// Search thru service info for service method name match
						for( int j = 0; j < currentTreaty.m_ServiceInfo[i].m_ServiceMethod.Length; j++ )
						{
							// When a match is found return the method implementation 
							if( currentTreaty.m_ServiceInfo[i].m_ServiceMethod[j].MethodName.CompareTo( actionReq.m_Req.MethodName ) == 0 )
							{
								return currentTreaty.m_ServiceInfo[i].m_ServiceMethod[j].MethodImplementation;
							}
						}
					}
				}
			}
			catch( Exception e )
			{
				LogError( e.Message );
			}
			return strRetVal;
		}
		
		/* Procedure gets the SecurityManagerSerivce to
		 * revoke a given treaty
		 * Input: nTreatyID - the treaty to revoke
		 */
		public void RevokeTreaty( int nTreatyID )
		{
			if( m_bTraceOn )
				TraceInfo( "Revoking Treaty " + nTreatyID );
				
		}//End RevokeTreaty

		/* Function uses the SecurityManagerService to
		 * sign xml documents.
		 * Input: strXmlDoc - the document to be signed
		 * Return values: the signed document 
		 */
		private string SignDocument( string strXmlDoc )
		{
			string strRetVal = "";
			try
			{
				if( m_bTraceOn )
					TraceInfo( "Signing document " + strXmlDoc );
				
				string[] arrSecurityManagerResponse = null;
				// Interact with SecurityManagerService via reflection
				InternalRegistry ireg = new InternalRegistry();
				string strLocation = ireg.GetServiceLocation( SECURITY_MANAGER );
				if( strLocation.Length == 0 )
					throw new System.Exception( "Cannot Find Security Manager" );
				
				// Pass document to be signed to SecurityManagerService
				object[] objParams = new Object[1];
				objParams[0] = strXmlDoc;
				object objExecResult = ExecuteService( SECURITY_MANAGER, SIGN_DOCUMENT_METHOD, objParams );
				if( objExecResult != null )
				{
					// SecurityManagerService returns a string array
					arrSecurityManagerResponse = objExecResult as string[];
					strRetVal = arrSecurityManagerResponse[SECURITY_MANAGER_RETURNED_SIGNATURE_INDEX];
					if( strRetVal.Length > 0 && m_bTraceOn )
					{
						LogInfo( "Signed doc = " + strRetVal );
					}
				}
			}
			catch( System.Exception e )
			{
				LogError( e.Message );		
			}
			return strRetVal;
		}//End SignDocument
		
		public void TraceError( string strMsg )
		{
			m_objTracer.TraceError( strMsg );
		}

		public void TraceInfo( string strMsg )
		{
			m_objTracer.TraceInfo( strMsg );
		}

		public void TraceWarning( string strMsg )
		{
			m_objTracer.TraceWarning( strMsg );
		}
		
		/* Function uses the SecurityManagerService to verify
		 * the signature on a signed document.
		 * Inputs: strXmlDoc - the signed document to be verify
		 * Return values: verified document
		 */
		private string VerifyDocument( string strXmlDoc )
		{
			string strRetVal = "";
			try
			{
				if( m_bTraceOn )
					TraceInfo( "Verifying Document " + strXmlDoc );
				
				string[] arrSecurityManagerResponse = null;
				// Interact with SecurityManagerService via reflection
				InternalRegistry ireg = new InternalRegistry();
				string strLocation = ireg.GetServiceLocation( SECURITY_MANAGER );
				if( strLocation.Length == 0 )
					throw new System.Exception( "Cannot Find Security Manager" );
				
				// Pass document to verify to SecurityManagerService
				object[] objParams = new Object[1];
				objParams[0] = strXmlDoc;
				object objExecResult = ExecuteService( SECURITY_MANAGER, VERIFY_DOCUMENT_METHOD, objParams );
				if( objExecResult != null )
				{
					// SecurityManagerService returns a string array
					arrSecurityManagerResponse = objExecResult as string[];
					strRetVal = arrSecurityManagerResponse[SECURITY_MANAGER_RETURNED_VERIFICATION_INDEX];
					
					if( strRetVal.Length > 0 && m_bTraceOn )
					{
						LogInfo( "Verified doc = " + strRetVal );
					}
				}
			}
			catch( System.Exception e )
			{
				LogError( e.Message );		
			}
			return strRetVal;
		}
	}// End GateKeeper
}
