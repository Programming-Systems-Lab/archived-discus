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
		public DAMLProcessModel()
		{
			// Init inherited members
			m_EvtLog = new EventLog( "Application" );
			m_EvtLog.Source = "DAMLProcessModel";
			m_doc = new XmlDocument();
			m_mgr = null;
		}

			
		public DAMLProcess GetProcessData( string strProcessName, enuProcessType processType )
		{
			DAMLProcess retVal = new DAMLProcess();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				string strBaseUri = "";
				string strUri = "";

				foreach( string prefix in m_mgr )
				{
					if( prefix == DAMLConstants.PROCESS_NS )
					{
						strBaseUri = m_mgr.LookupNamespace( prefix );
						break;
					}
				}

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

				// No such process exists so just exit
				if( processNode == null )
					return retVal;
				
				// Set process name
				retVal.Name = processNode.Attributes[DAMLConstants.RDF_ID].Value;
				// Set process type
				retVal.ProcessType = processType;
				
				// Get inputs
				strXPath = DAMLConstants.RDF_PROPERTY + "/" + DAMLConstants.RDFS_SUBPROPERTYOF + "[@" +  DAMLConstants.RDF_RESOURCE + "='" + strBaseUri + DAMLConstants.INPUT + "']" + "/" + "following-sibling::" + DAMLConstants.RDFS_DOMAIN + "[@" + DAMLConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
				XmlNodeList lstNodes = root.SelectNodes( strXPath, m_mgr );
				
				foreach( XmlNode node in lstNodes )
				{
					RDFProperty data = GetNodeData( node );
					retVal.AddInput( data );
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
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			return retVal;
		}
		
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

		public String[] AtomicProcesses
		{
			get
			{ return GetProcesses( enuProcessType.AtomicProcess ); }
		}

		public String[] CompositeProcesses
		{
			get
			{ return GetProcesses( enuProcessType.CompositeProcess ); }
		}
		
		public String[] SimpleProcesses
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
			
		private string[] GetProcesses( enuProcessType processType )
		{
			ArrayList arrProcess = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				string strProcessURI = "";

				foreach( string prefix in m_mgr )
				{
					if( prefix == DAMLConstants.PROCESS_NS )
					{
						strProcessURI = m_mgr.LookupNamespace( prefix );
						break;
					}
				}
				
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
