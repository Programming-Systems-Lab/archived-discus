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
using PSL.DISCUS.DataAccess;
using PSL.DISCUS.DynamicProxy;
using PSL.DISCUS.Logging;
using PSL.AsyncCore;

// DISCUS GateKeeper package
namespace PSL.DISCUS
{
	public enum enuGatekeeperStatus
	{
		Ok = 0,
		Error = 1
	};

	/// <summary>
	/// Implementation of the Gatekeeper
	/// </summary>
	/// <remarks>The Gatekeeper is one of the key components in DISCUS.
	/// It is the single accesspoint for interacting with services in a service space.
	/// It is the point where security policy is enforced (via using the SecurityManager)
	/// and it acts as an execution proxy to the web services making up the service space.
	/// </remarks>
	public class Gatekeeper:IGatekeeper
	{		
		// Constants

		public const int GK_STATUS_FIELD = 0;
		public const int GK_STATUS_MSG_FIELD = 1;
		public const string GK_SUCCESS_STATE_MSG = "Success State";
		public const string GK_ERROR_STATE_MSG = "Error State";

		/// <summary>
		/// SecurityManager service name constant
		/// </summary>
		public const string SECURITY_MANAGER = "SecurityManagerService";
		
		/// <summary>
		/// Request check method exposed by the SecurityManager service
		/// </summary>
		public const string REQUEST_CHECK_METHOD = "doRequestCheck";
		
		/// <summary>
		/// Sign document method exposed by the SecurityManager service
		/// </summary>
		public const string SIGN_DOCUMENT_METHOD = "signDocument";

		/// <summary>
		/// Verify document method exposed by the SecurityManager service
		/// </summary>
		public const string VERIFY_DOCUMENT_METHOD = "verifyDocument";
		
		/// <summary>
		/// Verify treaty method exposed by the SecurityManager service
		/// </summary>
		public const string VERIFY_TREATY_METHOD = "verifyTreaty";
		
		/// Security Manager constants
		
		/// <summary>
		/// Security Manager status field constant - every call to the Security 
		/// Manager returns an array where a set, usually the first array, position 
		/// contains a status code
		/// </summary>
		public const int SECURITY_MANAGER_STATUS_FIELD = 0;

		/// <summary>
		/// Security Manager error code
		/// </summary>
		public const string SECURITY_MANAGER_ERROR_CODE = "-1";

		/// <summary>
		/// Security Manager returned treaty index constant - every call to the Security 
		/// Manager returns an array where a set, usually the first, array position 
		/// contains a status code and another set, usually the second, array 
		/// position contains the results returned. 
		/// </summary>
		public const int SECURITY_MANAGER_RETURNED_TREATY_INDEX = 1;

		/// <summary>
		/// Security Manager returned signature index constant - every call to the Security 
		/// Manager returns an array where a set, usually the first, array position 
		/// contains a status code and another set, usually the second, array 
		/// position contains the results returned. 
		/// </summary>
		public const int SECURITY_MANAGER_RETURNED_SIGNATURE_INDEX = 1;
		
		/// <summary>
		/// Security Manager returned verification index constant - every call to the Security 
		/// Manager returns an array where a set, usually the first, array position 
		/// contains a status code and another set, usually the second, array 
		/// position contains the results returned. 
		/// </summary>
		public const int SECURITY_MANAGER_RETURNED_VERIFICATION_INDEX = 1;
		
		/// GateKeeper Methods
		public const string GK_EXECUTE_SERVICE_METHOD = "ExecuteServiceMethod";
		public const string GK_ENLIST_SERVICES_BY_NAME_METHOD = "EnlistServicesByName";
		public const string DEFAULT_CLIENT_SERVICE_SPACE = "1";

		// Fields
		
		/// <summary>
		/// Threadsafe settings object
		/// </summary>
		private GatekeeperSettings m_settings = new GatekeeperSettings();
		
		/*****************************************************************/
		// Clean up log trace infrastructure - objects need to be threadsafe

		/// <summary>
		/// Logger object
		/// </summary>
		private LoggerImpl m_objLogger; 
		
		/// <summary>
		/// Tracer object
		/// </summary>
		private TracerImpl m_objTracer; 
		
		/*****************************************************************/

		// Properties

		public string CurrentDirectoryFromCodebase
		{
			get
			{ return Assembly.GetExecutingAssembly().CodeBase; }
		}

		/// <summary>
		/// get/set Gatekeeper name 
		/// </summary>
		public string Name
		{
			get
			{ return m_settings.GatekeeperName; }
			set
			{ 
				// Set GK name
				m_settings.GatekeeperName = value; 
			}
		}
				
		/// <summary>
		/// get/set Location where dynamically generated proxies are stored 
		/// </summary>
		public string ProxyCache
		{
			get
			{ return m_settings.ProxyCache; }
			set
			{ m_settings.ProxyCache = value; }
		}
				
		/// <summary>
		/// get/set(true/false) tracing 
		/// </summary>
		public bool TraceOn
		{
			get
			{ return m_settings.TraceOn; }
			set
			{ m_settings.TraceOn = value; }
		}

		/// <summary>
		/// get/set the database connection string
		/// </summary>
		public string DatabaseConnectString
		{
			get
			{ return this.m_settings.DatabaseConnectString; }
			set
			{ this.m_settings.DatabaseConnectString = value; }
		}
		
		public enuLoggingType LoggingType
		{
			get
			{ return this.m_settings.LoggingType; }
			set
			{ this.m_settings.LoggingType = value; }
		}

		public enuTracingType TracingType
		{
			get
			{ return this.m_settings.TracingType; }
			set
			{ this.m_settings.TracingType = value; }
		}
		
		public string UrlLoggingHostname
		{
			get
			{ return this.m_settings.UrlLoggingHostname; }
			set
			{ this.m_settings.UrlLoggingHostname = value; }
		}

		public int UrlLoggingPort
		{
			get
			{ return this.m_settings.UrlLoggingPort; }
			set
			{ this.m_settings.UrlLoggingPort = value; }
		}

		public string UrlTracingHostname
		{
			get
			{ return this.m_settings.UrlTracingHostname; }
			set
			{ this.m_settings.UrlTracingHostname = value; }
		}

