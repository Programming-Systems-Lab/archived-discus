using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

namespace PSL.DAML
{
	/// <summary>
	/// Enumeration of the various attributes of a process 
	/// </summary>
	public enum enuIOPEType
	{
		None,
		Input, // ServiceProfile entity
		Output, // ServiceProfile entity
		Precondition, // ServiceProfile entity
		Effect, // ServiceProfile entity
		ConditionalOutput, // ProcessModel entity
		CoCondition, // ProcessModel entity
		CoOutput, // ProcessModel entity
		Parameter // ProcessModel entity
	}

	/// <summary>
	/// Enumeration of the various Daml Type Definitions
	/// </summary>
	public enum enuDamlType 
	{
		rdfProperty,
		damlClass
	}

	/// <summary>
	/// Enumeration of the various DamlClass types
	/// </summary>
	public enum enuDamlClassType
	{
		rdfsSubClassOf,
		damlUnionOf,
		damlOneOf
	}

	/// <summary>
	/// Enumeration of the various ParseTypes
	/// </summary>
	public enum enuRdfParseType
	{
		damlCollection
	}

	/// <summary>
	/// Enumeration of the various types of searches used by the ServiceProfile
	/// </summary>
	public enum enuIOPESearchBy
	{
		PARAM_DESC,
		PARAM_NAME,
		COND_DESC,
		COND_NAME,
		REFERS_TO
	}
		
	/// <summary>
	/// Enumeration of the various composite process sub task types/control structures
	/// </summary>
 	public enum enuProcessSubTaskType
	{
		Sequence, // process:Sequence attribute
		Choice	// process:Choice attribute
	}

	/// <summary>
	/// Enumeration of the various kinds of Daml Processes
	/// </summary>
	public enum enuProcessType
	{
		AtomicProcess,
		CompositeProcess,
		SimpleProcess
	}

	/// <summary>
	/// Abstract class of constant definitions
	/// </summary>
	public abstract class DamlConstants
	{
		//*********************************************************************//

		// Namespace Uri constants usually present in DAML docs

		//*********************************************************************//
		public const string PROCESS_BASE_URI			= "http://www.daml.org/services/daml-s/2001/10/Process.daml";

		public const string PROCESS_CONDITION_URI			= PROCESS_BASE_URI + "#" + "Condition";
		public const string PROCESS_PRECONDITION_URI		= PROCESS_BASE_URI + "#" + "precondition";
		public const string PROCESS_EFFECT_URI				= PROCESS_BASE_URI + "#" + "effect";
		public const string PROCESS_INPUT_URI				= PROCESS_BASE_URI + "#" + "input";
		public const string PROCESS_OUTPUT_URI				= PROCESS_BASE_URI + "#" + "output";
		public const string PROCESS_CO_CONDITION_URI		= PROCESS_BASE_URI + "#" + "coCondition";
		public const string PROCESS_CO_OUTPUT_URI			= PROCESS_BASE_URI + "#" + "coOutput";
		public const string PROCESS_CONDITIONAL_OUTPUT_URI	= PROCESS_BASE_URI + "#" + "conditionalOutput";
		public const string PROCESS_PARAMETER_URI			= PROCESS_BASE_URI + "#" + "parameter";
		public const string PROCESS_COMPOSED_OF_URI			= PROCESS_BASE_URI + "#" + "composedOf";
		public const string PROCESS_COMPONENTS_URI			= PROCESS_BASE_URI + "#" + "components";

		// Process types
		public const string SIMPLE_PROCESS_URI				= PROCESS_BASE_URI + "#" + "SimpleProcess";
		public const string ATOMIC_PROCESS_URI				= PROCESS_BASE_URI + "#" + "AtomicProcess";
		public const string COMPOSITE_PROCESS_URI			= PROCESS_BASE_URI + "#" + "CompositeProcess";
		
		public const string RDF_ROOT = RDF_NS + ":RDF";

		public const string RDF_NS_URI = "http://www/w3.org/1999/02/22-rdf-syntax-ns#";
		public const string RDFS_NS_URI = "http://www/w3.org/2000/01/rdf-schema#";
		public const string XSD_NS_URI = "http://www.w3.org/2000/10/XMLSchema#";
		public const string DAML_NS_URI = "http://www.daml.org/2001/03/daml+oil#";
		public const string DEX_NS_URI = "http://www.daml.org/2001/03/daml+oil-ex#";
		public const string EXD_NS_URI = "http://www.daml.org/2001/03/daml+oil-ex-dt#";
		public const string PROCESS_NS_URI = PROCESS_BASE_URI + "#";
		public const string TIME_NS_URI = "http://www.ai.sri.com/~daml/ontologies/sri-basic/1-0Time.daml#";
		public const string SERVICE_NS_URI = "http://daml.org/services/daml-s/2001/10/Service.daml#";
				
		//*********************************************************************//

		// Namespace constants usually present in DAML docs

		//*********************************************************************//
		
		public const string XMLNS = "xmlns";
		public const string EXD_NS = "exd";
		public const string DEX_NS = "dex";
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
		public const string DAML_HAS_VALUE = DAML_NS + ":hasValue";
		public const string DAML_ONE_OF = DAML_NS + ":oneOf";
		public const string DAML_UNION_OF = DAML_NS + ":unionOf";
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
		public const string DAML_TO_CLASS = DAML_NS + ":toClass";
		public const string DAML_SIMPLE_PROCESS = "SimpleProcess";
		public const string DAML_COMPOSITE_PROCESS = "CompositeProcess";
		public const string DAML_ATOMIC_PROCESS = "AtomicProcess";
		
		public DamlConstants()
		{}
	}
}
