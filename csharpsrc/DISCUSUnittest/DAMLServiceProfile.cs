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
		private Hashtable m_nsMapping;

		public DAMLServiceProfile()
		{
			m_EvtLog = new EventLog( "Application" );
			m_EvtLog.Source = "DAMLServiceProfile";
			m_nsMapping = new Hashtable();
			m_doc = new XmlDocument();

			

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
				// Extract only those attributes representing namespaces
				// i.e foreach attribute store xmlns:xxx
				m_nsMapping.Clear();
				
				for( int i = 0; i < attColl.Count; i++ )
				{
					// Extract all namespaces we can find in document root					
					if( attColl[i].Prefix == XMLNS )
						m_nsMapping.Add( attColl[i].LocalName, attColl[i].InnerText );
					// Add default namespace (if any)
					if( attColl[i].Prefix == "" )
						m_nsMapping.Add( DEFAULT_NS, attColl[i].InnerText );
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
		
		private EPType[] GetEffects()
		{
			ArrayList lstEffects = new ArrayList();

			try
			{
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				// Add Default namespace
				mgr.AddNamespace( DEFAULT_NS, (string) m_nsMapping[DEFAULT_NS] );
				// Add Profile namespace
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );

				XmlNode root = m_doc.DocumentElement;
				
				XmlNodeList nodeList = root.SelectNodes( DAMLServiceProfile.SERVICE_PROFILE + "/" +  DEFAULT_NS + ":" + DAMLServiceProfile.EFFECT , mgr );

				foreach( XmlNode node in nodeList )
				{
					EPType effect = new EPType();
					
					XmlNode descNode = node.SelectSingleNode( DAMLServiceProfile.PROFILE_CONDITION_DESC, mgr );
									
					XmlAttributeCollection attColl = descNode.Attributes;
					
					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_ID )
						{
							effect.ConditionDesc = att.Value;
							break;
						}
					}
					
					XmlNode nameNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_CONDITION_NAME, mgr );
					effect.ConditionName = nameNode.InnerText;
					
					XmlNode stmntNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_STATEMENT, mgr );
					attColl = stmntNode.Attributes;

					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
						{
							effect.Statement = att.Value;
							break;
						}
					}
					
					XmlNode referNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_REFERS_TO, mgr );
					attColl = referNode.Attributes;

					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
						{
							effect.RefersTo = att.Value;
							break;
						}
					}

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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				// Add Default namespace
				mgr.AddNamespace( DEFAULT_NS, (string) m_nsMapping[DEFAULT_NS] );
				// Add Profile namespace
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );

				XmlNode root = m_doc.DocumentElement;
				
				XmlNodeList nodeList = root.SelectNodes( DAMLServiceProfile.SERVICE_PROFILE + "/" +  DEFAULT_NS + ":" + DAMLServiceProfile.PRECONDITION , mgr );

				foreach( XmlNode node in nodeList )
				{
					EPType precond = new EPType();
					
					XmlNode descNode = node.SelectSingleNode( DAMLServiceProfile.PROFILE_CONDITION_DESC, mgr );
									
					XmlAttributeCollection attColl = descNode.Attributes;
					
					// Get ParamDesc
					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_ID )
						{
							precond.ConditionDesc = att.Value;
							break;
						}
					}
					
					XmlNode nameNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_CONDITION_NAME, mgr );
					precond.ConditionName = nameNode.InnerText;
					
					XmlNode stmntNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_STATEMENT, mgr );
					attColl = stmntNode.Attributes;

					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
						{
							precond.Statement = att.Value;
							break;
						}
					}
					
					XmlNode referNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_REFERS_TO, mgr );
					attColl = referNode.Attributes;

					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
						{
							precond.RefersTo = att.Value;
							break;
						}
					}

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
		
		private IOType[] GetOutputParameters()
		{
			ArrayList lstOutputs = new ArrayList();

			try
			{
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				// Add Default namespace
				mgr.AddNamespace( DEFAULT_NS, (string) m_nsMapping[DEFAULT_NS] );
				// Add Profile namespace
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );

				XmlNode root = m_doc.DocumentElement;
				
				// Get outputs
				XmlNodeList nodeList = root.SelectNodes( DAMLServiceProfile.SERVICE_PROFILE + "/" +  DEFAULT_NS + ":" + DAMLServiceProfile.OUTPUT , mgr );

				foreach( XmlNode node in nodeList )
				{
					IOType output = new IOType();
					// Get ParameterDescription child node
					XmlNode descNode = node.SelectSingleNode( DAMLServiceProfile.PROFILE_PARAM_DESC, mgr );
									
					XmlAttributeCollection attColl = descNode.Attributes;
					
					// Get ParamDesc
					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_ID )
						{
							output.ParameterDesc = att.Value;
							break;
						}
					}
					// Get Param name
					XmlNode nameNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_PARAM_NAME, mgr );
					output.ParameterName = nameNode.InnerText;
					
					// Get Param RestrictedTo
					XmlNode restrictNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_RESTRICTED_TO, mgr );
					attColl = restrictNode.Attributes;

					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
						{
							output.RestrictedTo = att.Value;
							break;
						}
					}
					
					// Get Param RefersTo
					XmlNode referNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_REFERS_TO, mgr );
					attColl = referNode.Attributes;

					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
						{
							output.RefersTo = att.Value;
							break;
						}
					}

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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				// Add Default namespace
				mgr.AddNamespace( DEFAULT_NS, (string) m_nsMapping[DEFAULT_NS] );
				// Add Profile namespace
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );

				XmlNode root = m_doc.DocumentElement;
				
				// Get inputs
				XmlNodeList nodeList = root.SelectNodes( DAMLServiceProfile.SERVICE_PROFILE + "/" +  DEFAULT_NS + ":" + DAMLServiceProfile.INPUT , mgr );

				foreach( XmlNode node in nodeList )
				{
					IOType input = new IOType();
					// Get ParameterDescription child node
					XmlNode descNode = node.SelectSingleNode( DAMLServiceProfile.PROFILE_PARAM_DESC, mgr );
									
					XmlAttributeCollection attColl = descNode.Attributes;
					
					// Get ParamDesc
					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_ID )
						{
							input.ParameterDesc = att.Value;
							break;
						}
					}
					// Get Param name
					XmlNode nameNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_PARAM_NAME, mgr );
					input.ParameterName = nameNode.InnerText;
					
					// Get Param RestrictedTo
					XmlNode restrictNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_RESTRICTED_TO, mgr );
					attColl = restrictNode.Attributes;

					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
						{
							input.RestrictedTo = att.Value;
							break;
						}
					}
					
					// Get Param RefersTo
					XmlNode referNode = descNode.SelectSingleNode( DAMLServiceProfile.PROFILE_REFERS_TO, mgr );
					attColl = referNode.Attributes;

					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLServiceProfile.RDF_RESOURCE )
						{
							input.RefersTo = att.Value;
							break;
						}
					}

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
		
		private string GetProcessModel()
		{
			string strRetVal = "";

			try
			{
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get ServiceName
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_HAS_PROCESS, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get ServiceName
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_QUALITY_RATING, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get ServiceName
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_GEOG_RADIUS, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER + "/" + DAMLServiceProfile.PROFILE_WEB_URL, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get RequestedBy
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER + "/" + DAMLServiceProfile.PROFILE_PHYSICAL_ADDRESS, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get RequestedBy
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER + "/" + DAMLServiceProfile.PROFILE_EMAIL, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get RequestedBy
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER + "/" + DAMLServiceProfile.PROFILE_FAX, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get RequestedBy
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER + "/" + DAMLServiceProfile.PROFILE_PHONE, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get RequestedBy
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER + "/" + DAMLServiceProfile.PROFILE_NAME, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get RequestedBy
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_PROVIDED_BY + "/" + DAMLServiceProfile.PROFILE_SERVICE_PROVIDER, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get RequestedBy
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_REQUESTED_BY, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get Intended Purpose
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_INTENDED_PURPOSE, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get Text Description
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_TEXT_DESC, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get PresentedBy
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.SERVICE_PRESENTED_BY, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Do XPath query using namespace manager, get ServiceProfile node											
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE, mgr );

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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add PROFILE namespace to our namespace manager
				mgr.AddNamespace( PROFILE_NS, (string) m_nsMapping[PROFILE_NS] );
				// Add SERVICE namespace to our namespace manager
				mgr.AddNamespace( SERVICE_NS, (string) m_nsMapping[SERVICE_NS] );
				XmlNode root = m_doc.DocumentElement;
				// Get ServiceName
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.SERVICE_PROFILE + "/" + DAMLServiceProfile.PROFILE_SERVICE_NAME, mgr );
				
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
				XmlNamespaceManager mgr = new XmlNamespaceManager( m_doc.NameTable );
				// Add DAML namespace to our namespace manager
				mgr.AddNamespace( DAML_NS, (string) m_nsMapping[DAML_NS] );	
				XmlNode root = m_doc.DocumentElement;
				// Do XPath query using namespace manager											
				XmlNode node = root.SelectSingleNode( DAMLServiceProfile.DAML_ONTOLOGY, mgr );
				
				if( node == null )
					return null;

				XmlNodeList lstImports = node.SelectNodes( DAMLServiceProfile.DAML_IMPORTS, mgr );

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