		public int UrlTracingPort
		{
			get
			{ return this.m_settings.UrlTracingPort; }
			set
			{ this.m_settings.UrlTracingPort = value; }
		}

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
		/// <summary>
		/// Ctor.
		/// </summary>
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public Gatekeeper()
		{
			try
			{
				string strLocalPath = new Uri( Assembly.GetExecutingAssembly().CodeBase ).LocalPath;
				this.m_settings.ProxyCache = new FileInfo( strLocalPath ).Directory.FullName + "\\" + Constants.DEFAULT_PROXY_CACHE;				
							
				// GatekeeperSettings object sets appropriate defaults
				// Create logging and tracing objects based on appsettings data
				LogTraceContext logCtx = new LogTraceContext();
				
				// This data may or may not be set let the factory method 
				// CreateLoggingInstance handle it
				logCtx.Hostname = this.UrlLoggingHostname;
				logCtx.Port = this.UrlLoggingPort;

				this.m_objLogger = CreateLoggingInstance( this.LoggingType, logCtx );

				if( this.TraceOn )
				{
					LogTraceContext traceCtx = new LogTraceContext();
					
					// This data may or may not be set let the factory method 
					// CreateTracingInstance handle it
					traceCtx.Hostname = this.UrlTracingHostname;
					traceCtx.Port = this.UrlTracingPort;
					
					// Turn off tracing if invalid data passed in
					if( traceCtx.Hostname.Length == 0 || this.UrlTracingPort == -1 )
						this.TracingType = enuTracingType.None;

					this.m_objTracer = CreateTracingInstance( this.TracingType, traceCtx );
				}
				
				// Determine whether ProxyCache Directory exists
				// If it does not exist then we try to create it
				bool bStatus = InitializeProxyCacheDir();
				if( this.TraceOn )
				{
					if( bStatus )
						TraceInfo( this.Name, "ProxyCache Initialized" );
					else TraceError( this.Name, "ProxyCache NOT Initialized" );
				}
			}
			catch( System.Exception e ) // Catch exception
			{
				// Report error
				LogError( this.Name, e.Message );
			}
		}

		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="settings">GatekeeperSettings object used to configure
		/// the Gatekeeper</param>
		public Gatekeeper( GatekeeperSettings settings )
		{
			// Set the Gatekeeper settings instance, this sets all the Gatekeeper
			// public properties
			if( settings == null )
				LogInfo( this.Name, "settings parameter is null, creating Gatekeeper using defaults" );
			else this.m_settings = settings; 
			
			try
			{
				// GatekeeperSettings object sets appropriate defaults
				// Create logging and tracing objects based on appsettings data
				LogTraceContext logCtx = new LogTraceContext();
				
				// This data may or may not be set let the factory method 
				// CreateLoggingInstance handle it
				logCtx.Hostname = this.UrlLoggingHostname;
				logCtx.Port = this.UrlLoggingPort;

				this.m_objLogger = CreateLoggingInstance( this.LoggingType, logCtx );

				if( this.TraceOn )
				{
					LogTraceContext traceCtx = new LogTraceContext();
					
					// This data may or may not be set let the factory method 
					// CreateTracingInstance handle it
					traceCtx.Hostname = this.UrlTracingHostname;
					traceCtx.Port = this.UrlTracingPort;
					
					// Turn off tracing if invalid data passed in
					if( traceCtx.Hostname.Length == 0 || this.UrlTracingPort == -1 )
						this.TracingType = enuTracingType.None;

					this.m_objTracer = CreateTracingInstance( this.TracingType, traceCtx );
				}
				
				// Determine whether ProxyCache Directory exists
				// If it does not exist then we try to create it
				bool bStatus = InitializeProxyCacheDir();
				if( this.TraceOn )
				{
					if( bStatus )
						TraceInfo( this.Name, "ProxyCache Initialized" );
					else TraceError( this.Name, "ProxyCache NOT Initialized" );
				}
			}
			catch( System.Exception e ) // Catch exception
			{
				// Report error
				LogError( this.Name, e.Message );
			}
		}
		
		/// <summary>
		/// Function builds an Execution Queue (sequence of steps)
		/// from an Alpha protocol
		/// </summary>
		/// <param name="strAlphaProtocol">Sequence of actions to be
		/// performed (expressed using a simple subset of xlang)</param>
		/// <returns>A Queue of steps to execute</returns>
		private Queue BuildAlphaProtocolExecutionQ( string strAlphaProtocol )
		{
			Queue execQ = new Queue();
			
			try
			{
				if( this.TraceOn )
					TraceInfo( this.Name, "About to build execQ from Alpha-Protocol" );// + strAlphaProtocol );
				
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
				LogError( this.Name, e.Message );				
			}
			
			if( this.TraceOn )
				TraceInfo( this.Name, "Built execQ: " + execQ.Count + " actions to perform" );
				
			return execQ; // return Execution Queue
		}// End BuildAlphaProtocolExecutionQ

