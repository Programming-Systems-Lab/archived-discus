using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PSL.DAML
{
	/// <summary>
	/// Summary description for DAMLProcessModel.
	/// </summary>
	public sealed class DamlProcessModelReader:DamlContainer
	{
		public DamlProcessModelReader( string strDaml ):base( strDaml )
		{
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

		private DamlTypeDefinition[] FindTypeDefinitions( RdfProperty data )
		{
			ArrayList lstDefinitions = new ArrayList();

			if( !data.Range.ToLower().StartsWith( "http://" ) )
			{
				DamlTypeDefinition defnRdf = this.FindTypeDefintionInRdfProperty( data.Range.TrimStart( new char[] { '#' } ) );
				if( defnRdf != null )
					lstDefinitions.Add( defnRdf );

				DamlTypeDefinition defnClass = this.FindTypeDefinitionInDamlClass( data.Range.TrimStart( new char[] { '#' } ) );
				if( defnClass != null )
					lstDefinitions.Add( defnClass );
			}

			if( !data.SameValueAs.ToLower().StartsWith( "http://" ) )
			{
				DamlTypeDefinition defnRdf = this.FindTypeDefintionInRdfProperty( data.SameValueAs.TrimStart( new char[] { '#' } ) );
				if( defnRdf != null )
					lstDefinitions.Add( defnRdf );

				DamlTypeDefinition defnClass = this.FindTypeDefinitionInDamlClass( data.SameValueAs.TrimStart( new char[] { '#' } ) );
				if( defnClass != null )
					lstDefinitions.Add( defnClass );
			}
			
			return (DamlTypeDefinition[]) lstDefinitions.ToArray( typeof(DamlTypeDefinition) );
		}

		private DamlTypeDefinition FindTypeDefintionInRdfProperty( string strTypeName )
		{
			DamlTypeDefinition definition = null;
			try
			{
				// A type defition could be in an rdf:Property or a damlClass
				XmlNode root = this.m_doc.DocumentElement;
				
				// Search for a matching rdf:Property
				string strXPath = DamlConstants.RDF_PROPERTY + "[@" + DamlConstants.RDF_ID + "='" + strTypeName.TrimStart( new char[] { '#' } )  + "'" + "]";

				XmlNode typeDefinitionNode = root.SelectSingleNode( strXPath, m_mgr );
				
				if( typeDefinitionNode == null )
					return null;

				definition = this.GetNodeData( typeDefinitionNode );
			}
			catch( Exception e )
			{
				throw new Exception( e.Message );
			}

			return definition;
		}
		
		private DamlTypeDefinition FindTypeDefinitionInDamlClass( string strTypeName )
		{
			DamlTypeDefinition definition = null;
			try
			{
				XmlNode root = this.m_doc.DocumentElement;

				string strXPath = DamlConstants.DAML_CLASS + "[@" + DamlConstants.RDF_ID + "='" + strTypeName + "'" + "]";
				XmlNode typeDefinitionNode = root.SelectSingleNode( strXPath, m_mgr  );
				if( typeDefinitionNode == null )
					return null;
				
				definition = this.GetDamlClassTypeData( typeDefinitionNode );
			}
			catch( Exception e )
			{
				throw new Exception( e.Message );
			}

			return definition;	
		}
		
		
		private DamlTypeDefinition GetDamlClassTypeData( XmlNode typeDefinitionNode )
		{
			if( typeDefinitionNode == null )
				throw new ArgumentNullException( "typeDefinitionNode", "Cannot be null" );

			DamlTypeDefinition definition = null;

			// Get the first child of this node, this will determine the "type" we actually have
			XmlNode typeDescriptionNode = typeDefinitionNode.FirstChild;

			if( typeDescriptionNode == null )
				throw new Exception( "Invalid/Unexpected typeDefinitionNode passed" );

			switch( typeDescriptionNode.Name )
			{
				case DamlConstants.DAML_ONE_OF:
				{
					DamlOneOf oneOf = new DamlOneOf();
					
					// Get attribute on description node, set the type name
					oneOf.Name = typeDefinitionNode.Attributes[DamlConstants.RDF_ID].Value;
					// Get the parseType on the description node
					string strParseType = typeDescriptionNode.Attributes[DamlConstants.RDF_PARSE_TYPE].Value;
					
					if( strParseType != DamlConstants.DAML_COLLECTION )
						throw new Exception( "Unexpected parse type on daml:Class node's first child" );
					else oneOf.ParseType = enuRdfParseType.damlCollection;

					XmlNodeList lstOptions = typeDescriptionNode.ChildNodes;
					foreach( XmlNode optionNode in lstOptions )
					{
						oneOf.AddOption( optionNode.Attributes[DamlConstants.RDF_ID].Value );
					}

					definition = oneOf;
				}
				break;
				case DamlConstants.DAML_UNION_OF:
				{
					DamlUnionOf unionOf = new DamlUnionOf();
					
					// Get attribute on description node, set the type name
					unionOf.Name = typeDefinitionNode.Attributes[DamlConstants.RDF_ID].Value;
					// Get the parseType on the description node
					string strParseType = typeDescriptionNode.Attributes[DamlConstants.RDF_PARSE_TYPE].Value;
					
					if( strParseType != DamlConstants.DAML_COLLECTION )
						throw new Exception( "Unexpected parse type on daml:Class node's first child" );
					else unionOf.ParseType = enuRdfParseType.damlCollection;

					XmlNodeList lstOptions = typeDescriptionNode.ChildNodes;
					foreach( XmlNode optionNode in lstOptions )
					{
						XmlAttribute att = optionNode.Attributes[DamlConstants.RDF_ABOUT];
						unionOf.AddOption( att.Value );
					}

					definition = unionOf;
				}
				break;
				case DamlConstants.RDFS_SUBCLASSOF:
				{
					RdfsSubClassOf subClassOf = new RdfsSubClassOf();
					
					subClassOf.Name = typeDefinitionNode.Attributes[DamlConstants.RDF_ID].Value;
					subClassOf.Value = typeDescriptionNode.Attributes[DamlConstants.RDF_RESOURCE].Value;

					definition = subClassOf;
				}
				break;

				default: throw new Exception( "Unexpected node. Cannot get daml:Class type data." );
			}

			return definition;
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
		 * Return value: DamlProcess containing all the relevant data
		 */ 
		public DamlProcess GetProcessData( string strProcessName, enuProcessType processType )
		{
			DamlProcess retVal = new DamlProcess();

			XmlNode root = m_doc.DocumentElement;
			string strBaseUri = GetNamespaceBaseUri( DamlConstants.PROCESS_NS );
			string strUri = "";
			strUri = strBaseUri;
				
			switch( processType )
			{
				case enuProcessType.AtomicProcess: strUri += DamlConstants.DAML_ATOMIC_PROCESS;
					break;
				case enuProcessType.CompositeProcess: strUri += DamlConstants.DAML_COMPOSITE_PROCESS;
					break;
				case enuProcessType.SimpleProcess: strUri += DamlConstants.DAML_SIMPLE_PROCESS;
					break;
				default:  throw new ArgumentException( "Invalid processType value" );
			};

			string strXPath = DamlConstants.DAML_CLASS + "[@" + DamlConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DamlConstants.RDFS_SUBCLASSOF + "[@" + DamlConstants.RDF_RESOURCE + "='" + strUri + "']";
			
			XmlNode processNode = root.SelectSingleNode( strXPath, m_mgr ).ParentNode;

			if( processNode == null )
				throw new Exception( "Process node found" );
			
			// Set process name
			retVal.Name = processNode.Attributes[DamlConstants.RDF_ID].Value;
			// Set process type
			retVal.ProcessType = processType;
			
			// Get Process restrictions - these are actually input restrictions on 
			// the process
			DamlRestriction[] arrProcessRestrict = this.GetRestrictions( strProcessName );
			retVal.AddRestriction( enuIOPEType.Input, arrProcessRestrict );

			// Get inputs from querying RdfProperty nodes in document
			strXPath = DamlConstants.RDF_PROPERTY + "/" + DamlConstants.RDFS_SUBPROPERTYOF + "[@" +  DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.INPUT + "']" + "/" + "following-sibling::" + DamlConstants.RDFS_DOMAIN + "[@" + DamlConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
			XmlNodeList lstNodes = root.SelectNodes( strXPath, m_mgr );
			
			foreach( XmlNode node in lstNodes )
			{
				RdfProperty data = GetNodeData( node.ParentNode );
				retVal.AddInput( data );

				// Get restrictions (if any)
				enuIOPEType restrictionType = enuIOPEType.Input;
				DamlRestriction[] arrRestrict = this.GetRestrictions( data.Name );
				retVal.AddRestriction( restrictionType, arrRestrict );
				retVal.AddDataTypeDefinition( this.FindTypeDefinitions( data ) );					
			}
			
			// Get additional inputs from the process node itself
			// they may be hidden under restictions tagged with
			// daml:sameValueAs
			strXPath =  DamlConstants.DAML_CLASS + "[@" + DamlConstants.RDF_ID + "='" + retVal.Name + "']" + "/" + DamlConstants.RDFS_SUBCLASSOF + "/" + DamlConstants.DAML_RESTRICTION + "/" + DamlConstants.DAML_ON_PROPERTY + "[@" + DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.INPUT + "']" + "/" + "following-sibling::" + DamlConstants.DAML_SAMEVALUESAS;
			lstNodes = root.SelectNodes( strXPath, m_mgr );

			foreach( XmlNode node in lstNodes )
			{
				string strSameValueAs = node.Attributes[DamlConstants.RDF_RESOURCE].Value;
				strSameValueAs = strSameValueAs.Trim( new char[] {'#'} );
									
				// Go get RdfProperty data
				strXPath = DamlConstants.RDF_PROPERTY + "[@" + DamlConstants.RDF_ID + "='" + strSameValueAs + "']" + "/" + DamlConstants.RDFS_DOMAIN;
				XmlNode domainNode = root.SelectSingleNode( strXPath, m_mgr );

				// Add to list of inputs
				if( domainNode != null )
				{
					RdfProperty data = GetNodeData( domainNode.ParentNode );
					retVal.AddInput( data );

					// Get restrictions (if any)
					enuIOPEType restrictionType = enuIOPEType.Input;
					DamlRestriction[] arrRestrict = this.GetRestrictions( data.Name );
					retVal.AddRestriction( restrictionType, arrRestrict );
					retVal.AddDataTypeDefinition( this.FindTypeDefinitions( data ) );
				}
			}
							
			// Get outputs
			strXPath = DamlConstants.RDF_PROPERTY + "/" + DamlConstants.RDFS_SUBPROPERTYOF + "[@" +  DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.OUTPUT + "']" + "/" + "following-sibling::" + DamlConstants.RDFS_DOMAIN + "[@" + DamlConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
			lstNodes = root.SelectNodes( strXPath, m_mgr );

			foreach( XmlNode node in lstNodes )
			{
				RdfProperty data = GetNodeData( node.ParentNode );
				retVal.AddOutput( data );

				// Get restrictions (if any)
				enuIOPEType restrictionType = enuIOPEType.Output;
				DamlRestriction[] arrRestrict = this.GetRestrictions( data.Name );
				retVal.AddRestriction( restrictionType, arrRestrict );
				retVal.AddDataTypeDefinition( this.FindTypeDefinitions( data ) );
			}

			// Get preconditions
			strXPath = DamlConstants.RDF_PROPERTY + "/" + DamlConstants.RDFS_SUBPROPERTYOF + "[@" +  DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.PRECONDITION + "']" + "/" + "following-sibling::" + DamlConstants.RDFS_DOMAIN + "[@" + DamlConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
			lstNodes = root.SelectNodes( strXPath, m_mgr );

			foreach( XmlNode node in lstNodes )
			{
				RdfProperty data = GetNodeData( node.ParentNode );
				retVal.AddPrecondition( data );

				// Get restrictions (if any)
				enuIOPEType restrictionType = enuIOPEType.Precondition;
				DamlRestriction[] arrRestrict = this.GetRestrictions( data.Name );
				retVal.AddRestriction( restrictionType, arrRestrict );
				retVal.AddDataTypeDefinition( this.FindTypeDefinitions( data ) );
			}

			// Get effects
			strXPath = DamlConstants.RDF_PROPERTY + "/" + DamlConstants.RDFS_SUBPROPERTYOF + "[@" +  DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.EFFECT + "']" + "/" + "following-sibling::" + DamlConstants.RDFS_DOMAIN + "[@" + DamlConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
			lstNodes = root.SelectNodes( strXPath, m_mgr );

			foreach( XmlNode node in lstNodes )
			{
				RdfProperty data = GetNodeData( node.ParentNode );
				retVal.AddEffect( data );

				// Get restrictions (if any)
				enuIOPEType restrictionType = enuIOPEType.Effect;
				DamlRestriction[] arrRestrict = this.GetRestrictions( data.Name );
				retVal.AddRestriction( restrictionType, arrRestrict );
				retVal.AddDataTypeDefinition( this.FindTypeDefinitions( data ) );
			}
			
			// Get conditional outputs
			strXPath = DamlConstants.RDF_PROPERTY + "/" + DamlConstants.RDFS_SUBPROPERTYOF + "[@" +  DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.CONDITIONAL_OUTPUT + "']" + "/" + "following-sibling::" + DamlConstants.RDFS_DOMAIN + "[@" + DamlConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
			lstNodes = root.SelectNodes( strXPath, m_mgr );

			foreach( XmlNode node in lstNodes )
			{
				RdfProperty data = GetNodeData( node.ParentNode );
				retVal.AddConditionalOutput( data );

				// Get restrictions (if any)
				enuIOPEType restrictionType = enuIOPEType.ConditionalOutput;
				DamlRestriction[] arrRestrict = this.GetRestrictions( data.Name );
				retVal.AddRestriction( restrictionType, arrRestrict );
				retVal.AddDataTypeDefinition( this.FindTypeDefinitions( data ) );
			}

			// Get co-conditions
			strXPath = DamlConstants.RDF_PROPERTY + "/" + DamlConstants.RDFS_SUBPROPERTYOF + "[@" +  DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.CO_CONDITION + "']" + "/" + "following-sibling::" + DamlConstants.RDFS_DOMAIN + "[@" + DamlConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
			lstNodes = root.SelectNodes( strXPath, m_mgr );

			foreach( XmlNode node in lstNodes )
			{
				RdfProperty data = GetNodeData( node.ParentNode );
				retVal.AddCoCondition( data );

				// Get restrictions (if any)
				enuIOPEType restrictionType = enuIOPEType.CoCondition;
				DamlRestriction[] arrRestrict = this.GetRestrictions( data.Name );
				retVal.AddRestriction( restrictionType, arrRestrict );
				retVal.AddDataTypeDefinition( this.FindTypeDefinitions( data ) );
			}

			// Get co-outputs
			strXPath = DamlConstants.RDF_PROPERTY + "/" + DamlConstants.RDFS_SUBPROPERTYOF + "[@" +  DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.CO_OUTPUT + "']" + "/" + "following-sibling::" + DamlConstants.RDFS_DOMAIN + "[@" + DamlConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
			lstNodes = root.SelectNodes( strXPath, m_mgr );

			foreach( XmlNode node in lstNodes )
			{
				RdfProperty data = GetNodeData( node.ParentNode );
				retVal.AddCoOutput( data );

				// Get restrictions (if any)
				enuIOPEType restrictionType = enuIOPEType.CoOutput;
				DamlRestriction[] arrRestrict = this.GetRestrictions( data.Name );
				retVal.AddRestriction( restrictionType, arrRestrict );
				retVal.AddDataTypeDefinition( this.FindTypeDefinitions( data ) );
			}

			// Get parameters
			strXPath = DamlConstants.RDF_PROPERTY + "/" + DamlConstants.RDFS_SUBPROPERTYOF + "[@" +  DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.PARAMETER + "']" + "/" + "following-sibling::" + DamlConstants.RDFS_DOMAIN + "[@" + DamlConstants.RDF_RESOURCE + "='#" + strProcessName + "']"; 
			lstNodes = root.SelectNodes( strXPath, m_mgr );

			foreach( XmlNode node in lstNodes )
			{
				RdfProperty data = GetNodeData( node.ParentNode );
				retVal.AddParameter( data );

				// Get restrictions (if any)
				enuIOPEType restrictionType = enuIOPEType.Parameter;
				DamlRestriction[] arrRestrict = this.GetRestrictions( data.Name );
				retVal.AddRestriction( restrictionType, arrRestrict );
				retVal.AddDataTypeDefinition( this.FindTypeDefinitions( data ) );
			}

			// If we are dealing with a complex process we must go get
			// the substeps - need to get process:<type> data
			if( processType == enuProcessType.CompositeProcess )
			{
				retVal.SubTaskType = GetProcessSubTaskType( retVal.Name );
				retVal.AddSubProcess( GetSubProcesses( retVal.Name ) );
			}
			return retVal;
		}
	
        /* Function extracts the RdfProperty data from an XmlNode. Function expects
		 * specific RdfProperty information available.
		 * 
		 * Inputs: node - the XmlNode to extract RdfProperty data
		 * 
		 * Return values: the RdfProperty instance containing the node data
		 */ 
		private RdfProperty GetNodeData( XmlNode propertyNode )
		{
			if( propertyNode == null )
				throw new ArgumentNullException( "propertyNode", "Cannot be null" );

			RdfProperty data = new RdfProperty();

			// Set name
			data.Name = propertyNode.Attributes[DamlConstants.RDF_ID].Value;
			// Set Domain
			data.Domain = propertyNode.SelectSingleNode( DamlConstants.RDFS_DOMAIN, m_mgr ).Attributes[DamlConstants.RDF_RESOURCE].Value;
			// Set Range
			data.Range = propertyNode.SelectSingleNode( DamlConstants.RDFS_RANGE, m_mgr ).Attributes[DamlConstants.RDF_RESOURCE].Value;
			// Set SubPropertyOf
			data.SubPropertyOf = propertyNode.SelectSingleNode( DamlConstants.RDFS_SUBPROPERTYOF, m_mgr ).Attributes[DamlConstants.RDF_RESOURCE].Value;
			
			// Fill in sameValueAs data (if any)
			XmlNode sameValueAsNode = propertyNode.SelectSingleNode( DamlConstants.DAML_SAMEVALUESAS, m_mgr );
			if( sameValueAsNode != null )
				data.SameValueAs = sameValueAsNode.Attributes[DamlConstants.RDF_RESOURCE].Value;

			return data;
		}
		
		private DamlRestriction[] GetRestrictions( string strOwnerName )
		{
			ArrayList lstRestrictions = new ArrayList();
			
			XmlNode root = m_doc.DocumentElement;

			string strXPath = DamlConstants.DAML_CLASS + "[@" + DamlConstants.RDF_ID + "='" + strOwnerName + "']" + "/" + DamlConstants.RDFS_SUBCLASSOF + "/" + DamlConstants.DAML_RESTRICTION;

			XmlNodeList lstNodes = root.SelectNodes( strXPath, m_mgr );

			if( lstNodes.Count == 0 )
				return (DamlRestriction[]) lstRestrictions.ToArray( typeof(DamlRestriction) );
			
			foreach( XmlNode restrictionNode in lstNodes )
			{
				// Create a daml restriction
				DamlRestriction res = GetRestrictionNodeData( restrictionNode );
				
				res.Owner = strOwnerName;
				// Add it to our list of restrictions
				lstRestrictions.Add( res );
			}

			return (DamlRestriction[]) lstRestrictions.ToArray( typeof(DamlRestriction) );
		}
		
		private DamlRestriction GetRestrictionNodeData( XmlNode restrictionNode )
		{
			if( restrictionNode == null )
				throw new ArgumentNullException( "restriction", "Node cannnot be null" );
			
			DamlRestriction res = new DamlRestriction();
			
			// Restriction nodes may specify a cardinality
			if( restrictionNode.Attributes.Count > 0 )
			{
				XmlAttribute cardinalityAtt = restrictionNode.Attributes[DamlConstants.DAML_CARDINALITY];
				if( cardinalityAtt != null )
					res.Cardinality = int.Parse( cardinalityAtt.Value );
			}
					
			// Find the property which this restriction applies to
			XmlNode onPropertyNode = restrictionNode.SelectSingleNode( DamlConstants.DAML_ON_PROPERTY, m_mgr );

			if( onPropertyNode != null )
				res.OnProperty = onPropertyNode.Attributes[DamlConstants.RDF_RESOURCE].Value;
				
			XmlNode hasValueNode = restrictionNode.SelectSingleNode( DamlConstants.DAML_HAS_VALUE, m_mgr );
			if( hasValueNode != null )
				res.HasValue = hasValueNode.Attributes[DamlConstants.RDF_ID].Value;
		
			return res;
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
			string strBaseUri = GetNamespaceBaseUri( DamlConstants.PROCESS_NS );
			
			string strXPath = DamlConstants.DAML_CLASS + "[@" + DamlConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DamlConstants.RDFS_SUBCLASSOF + "[@" + DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.DAML_COMPOSITE_PROCESS + "']" + "/" + "following-sibling::" + DamlConstants.RDFS_SUBCLASSOF;
			XmlNode SubClassOfNode = root.SelectSingleNode( strXPath, m_mgr );
				
			if( SubClassOfNode == null )
				throw new Exception( "Complex process " + strProcessName + " data not found" );

			// Get process:<type>
			strXPath = ".//" + DamlConstants.DAML_CLASS + "[@" + DamlConstants.RDF_ABOUT + "]";
			XmlNode dataNode = SubClassOfNode.SelectSingleNode( strXPath, m_mgr );

			if( dataNode == null )
				throw new Exception( "No process:<type> data provided for complex process " + strProcessName + " document is invalid" );

			if( dataNode.Attributes[DamlConstants.RDF_ABOUT].Value == DamlConstants.PROCESS_CHOICE )
				return enuProcessSubTaskType.Choice;
			else return enuProcessSubTaskType.Sequence;
		}
		
		/* Function returns all the sub processes of a named process
		 * 
		 * Inputs: strProcessName - named process
		 * 
		 * Return values: an array of its sub processes
		 */
		private DamlProcess[] GetSubProcesses( string strProcessName )
		{
			ArrayList lstSubProcesses = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				string strBaseUri = GetNamespaceBaseUri( DamlConstants.PROCESS_NS );
				
				string strXPath = DamlConstants.DAML_CLASS + "[@" + DamlConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DamlConstants.RDFS_SUBCLASSOF + "[@" + DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.DAML_COMPOSITE_PROCESS + "']" + "/" + "following-sibling::" + DamlConstants.RDFS_SUBCLASSOF;
				XmlNode SubClassOfNode = root.SelectSingleNode( strXPath, m_mgr );
				
				if( SubClassOfNode == null )
					return (DamlProcess[]) lstSubProcesses.ToArray( typeof(DamlProcess) );

				// Use fuzzy paths from here -> "//" operator looking for any matching
				// child node - more expensive but intermediate nodes are not 
				// interesting/contain no info we can use

				strXPath = ".//" + DamlConstants.DAML_LIST_OF_INSTANCES_OF + "/" + DamlConstants.DAML_CLASS;
				XmlNodeList lstInstances = SubClassOfNode.SelectNodes( strXPath, m_mgr );
				
				foreach( XmlNode node in lstInstances )
				{
					string strProcess = node.Attributes[DamlConstants.RDF_ABOUT].Value;
					strProcess = strProcess.Trim( new Char[] { '#' } );
					enuProcessType processType = GetProcessType( strProcess );
					DamlProcess process = GetProcessData( strProcess, processType );
					lstSubProcesses.Add( process );
				}
			}
			catch( Exception /*e*/ )
			{
			}
			return (DamlProcess[]) lstSubProcesses.ToArray( typeof(DamlProcess) );
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
			string strBaseUri = GetNamespaceBaseUri( DamlConstants.PROCESS_NS );
			XmlNode root = m_doc.DocumentElement;

			string strXPathAtomicProcess = DamlConstants.DAML_CLASS + "[@" + DamlConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DamlConstants.RDFS_SUBCLASSOF + "[@" + DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.DAML_ATOMIC_PROCESS + "']";
			string strXPathSimpleProcess = DamlConstants.DAML_CLASS + "[@" + DamlConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DamlConstants.RDFS_SUBCLASSOF + "[@" + DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.DAML_SIMPLE_PROCESS + "']";
			string strXPathCompositeProcess = DamlConstants.DAML_CLASS + "[@" + DamlConstants.RDF_ID + "='" + strProcessName + "']" + "/" + DamlConstants.RDFS_SUBCLASSOF + "[@" + DamlConstants.RDF_RESOURCE + "='" + strBaseUri + DamlConstants.DAML_COMPOSITE_PROCESS + "']";

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
				string strProcessURI = GetNamespaceBaseUri( DamlConstants.PROCESS_NS );
				string strProcessType = "";

				switch( processType )
				{
					case enuProcessType.AtomicProcess: strProcessType = DamlConstants.DAML_ATOMIC_PROCESS;
													   break;

					case enuProcessType.CompositeProcess: strProcessType = DamlConstants.DAML_COMPOSITE_PROCESS;
														break;

					case enuProcessType.SimpleProcess: strProcessType = DamlConstants.DAML_SIMPLE_PROCESS;
													   break;
				};
				
				string strXPath = DamlConstants.DAML_CLASS + "/" + DamlConstants.RDFS_SUBCLASSOF + "[@" + DamlConstants.RDF_RESOURCE + "='" + strProcessURI + strProcessType + "']";

				XmlNodeList lstProcess = root.SelectNodes( strXPath, m_mgr );
			
				foreach( XmlNode processNode in lstProcess )
				{
					// Move up to parent
					XmlNode parentNode = processNode.ParentNode;

					string strValue = parentNode.Attributes[DamlConstants.RDF_ID].Value;
					
					if( strValue.Length > 0 )
						arrProcess.Add( strValue );
				}
			}
			catch( Exception /*e*/ )
			{
			}
			
			return (string[]) arrProcess.ToArray( typeof( System.String ) );
		}
	}
}
