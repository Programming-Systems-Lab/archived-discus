using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PSL.DISCUS.DAML
{
	public enum enuProcessType
	{
		AtomicProcess,
		CompositeProcess,
		SimpleProcess
	};
	
	/// <summary>
	/// Summary description for DAMLProcessModel.
	/// </summary>
	public class DAMLProcessModel:DAMLContainer
	{
		public const int DEFAULT_RESTRICTION = 1;

		public DAMLProcessModel()
		{
			// Init inherited members
			m_EvtLog = new EventLog( "Application" );
			m_EvtLog.Source = "DAMLProcessModel";
			m_doc = new XmlDocument();
			m_mgr = null;
		}

			
		public string[] AtomicProcesses
		{
			get
			{ return GetProcesses( enuProcessType.AtomicProcess ); }
		}

		public string[] CompositeProcesses
		{
			get
			{ return GetProcesses( enuProcessType.CompositeProcess ); }
		}
		
		public string[] SimpleProcesses
		{
			get
			{ return GetProcesses( enuProcessType.SimpleProcess ); }
		}

		public string[] AllProcesses
		{
			get
			{
				ArrayList arrProcesses = new ArrayList();
				
				string[] arrAtomic = AtomicProcesses;
				string[] arrComposite = CompositeProcesses;
				string[] arrSimple = SimpleProcesses;
				
				// Add all to our collection
				arrProcesses.AddRange( arrAtomic );
				arrProcesses.AddRange( arrComposite );
				arrProcesses.AddRange( arrSimple );

				return (string[]) arrProcesses.ToArray( typeof( System.String ) );
			}
		}

		/* Function retrieves all the interesting data about a process given its name and 
		 * type.
		 * 
		 * Interesting data:
		 * Inputs, Outputs, Preconditions, Effects, Parameters, ConditionalOutputs,
		 * Co-Conditions, Sub Processes (if process is a composite process)
		 * 
		 * Inputs: strProcessName - named process
		 *		   processType - process type (atomic, simple, composite)
		 * 
		 * Return value: DAMLProcess containing all the relevant data
		 */ 
		public DAMLProcess GetProcessData( string strProcessName, enuProcessType processType )
		{
			DAMLProcess retVal = new DAMLProcess();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				string strBaseUri = GetNamespaceBaseUri( DAMLConstants.PROCESS_NS );
				string strUri = "";
				strUri = strBaseUri;
				
				switch( processType )
				{
					case enuProcessType.AtomicProcess: strUri += DAMLConstants.DAML_ATOMIC_PROCESS;
						break;
					case enuProcessType.CompositeProcess: strUri += DAMLConstants.DAML_COMPOSITE_PROCESS;
						break;
					case enuProcessType.SimpleProcess: strUri += DAMLConstants.DAML_SIMPLE_PROCESS;
						break;
					default:  throw new ArgumentException( "Invalid processType value" );
				};

				string strXPath = DAMLConstants.DAML_CLASS + "[@" + DAMLConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DAMLConstants.RDFS_SUBCLASSOF + "[@" + DAMLConstants.RDF_RESOURCE + "='" + strUri + "']";
				
				XmlNode processNode = root.SelectSingleNode( strXPath, m_mgr ).ParentNode;

				// No such process exists so just exit - should throw exception since
				// returned DAMLProcess is useless?
				if( processNode == null )
					return retVal;
				
				// Set process name
				retVal.Name = processNode.Attributes[DAMLConstants.RDF_ID].Value;
				// Set process type
				retVal.ProcessType = processType;
				
				// Get inputs from querying RDFProperty nodes in document
				strXPath = DAMLConstants.RDF_PROPERTY + "/" + DAMLConstants.RDFS_SUBPROPERTYOF + "[@" +  DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.INPUT + "']" + "/" + "following-sibling::" + DAMLConstants.RDFS_DOMAIN + "[@" + DAMLConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
				XmlNodeList lstNodes = root.SelectNodes( strXPath, m_mgr );
				
				foreach( XmlNode node in lstNodes )
				{
					RDFProperty data = GetNodeData( node );
					retVal.AddInput( data );
				}
				
				// Get additional inputs from the process node itself
				// they may be hidden under restictions tagged with
				// daml:sameValueAs
				strXPath =  DAMLConstants.DAML_CLASS + "[@" + DAMLConstants.RDF_ID + "='" + retVal.Name + "']" + "/" + DAMLConstants.RDFS_SUBCLASSOF + "/" + DAMLConstants.DAML_RESTRICTION + "/" + DAMLConstants.DAML_ON_PROPERTY + "[@" + DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.INPUT + "']" + "/" + "following-sibling::" + DAMLConstants.DAML_SAMEVALUESAS;
				lstNodes = root.SelectNodes( strXPath, m_mgr );

				foreach( XmlNode node in lstNodes )
				{
					string strSameValueAs = node.Attributes[DAMLConstants.RDF_RESOURCE].Value;
					strSameValueAs = strSameValueAs.Trim( new char[] {'#'} );
										
					// Go get RDFProperty data
					strXPath = DAMLConstants.RDF_PROPERTY + "[@" + DAMLConstants.RDF_ID + "='" + strSameValueAs + "']" + "/" + DAMLConstants.RDFS_DOMAIN;
					XmlNode domainNode = root.SelectSingleNode( strXPath, m_mgr );

					// Add to list of inputs
					if( domainNode != null )
					{
						RDFProperty data = GetNodeData( domainNode );
						retVal.AddInput( data );
					}
				}
								
				// Get outputs
				strXPath = DAMLConstants.RDF_PROPERTY + "/" + DAMLConstants.RDFS_SUBPROPERTYOF + "[@" +  DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.OUTPUT + "']" + "/" + "following-sibling::" + DAMLConstants.RDFS_DOMAIN + "[@" + DAMLConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
				lstNodes = root.SelectNodes( strXPath, m_mgr );

				foreach( XmlNode node in lstNodes )
				{
					RDFProperty data = GetNodeData( node );
					retVal.AddOutput( data );
				}

				// Get preconditions
				strXPath = DAMLConstants.RDF_PROPERTY + "/" + DAMLConstants.RDFS_SUBPROPERTYOF + "[@" +  DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.PRECONDITION + "']" + "/" + "following-sibling::" + DAMLConstants.RDFS_DOMAIN + "[@" + DAMLConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
				lstNodes = root.SelectNodes( strXPath, m_mgr );

				foreach( XmlNode node in lstNodes )
				{
					RDFProperty data = GetNodeData( node );
					retVal.AddPrecondition( data );
				}

				// Get effects
				strXPath = DAMLConstants.RDF_PROPERTY + "/" + DAMLConstants.RDFS_SUBPROPERTYOF + "[@" +  DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.EFFECT + "']" + "/" + "following-sibling::" + DAMLConstants.RDFS_DOMAIN + "[@" + DAMLConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
				lstNodes = root.SelectNodes( strXPath, m_mgr );

				foreach( XmlNode node in lstNodes )
				{
					RDFProperty data = GetNodeData( node );
					retVal.AddEffect( data );
				}
				
				// Get conditional outputs
				strXPath = DAMLConstants.RDF_PROPERTY + "/" + DAMLConstants.RDFS_SUBPROPERTYOF + "[@" +  DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.CONDITIONAL_OUTPUT + "']" + "/" + "following-sibling::" + DAMLConstants.RDFS_DOMAIN + "[@" + DAMLConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
				lstNodes = root.SelectNodes( strXPath, m_mgr );

				foreach( XmlNode node in lstNodes )
				{
					RDFProperty data = GetNodeData( node );
					retVal.AddConditionalOutput( data );
				}

				// Get co-conditions
				strXPath = DAMLConstants.RDF_PROPERTY + "/" + DAMLConstants.RDFS_SUBPROPERTYOF + "[@" +  DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.CO_CONDITION + "']" + "/" + "following-sibling::" + DAMLConstants.RDFS_DOMAIN + "[@" + DAMLConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
				lstNodes = root.SelectNodes( strXPath, m_mgr );

				foreach( XmlNode node in lstNodes )
				{
					RDFProperty data = GetNodeData( node );
					retVal.AddCoCondition( data );
				}

				// Get co-outputs
				strXPath = DAMLConstants.RDF_PROPERTY + "/" + DAMLConstants.RDFS_SUBPROPERTYOF + "[@" +  DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.CO_OUTPUT + "']" + "/" + "following-sibling::" + DAMLConstants.RDFS_DOMAIN + "[@" + DAMLConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
				lstNodes = root.SelectNodes( strXPath, m_mgr );

				foreach( XmlNode node in lstNodes )
				{
					RDFProperty data = GetNodeData( node );
					retVal.AddCoOutput( data );
				}

				// Get parameters
				strXPath = DAMLConstants.RDF_PROPERTY + "/" + DAMLConstants.RDFS_SUBPROPERTYOF + "[@" +  DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.PARAMETER + "']" + "/" + "following-sibling::" + DAMLConstants.RDFS_DOMAIN + "[@" + DAMLConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
				lstNodes = root.SelectNodes( strXPath, m_mgr );

				foreach( XmlNode node in lstNodes )
				{
					RDFProperty data = GetNodeData( node );
					retVal.AddParameter( data );
				}

				// For each input, fill the process' InputRestrictionMap
				// search on process name
				if( retVal.HasInputs )
				{
					foreach( RDFProperty Input in retVal.Inputs )
					{
						int nRestriction = GetInputRestrictions( retVal.Name, Input.Name );
						if( nRestriction == 0 )
							nRestriction = DEFAULT_RESTRICTION;
						retVal.AddInputRestriction( Input.Name, nRestriction );
					}
				}

				// If we are dealing with a complex process we must go get
				// the substeps - need to get process:<type> data
				if( processType == enuProcessType.CompositeProcess )
				{
					retVal.SubTaskType = GetProcessSubTaskType( retVal.Name );
					retVal.AddSubProcess( GetSubProcesses( retVal.Name ) );
				}
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			return retVal;
		}
		
        /* Function extracts the RDFProperty data from an XmlNode. Function expects
		 * specific RDFProperty information available.
		 * 
		 * Inputs: node - the XmlNode to extract RDFProperty data
		 * 
		 * Return values: the RDFProperty instance containing the node data
		 */ 
		private RDFProperty GetNodeData( XmlNode node )
		{
			// Go up to parent node
			XmlNode propertyNode = node.ParentNode;
			RDFProperty data = new RDFProperty();

			// Set name
			data.Name = propertyNode.Attributes[DAMLConstants.RDF_ID].Value;
			// Set Domain
			data.Domain = propertyNode.SelectSingleNode( DAMLConstants.RDFS_DOMAIN, m_mgr ).Attributes[DAMLConstants.RDF_RESOURCE].Value;
			// Set Range
			data.Range = propertyNode.SelectSingleNode( DAMLConstants.RDFS_RANGE, m_mgr ).Attributes[DAMLConstants.RDF_RESOURCE].Value;
			// Set SubPropertyOf
			data.SubPropertyOf = propertyNode.SelectSingleNode( DAMLConstants.RDFS_SUBPROPERTYOF, m_mgr ).Attributes[DAMLConstants.RDF_RESOURCE].Value;
			
			// Fill in sameValueAs data (if any)
			XmlNode sameValueAsNode = propertyNode.SelectSingleNode( DAMLConstants.DAML_SAMEVALUESAS, m_mgr );
			if( sameValueAsNode != null )
				data.SameValueAs = sameValueAsNode.Attributes[DAMLConstants.RDF_RESOURCE].Value;

			return data;
		}
		
		/* Function retrieves the input restriction data given a process name and 
		 * a named input.
		 * 
		 * Inputs: strProcessName - named process we are interested in
		 *		   strInput - named input of the process
		 */
		private int GetInputRestrictions( string strProcessName, string strInput )
		{
			XmlNode root = m_doc.DocumentElement;

			string strXPath = DAMLConstants.DAML_CLASS + "[@" + DAMLConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DAMLConstants.RDFS_SUBCLASSOF + "/" + DAMLConstants.DAML_RESTRICTION + "/" + DAMLConstants.DAML_ON_PROPERTY + "[@" + DAMLConstants.RDF_RESOURCE + "='#" + strInput + "']";

			XmlNode node = root.SelectSingleNode( strXPath, m_mgr );

			if( node == null )
				return 0;

			XmlNode restrictionNode = node.ParentNode;
			XmlNode cardinalityNode = restrictionNode.Attributes[DAMLConstants.DAML_CARDINALITY];
			if( cardinalityNode == null )
				return 0;
			else return Int32.Parse( cardinalityNode.Value );
		}
		
		/* Private helper function used to extract URIs from our namespace
		 * manager given a namespace prefix - even though the namespace manager is 
		 * *supposed* to be accessible like a hashtable (m_mgr[<prefixname>]) this
		 * does not work, a for each construct is needed to iterate through all
		 * entries in the namespace manager.
		 * 
		 * Inputs: strNamespacePrefix - the namespace prefix we want to find the 
		 *								base Uri of
		 * 
		 * Return values: the Basr Uri of the namespace prefix
		 */ 
		private string GetNamespaceBaseUri( string strNamespacePrefix )
		{
			string strBaseUri = "";

			foreach( string prefix in m_mgr )
			{
				if( prefix == strNamespacePrefix )
				{
					strBaseUri = m_mgr.LookupNamespace( prefix );
					break;
				}
			}

			return strBaseUri;
		}

		/* Function returns the sub task type of a named process
		 * 
		 * Inputs: strProcessName - named process
		 * 
		 * Return values: the type of the named process' subtasks 
		 *				  sequence, choice etc.
		 */
		private enuProcessSubTaskType GetProcessSubTaskType( string strProcessName )
		{
			XmlNode root = m_doc.DocumentElement;
			string strBaseUri = GetNamespaceBaseUri( DAMLConstants.PROCESS_NS );
			
			string strXPath = DAMLConstants.DAML_CLASS + "[@" + DAMLConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DAMLConstants.RDFS_SUBCLASSOF + "[@" + DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.DAML_COMPOSITE_PROCESS + "']" + "/" + "following-sibling::" + DAMLConstants.RDFS_SUBCLASSOF;
			XmlNode SubClassOfNode = root.SelectSingleNode( strXPath, m_mgr );
				
			if( SubClassOfNode == null )
				throw new Exception( "Complex process " + strProcessName + " data not found" );

			// Get process:<type>
			strXPath = ".//" + DAMLConstants.DAML_CLASS + "[@" + DAMLConstants.RDF_ABOUT + "]";
			XmlNode dataNode = SubClassOfNode.SelectSingleNode( strXPath, m_mgr );

			if( dataNode == null )
				throw new Exception( "No process:<type> data provided for complex process " + strProcessName + " document is invalid" );

			if( dataNode.Attributes[DAMLConstants.RDF_ABOUT].Value == DAMLConstants.PROCESS_CHOICE )
				return enuProcessSubTaskType.Choice;
			else return enuProcessSubTaskType.Sequence;
		}
		
		/* Function returns all the sub processes of a named process
		 * 
		 * Inputs: strProcessName - named process
		 * 
		 * Return values: an array of its sub processes
		 */
		private DAMLProcess[] GetSubProcesses( string strProcessName )
		{
			ArrayList lstSubProcesses = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				string strBaseUri = GetNamespaceBaseUri( DAMLConstants.PROCESS_NS );
				
				string strXPath = DAMLConstants.DAML_CLASS + "[@" + DAMLConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DAMLConstants.RDFS_SUBCLASSOF + "[@" + DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.DAML_COMPOSITE_PROCESS + "']" + "/" + "following-sibling::" + DAMLConstants.RDFS_SUBCLASSOF;
				XmlNode SubClassOfNode = root.SelectSingleNode( strXPath, m_mgr );
				
				if( SubClassOfNode == null )
					return (DAMLProcess[]) lstSubProcesses.ToArray( typeof(DAMLProcess) );

				// Use fuzzy paths from here -> "//" operator looking for any matching
				// child node - more expensive but intermediate nodes are not 
				// interesting/contain no info we can use

				strXPath = ".//" + DAMLConstants.DAML_LIST_OF_INSTANCES_OF + "/" + DAMLConstants.DAML_CLASS;
				XmlNodeList lstInstances = SubClassOfNode.SelectNodes( strXPath, m_mgr );
				
				foreach( XmlNode node in lstInstances )
				{
					string strProcess = node.Attributes[DAMLConstants.RDF_ABOUT].Value;
					strProcess = strProcess.Trim( new Char[] { '#' } );
					enuProcessType processType = GetProcessType( strProcess );
					DAMLProcess process = GetProcessData( strProcess, processType );
					lstSubProcesses.Add( process );
				}
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			return (DAMLProcess[]) lstSubProcesses.ToArray( typeof(DAMLProcess) );
		}

		/* Function returns the process type of a named process
		 * 
		 * Inputs: strProcessName - named process
		 * 
		 * Return values: the type of the named process (atomic, simple, composite)
		 */
		private enuProcessType GetProcessType( string strProcessName )
		{
			// process may be atomic, simple or complex
			string strBaseUri = GetNamespaceBaseUri( DAMLConstants.PROCESS_NS );
			XmlNode root = m_doc.DocumentElement;

			string strXPathAtomicProcess = DAMLConstants.DAML_CLASS + "[@" + DAMLConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DAMLConstants.RDFS_SUBCLASSOF + "[@" + DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.DAML_ATOMIC_PROCESS + "']";
			string strXPathSimpleProcess = DAMLConstants.DAML_CLASS + "[@" + DAMLConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DAMLConstants.RDFS_SUBCLASSOF + "[@" + DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.DAML_SIMPLE_PROCESS + "']";
			string strXPathCompositeProcess = DAMLConstants.DAML_CLASS + "[@" + DAMLConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DAMLConstants.RDFS_SUBCLASSOF + "[@" + DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.DAML_COMPOSITE_PROCESS + "']";

			XmlNode resultNode = root.SelectSingleNode( strXPathAtomicProcess, m_mgr );
			if( resultNode != null )
				return enuProcessType.AtomicProcess;

			resultNode = root.SelectSingleNode( strXPathSimpleProcess, m_mgr );
			if( resultNode != null )
				return enuProcessType.SimpleProcess;

			resultNode = root.SelectSingleNode( strXPathCompositeProcess, m_mgr );
			if( resultNode != null )
				return enuProcessType.CompositeProcess;
			
			throw new Exception( "Process " + strProcessName + " does not exist" );
		}

		/* Function returns all the names of processes of a given type.
		 * 
		 * Inputs: processType - types of processes to retrieve
		 * 
		 * Return values: an array of process names of a given type (atomic, simple, 
		 *				  composite)
		 */
		private string[] GetProcesses( enuProcessType processType )
		{
			ArrayList arrProcess = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				string strProcessURI = GetNamespaceBaseUri( DAMLConstants.PROCESS_NS );
				string strProcessType = "";

				switch( processType )
				{
					case enuProcessType.AtomicProcess: strProcessType = DAMLConstants.DAML_ATOMIC_PROCESS;
													   break;

					case enuProcessType.CompositeProcess: strProcessType = DAMLConstants.DAML_COMPOSITE_PROCESS;
														break;

					case enuProcessType.SimpleProcess: strProcessType = DAMLConstants.DAML_SIMPLE_PROCESS;
													   break;
				};
				
				string strXPath = DAMLConstants.DAML_CLASS + "/" + DAMLConstants.RDFS_SUBCLASSOF + "[@" + DAMLConstants.RDF_RESOURCE + "='" + strProcessURI + strProcessType + "']";

				XmlNodeList lstProcess = root.SelectNodes( strXPath, m_mgr );
			
				foreach( XmlNode processNode in lstProcess )
				{
					// Move up to parent
					XmlNode parentNode = processNode.ParentNode;

					string strValue = parentNode.Attributes[DAMLConstants.RDF_ID].Value;
					
					if( strValue.Length > 0 )
						arrProcess.Add( strValue );
					
				}
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			
			return (string[]) arrProcess.ToArray( typeof( System.String ) );
		}
	}
}
