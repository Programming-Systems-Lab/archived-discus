using System;

namespace PSL.DISCUS.DAML
{
	public enum enuIOPEType
	{
		Input, // ServiceProfile entity
		Output, // ServiceProfile entity
		Precondition, // ServiceProfile entity
		Effect, // ServiceProfile entity
		ConditionalOutput, // ProcessModel entity
		CoCondition, // ProcessModel entity
		CoOutput, // ProcessModel entity
		Parameter // ProcessModel entity
	};

	public class RDFProperty
	{
		private string m_strName = "";
		private string m_strSubPropertyOf = "";
		private string m_strDomain = "";
		private string m_strRange = "";
		private string m_strSameValueAs = "";

		public RDFProperty()
		{}

		public string Name
		{
			get
			{ return m_strName; }
			set
			{ m_strName = value; }
		}
		public string SubPropertyOf
		{
			get
			{ return m_strSubPropertyOf; }
			set
			{ m_strSubPropertyOf = value; }
		}

		public string Domain
		{
			get
			{ return m_strDomain; }
			set
			{ m_strDomain = value; }
		}

		public string Range
		{
			get
			{ return m_strRange; }
			set
			{ m_strRange = value; }
		}

		public string SameValueAs
		{
			get
			{ return m_strSameValueAs; }
			set
			{ m_strSameValueAs = value; }
		}
	}

	public abstract class DAMLConstants
	{
		//*********************************************************************//

		// Namespace constants usually present in DAML docs

		//*********************************************************************//
		
		public const string XMLNS = "xmlns";
		// Known namespaces expected in Profile
		public const string RDFS_NS = "rdfs";
		public const string RDF_NS = "rdf";
		public const string DAML_NS = "daml";
		public const string SERVICE_NS = "service";
		public const string PROCESS_NS = "process";
		public const string PROFILE_NS = "profile";
		public const string TIME_NS = "time";
		public const string XSD_NS = "xsd";
		public const string DEFAULT_NS = "DEFAULT_NS";
		// Known elements expected in Profile
		// DAML Constants (nodes)
		public const string DAML_ONTOLOGY = DAML_NS + ":Ontology";
		public const string DAML_VERSIONINFO = DAML_NS + ":versionInfo";
		public const string DAML_IMPORTS = DAML_NS + ":imports";
		public const string DAML_SAMEVALUESAS = DAML_NS + ":sameValuesAs";
		public const string DAML_RESTRICTION = DAML_NS + ":Restriction";
		public const string DAML_ON_PROPERTY = DAML_NS + ":onProperty";
		public const string DAML_CARDINALITY = DAML_NS + ":cardinality";
		public const string DAML_COLLECTION = DAML_NS + ":collection";
		public const string DAML_INTERSECTION_OF = DAML_NS + ":intersectionOf";
		public const string DAML_LIST_OF_INSTANCES_OF = DAML_NS + ":listOfInstancesOf";
		// RDF Constants (attributes)
		public const string RDF_RESOURCE = RDF_NS + ":resource";
		public const string RDF_COMMENT = RDF_NS + ":comment";
		public const string RDF_ID = RDF_NS + ":ID";
		public const string RDF_PROPERTY = RDF_NS + ":Property";
		public const string RDF_PARSE_TYPE = RDF_NS + ":parseType";
		public const string RDF_ABOUT = RDF_NS + ":about";
		// RDFS Constants (attributes)
		public const string RDFS_SUBCLASSOF = RDFS_NS + ":subClassOf";
		public const string RDFS_SUBPROPERTYOF = RDFS_NS + ":subPropertyOf";
		public const string RDFS_DOMAIN = RDFS_NS + ":domain";
		public const string RDFS_RANGE = RDFS_NS + ":range";

		//*********************************************************************//

		// DAML-S Service Profile constants

		//*********************************************************************//

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
		// Constants related to Processes
		public const string PROCESS_SEQUENCE = PROCESS_NS + ":Sequence";
		public const string PROCESS_CHOICE = PROCESS_NS + ":Choice";
		// IOPE Constants - Inputs, Outputs, PreConditions, Effects
		public const string INPUT = "input";
		public const string OUTPUT = "output";
		public const string PRECONDITION = "precondition";
		public const string EFFECT = "effect";
		// Additional IOPE Constants
		public const string CONDITIONAL_OUTPUT = "conditionalOutput";
		public const string CO_CONDITION = "coCondition";
		public const string CO_OUTPUT = "coOutput";
		public const string PARAMETER = "parameter";
		// Misc constants used in ProcessModels
		public const string PROCESS_COMPONENTS = "components";
		public const string PROCESS_COMPOSED_OF = "composedOf";

		//*********************************************************************//

		// DAML-S Process Model constants

		//*********************************************************************//
		
		public const string DAML_CLASS = DAML_NS + ":Class";
		public const string DAML_SIMPLE_PROCESS = "SimpleProcess";
		public const string DAML_COMPOSITE_PROCESS = "CompositeProcess";
		public const string DAML_ATOMIC_PROCESS = "AtomicProcess";
		
		public DAMLConstants()
		{}
	}
}
