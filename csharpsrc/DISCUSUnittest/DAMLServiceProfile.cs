using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PSL.DISCUS.DAML
{
	// Value types representing IOPEs
	public struct IOType
	{
		private string m_strParamDesc;
		private string m_strParamName;
		private string m_strRestrictedTo;
		private string m_strRefersTo;

		public bool isValid
		{
			get
			{ return ( m_strParamDesc.Length > 0 && m_strParamName.Length > 0 ) && ( m_strRestrictedTo.Length > 0 && m_strRefersTo.Length > 0 ); }
		}
		
		public string ParameterDesc
		{
			get
			{ return m_strParamDesc; }
			set
			{ m_strParamDesc = value; }
		}

		public string ParameterName
		{
			get
			{ return m_strParamName; }
			set
			{ m_strParamName = value; }
		}

		public string RestrictedTo
		{
			get
			{ return m_strRestrictedTo; }
			set
			{ m_strRestrictedTo = value; }
		}

		public string RefersTo
		{
			get
			{ return m_strRefersTo; }
			set
			{ m_strRefersTo = value; }
		}

	}
		
	public struct EPType
	{
		private string m_strConditionDesc;
		private string m_strConditionName;
		private string m_strStatement;
		private string m_strRefersTo;

		public bool isValid
		{
			get
			{ return ( m_strConditionDesc.Length > 0 && m_strConditionName.Length > 0 ) && ( m_strStatement.Length > 0 && m_strRefersTo.Length > 0 ); }
		}

		public string ConditionDesc
		{
			get
			{ return m_strConditionDesc; }
			set
			{ m_strConditionDesc = value; }
		}

		public string ConditionName
		{
			get
			{ return m_strConditionName; }
			set
			{ m_strConditionName = value; }
		}

		public string Statement
		{
			get
			{ return m_strStatement; }
			set
			{ m_strStatement = value; }
		}

		public string RefersTo
		{
			get
			{ return m_strRefersTo; }
			set
			{ m_strRefersTo = value; }
		}
	}


	public enum enuIOPEType
	{
		Input,
		Output,
		Precondition,
		Effect
	};

	public enum enuIOPESearchBy
	{
		PARAM_DESC,
		PARAM_NAME,
		COND_DESC,
		COND_NAME
	};

	/// <summary>
	/// Summary description for DAMLServiceProfile.
	/// </summary>
	public class DAMLServiceProfile
	{
		public const string XMLNS = "xmlns";
		// Known namespaces expected in Profile
		public const string RDF_SCHEMA_NS = "rdfs";
		public const string RDF_NS = "rdf";
		public const string DAML_NS = "daml";
		public const string SERVICE_NS = "service";
		public const string PROCESS_NS = "process";
		public const string PROFILE_NS = "profile";
		public const string XSD_NS = "xsd";
		public const string DEFAULT_NS = "DEFAULT_NS";
		// Known elements expected in Profile
		// DAML Constants (nodes)
		public const string DAML_ONTOLOGY = DAML_NS + ":Ontology";
		public const string DAML_VERSIONINFO = DAML_NS + ":versionInfo";
		public const string DAML_IMPORTS = DAML_NS + ":imports";
		// Service Constants (nodes)
		public const string SERVICE_PROFILE = SERVICE_NS + ":ServiceProfile";
		public const string SERVICE_PRESENTED_BY = SERVICE_NS + ":isPresentedBy";
		// Profile Constants (nodes)
		public const string PROFILE_SERVICE_NAME = PROFILE_NS + ":serviceName";
		public const string PROFILE_TEXT_DESC = PROFILE_NS + ":textDescription";
		public const string PROFILE_INTENDED_PURPOSE = PROFILE_NS + ":intendedPurpose";
		public const string PROFILE_PROVIDED_BY = PROFILE_NS + ":providedBy";
		public const string PROFILE_REQUESTED_BY = PROFILE_NS + ":requestedBy";
		public const string PROFILE_SERVICE_PROVIDER = PROFILE_NS + ":ServiceProvider";
		public const string PROFILE_NAME = PROFILE_NS + ":name";
		public const string PROFILE_PHONE = PROFILE_NS + ":phone";
		public const string PROFILE_FAX = PROFILE_NS + ":fax";
		public const string PROFILE_EMAIL = PROFILE_NS + ":email";
		public const string PROFILE_PHYSICAL_ADDRESS = PROFILE_NS + ":physicalAddress";
		public const string PROFILE_WEB_URL = PROFILE_NS + ":webURL";
		public const string PROFILE_GEOG_RADIUS = PROFILE_NS + ":geographicRadius";
		public const string PROFILE_QUALITY_RATING = PROFILE_NS + ":qualityRating";
		public const string PROFILE_HAS_PROCESS = PROFILE_NS + ":has_process";
		// Constants related to IOPEs
		public const string PROFILE_PARAM_DESC = PROFILE_NS + ":ParameterDescription";
		public const string PROFILE_PARAM_NAME = PROFILE_NS + ":parameterName";
		public const string PROFILE_RESTRICTED_TO = PROFILE_NS + ":restrictedTo";
		public const string PROFILE_REFERS_TO = PROFILE_NS + ":refersTo";
		public const string PROFILE_CONDITION_DESC = PROFILE_NS + ":ConditionDescription";
		public const string PROFILE_CONDITION_NAME = PROFILE_NS + ":conditionName";
		public const string PROFILE_STATEMENT = PROFILE_NS + ":statement";
		// IOPE Constants - Inputs, Outputs, PreConditions, Effects
		public const string INPUT = "input";
		public const string OUTPUT = "output";
		public const string PRECONDITION = "precondition";
		public const string EFFECT = "effect";
		// RDF Constants (attributes)
		public const string RDF_RESOURCE = RDF_NS + ":resource";
		public const string RDF_COMMENT = RDF_NS + ":comment";
		public const string RDF_ID = RDF_NS + ":ID";
		// Member variables
		private XmlDocument m_doc;
		private EventLog m_EvtLog;
		//private Hashtable m_nsMapping;
		private XmlNamespaceManager m_mgr;

		public DAMLServiceProfile()
		{
			m_EvtLog = new EventLog( "Application" );
			m_EvtLog.Source = "DAMLServiceProfile";
			//m_nsMapping = new Hashtable();
			m_doc = new XmlDocument();
			m_mgr = null;
		}

		public bool LoadProfile( string strProfileXml )
		{
			bool bStatus = false;

			try
			{
				m_doc.LoadXml( strProfileXml );
				// Move to root element
				XmlNode root = m_doc.DocumentElement;
				// Get attributes of root element
				XmlAttributeCollection attColl = root.Attributes;
				// TODO: Should use PopScope instead??
				if( m_mgr != null )
				{
					m_mgr.PopScope();
					m_mgr = null;
				}

				m_mgr = new XmlNamespaceManager( m_doc.NameTable );
				m_mgr.PushScope();

				for( int i = 0; i < attColl.Count; i++ )
				{
					// Extract all namespaces we can find in document root
					// and add to namespace manager
					if( attColl[i].Prefix == XMLNS )
						m_mgr.AddNamespace( attColl[i].LocalName, attColl[i].InnerText );
					 
					// Add default namespace (if any) and add to namespace manager
					if( attColl[i].Prefix == "" )
						m_mgr.AddNamespace( DEFAULT_NS, attColl[i].InnerText );
				}
				
				bStatus = true;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );	
			}

			return bStatus;
		}

		// Properties
		public string[] OntologyImports
		{
			get
			{ return GetOntologyImports(); }
		}
		
		public string ServiceName
		{
			get
			{ return GetServiceName(); }
		}
		
		public string ServiceProfile
		{
			get
			{ return GetServiceProfile(); }
		}

		public string PresentedBy
		{
			get
			{ return GetPresentedBy(); }
		}

		public string TextDescription
		{
			get
			{ return GetTextDescription(); }
		}
		
		public string IntendedPurpose
		{
			get
			{ return GetIntendedPurpose(); }
		}
			
		public string RequestedBy
		{
			get
			{ return GetRequestedBy(); }
		}
		
		public string ServiceProvider
		{
			get
			{ return GetServiceProvider(); }
		}

		public string ProfileName
		{
			get
			{ return GetProfileName(); }
		}

		public string ProfilePhone
		{
			get
			{ return GetProfilePhone(); }
		}
		
		public string ProfileFax
		{
			get
			{ return GetProfileFax(); }
		}
		
		public string ProfileEmail
		{
			get
			{ return GetProfileEmail(); }
		}

		public string PhysicalAddress
		{
			get
			{ return GetPhysicalAddress(); }
		}

		public string WebUrl
		{
			get
			{ return GetWebUrl(); }
		}

		public string GeographicRadius
		{
			get
			{ return GetGeographicRadius(); }
		}
		
		public string QualityRating
		{
			get
			{ return GetQualityRating(); }
		}

		public string ProcessModel
		{
			get
			{ return GetProcessModel(); }
		}
		
		public IOType[] InputParameters
		{
			get
			{ return GetInputParameters(); }
		}

		public IOType[] OutputParameters
		{
			get
			{ return GetOutputParameters(); }
		}

		public EPType[] Preconditions
		{
			get
			{ return GetPreconditions(); }
		}

		public EPType[] Effects
		{
			get
			{ return GetEffects(); }
		}
		
		
		// Methods
		public IOType GetInputByName( string strName )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Input );
			// Build XPath Expression
			string strXPathExpr = DAMLServiceProfile.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.PARAM_NAME, strName );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			node = node.ParentNode;

			IOType ioData = GetIONodeData( node );

			return ioData;
		}
		
		public IOType GetInputByDescription( string strDesc )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Input );
			// Build XPath Expression
			string strXPathExpr = DAMLServiceProfile.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.PARAM_DESC, strDesc );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			
			IOType ioData = GetIONodeData( node );

			return ioData;
		}
		
		public IOType GetOutputByName( string strName )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Output );
			// Build XPath Expression
			string strXPathExpr = DAMLServiceProfile.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.PARAM_NAME, strName );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			node = node.ParentNode;

			IOType ioData = GetIONodeData( node );

			return ioData;
		}

		public IOType GetOutputByDescription( string strDesc )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Output );
			// Build XPath Expression
			string strXPathExpr = DAMLServiceProfile.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.PARAM_DESC, strDesc );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			
			IOType ioData = GetIONodeData( node );

			return ioData;
		}
				
		public EPType GetPreconditionByName( string strName )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Precondition );
			// Build XPath Expression
			string strXPathExpr = DAMLServiceProfile.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.COND_NAME, strName );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			node = node.ParentNode;

			EPType epData = GetEPNodeData( node );

			return epData;
		}

		public EPType GetPreconditionByDescription( string strDesc )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Precondition );
			// Build XPath Expression
			string strXPathExpr = DAMLServiceProfile.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.COND_DESC, strDesc );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			
			EPType epData = GetEPNodeData( node );

			return epData;
		}

		public EPType GetEffectByName( string strName )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Effect );
			// Build XPath Expression
			string strXPathExpr = DAMLServiceProfile.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.COND_NAME, strName );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			node = node.ParentNode;
			
			EPType epData = GetEPNodeData( node );

			return epData;
		}

		public EPType GetEffectByDescription( string strDesc )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Effect );
			// Build XPath Expression
			string strXPathExpr = DAMLServiceProfile.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.COND_DESC, strDesc );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			
			EPType epData = GetEPNodeData( node );

			return epData;
		}
		

		private EPType[] GetEffects()
		{
			ArrayList lstEffects = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				
				// Create Expression Builder instance
				IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Effect );
				// Build XPath Expression
				string strXPathExpr = DAMLServiceProfile.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.COND_DESC );
							
				XmlNodeList nodeList = root.SelectNodes( strXPathExpr, m_mgr );

				foreach( XmlNode descNode in nodeList )
				{
					EPType effect = GetEPNodeData( descNode );
					
					if( effect.isValid )
						lstEffects.Add( effect );
				}
			
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return (EPType[]) lstEffects.ToArray( typeof(EPType) );
		}

		private EPType[] GetPreconditions()
		{
			ArrayList lstPreconds = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				
				// Create Expression Builder instance
				IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Precondition );
				// Build XPath Expression
				string strXPathExpr = DAMLServiceProfile.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.COND_DESC );
							
				XmlNodeList nodeList = root.SelectNodes( strXPathExpr, m_mgr );

				foreach( XmlNode descNode in nodeList )
				{
					EPType precond = this.GetEPNodeData( descNode );
					
					if( precond.isValid )
						lstPreconds.Add( precond );
				}
			
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return (EPType[]) lstPreconds.ToArray( typeof(EPType) );
		}
		
		private EPType GetEPNodeData( XmlNode descNode )
		{
			EPType epData = new EPType();
				
			XmlAttributeCollection attColl = descNode.Attributes;
					
			foreach( XmlAttribute att in attColl )
			{
				if( att.Name == DAMLServiceProfile.RDF_ID )
				{
					epData.ConditionDesc = att.Value;
					break;
				}
			}
					
			XmlNode nameNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_CONDITION_NAME, m_mgr );
			epData.ConditionName = nameNode.InnerText;
					
			XmlNode stmntNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_STATEMENT, m_mgr );
			attColl = stmntNode.Attributes;

			foreach( XmlAttribute att in attColl )
			{
				if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
				{
					epData.Statement = att.Value;
					break;
				}
			}
					
			XmlNode referNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_REFERS_TO, m_mgr );
			attColl = referNode.Attributes;

			foreach( XmlAttribute att in attColl )
			{
				if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
				{
					epData.RefersTo = att.Value;
					break;
				}
			}

			return epData;
		}

		private IOType[] GetOutputParameters()
		{
			ArrayList lstOutputs = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				
				// Create Expression Builder instance
				IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Output );
				// Build XPath Expression
				string strXPathExpr = DAMLServiceProfile.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.PARAM_DESC );
							
				XmlNodeList nodeList = root.SelectNodes( strXPathExpr, m_mgr );

				foreach( XmlNode descNode in nodeList )
				{
					IOType output = GetIONodeData( descNode );
									
					if( output.isValid )
						lstOutputs.Add( output );
				}
			
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return (IOType[]) lstOutputs.ToArray( typeof(IOType) );
		}

		private IOType[] GetInputParameters()
		{
			ArrayList lstInputs = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				
				// Create Expression Builder instance
				IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Input );
				// Build XPath Expression
				string strXPathExpr = DAMLServiceProfile.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.PARAM_DESC );
							
				XmlNodeList nodeList = root.SelectNodes( strXPathExpr, m_mgr );

				foreach( XmlNode descNode in nodeList )
				{
					IOType input = GetIONodeData( descNode );
					
					if( input.isValid )
						lstInputs.Add( input );
				}
			
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return (IOType[]) lstInputs.ToArray( typeof(IOType) );
		}
		
		private IOType GetIONodeData( XmlNode descNode )
		{
			IOType ioData = new IOType();
									
			XmlAttributeCollection attColl = descNode.Attributes;
					
			// Get ParamDesc
			foreach( XmlAttribute att in attColl )
			{
				if( att.Name == DAMLServiceProfile.RDF_ID )
				{
					ioData.ParameterDesc = att.Value;
					break;
				}
			}
			// Get Param name
			XmlNode nameNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_PARAM_NAME, m_mgr );
			ioData.ParameterName = nameNode.InnerText;
					
			// Get Param RestrictedTo
			XmlNode restrictNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_RESTRICTED_TO, m_mgr );
			attColl = restrictNode.Attributes;

			foreach( XmlAttribute att in attColl )
			{
				if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
				{
					ioData.RestrictedTo = att.Value;
					break;
				}
			}
					
			// Get Param RefersTo
			XmlNode referNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_REFERS_TO, m_mgr );
			attColl = referNode.Attributes;

			foreach( XmlAttribute att in attColl )
			{
				if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
				{
					ioData.RefersTo = att.Value;
					break;
				}
			}

			return ioData;
		}

		private string GetProcessModel()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_HAS_PROCESS, m_mgr );
				
				if( node == null )
					return "";

				XmlAttributeCollection attColl = node.Attributes;

				foreach( XmlAttribute att in attColl )
				{
					if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
					{
						strRetVal = att.Value;
						return strRetVal;
					}
				}
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		private string GetQualityRating()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_QUALITY_RATING, m_mgr );
				
				if( node == null )
					return "";

				XmlAttributeCollection attColl = node.Attributes;

				foreach( XmlAttribute att in attColl )
				{
					if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
					{
						strRetVal = att.Value;
						return strRetVal;
					}
				}
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}
		
		private string GetGeographicRadius()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_GEOG_RADIUS, m_mgr );
				
				if( node == null )
					return "";

				XmlAttributeCollection attColl = node.Attributes;

				foreach( XmlAttribute att in attColl )
				{
					if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
					{
						strRetVal = att.Value;
						return strRetVal;
					}
				}
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		private string GetWebUrl()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER + "/" + DAMLServiceProfile.PROFILE_WEB_URL, m_mgr );
				
				if( node == null )
					return "";
				
				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		private string GetPhysicalAddress()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER + "/" + DAMLServiceProfile.PROFILE_PHYSICAL_ADDRESS, m_mgr );
				
				if( node == null )
					return "";
				
				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		private string GetProfileEmail()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER + "/" + DAMLServiceProfile.PROFILE_EMAIL, m_mgr );
				
				if( node == null )
					return "";
				
				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		private string GetProfileFax()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER + "/" + DAMLServiceProfile.PROFILE_FAX, m_mgr );
				
				if( node == null )
					return "";
				
				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}
		
		private string GetProfilePhone()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER + "/" + DAMLServiceProfile.PROFILE_PHONE, m_mgr );
				
				if( node == null )
					return "";
				
				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}
	
		private string GetProfileName()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER + "/" + DAMLServiceProfile.PROFILE_NAME, m_mgr );
				
				if( node == null )
					return "";
				
				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		private string GetServiceProvider()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER, m_mgr );
				
				if( node == null )
					return "";

				XmlAttributeCollection attColl = node.Attributes;

				foreach( XmlAttribute att in attColl )
				{
					if( att.Name == DAMLServiceProfile.RDF_ID )
					{
						strRetVal = att.Value;
						return strRetVal;
					}
				}
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			
			return strRetVal;
		}
		
		private string GetRequestedBy()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_REQUESTED_BY, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		private string GetIntendedPurpose()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_INTENDED_PURPOSE, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		private string GetTextDescription()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_TEXT_DESC, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		private string GetPresentedBy()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.SERVICE_PRESENTED_BY, m_mgr );
				
				if( node == null )
					return "";

				XmlAttributeCollection attColl = node.Attributes;
				
				foreach( XmlAttribute att in attColl )
				{
					if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
					{
						strRetVal = att.Value;
						return strRetVal;
					}
				}
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}
		
		private string GetServiceProfile()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE, m_mgr );

				if( node == null )
					return "";

				XmlAttributeCollection attColl = node.Attributes;

				foreach( XmlAttribute att in attColl )
				{
					if( att.Name == DAMLServiceProfile.RDF_ID )
					{
						strRetVal = att.Value;
						return strRetVal;
					}
				}
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}
		
		private string GetServiceName()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_SERVICE_NAME, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		private string[] GetOntologyImports()
		{
			ArrayList arrImports = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.DAML_ONTOLOGY, m_mgr );
				
				if( node == null )
					return null;

				XmlNodeList lstImports = node.SelectNodes( DAMLServiceProfile.DAML_IMPORTS, m_mgr );

				if( lstImports.Count == 0 )
					return null;

				// Go thru list of imports and get all rdf:resource attribute values
				// these are the imports
				for( int i = 0; i < lstImports.Count; i++ )
				{
					XmlAttributeCollection attColl = lstImports[i].Attributes;
					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
							arrImports.Add( att.Value );
					}
				}
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			
			return (string[]) arrImports.ToArray( typeof( System.String ) );
		}// End GetOntologyImports
	}

}