		/// <summary>
		/// Function Builds a Treaty based on a stack of
		/// services requested from the same provider 
		/// service space 
		/// </summary>
		/// <param name="stkProviders">A stack of requests to a 
		/// provider</param>
		/// <returns>A treaty request to be sent out
		/// </returns>
		TreatyType BuildTreaty( Stack stkProviders )
		{
			// If Stack is empty, nothing to do so exit
			if( stkProviders.Count == 0 )
				return null;
			
			if( this.TraceOn )
				TraceInfo( this.Name, "About to build treaty" );
			
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
				treaty.ProviderServiceSpace = ( (AlphaRequest) stkProviders.Peek() ).Provider;
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
				
				if( this.TraceOn )
					TraceInfo( this.Name, "Built treaty" );// + treaty.ToXml() );
			}
			catch( Exception e )
			{
				LogError( this.Name, e.Message );				
			}
			return treaty; // return the Treaty we create from the stack
		}// End BuildTreaty
		
		
		/// <summary>
		/// Function analyzes all Treaties created and returns 
		/// true if for each treaty the requested methods have
		/// been authorized, i.e. permission has been granted
		/// to use all the methods.
		/// </summary>
		/// <param name="mapping">The mapping between external GKs
		/// and the treaties created</param>
		/// <returns>True if for all treaties all methods
		/// autorized, otherwise returns false 
		/// </returns>
		private bool CanExecuteAllSteps( Hashtable mapping )
		{
			if( mapping == null || mapping.Count == 0 )
				return false;

			bool bRetVal = false;

			if( this.TraceOn )
				TraceInfo( this.Name, "Checking whether we can execute all requested service methods" );
			
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
				LogError( this.Name, e.Message );				
			}
			return bRetVal;
		}

		/// <summary>
		/// Function creates a logging instance. 
		/// </summary>
		/// <param name="logType">The type of logging instance to create
		/// EvtLoggerImpl or UrlLoggerImpl
		/// based on the logging types
		/// </param>
		/// <param name="logCtx"></param>
		/// <returns>An instance of the specified logging implementation OR
		/// an instance of the default logging implementation, EvtLoggerImpl
		/// </returns>
		private LoggerImpl CreateLoggingInstance( enuLoggingType logType, LogTraceContext logCtx )
		{
			LoggerImpl objLoggingInst = null;
			
			try
			{
				if( logType == enuLoggingType.UrlLog )
				{
					// UrlLoggerImpl
					objLoggingInst = new UrlLoggerImpl( logCtx );
				}
				else objLoggingInst = new EvtLoggerImpl( logCtx );
			}
			catch( Exception e )
			{
				objLoggingInst = new EvtLoggerImpl( logCtx );
				objLoggingInst.LogError( this.Name, e.Message );
			}

			return objLoggingInst;
		}
		
		/// <summary>
		/// Function creates a tracing instance.  
		/// </summary>
		/// <param name="traceType">The type of tracing instance to create
		/// EvtTracerImpl or UrlTracerImpl
		/// based on the tracing types
		/// </param>
		/// <param name="traceCtx"></param>
		/// <returns>An instance of the specified tracing implementation OR
		/// an instance of the default tracing implementation, EvtTracerImpl
		/// </returns>
		private TracerImpl CreateTracingInstance( enuTracingType traceType, LogTraceContext traceCtx )
		{
			TracerImpl objTracingInst = null;
			
			try
			{
				if( traceType == enuTracingType.None )
				{
					this.TraceOn = false;
					return null;
				}
				else if( traceType ==  enuTracingType.UrlTrace )
				{
					// UrlTracerImpl
					objTracingInst = new UrlTracerImpl( traceCtx );
				}
				else objTracingInst = new EvtTracerImpl( traceCtx );
			}
			catch( Exception e )
			{
				objTracingInst = new EvtTracerImpl( traceCtx );
				objTracingInst.TraceError( this.Name, e.Message );
			}

			return objTracingInst;
		}
		
		/// <summary>
		/// Procedure dissolves a Treaty with a given TreatyID 
		/// </summary>
		/// <param name="nTreatyID">The treaty to be dissolved</param>
		public void DissolveTreaty( int nTreatyID )
		{
			if( this.TraceOn )
				TraceInfo( this.Name, "Dissolving Treaty " + nTreatyID );
			
		}// End DissolveTreaty

		/* 
		// Async version
		private string[] DoRequestCheck( string[] strXmlExecRequest, bool bSigned )
		{
			ArrayList lstResults = new ArrayList();
				
			try
			{
				if( this.TraceOn )
					TraceInfo( "Doing request check - Async" );

				// Resolve SecurityManager location first, in the event that
				// a proxy has to be generated
				
			}
			catch( System.Exception e )
			{
				LogError( e.Message );		
			}

			return (string[]) lstResults.ToArray( typeof(System.String) );
		}
		*/

		/// <summary>
		/// Function uses the SecurityManagerService to check whether
		/// a request for service use is allowed or not.
		/// </summary>
		/// <param name="strXmlExecRequest">Execution request to check</param>
		/// <param name="bSigned"></param>
		/// <returns>An array of strings containing the status code and response 
		/// from the SecurityManagerService
		/// </returns>
		private string[] DoRequestCheck( string strXmlExecRequest, bool bSigned )
		{
			string[] arrRetVal = null;
			
			try
			{
				if( this.TraceOn )
					TraceInfo( this.Name, "Doing request check" );
			
				WSDetails details = ServiceDetails( SECURITY_MANAGER );
				ResolveServiceLocation( ref details );
								
				if( details.Location.Length == 0  )
					throw new Exception( "Cannot Find Security Manager" );
				
				// Create array of parameters to pass to SecurityManagerService
				object[] objParams = new object[2];
				objParams[0] = strXmlExecRequest;
				objParams[1] = bSigned;
				
				ExecServiceContext ctx = new ExecServiceContext();
				ctx.AccessPointUrl = details.AccessPoint;
				ctx.Assembly = details.Location;
				ctx.ServiceName = details.Name;
				ctx.MethodName = REQUEST_CHECK_METHOD;
				ctx.Parameters.AddRange( objParams );
				
				// Execute Request Check method of security manager
				object objExecResult = ExecuteServiceDirect( ctx );
				
				// Convert result to string[] 
				if( objExecResult != null )
					arrRetVal = objExecResult as string[];
			}
			catch( System.Exception e )
			{
				LogError( this.Name, e.Message );		
			}
			return arrRetVal; // return SecurityManagerService response
		}//End DoRequestCheck

		/// <summary>
		/// Function responds to a treaty request for services 
		/// </summary>
		/// <param name="strXmlTreatyReq">Treaty request</param>
		/// <returns>Treaty returned from the SecurityManagerService</returns>
		public string EnlistServicesByName( string strXmlTreatyReq )
		{
			string strRetVal = "";
			try
			{
				if( this.TraceOn )
					TraceInfo( this.Name, "Received treaty request, passing to SecurityManager to verify" );// + strXmlTreatyReq );
			
				// Create array of params to pass to VerifyTreaty
				// of SecurityManagerService
				object[] objParams = new object[2];
				objParams[0] = strXmlTreatyReq;
				objParams[1] = false; // request not signed (yet)
				object objRes = null; // return value
				
				//TODO: Remove later...for demo only
				if( this.TraceOn )
					TraceInfo( this.Name, "EXECGUI_5" );

				WSDetails details = ServiceDetails( SECURITY_MANAGER );
				ResolveServiceLocation( ref details );
								
				if( details.Location.Length == 0  )
					throw new Exception( "Cannot Find Security Manager" );
				
				ExecServiceContext ctx = new ExecServiceContext();
				ctx.AccessPointUrl = details.AccessPoint;
				ctx.Assembly = details.Location;
				ctx.ServiceName = details.Name;
				ctx.MethodName = VERIFY_TREATY_METHOD;
				ctx.Parameters.AddRange( objParams );

				// Execute method
				objRes = ExecuteServiceDirect( ctx );
								
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
						if( this.TraceOn )
						{
							//TraceInfo( "SecurityManager verification returned: " + strRetVal );
							//TODO: Remove later...for demo only
							TraceInfo( this.Name, "EXECGUI_6" );
						}
					}
				}
			}
			catch( Exception e )
			{
				LogError( this.Name, e.Message );		
			}
			return strRetVal;
		}// End EnlistServicesByName
		
		/// <summary>
		/// Function executes an Alpha-protocol (Simple XLANG workflow).  
		/// </summary>
		/// <param name="strAlphaProtocol">Alpha-protocol to execute</param>
		/// <returns>A string array of the returned results for each action</returns>
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public string[] ExecuteAlphaProtocol( string strAlphaProtocol )
		{
			// If Alpha-protocol empty then exit
			if( strAlphaProtocol.Length == 0 )
				return null;

			// TODO: Remove later...for demo only
			if( this.TraceOn )
				TraceInfo( this.Name, "EXECGUI_2" );

			ArrayList lstResults = new ArrayList();
			ArrayList lstTaskIDs = new ArrayList();
			
			// 4 stage execution			
			// 1) Build the execution queue (steps to perform)
			// 2) Create nec treaties with other GKs - Resource Acquire stage
			// 3) Ensure that all requested service authorized
			// 4) Execution stage
			
			try
			{
				// Stage 1 - Build Execution queue (steps to execute)
				if( this.TraceOn )
					TraceInfo( this.Name, "MESSAGE " + "Executing Alpha Protocol Stage 1 - Building ExecutionQ" );
			
				Queue execQ = BuildAlphaProtocolExecutionQ( strAlphaProtocol );
				if( execQ == null || execQ.Count == 0 )
					throw new Exception( "Error obtaining sequence of actions to perform" );

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
				if( this.TraceOn )
					TraceInfo( this.Name, "MESSAGE " + "Executing Alpha Protocol Stage 2 - Creating Treaties" );
				
				Hashtable mapping = FormTreaties( arrReqs );
				if( mapping == null || mapping.Count == 0 )
					throw new Exception( "Resource Acquire Failed, Error Creating Treaties" );

				if( this.TraceOn )
					TraceInfo( this.Name, "Executing Alpha Protocol Stage 3 - Analyzing Treaty Responses" );
				// Stage 3 - Analyze mapping to make sure all requested methods authorized
				// if not then we report the failure and exit
				if( !CanExecuteAllSteps( mapping ) )
				{
					// TODO: Edit later...for demo only
					if( this.TraceOn )
						TraceError( this.Name, "All Necessary Resources NOT Acquired" );
					
					// Write Treaties to event log for analysis later
					throw new Exception( "Resource Acquire Failed, All Requests NOT Authorized" );
				}
				else
				{
					// TODO: Edit later...for demo only
					if( this.TraceOn )
					{
						TraceInfo( this.Name, "MESSAGE " + "Acquired All Necessary Resources" );
						TraceInfo( this.Name, "EXECGUI_7" );
					}
				}

				// Stage 4 - Execution stage
				if( this.TraceOn )
					TraceInfo( this.Name, "MESSAGE " + "Executing Alpha Protocol Stage 4 - Execution" );
				
				IEnumerator it = execQ.GetEnumerator();
				InternalRegistry ireg = new InternalRegistry( this.DatabaseConnectString );
				TaskProxy pxy = new TaskProxy();
			
				// TODO: Edit later...for demo only
				if( this.TraceOn )
					TraceInfo( this.Name, "EXECGUI_8" );

				while( it.MoveNext() )
				{
					AlphaRequest actionReq = (AlphaRequest) it.Current;
					// GK expects one parameter - an XML document
					// representing a request
					object[] objParams = new object[1];
					objParams[0] = actionReq.m_Req.ToXml();
					// Retrieve current treaty (applicable for provider)
					TreatyType currentTreaty = (TreatyType) mapping[actionReq.Provider];
					if( currentTreaty == null )
						throw new Exception( "Error retrieving treaty for " + actionReq.Provider );

					// Set TreatyID
					actionReq.m_Req.TreatyID = currentTreaty.TreatyID;
					// Resolve method implementation as specified by treaty
					actionReq.m_Req.MethodName = ResolveMethodImplementation( actionReq, currentTreaty );
					
					WSDetails details = GatekeeperDetails( actionReq.Provider );
					ResolveGatekeeperLocation( ref details );
					
					ExecServiceContext ctx = new ExecServiceContext();
					ctx.AccessPointUrl = details.AccessPoint;
					ctx.Assembly = details.Location;
					ctx.ServiceName = details.Name;
					ctx.MethodName = GK_EXECUTE_SERVICE_METHOD;
					ctx.Parameters.AddRange( objParams );

                    lstTaskIDs.Add( pxy.AddTaskRequest( new TaskRequest( new TaskRequest.TaskCallback(ExecuteServiceDirect), ctx ) ) );
				}// End while it.MoveNext()

				// Start proxy wait
				pxy.StartWait();

				// Do Something here
				
				// Wait on proxy signal
				pxy.WaitOnProxySignal();

				for( int i = 0; i < lstTaskIDs.Count; i++ )
				{
					TPTaskCompleteEventArgs tceArg = pxy.QueryResult( (Guid) lstTaskIDs[i] );
					
					if( tceArg != null )
					{
						string strTemp = (string) tceArg.Result;
						string strVerify = VerifyDocument( strTemp );
						if( strVerify.Length > 0 )
							lstResults.Add( strVerify );
						else lstResults.Add( strTemp );
					}
				}
			}
			catch( System.Exception e )
			{
				LogError( this.Name, e.Message );
			}
			
			if( this.TraceOn )
			{
				TraceInfo( this.Name, "Finished Executing Alpha Protocol" );
				TraceInfo( this.Name, "EXECGUI_9" );
			}
				
			return (string[]) lstResults.ToArray( typeof(System.String) );
		}//End ExecuteAlphaProtocol
		
		/// <summary>
		/// Function executes a method against a Gatekeeper proxy via reflection. 
		/// </summary>
		/// <param name="strGKName">Name of GK to execute against</param>
		/// <param name="strGKMethod">Name of method to execute</param>
		/// <param name="objParams">Array of parameters to pass to proxy method</param>
		/// <returns></returns>
		/*private object ExecuteGatekeeper( string strGKName, string strGKMethod, object[] objParams )
		{
			object objInvokeResult = null;
			try
			{
				if( this.TraceOn )
					TraceInfo( "Executing method " + strGKMethod + " against GK " + strGKName  );
							
				// Check that we have been given a GK name and a method name to execute
				if( strGKName.Length == 0 || strGKMethod.Length == 0 )
					throw new System.Exception( "Invalid Arguments Passed to ExecuteGateKeeper" );
				
				// Get GK details
				WSDetails details = GatekeeperDetails( strGKName );
				details.Method = strGKMethod;

				// Quick checks for valid data returned from database
				if( details.Location.Length == 0 )
					throw new Exception( "Cannot Find Location of GateKeeper: " + details.Name );
				
				// Resolve Web Service Proxy Location
				// Do Runtime ProxyGeneration if necessary				
				// If GK location is a link to a WSDL file
				// then we must generate a proxy to the GK,
				// store the proxy in the proxy cache, update our
				// database and set strGKLocation to the generated
				// proxy. Must also change GK namespace in dbase
				ResolveGatekeeperLocation( ref details );
				
				// Set execution context
				ExecServiceContext execCtx = new ExecServiceContext();
				execCtx.AccessPointUrl = details.AccessPoint;
				execCtx.Assembly = details.Location;
				execCtx.MethodName = details.Method;
				execCtx.ServiceName = details.Name;
				execCtx.Parameters.AddRange( objParams );

				// Use Async invocation
				if( m_bUseAsync )
				{
					// Create condition variable, set to non-signaled state
					AutoResetEvent cond = new AutoResetEvent( false );
					// Create a task proxy with the condition variable
					TaskProxy pxy = new TaskProxy( ref cond );
					// Give tasks to the TaskProxy
					Guid execTaskId = pxy.AddTaskRequest( new TaskRequest( new TaskRequest.TaskCallback( ExecuteServiceDirect ), execCtx ) );
					// Let the TaskProxy wait on them
					pxy.WaitOnTasks();
					
					// TODO: Find stuff to do here while waiting on task(s) to finish
					// Could do logging, context save etc.
					
					// Sit and wait for proxy to signal tasks done
					pxy.Condition.WaitOne();

					if( !pxy.QueryResult( execTaskId ).HasErrors )
						objInvokeResult = pxy.QueryResult( execTaskId ).Result;
					else throw new Exception( pxy.QueryResult( execTaskId ).ExceptionMessage );
				}
				else 
				

				objInvokeResult = ExecuteServiceDirect( execCtx );
			}
			catch( System.Exception e )
			{
				LogError( e.Message );		
			}

			return objInvokeResult;		
		}//End ExecuteGatekeeper
		*/
				 
		/// <summary>
		/// Function takes an array of AlphaRequests and attempts
		/// to create the necessary Treaties that must go out
		/// to the external Gatekeepers from who we request
		/// use of service. Returns a mapping between the 
		/// Gatekeeper (provider) and the Treaty sent back as
		/// a response to a request for service use.
		/// </summary>
		/// <param name="arrReqs">The array of AlphaRequests sorted by Gatekeeper
		/// name</param>
		/// <returns>A mapping between each external
		/// gatekeeper and the treaty created with
		/// with them.</returns>
		private Hashtable FormTreaties( AlphaRequest[] arrReqs )
		{
			if( arrReqs.Length == 0 )
				return null;
			Hashtable mapping = null;
			try
			{
				if( this.TraceOn )
				{
					TraceInfo( this.Name, "Forming Treaties for " + arrReqs.Length + " requests" );
					
					//  TODO: Remove later...for demo only
					string strServicesTraceMsg = "SERVICE ";
					for( int i = 0; i < arrReqs.Length; i++ )
					{
						strServicesTraceMsg += arrReqs[i].m_Req.ServiceName;
						if( i + 1 < arrReqs.Length )
							strServicesTraceMsg += ";";
					}
					// Send Trace message
					TraceInfo( this.Name, strServicesTraceMsg );
				}
				
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
								
				TaskProxy pxy = new TaskProxy();
				Hashtable providers = new Hashtable();

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
						// TODO: Remove later...for demo only
						if( this.TraceOn )
							TraceInfo( this.Name, "EXECGUI_3" );
						
						// Build treaty
						TreatyType treaty = BuildTreaty( stkProviders );
						// Pass Treaty to remote GK to have it 
						// verified so we can get a TreatyID to use
						object[] objParams = new object[1];
						objParams[0] = treaty.ToXml();

						//  TODO: Remove later...for demo only
						if( this.TraceOn )
						{
							TraceInfo( this.Name, "EXECGUI_4" );
						}

						// Give operation to TaskProxy
						WSDetails details = GatekeeperDetails( treaty.ProviderServiceSpace );
						ResolveGatekeeperLocation( ref details );
						
						ExecServiceContext ctx = new ExecServiceContext();
						ctx.ServiceName = details.Name;
						ctx.MethodName = GK_ENLIST_SERVICES_BY_NAME_METHOD;
						ctx.Parameters.AddRange( objParams );
						ctx.AccessPointUrl = details.AccessPoint;
						ctx.Assembly = details.Location;
						
						Guid taskID = pxy.AddTaskRequest( new TaskRequest( new TaskRequest.TaskCallback(ExecuteServiceDirect), ctx ) );	
						// Map the taskIDs to specific provider service spaces
						providers.Add( treaty.ProviderServiceSpace, taskID );
												
						// Clear stack	
						stkProviders.Clear();
						// If no nore requests then exit otherwise get the next one
						if( !it.MoveNext() )
							break;
                        else it.MoveNext();
					}
					
					stkProviders.Push( (AlphaRequest) it.Current );
					currReq = (AlphaRequest) it.Current;
				}// End While true

				// Start proxy waiting
				pxy.StartWait();

				// Do something here

				// Wait on proxy signal
				pxy.WaitOnProxySignal();

				IDictionaryEnumerator ide = providers.GetEnumerator();

				while( ide.MoveNext() )
				{
					Guid task = (Guid) ide.Value;
					string strProvider = (string) ide.Key;

					object objRes = pxy.QueryResult( task ).Result;
					
					if( objRes != null && ((string) objRes).Length > 0 )
					{
						// Deserialize into TreatyType
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
					else throw new Exception( "Error Creating Treaty with " + strProvider );
				}
			}
			catch( Exception e )
			{
				LogError( this.Name, e.Message );				
			}
			return mapping;
		}// End FormTreaties
		
		/// <summary>
		/// Function tests whether the proxy cache directory
		/// exists, if it does then ok, otherwise we try to
		/// create it.
		/// </summary>
		/// <returns>True if proxy cache set up, false otherwise</returns>
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		private bool InitializeProxyCacheDir()
		{
			bool bRetVal = false;
			
			try
			{
				if( this.TraceOn )
					TraceInfo( this.Name, "Initizlize Proxy Cache Dir" );
				
				// Determine if directory name of the proxy cache
				// read from the system config file is NOT Fully 
				// Quallified i.e <drive letter>:\<path>
				// If not Fully Qualified then we must prefix
				// with Current Directory
				if( m_settings.ProxyCache.IndexOf( ":" ) == -1 )
				{
					// Link to current dir
					DirectoryInfo temp = new DirectoryInfo( "." );
					// Get FullQName
					string strPath = temp.FullName;
					// Append proxy cache sub dir
					strPath += "\\";
					strPath += m_settings.ProxyCache;
					m_settings.ProxyCache = strPath;
					temp = null;
				}
				
				// Try to access DirectoryInfo of ProxyCache
				DirectoryInfo dirInfo = new DirectoryInfo( m_settings.ProxyCache );
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
					Directory.CreateDirectory( m_settings.ProxyCache ); 
					bRetVal = true;
				}
				catch( System.Exception sysE )
				{
					// Report Error
					string strError = "Error Creating ProxyCache ";
					strError += sysE.Message;
					LogError( this.Name, strError );
				}
			}
			return bRetVal;
		}//End InitializeProxyCacheDir
			
		/// <summary>
		/// Procedure writes an error message
		/// </summary>
		/// <param name="strMsg">The messsage to write</param>
		public void LogError( string strSource, string strMsg )
		{
			m_objLogger.LogError( strSource, strMsg );
		}
	
		/// <summary>
		/// Procedure writes an info message
		/// </summary>
		/// <param name="strMsg">The messsage to write</param>
		public void LogInfo( string strSource, string strMsg )
		{
			m_objLogger.LogInfo( strSource, strMsg );
		}

		/// <summary>
		/// Procedure writes a warning message
		/// </summary>
		/// <param name="strMsg">The messsage to write</param>
		public void LogWarning( string strSource, string strMsg )
		{
			m_objLogger.LogWarning( strSource, strMsg );
		}
		
		/// <summary>
		/// Function resolves the name of a service method specified in a Treaty
		/// with the name of the actual implementation method
		/// </summary>
		/// <param name="actionReq"></param>
		/// <param name="currentTreaty"></param>
		/// <returns>The name of the actual implementation method</returns>
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
				LogError( this.Name, e.Message );
			}
			return strRetVal;
		}
		
		/// <summary>
		/// Procedure gets the SecurityManagerSerivce to
		/// revoke a given treaty
		/// </summary>
		/// <param name="nTreatyID">The treaty to revoke</param>
		public void RevokeTreaty( int nTreatyID )
		{
			if( this.TraceOn )
				TraceInfo( this.Name, "Revoking Treaty " + nTreatyID );
				
		}//End RevokeTreaty

		/// <summary>
		/// Procedure writes an error message
		/// </summary>
		/// <param name="strMsg">The message to write</param>
		public void TraceError( string strSource, string strMsg )
		{
			m_objTracer.TraceError( strSource, strMsg );
		}

		/// <summary>
		/// Procedure writes an Info message
		/// </summary>
		/// <param name="strMsg">The message to write</param>
		public void TraceInfo( string strSource, string strMsg )
		{
			m_objTracer.TraceInfo( strSource, strMsg );
		}

		/// <summary>
		/// Procedure writes a Warning trace message
		/// </summary>
		/// <param name="strMsg">The message to write</param>
		public void TraceWarning( string strSource, string strMsg )
		{
			m_objTracer.TraceWarning( strSource, strMsg );
		}
				
		
		/*****************************************************************/
		// Refactored methods

		/// <summary>
		/// Function executes a service method based on a request.
		/// </summary>
		/// <param name="strXmlExecRequest">The Xml representation of a request</param>
		/// <returns>the results of executing the requested method
		/// serialized to xml if the request is allowed by the 
		/// SecurityManagerService.</returns>
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public string[] ExecuteServiceMethod( string strXmlExecRequest )
		{
			ArrayList lstRetVal = new ArrayList();
			object[] arrStatus = new object[2];
			string strRetVal = "";
			ExecServiceMethodRequestType execReq = null;
			InternalRegistry ireg = new InternalRegistry( this.DatabaseConnectString );

			try
			{
				if( this.TraceOn )
					TraceInfo( this.Name, "Received ExecServiceMethodRequest" );
				
				arrStatus[Gatekeeper.GK_STATUS_FIELD] = enuGatekeeperStatus.Ok.ToString();
				arrStatus[Gatekeeper.GK_STATUS_MSG_FIELD] = Gatekeeper.GK_SUCCESS_STATE_MSG;
				
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
				if( this.TraceOn )
					TraceInfo( this.Name, "Stage 1 - Do Security Check" );
				
				// TODO: Ignoring SecurityManager for now
				//string[] arrSecurityManagerResponse = DoRequestCheck( strXmlExecRequest, false );
				//if( arrSecurityManagerResponse == null || arrSecurityManagerResponse[SECURITY_MANAGER_STATUS_FIELD].CompareTo( SECURITY_MANAGER_ERROR_CODE ) == 0 )
				//	throw new System.Exception( "Security Exception: Request Not Verified - Reason: " + arrSecurityManagerResponse[1] );
				
				if( this.TraceOn )
					TraceInfo( this.Name, "Passed Security Check" );
				
				try
				{
					// Stage 2 - Deserialize Execution request
					if( this.TraceOn )
						TraceInfo( this.Name, "Stage 2 - Deserialize Execution Request" );
				
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
				if( this.TraceOn )
					TraceInfo( this.Name, "Stage 3 - Resolve Web Service Proxy Location" );
				
				WSDetails details = ServiceDetails( execReq.ServiceName );
				// Fill in method details
				details.Method = execReq.MethodName;
				details.MethodExists = ireg.MethodExists( details.Name, details.Method );

				// Quick checks for valid data returned from database
				if( details.Location.Length == 0 )
					throw new Exception( "Cannot Find Location of Service: " + details.Name );
				// Verify that service supports method
				if( !details.MethodExists )
					throw new Exception( "Service: " + details.Name + " does not support method: " + details.Method );
	
				// Do Runtime ProxyGeneration if necessary				
				// If service location is a link to a WSDL file
				// then we must generate a proxy to the webservice,
				// store the proxy in the proxy cache, update our
				// database and set strServiceLocation to the generated
				// proxy. Must also change service namespace in dbase
				ResolveServiceLocation( ref details );
								
				// Set context info for service invocation                     
				ExecServiceContext execCtx = new ExecServiceContext();
				execCtx.Assembly = details.Location;
				execCtx.ServiceName = details.Name;
				execCtx.MethodName = details.Method;
				execCtx.AccessPointUrl = details.AccessPoint;
				execCtx.Parameters = execReq.m_ParamValue;
				
				strRetVal = (string) ExecuteServiceRequest( execCtx );
				if( strRetVal.Length == 0 )
					throw new Exception( "Executing request returned no results" );

				// New protocol to give clients more info
				lstRetVal.Add( strRetVal );
			}
			catch( System.Exception e )
			{
				LogError( this.Name, e.Message );
				// Update status data	
				arrStatus[Gatekeeper.GK_STATUS_FIELD] = enuGatekeeperStatus.Error.ToString();
				arrStatus[Gatekeeper.GK_STATUS_MSG_FIELD] = Gatekeeper.GK_ERROR_STATE_MSG;
				lstRetVal.Add( e.Message );
			}
			finally
			{
				// put arrStatus at the start of lstResults
				lstRetVal.InsertRange( 0, arrStatus );
			}
						
			if( this.TraceOn )
				TraceInfo( this.Name, "Finished Executing Service Method Request" );
						
			return (string[]) lstRetVal.ToArray( typeof(System.String) );
		}//End ExecuteServiceMethod

		/// <summary>
		/// Function executes an array of requests
		/// </summary>
		/// <param name="strXmlExecRequest">XML representation of an execution request</param>
		/// <returns>The result of executing all requests</returns>
		public string[] ExecuteMultipleServiceMethods( string[] arrXmlExecRequest )
		{
			object[] arrStatus = new object[2];
			// Method specifically for async operations
			ArrayList lstResults = new ArrayList();
			// Stores the deserialized execution requests
			ArrayList lstRequests = new ArrayList();
			// Stores the taskIDs returned when we give tasks to a task proxy
			// to wait on
			ArrayList lstTaskIDs = new ArrayList();
			// Stores the names of services for which we must generate
			// proxy assemblies
			Hashtable proxiesToGen = new Hashtable();
			// Create a TaskProxy to give tasks to
			TaskProxy pxy = new TaskProxy();
				
			try
			{
				arrStatus[Gatekeeper.GK_STATUS_FIELD] = enuGatekeeperStatus.Ok.ToString();
				arrStatus[Gatekeeper.GK_STATUS_MSG_FIELD] = Gatekeeper.GK_SUCCESS_STATE_MSG;
							
				// Stage 1 - Async deserialization
				for( int i = 0; i < arrXmlExecRequest.Length; i++ )
				{
					lstTaskIDs.Add( pxy.AddTaskRequest( new TaskRequest( new TaskRequest.TaskCallback( DeserializeExecRequest ), (object) arrXmlExecRequest[i] ) ) );
				}
				
				if( pxy.TasksPending > 0 )
				{
					// Let proxy wait on tasks
					pxy.StartWait();
					// Do stuff here
				
					// Wait on proxy
					pxy.WaitOnProxySignal();
				
					// Get results - deserialized execution requests
					for( int i = 0; i < lstTaskIDs.Count; i++ )
					{
						TPTaskCompleteEventArgs tce = pxy.QueryResult( (Guid) lstTaskIDs[i] );
						if( tce.HasErrors )
							throw new Exception( "Deserialization of execution requests failed error msg: " + tce.ExceptionMessage );
						else lstRequests.Add( tce.Result );
					}

					// Reset proxy
					pxy.Reset();
					// Reset taskIDs list
					lstTaskIDs.Clear();
				}

				// Stage 2 - Security check
				pxy.Reset();
				lstTaskIDs.Clear();

				/*
				// Determine if we must generate a proxy to the SecurityManager first
				// if so
				WSDetails details = ServiceDetails( SECURITY_MANAGER );
				ResolveServiceLocation( ref details );
				
				// Do security check on each request in parallel
				for( int i = 0; i < lstRequests.Count; i++ )
				{
					ExecServiceContext execCtx = new ExecServiceContext();
					execCtx.MethodName = REQUEST_CHECK_METHOD;
					execCtx.Parameters.Add( lstRequests[i] );
					lstTaskIDs.Add( pxy.AddTaskRequest( new TaskRequest( new TaskRequest.TaskCallback( ExecuteServiceDirect ), execCtx ) ) );			
				}
				
				if( pxy.TasksPending > 0 )
				{
					// Let proxy wait on tasks
					pxy.StartWait();
					// Do stuff here

					// Wait on proxy
					pxy.WaitOnProxySignal();
				
				
					// Get results
					for( int i = 0; i < lstTaskIDs.Count; i++ )
					{
						TPTaskCompleteEventArgs tce = pxy.QueryResult( (Guid) lstTaskIDs[i] );
						if( tce.HasErrors )
							throw new Exception( "Security check failed error message: " + tce.ExceptionMessage );
					}

					// Reset proxy
					pxy.Reset();
					// Reset taskIDs list
					lstTaskIDs.Clear();
				}
				*/

				// Stage 3 - Generate any web service proxies we may need
				pxy.Reset();
				lstTaskIDs.Clear();

				// Do proxy generation in parallel once its for a set of
				// distinct web service proxies
				// Determine for which services we need to generate proxy
				// assemblies
				// Go thru the requests, for each service name get the details
				// and determine of to add it to our list of proxies to generate
				for( int i = 0; i < lstRequests.Count; i++ )
				{
					// Get the service details
					WSDetails svcDetails = ServiceDetails( ( (ExecServiceMethodRequestType) lstRequests[i] ).ServiceName );
					// Store the proxies that we must generate
					if( svcDetails.Location.ToLower().StartsWith( "http" ) )
					{
						if( !proxiesToGen.ContainsKey( svcDetails.Name ) )
							proxiesToGen.Add( svcDetails.Name, svcDetails );
					}
				}
				
				IDictionaryEnumerator it = proxiesToGen.GetEnumerator();
				Hashtable wsLocationUpdates = new Hashtable();

				while( it.MoveNext() )
				{
					// Create WSProxyGenContext
					WSProxyGenContext ctx = new WSProxyGenContext();
					WSDetails svcDetails = (WSDetails) it.Value;
					ctx.ProxyCache = ProxyCache;
					ctx.AccessPointUrl = svcDetails.AccessPoint;
					ctx.Name = svcDetails.Name;
					ctx.WsdlUrl = svcDetails.Location;
					Guid taskID = pxy.AddTaskRequest( new TaskRequest( new TaskRequest.TaskCallback( GenerateWSProxy ), ctx ) );
					lstTaskIDs.Add( taskID );
					// Link taskID to the service for which proxy is going to be generated
					wsLocationUpdates.Add( taskID, ctx.Name );
				}
					
				if( pxy.TasksPending > 0 )
				{
					// Let proxy wait on tasks
					pxy.StartWait();
					// Do stuff here

					// Wait on proxy
					pxy.WaitOnProxySignal();
								
					// Get results
					for( int i = 0; i < lstTaskIDs.Count; i++ )
					{
						TPTaskCompleteEventArgs tce = pxy.QueryResult( (Guid) lstTaskIDs[i] );
						if( tce.HasErrors )
							throw new Exception( "Proxy generation failed error message: " + tce.ExceptionMessage );
						
						// Update web service location
						string strAssembly = (string) tce.Result;
						string strServiceName = (string) wsLocationUpdates[tce.TaskID];
						InternalRegistry ireg = new InternalRegistry( this.DatabaseConnectString );
						ireg.UpdateServiceLocation( strServiceName, strAssembly );
					}

					// Reset proxy
					pxy.Reset();
					// Reset taskIDs list
					lstTaskIDs.Clear();
				}//End-if pxy.TasksPending > 0
		
				// Clear list of web service location updates
				wsLocationUpdates.Clear();
				// Stage 4 - Execution 
				pxy.Reset();
				lstTaskIDs.Clear();

				// Do proxy method invocations in parallel
				// Create an execution context for each request
				for( int i = 0; i < lstRequests.Count; i++ )
				{
					// Get the request
					ExecServiceMethodRequestType req = (ExecServiceMethodRequestType) lstRequests[i];
					// Get the service details
					WSDetails svcDetails = ServiceDetails( req.ServiceName );
					svcDetails.Name = svcDetails.Namespace + "." + svcDetails.Name;
					// Create Execute Service context 
					ExecServiceContext ctx = new ExecServiceContext();
					
					ctx.MethodName = req.MethodName;
					ctx.ServiceName = svcDetails.Name;
					// Add array or parameters to deserialize to context parameters
					ctx.Parameters.AddRange( req.m_ParamValue );
					ctx.AccessPointUrl = svcDetails.AccessPoint;
					ctx.Assembly = svcDetails.Location;

					// Give task to TaskProxy
					lstTaskIDs.Add( pxy.AddTaskRequest( new TaskRequest( new TaskRequest.TaskCallback( ExecuteServiceRequest ), ctx ) ) );
				}
				
				if( pxy.TasksPending > 0  )
				{
					// Let proxy wait on tasks
					pxy.StartWait();

					// Do stuff here
				
					// Wait on proxy signal
					pxy.WaitOnProxySignal();
			
					// Consolidate results and return to client
					for( int i = 0; i < lstTaskIDs.Count; i++ )
					{
						TPTaskCompleteEventArgs tce = pxy.QueryResult( (Guid) lstTaskIDs[i] );
						if( tce.HasErrors )
						{
							arrStatus[Gatekeeper.GK_STATUS_FIELD] = enuGatekeeperStatus.Error.ToString();
							arrStatus[Gatekeeper.GK_STATUS_MSG_FIELD] = Gatekeeper.GK_ERROR_STATE_MSG;
							lstResults.Add( tce.ExceptionMessage );
						}
						else lstResults.Add( tce.Result );	
					}
				}

				// Do Async bulk signing of results using SecurityManager

			}
			catch( Exception e )
			{
				arrStatus[Gatekeeper.GK_STATUS_FIELD] = enuGatekeeperStatus.Error.ToString();
				arrStatus[Gatekeeper.GK_STATUS_MSG_FIELD] = Gatekeeper.GK_ERROR_STATE_MSG;
				lstResults.Add( e.Message );
			}
			finally
			{
				// put arrStatus at the start of lstResults
				lstResults.InsertRange( 0, arrStatus );
				lstRequests.Clear();
				lstTaskIDs.Clear();
				proxiesToGen.Clear();
				pxy.Reset();
			}

			return (string[]) lstResults.ToArray( typeof(System.String) );
		}

		/// <summary>
		/// Function uses the SecurityManagerService to sign xml documents.
		/// </summary>
		/// <param name="strXmlDoc">The document to be signed</param>
		/// <returns>The signed document</returns>
		private string SignDocument( string strXmlDoc )
		{
			string strRetVal = "";
			try
			{
				if( this.TraceOn )
					TraceInfo( this.Name, "Signing document" );
				
				string[] arrSecurityManagerResponse = null;
				
				// Interact with SecurityManagerService via reflection
				WSDetails details = ServiceDetails( SECURITY_MANAGER );
				ResolveServiceLocation( ref details );

				if( details.Location.Length == 0 )
					throw new System.Exception( "Cannot Find Security Manager" );
				
				// Pass document to be signed to SecurityManagerService
				object[] objParams = new Object[1];
				objParams[0] = strXmlDoc;

				ExecServiceContext ctx = new ExecServiceContext();
				ctx.AccessPointUrl = details.AccessPoint;
				ctx.Assembly = details.Location;
				ctx.ServiceName = details.Name;
				ctx.MethodName = SIGN_DOCUMENT_METHOD;
				ctx.Parameters.AddRange( objParams );

				object objExecResult = ExecuteServiceDirect( ctx );
				
				if( objExecResult != null )
				{
					// SecurityManagerService returns a string array
					arrSecurityManagerResponse = objExecResult as string[];
						strRetVal = arrSecurityManagerResponse[SECURITY_MANAGER_RETURNED_SIGNATURE_INDEX];
					if( strRetVal.Length > 0 && this.TraceOn )
					{
						LogInfo( this.Name, "Signed doc" );
					}
				}
			}
			catch( System.Exception e )
			{
				LogError( this.Name, e.Message );		
			}
			return strRetVal;
		}//End SignDocument
		
		/// <summary>
		/// Function uses the SecurityManagerService to verify the 
		/// signature on a signed document.
		/// </summary>
		/// <param name="strXmlDoc">The signed document to be verify</param>
		/// <returns>Verified document</returns>
		private string VerifyDocument( string strXmlDoc )
		{
			string strRetVal = "";
			try
			{
				if( this.TraceOn )
					TraceInfo( this.Name, "Verifying Document " );
				
				string[] arrSecurityManagerResponse = null;
				// Interact with SecurityManagerService via reflection
				WSDetails details = ServiceDetails( SECURITY_MANAGER );
				ResolveServiceLocation( ref details );

				if( details.Location.Length == 0 )
					throw new System.Exception( "Cannot Find Security Manager" );
				
				// Pass document to verify to SecurityManagerService
				object[] objParams = new object[1];
				objParams[0] = strXmlDoc;
				
				ExecServiceContext ctx = new ExecServiceContext();
				ctx.AccessPointUrl = details.AccessPoint;
				ctx.Assembly = details.Location;
				ctx.ServiceName = details.Name;
				ctx.MethodName = VERIFY_DOCUMENT_METHOD;
				ctx.Parameters.AddRange( objParams );

				object objExecResult = ExecuteServiceDirect( ctx );
						
				if( objExecResult != null )
				{
					// SecurityManagerService returns a string array
					arrSecurityManagerResponse = objExecResult as string[];
					strRetVal = arrSecurityManagerResponse[SECURITY_MANAGER_RETURNED_VERIFICATION_INDEX];
					
					if( strRetVal.Length > 0 && this.TraceOn )
					{
						LogInfo( this.Name, "Verified doc" );
					}
				}
			}
			catch( System.Exception e )
			{
				LogError( this.Name, e.Message );		
			}
			return strRetVal;
		}//End-VerifyDocument

		/*****************************************************************/
		
		
		
		// New Methods to lead to using Async operations in GK
		// No exception catching logic here
		// if this method is queued to the threadpool, the threadpool will catch
		// any exceptions thrown and report failure to higher level clients
		// if this method is not queued then caller responsible to catch
		// (and optionally handle) any exceptions thrown
				
		/// <summary>
		/// Function invokes a web service directly - no parameter serialization
		/// or deserialization - parameters passed directly as objects, result
		/// returned as object
		/// </summary>
		/// <param name="objCtx">ExecutionServiceContext instance</param>
		/// <returns>Result of execution</returns>
		private object ExecuteServiceDirect( object objCtx )
		{
			object objInvokeResult = null;
			ExecServiceContext ctx = objCtx as ExecServiceContext;
			if( ctx == null )
				throw new Exception( "Invalid context passed" );
			// Verify context object valid
			
			// Proxy method invoke logic goes here
			string strError = "";
			// Load Assembly containing web service proxy
			Assembly a = Assembly.LoadFrom( ctx.Assembly );
			// Get the correct type (ProxyClass)
			Type ProxyType = a.GetType( ctx.ServiceName );
			// Create an instance of the Proxy Class
			Object objProxy = a.CreateInstance( ctx.ServiceName );
			if( objProxy == null || ProxyType == null )
			{
				strError = "Cannot create type/proxy instance ";
				strError += ctx.ServiceName;
				strError += " in assembly ";
				strError += ctx.Assembly;
				throw new Exception( strError );
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
					Url.SetValue( objProxy, ctx.AccessPointUrl, null );
			}

			// Parameters passed as objects directly
			objInvokeResult = ProxyType.InvokeMember( ctx.MethodName,
				BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
				null,
				objProxy,
				ctx.Parameters.ToArray() );
 
			return objInvokeResult;
		}

		/// <summary>
		/// Function invoke a web service from an ExecServiceRequest
		/// Parameters passed must be deserialzed and returned values must
		/// be serialized to (XML) strings
		/// </summary>
		/// <param name="objCtx">ExecutionServiceContext instance</param>
		/// <returns>The result of execution</returns>
		private object ExecuteServiceRequest( object objCtx )
		{
			object objInvokeResult = null;
			string strRetVal = "";

			ExecServiceContext ctx = objCtx as ExecServiceContext;
			if( ctx == null )
				throw new Exception( "Invalid context passed" );
			// Verify context object valid
		
			// Proxy method invoke logic goes here
			string strError = "";
			// Load Assembly containing web service proxy
			bool bProxyMethodHasParameters = false;
			Assembly a = Assembly.LoadFrom( ctx.Assembly );
			// Get the correct type (ProxyClass)
			Type ProxyType = a.GetType( ctx.ServiceName );
			// Create an instance of the Proxy Class
			Object objProxy = a.CreateInstance( ctx.ServiceName );
			if( objProxy == null || ProxyType == null )
			{
				strError = "Cannot create type/proxy instance ";
				strError += ctx.ServiceName;
				strError += " in assembly ";
				strError += ctx.Assembly;
				throw new Exception( strError );
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
					Url.SetValue( objProxy, ctx.AccessPointUrl, null );
			}
				
			// Once we have a Proxy Object and a Type instance
			// use reflection to get info on method to be
			// executed.
			MethodInfo mInfo = ProxyType.GetMethod( ctx.MethodName );
			if( mInfo == null )
			{
				strError = "Cannot find method ";
				strError += ctx.MethodName;
				strError += " of Proxy ";
				strError += ctx.ServiceName;
				strError += " loaded from assembly ";
				strError += ctx.Assembly;
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
				if( ctx.Parameters.Count != arrParamInfo.Length )
					throw new Exception( "Wrong Number of Arguments Passed to Proxy " + ctx.ServiceName + " Method " + ctx.MethodName );
					
				// Create array to hold parameters
				param = new Object[arrParamInfo.Length];

				// Try deserialization
				for( int i = 0; i < ctx.Parameters.Count; i++ )
				{
					// Get the expected type
					Type paramType = arrParamInfo[i].ParameterType;
					// Create XmlSerializer
					XmlSerializer xs = new XmlSerializer( paramType );
					// Read in Xml doc representing parameter
					System.Xml.XmlReader xt = new XmlTextReader( (string) ctx.Parameters[i], XmlNodeType.Document, null );
					xt.Read(); 
					// Deserialize
					Object paramInst = xs.Deserialize( xt );
					// Store in parameter array
					param[i] = (Object) paramInst;
				}
			}// End if bProxyMethodHasParameters
			
			if( bProxyMethodHasParameters )
				objInvokeResult = ProxyType.InvokeMember( ctx.MethodName,
					BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
					null,
					objProxy,
					param );
 
			else objInvokeResult = ProxyType.InvokeMember( ctx.MethodName,
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
				// Close reader
				reader.Close();
				// Close stream
				ms.Close();
			}
			return strRetVal;	
		}//End-InvokeService

		/// <summary>
		/// Function generates a proxy assembly to a web service
		/// </summary>
		/// <param name="objCtx">WSProxyGenTaskContext instance</param>
		/// <returns>A string representing the location of the assembly generated</returns>
		/// <remarks>Care must be taken that this method is not called in parallel
		/// for the same context data e.g. two simultaneous generations of
		/// a proxy to service x will result in file locking and IO exceptions (at best) 
		/// </remarks>
		private object GenerateWSProxy( object objCtx )
		{
			string strRetVal = "";
			WSProxyGenContext ctx = objCtx as WSProxyGenContext;
			if( ctx == null )
				throw new Exception( "Invalid context passed" );
						
			DynamicRequest req = new DynamicRequest();
			req.FilenameSource = ctx.Name;
			req.ServiceName = ctx.Name;
			req.ProxyPath = ctx.ProxyCache;
			req.WsdlFile = ctx.WsdlUrl;
						
			// Create a proxy generator
			ProxyGen pxyGen = new ProxyGen();
			// Set mutator
			strRetVal = pxyGen.GenerateAssembly( req );

			return strRetVal;
		}//End-GenerateWSProxy
		
		/// <summary>
		/// Function returns the details (location, accesspoint url etc.) of the
		/// web service.
		/// <seealso cref="WSDetails"/>
		/// </summary>
		/// <param name="strName">Name of web service</param>
		/// <returns>WSDetails instance</returns>
		private WSDetails ServiceDetails( string strName )
		{
			InternalRegistry ireg = new InternalRegistry( this.DatabaseConnectString );
			WSDetails details = new WSDetails();

			details.Location = ireg.GetServiceLocation( strName );
			details.Namespace = ireg.GetServiceNamespace( strName );
			details.AccessPoint = ireg.GetServiceAccessPoint( strName );
			details.Name = strName;
			
			return details;
		}

		/// <summary>
		/// Function returns the details (location, accesspoint url etc.) of the
		/// named Gatekeeper.
		/// <seealso cref="WSDetails"/>
		/// </summary>
		/// <param name="strName">Name of Gatekeeper service</param>
		/// <returns>WSDetails instance</returns>
		private WSDetails GatekeeperDetails( string strName )
		{
			InternalRegistry ireg = new InternalRegistry( this.DatabaseConnectString );
			WSDetails details = new WSDetails();
			
			details.Location = ireg.GetGateKeeperLocation( strName );
			details.Namespace = ireg.GetGateKeeperNamespace( strName );
			details.AccessPoint = ireg.GetGateKeeperAccessPoint( strName );
			details.Name = strName;

			return details;
		}

		/// <summary>
		/// Procedure resolves the location of a service. If the location is
		/// a url pointing to a wsdl file, then an assembly containing a proxy
		/// class to the web service is automatically generated
		/// </summary>
		/// <param name="details">WSDetails instance of the service</param>
		private void ResolveServiceLocation( ref WSDetails details )
		{
			if( details.Location.Length == 0 )
				throw new Exception( "Invalid details data passed for service " + details.Name );

			if( details.Location.ToLower().StartsWith( "http" ) )
			{
				if( this.TraceOn )
					TraceInfo( this.Name, "Generating Proxy to " + details.Name + " located at " + details.Location + " AccessPoint: " + details.AccessPoint );		
			
				// Set context for proxy generation
				WSProxyGenContext ctx = new WSProxyGenContext();
				ctx.Name = details.Name;
				ctx.AccessPointUrl = details.AccessPoint;
				ctx.ProxyCache = ProxyCache;
				ctx.WsdlUrl = details.Location;
				// Gen assembly and update location in details
				details.Location = (string) GenerateWSProxy( ctx );
						
				// If no assembly generated, report error and exit
				if( details.Location.Length == 0 )
					throw new Exception( "Error generating proxy to " + details.Name + " using WSDL ref: " + details.Location );
								
				// Do not update location of security manager
				if( details.Name.CompareTo( SECURITY_MANAGER ) != 0 )
				{
					InternalRegistry ireg = new InternalRegistry( this.DatabaseConnectString );
					// Update database location of service, point to
					// dynamic proxy, change namespace the dynamic proxy namespace
					ireg.UpdateServiceLocation( details.Name, details.Location );
					ireg.UpdateServiceNamespace( details.Name, DynamicRequest.DEFAULT_PROXY_NAMESPACE );
				}
				details.Name = DynamicRequest.DEFAULT_PROXY_NAMESPACE + "." + details.Name;
			}
			else
			{
				// Fully qualify service name with namespace
				// necesary for reflection
				if( details.Namespace.Length > 0 )
					details.Name = details.Namespace + "." + details.Name;
			}
		}//End-ResolveServiceLocation
		
		/// <summary>
		/// Procedure resolves the location of a Gatekeeper. If the location is
		/// a url pointing to a wsdl file, then an assembly containing a proxy
		/// class to the Gatekeeper is automatically generated
		/// </summary>
		/// <param name="details">WSDetails instance of the service</param>
		private void ResolveGatekeeperLocation( ref WSDetails details )
		{
			if( details.Location.Length == 0 )
				throw new Exception( "Invalid details data passed for service " + details.Name );

			if( details.Location.ToLower().StartsWith( "http" ) )
			{
				if( this.TraceOn )
					TraceInfo( this.Name, "Generating Proxy to " + details.Name + " located at " + details.Location + " AccessPoint: " + details.AccessPoint );		
			
				// Set context for proxy generation
				WSProxyGenContext ctx = new WSProxyGenContext();
				ctx.Name = details.Name;
				ctx.AccessPointUrl = details.AccessPoint;
				ctx.ProxyCache = ProxyCache;
				ctx.WsdlUrl = details.Location;
				// Gen assembly and update location in details
				details.Location = (string) GenerateWSProxy( ctx );
						
				// If no assembly generated, report error and exit
				if( details.Location.Length == 0 )
					throw new Exception( "Error generating proxy to " + details.Name + " using WSDL ref: " + details.Location );
								
				// Do not update location of security manager
				if( details.Name.CompareTo( SECURITY_MANAGER ) != 0 )
				{
					InternalRegistry ireg = new InternalRegistry( this.DatabaseConnectString );
					// Update database location of service, point to
					// dynamic proxy, change namespace the dynamic proxy namespace
					ireg.UpdateGateKeeperLocation( details.Name, details.Location );
					ireg.UpdateGateKeeperNamespace( details.Name, DynamicRequest.DEFAULT_PROXY_NAMESPACE );
				}
				details.Name = DynamicRequest.DEFAULT_PROXY_NAMESPACE + "." + details.Name;
			}
			else
			{
				// Fully qualify service name with namespace
				// necesary for reflection
				if( details.Namespace.Length > 0 )
					details.Name = details.Namespace + "." + details.Name;
			}
		}//End-ResolveGatekeeperLocation

		/// <summary>
		/// Function derserializes an Execution request from XML to an object
		/// instance.
		/// </summary>
		/// <param name="obj">The XML string to deserialize</param>
		/// <returns>The deserialzed ExecServiceMethodRequest instance</returns>
		private object DeserializeExecRequest( object obj )
		{
			ExecServiceMethodRequestType execReq = new ExecServiceMethodRequestType();
			XmlSerializer ser = new XmlSerializer( execReq.GetType() );
			XmlReader xt = new XmlTextReader( (string) obj, XmlNodeType.Document, null );
			xt.Read();
			object objInst = ser.Deserialize( xt );
			return objInst;		
		}
	}// End GateKeeper
}
