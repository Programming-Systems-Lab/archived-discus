using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.DAML
{
	public sealed class DamlProcess
	{
		// All Processes have...
		private enuProcessType m_Type = enuProcessType.AtomicProcess;
		private string m_strName = "";
		// Composite processes only have...
		private enuProcessSubTaskType m_SubTaskType;
		// Restriction Map - Hashtable of Hashtables
		// Outer hashtable keyed on restriction types Inputs, outputs etc.
		// Inner hashtable keyed on type names e.g. input name, output name etc.
		Hashtable m_restrictionMap = new Hashtable();
		// DataTypeMap keyed on type names
		Hashtable m_dataTypeDefinitionMap = new Hashtable();

		// Each of these types represented as an RdfProperty
		// A collection of inputs 
		private ArrayList m_arrInputs = new ArrayList();
		// A collection of outputs
		private ArrayList m_arrOutputs = new ArrayList();
		// A collection of preconditions
		private ArrayList m_arrPreconditions = new ArrayList();
		// A collection of effects
		private ArrayList m_arrEffects = new ArrayList();
		// A collection of ConditionalOutputs
		private ArrayList m_arrConditionalOutputs = new ArrayList();
		// A collection of CoConditions
		private ArrayList m_arrCoConditions = new ArrayList();
		// A collection of CoOutputs
		private ArrayList m_arrCoOutputs = new ArrayList();
		// A collection of Parameters
		private ArrayList m_arrParameters = new ArrayList();
		// A collection of sub-steps (applicable for composite processes only)
		private ArrayList m_arrSubProcesses = new ArrayList();
		
		/* Constructor */
		public DamlProcess()
		{
		}

		public DamlTypeDefinition[] TypeDefinitions
		{
			get
			{
				ArrayList lstDefinitions = new ArrayList();

				IDictionaryEnumerator it = this.m_dataTypeDefinitionMap.GetEnumerator();

				while( it.MoveNext() )
				{
					lstDefinitions.Add( it.Value );
				}
				
				return (DamlTypeDefinition[]) lstDefinitions.ToArray( typeof( DamlTypeDefinition ) );
			}
		}

		public void AddDataTypeDefinition( DamlTypeDefinition[] arrDefinitions )
		{
			if( arrDefinitions == null || arrDefinitions.Length == 0 )
				return;

			for( int i = 0; i < arrDefinitions.Length; i++ )
				AddDataTypeDefinition( arrDefinitions[i] );
		}

		public void AddDataTypeDefinition( DamlTypeDefinition definition )
		{
			if( definition == null || definition.Name.Length == 0 )
				throw new ArgumentException( "Invalid DamlTypeDefintion passed, definition either null or has empty name", "definition" );
			
			if( !this.m_dataTypeDefinitionMap.Contains( definition.Name ) )
				this.m_dataTypeDefinitionMap.Add( definition.Name, definition );
		}

		public void RemoveDataTypeDefinition( string strTypeDefinitionName )
		{
			if( strTypeDefinitionName == null || strTypeDefinitionName.Length == 0 )
				return;

			this.m_dataTypeDefinitionMap.Remove( strTypeDefinitionName );
		}

		public void AddRestriction( enuIOPEType restrictionType, DamlRestriction[] arrRestrict )
		{
			if( arrRestrict == null || arrRestrict.Length == 0 )
				return;

			// Based on type determine if there is an entry for this type of restriction 
			// first

			if( !this.m_restrictionMap.ContainsKey( restrictionType ) )
			{
				Hashtable newInnerMap = new Hashtable();
				
				for( int i = 0; i < arrRestrict.Length; i++ )
				{
					if( !newInnerMap.Contains( arrRestrict[i].OnProperty ) )
						newInnerMap.Add( arrRestrict[i].OnProperty, arrRestrict[i] );
				}
				
				m_restrictionMap.Add( restrictionType, newInnerMap );
			}
			else // There is an inner map for this type of restriction
			{
				Hashtable innerMap = (Hashtable) m_restrictionMap[restrictionType];
				
				for( int i = 0; i < arrRestrict.Length; i++ )
				{
					if( !innerMap.Contains( arrRestrict[i].OnProperty ) )
						innerMap.Add( arrRestrict[i].OnProperty, arrRestrict[i] );
				}
			}
		}

		public DamlRestriction[] GetRestrictions( enuIOPEType restrictionType )
		{
			ArrayList lstRestrict = new ArrayList();

			Hashtable innerMap = (Hashtable) this.m_restrictionMap[restrictionType];

			if( innerMap != null )
			{
				IDictionaryEnumerator it = innerMap.GetEnumerator();
				
				while( it.MoveNext() )
				{
					lstRestrict.Add( it.Value );
				}
			}
			
			return (DamlRestriction[]) lstRestrict.ToArray( typeof(DamlRestriction) );
		}

		public enuIOPEType[] GetRestrictionTypes()
		{
			ArrayList lstRestrictionTypes = new ArrayList();

			IDictionaryEnumerator it = this.m_restrictionMap.GetEnumerator();

			while( it.MoveNext() )
				lstRestrictionTypes.Add( it.Key );

			return (enuIOPEType[]) lstRestrictionTypes.ToArray( typeof( enuIOPEType ) );
		}

		public void ClearRestrictionMap()
		{
			this.m_restrictionMap.Clear();
		}

		public void ClearRestrictionMap( enuIOPEType restrictionType )
		{
			if( this.m_restrictionMap.Contains( restrictionType ) )
				this.m_restrictionMap.Remove( restrictionType );
		}

		public string ToXml( bool bAddTypeDefintions )
		{
			XmlDocument doc = DamlContainer.BuildDamlDocumentTemplate( true );
			XmlNode root = doc.DocumentElement;
									
			// Add DAML Process body
			
			// Add this process' details name etc., if this process is complex
			// there is additional data we must add
			
			// Given a document (by ref) and an array of RDF properties
			// we repeat the same basic steps

			// Add Inputs (if any)
			if( this.m_arrInputs.Count > 0 )
				AddRdfProperties( ref doc, (RdfProperty[]) this.m_arrInputs.ToArray( typeof(RdfProperty) ), enuIOPEType.Input );

			// Add Outputs (if any)
			if( this.m_arrOutputs.Count > 0 )
				AddRdfProperties( ref doc, (RdfProperty[]) this.m_arrOutputs.ToArray( typeof(RdfProperty) ), enuIOPEType.Output );

			// Add Preconditions (if any)
			if( this.m_arrPreconditions.Count > 0 )
				AddRdfProperties( ref doc, (RdfProperty[]) this.m_arrPreconditions.ToArray( typeof(RdfProperty) ), enuIOPEType.Precondition );

			// Add Effects (if any)
			if( this.m_arrEffects.Count > 0 )
				AddRdfProperties( ref doc, (RdfProperty[]) this.m_arrEffects.ToArray( typeof(RdfProperty) ), enuIOPEType.Effect );
			
			// Add CoConditions (if any)
			if( this.m_arrCoConditions.Count > 0 )
				AddRdfProperties( ref doc, (RdfProperty[]) this.m_arrCoConditions.ToArray( typeof(RdfProperty) ), enuIOPEType.CoCondition );

			// Add ConditionalOutputs (if any)
			if( this.m_arrConditionalOutputs.Count > 0 )
				AddRdfProperties( ref doc, (RdfProperty[]) this.m_arrConditionalOutputs.ToArray( typeof(RdfProperty) ), enuIOPEType.ConditionalOutput );

			// Add CoOutputs (if any)
			if( this.m_arrCoOutputs.Count > 0 )
				AddRdfProperties( ref doc, (RdfProperty[]) this.m_arrCoOutputs.ToArray( typeof(RdfProperty) ), enuIOPEType.CoOutput );

			// Add Parameters (if any)
			if( this.m_arrParameters.Count > 0 )
				AddRdfProperties( ref doc, (RdfProperty[]) this.m_arrParameters.ToArray( typeof(RdfProperty) ), enuIOPEType.Parameter );

			// Create process node
			XmlNode processNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_CLASS, DamlConstants.DAML_NS_URI );
			// Create process attribute
			XmlAttribute processAtt = doc.CreateAttribute( DamlConstants.RDF_ID, DamlConstants.RDF_NS_URI );
			// Set attribute value (process name)
			processAtt.Value = this.m_strName;
			// Add attribute to node
			processNode.Attributes.Append( processAtt );
			
			// Specify what type of process this is - this will be a child of processNode
			// Create process type node (rdfs:subClassOf)
			XmlNode processTypeNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.RDFS_SUBCLASSOF, DamlConstants.RDFS_NS_URI );
			// Create process type node attribute
			XmlAttribute processTypeAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
			// Set the type of process
			switch( this.ProcessType )
			{
				case enuProcessType.AtomicProcess:
					processTypeAtt.Value = DamlConstants.ATOMIC_PROCESS_URI; break;

				case enuProcessType.SimpleProcess: 
					processTypeAtt.Value = DamlConstants.SIMPLE_PROCESS_URI; break;

				case enuProcessType.CompositeProcess:
					processTypeAtt.Value = DamlConstants.COMPOSITE_PROCESS_URI; break;

				default: throw new ArgumentException( "Unknown process type" );
			}
			// Add attribute to process type node
			processTypeNode.Attributes.Append( processTypeAtt );

			// Add the processType node as a child of the process node
			processNode.AppendChild ( processTypeNode );
			
			// Add restrictions to process node if any

			DamlRestriction[] arrRestrictions = this.GetRestrictions( enuIOPEType.Input );
				
			for( int j = 0; j < arrRestrictions.Length; j++ )
			{
				if( arrRestrictions[j].Owner == this.Name )
				{
					// Create subClassOf node
					XmlNode subClassOfNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.RDFS_SUBCLASSOF, DamlConstants.RDFS_NS_URI );
					// Create a node for each restriction (child of subClassOfNode)
					XmlNode restrictionNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_RESTRICTION, DamlConstants.DAML_NS_URI );
					// Fill in restrictionNode data
					
					// Add cardinality attribute if value has been set
					if( arrRestrictions[j].Cardinality != DamlRestriction.NO_CARDINALITY )
					{
						// Create attribute
						XmlAttribute cardinalityAtt = doc.CreateAttribute( DamlConstants.DAML_CARDINALITY, DamlConstants.DAML_NS_URI );
						// Set attribute value
						cardinalityAtt.Value = arrRestrictions[j].Cardinality.ToString();
						// Add attribute to node
						restrictionNode.Attributes.Append( cardinalityAtt );
					}

					// Create onPropertyNode
					XmlNode onPropertyNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_ON_PROPERTY, DamlConstants.DAML_NS_URI );
					// Create onProperty attribute
					XmlAttribute onPropertyAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
					// Set attribute value
					onPropertyAtt.Value = arrRestrictions[j].OnProperty;
					// Add attribute to node
					onPropertyNode.Attributes.Append( onPropertyAtt );
					
					// Add onPropertyNode to restrictionNode
					restrictionNode.AppendChild( onPropertyNode );
					
					// If this instance is a composite process add extra nodes
					// to the (composedOf) restriction node
					if( this.ProcessType == enuProcessType.CompositeProcess && arrRestrictions[j].OnProperty == DamlConstants.PROCESS_COMPOSED_OF_URI )
					{
						XmlNode toClassNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_TO_CLASS, DamlConstants.DAML_NS_URI );
						XmlNode classNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_CLASS, DamlConstants.DAML_NS_URI );
						// Create intersection of node
						XmlNode intersectionOfNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_INTERSECTION_OF, DamlConstants.DAML_NS_URI );
						// Create intersectionOf node attribute
						XmlAttribute intersectionOfAtt = doc.CreateAttribute( DamlConstants.RDF_PARSE_TYPE, DamlConstants.RDF_NS_URI );
						// Set attribute value
						intersectionOfAtt.Value = DamlConstants.DAML_COLLECTION;
						// Add attribute to intersecionOfNode
						intersectionOfNode.Attributes.Append( intersectionOfAtt );

						// Add a Daml class node and another restriction node
						// to the intersectionOfNode
						// Create a node to store the type of sub task "list" we have
						// one of enuProcessSubTaskType
						XmlNode subTaskTypeNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_CLASS, DamlConstants.DAML_NS_URI );
						// Create an attribute for the subTaskType node
						XmlAttribute subTaskTypeAtt = doc.CreateAttribute( DamlConstants.RDF_ABOUT, DamlConstants.RDF_NS_URI );
						
						// Set the atribute value
						switch( this.SubTaskType )
						{
							case enuProcessSubTaskType.Choice:
								subTaskTypeAtt.Value = DamlConstants.PROCESS_CHOICE; break;
							case enuProcessSubTaskType.Sequence:
								subTaskTypeAtt.Value = DamlConstants.PROCESS_SEQUENCE; break;
							default: throw new Exception( "Unknown process sub-task type" );
						}

						// Add subTaskTypeAtt attribute to subTaskType node
						subTaskTypeNode.Attributes.Append( subTaskTypeAtt );

						// Add a restriction to the intersectionOf node
						// this is where we list the names of the subprocesses
						XmlNode subProcessRestrictionNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_RESTRICTION, DamlConstants.DAML_NS_URI );
						XmlNode subProcessRestrictionOnPropertyNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_ON_PROPERTY, DamlConstants.DAML_NS_URI );
						// Add attribute
						XmlAttribute subProcessRestrictionOnPropertyAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
						// Set attribute value
						subProcessRestrictionOnPropertyAtt.Value = DamlConstants.PROCESS_COMPONENTS_URI;
						// Add attribute to node
						subProcessRestrictionOnPropertyNode.Attributes.Append( subProcessRestrictionOnPropertyAtt );
						
						// last daml:toClass/daml:Class construct added to the subProcessRestrictionNode
						XmlNode processListToClassNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_TO_CLASS, DamlConstants.DAML_NS_URI );
						XmlNode processListClassNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_CLASS, DamlConstants.DAML_NS_URI );
						XmlNode processListOfInstancesNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_LIST_OF_INSTANCES_OF, DamlConstants.DAML_NS_URI );
						// Add attribute
						XmlAttribute processListOfInstancesAtt = doc.CreateAttribute( DamlConstants.RDF_PARSE_TYPE, DamlConstants.RDF_NS_URI );
						// Set attribute value
						processListOfInstancesAtt.Value = DamlConstants.DAML_COLLECTION;
						// Add attribute to node
						processListOfInstancesNode.Attributes.Append( processListOfInstancesAtt );

						for( int i = 0; i < this.m_arrSubProcesses.Count; i++ )
						{
							// Create process name node
							XmlNode processNameNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_CLASS, DamlConstants.DAML_NS_URI );
							// Create process name attribute
							XmlAttribute processNameAtt = doc.CreateAttribute( DamlConstants.RDF_ABOUT, DamlConstants.RDF_NS_URI );
							// Set processNameAtt value
							processNameAtt.Value = "#" + ((DamlProcess) this.m_arrSubProcesses[i]).Name;
							// Add attribute to node
							processNameNode.Attributes.Append( processNameAtt );
							// Add to list of instances node
							processListOfInstancesNode.AppendChild( processNameNode );
						}

						processListClassNode.AppendChild( processListOfInstancesNode );
						processListToClassNode.AppendChild( processListClassNode );
						subProcessRestrictionNode.AppendChild( subProcessRestrictionOnPropertyNode );
						subProcessRestrictionNode.AppendChild( processListToClassNode );
						intersectionOfNode.AppendChild( subTaskTypeNode );
						intersectionOfNode.AppendChild( subProcessRestrictionNode );
						classNode.AppendChild( intersectionOfNode );
						toClassNode.AppendChild( classNode );
						restrictionNode.AppendChild( toClassNode );
					}
										
					// Add restrictionNode to subClassOfNode
					subClassOfNode.AppendChild( restrictionNode );
					//Add subClassOfNode to root
					processNode.AppendChild( subClassOfNode );
				}
			}
									
			// Add process node to root
			root.AppendChild( processNode );
			
			if( bAddTypeDefintions )
			{
				// Add data type definitions
				IDictionaryEnumerator it = this.m_dataTypeDefinitionMap.GetEnumerator();
			
				while( it.MoveNext() )
				{
					// Ask each type definition to ToXml() itself
					// this gives us the nodes to add to our document
					DamlTypeDefinition definition = (DamlTypeDefinition) it.Value;
					XmlDocument typeDoc = new XmlDocument();
					// Load the xml of the type definition
					typeDoc.LoadXml( definition.ToXml() );
					// Get the document element
					XmlNode typeDocRoot = typeDoc.DocumentElement;
					// Import the first child of the root into the damlProcess xml document
					// being created
					XmlNode temp = doc.ImportNode( typeDocRoot.FirstChild, true );
					// Append that node to our current document root
					root.AppendChild( temp );
				}
			}
	
			return root.OuterXml;
		}
		
		private void AddRdfProperties( ref XmlDocument doc, RdfProperty[] arrData, enuIOPEType restrictionType )
		{
			XmlNode root = doc.DocumentElement;

			for( int i = 0; i < arrData.Length; i++ )
			{
				RdfProperty prop = (RdfProperty) arrData[i];
				XmlDocument propDoc = new XmlDocument();
				propDoc.LoadXml( prop.ToXml() );
				XmlNode propDocRoot = propDoc.DocumentElement;
				XmlNode propertyNode = doc.ImportNode( propDocRoot.FirstChild, true );
				
				// Get restrictions on property (if any)
				DamlRestriction[] arrRestrictions = this.GetRestrictions( restrictionType );
				
				for( int j = 0; j < arrRestrictions.Length; j++ )
				{
					if( arrRestrictions[j].Owner == prop.Name )
					{
						// Create subClassOf node
						XmlNode subClassOfNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.RDFS_SUBCLASSOF, DamlConstants.RDFS_NS_URI );
						// Create a node for each restriction (child of subClassOfNode)
						XmlNode restrictionNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_RESTRICTION, DamlConstants.DAML_NS_URI );
						// Fill in restrictionNode data
					
						// Add cardinality attribute if value has been set
						if( arrRestrictions[j].Cardinality != DamlRestriction.NO_CARDINALITY )
						{
							// Create attribute
							XmlAttribute cardinalityAtt = doc.CreateAttribute( DamlConstants.DAML_CARDINALITY, DamlConstants.DAML_NS_URI );
							// Set attribute value
							cardinalityAtt.Value = arrRestrictions[j].Cardinality.ToString();
							// Add attribute to node
							restrictionNode.Attributes.Append( cardinalityAtt );
						}

						// Create onPropertyNode
						XmlNode onPropertyNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_ON_PROPERTY, DamlConstants.DAML_NS_URI );
						// Create onProperty attribute
						XmlAttribute onPropertyAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
						// Set attribute value
						onPropertyAtt.Value = arrRestrictions[j].OnProperty;
						// Add attribute to node
						onPropertyNode.Attributes.Append( onPropertyAtt );
					
						// Add onPropertyNode to restrictionNode
						restrictionNode.AppendChild( onPropertyNode );
						// Add restrictionNode to subClassOfNode
						subClassOfNode.AppendChild( restrictionNode );
						//Add subClassOfNode to root
						propertyNode.AppendChild( subClassOfNode );
					}
				}

				// Add rdf:Property to root of document
				root.AppendChild( propertyNode );
			}
		}

		// Properties
		public enuProcessType ProcessType
		{
			get
			{ return m_Type; }
			set
			{ m_Type = value; }
		}

		// ONLY for Composite processes - should subclass to hide?
		public enuProcessSubTaskType SubTaskType
		{
			get
			{ return m_SubTaskType; }
			set
			{ m_SubTaskType = value; }
		}

		public string Name
		{
			get
			{ return m_strName; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;

				m_strName = value; 
			}
		}
		
		public RdfProperty[] Inputs
		{
			get
			{ return (RdfProperty[]) m_arrInputs.ToArray( typeof(RdfProperty) ); }
		}

		public RdfProperty[] Outputs
		{
			get
			{ return (RdfProperty[]) m_arrOutputs.ToArray( typeof(RdfProperty) ); }
		}

		public RdfProperty[] Preconditions
		{
			get
			{ return (RdfProperty[]) m_arrPreconditions.ToArray( typeof(RdfProperty) ); }
		}

		public RdfProperty[] Effects
		{
			get
			{ return (RdfProperty[]) m_arrEffects.ToArray( typeof(RdfProperty) ); }
		}

		public RdfProperty[] ConditionalOutputs
		{
			get
			{ return (RdfProperty[]) m_arrConditionalOutputs.ToArray( typeof(RdfProperty) ); }
		}

		public RdfProperty[] CoConditions
		{
			get
			{ return (RdfProperty[]) m_arrCoConditions.ToArray( typeof(RdfProperty) ); }
		}

		public RdfProperty[] CoOutputs
		{
			get
			{ return (RdfProperty[]) m_arrCoOutputs.ToArray( typeof(RdfProperty) ); }
		}

		public RdfProperty[] Parameters
		{
			get
			{ return (RdfProperty[]) m_arrParameters.ToArray( typeof(RdfProperty) ); }
		}

		public DamlProcess[] SubProcesses
		{
			get
			{ return (DamlProcess[]) m_arrSubProcesses.ToArray( typeof(DamlProcess) ); }
		}
		
		/* Procedure Adds an input to a DamlProcess
		 * 
		 * Inputs: data - RdfProperty details of the input
		 */ 
		public void AddInput( RdfProperty data )
		{
			m_arrInputs.Add( data );
		}

		/* Procedure Adds an output to a DamlProcess
		 * 
		 * Inputs: data - RdfProperty details of the output
		 */ 
		public void AddOutput( RdfProperty data )
		{
			m_arrOutputs.Add( data );
		}

		/* Procedure Adds a precondition to a DamlProcess
		 * 
		 * Inputs: data - RdfProperty details of the precondition
		 */ 
		public void AddPrecondition( RdfProperty data )
		{
			m_arrPreconditions.Add( data );
		}

		/* Procedure Adds an effect to a DamlProcess
		 * 
		 * Inputs: data - RdfProperty details of the effect
		 */ 
		public void AddEffect( RdfProperty data )
		{
			m_arrEffects.Add( data );
		}
		
		/* Procedure Adds a conditional output to a DamlProcess
		 * 
		 * Inputs: data - RdfProperty details of the conditional output
		 */ 
		public void AddConditionalOutput( RdfProperty data )
		{
			m_arrConditionalOutputs.Add( data );
		}
		
		/* Procedure Adds a co-condition to a DamlProcess
		 * 
		 * Inputs: data - RdfProperty details of the co-condition
		 */ 
		public void AddCoCondition( RdfProperty data )
		{
			m_arrCoConditions.Add( data );
		}
		
		/* Procedure Adds a co-output to a DamlProcess
		 * 
		 * Inputs: data - RdfProperty details of the co-output
		 */ 
		public void AddCoOutput( RdfProperty data )
		{
			m_arrCoOutputs.Add( data );
		}

		/* Procedure Adds a parameter to a DamlProcess
		 * 
		 * Inputs: data - RdfProperty details of the parameter
		 */ 
		public void AddParameter( RdfProperty data )
		{
			m_arrParameters.Add( data );
		}
		
		/* Procedure Adds a subprocess to a (composite) DamlProcess
		 * 
		 * Inputs: data - DamlProcess details of the process
		 * 
		 * Exceptions: throws InvalidOperationException if the process is not being
		 *			   added to a Composite DamlProcess. ONLY Composite DAMLProcesses
		 *			   support subprocesses.
		 */ 
		public void AddSubProcess( DamlProcess data )
		{
			if( ProcessType == enuProcessType.CompositeProcess )
				m_arrSubProcesses.Add( data );
			else throw new InvalidOperationException( "Only Composite Processes can have SubProcesses" );
		}

		/* Procedure Adds an array of subprocess to a (composite) DamlProcess
		 * 
		 * Inputs: arrData - DamlProcess details of the processes
		 * 
		 * Exceptions: throws InvalidOperationException if the processes are not being
		 *			   added to a Composite DamlProcess. ONLY Composite DAMLProcesses
		 *			   support subprocesses.
		 */ 
		public void AddSubProcess( DamlProcess[] arrData )
		{
			if( ProcessType == enuProcessType.CompositeProcess )
				m_arrSubProcesses.AddRange( arrData );
			else throw new InvalidOperationException( "Only Composite Processes can have SubProcesses" );
		}

		/* Procedure clears all inputs
		 */ 
		public void ClearInputs()
		{
			m_arrInputs.Clear();
		}

		/* Procedure clears all outputs
		 */ 
		public void ClearOutputs()
		{
			m_arrOutputs.Clear();
		}

		/* Procedure clears all preconditions
		 */ 
		public void ClearPreconditons()
		{
			m_arrPreconditions.Clear();
		}

		/* Procedure clears all effects
		 */ 
		public void ClearEffects()
		{
			m_arrEffects.Clear();
		}

		/* Procedure clears all conditional outputs
		 */ 
		public void ClearConditionalOutputs()
		{
			m_arrConditionalOutputs.Clear();
		}

		/* Procedure clears all co-conditions
		 */ 
		public void ClearCoConditions()
		{
			m_arrCoConditions.Clear();
		}

		/* Procedure clears all co-outputs
		 */ 
		public void ClearCoOutputs()
		{
			m_arrCoOutputs.Clear();
		}

		/* Procedure clears all parameters
		 */ 
		public void ClearParameters()
		{
			m_arrParameters.Clear();
		}

		/* Procedure clears all sub processes
		 */ 
		public void ClearSubProcesses()
		{
			m_arrSubProcesses.Clear();
		}
		
		public void ClearDataTypeDefinitionMap()
		{
			m_dataTypeDefinitionMap.Clear();
		}

		/* Procedure clears all the constituent data structures used to store
		 * process info
		 */ 
		public void ClearAll()
		{
			this.ClearCoConditions();
			this.ClearConditionalOutputs();
			this.ClearCoOutputs();
			this.ClearEffects();
			this.ClearInputs();
			this.ClearOutputs();
			this.ClearParameters();
			this.ClearPreconditons();
			this.ClearSubProcesses();
			this.ClearRestrictionMap();

			this.ClearDataTypeDefinitionMap();
		}

		// HasXXXX
		public bool HasInputs
		{
			get
			{ return m_arrInputs.Count > 0; }
		}

		public bool HasOutputs
		{
			get
			{ return m_arrOutputs.Count > 0; }
		}

		public bool HasPreconditions
		{
			get
			{ return m_arrPreconditions.Count > 0; }
		}
		public bool HasEffects
		{
			get
			{ return m_arrEffects.Count > 0; }
		}

		public bool HasConditionalOutputs
		{
			get
			{ return m_arrConditionalOutputs.Count > 0; }
		}

		public bool HasCoConditions
		{
			get
			{ return m_arrCoConditions.Count > 0; }
		}
		public bool HasCoOutputs
		{
			get
			{ return m_arrCoOutputs.Count > 0; }
		}
		public bool HasParameters
		{
			get
			{ return m_arrParameters.Count > 0; }
		}
		
		public bool HasSubProcesses
		{
			get
			{
				// Only Composite Processes can have subprocesses
				if( ProcessType != enuProcessType.CompositeProcess )
					return false;

				return m_arrSubProcesses.Count > 0;
			}
		}

		public bool HasData
		{
			get
			{
				bool bLVal = ( HasInputs || HasOutputs ) || ( HasPreconditions || HasEffects );
				bool bRVal = ( HasConditionalOutputs || HasCoConditions ) || ( HasCoOutputs || HasParameters );
				return ( bLVal || bRVal ) || HasSubProcesses;
			}
		}

		public bool isEmpty
		{
			get
			{ return !HasData; }
		}
	}
}
