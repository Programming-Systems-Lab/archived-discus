using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PSL.DAML
{
	/// <summary>
	/// Abstract base class for all DAML Document containers e.g DAMLProcessModelReader 
	/// and DAMLServiceProfile
	/// </summary>
	public abstract class DamlContainer
	{
		// Member variables
		protected XmlDocument m_doc = null;
		protected XmlNamespaceManager m_mgr = null;
				
		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="strDaml">Daml string to load the DamlContainer with</param>
		public DamlContainer( string strDaml )
		{
			// Load the daml string
			LoadXml( strDaml );

			// If the documentElement is null, there was some error loading the document
			// so throw an exception
			if( m_doc.DocumentElement == null )
				throw new Exception( "Invalid document. Cannot get document root." );
		}

		/// <summary>
		/// Method builds a basic Daml Root Tag string
		/// </summary>
		/// <returns>The basic Daml root tag</returns>
		public static string BuildDamlDocRoot()
		{
			StringBuilder strXmlBuilder = new StringBuilder();

			// Build root tag
			strXmlBuilder.Append( "<" + DamlConstants.RDF_ROOT );
			// Add namespace attributes
			strXmlBuilder.Append( " " + DamlConstants.XMLNS + ":" + DamlConstants.RDF_NS + "=" + "\"" +  DamlConstants.RDF_NS_URI + "\"" );
			strXmlBuilder.Append( " " + DamlConstants.XMLNS + ":" + DamlConstants.RDFS_NS + "=" + "\"" +  DamlConstants.RDFS_NS_URI + "\"" );
			strXmlBuilder.Append( " " + DamlConstants.XMLNS + ":" + DamlConstants.XSD_NS + "=" + "\"" +  DamlConstants.XSD_NS_URI + "\"" );
			strXmlBuilder.Append( " " + DamlConstants.XMLNS + ":" + DamlConstants.DAML_NS + "=" + "\"" +  DamlConstants.DAML_NS_URI + "\"" );
			strXmlBuilder.Append( " " + DamlConstants.XMLNS + ":" + DamlConstants.DEX_NS + "=" + "\"" +  DamlConstants.DEX_NS_URI + "\"" );
			strXmlBuilder.Append( " " + DamlConstants.XMLNS + ":" + DamlConstants.EXD_NS + "=" + "\"" +  DamlConstants.EXD_NS_URI + "\"" );
			strXmlBuilder.Append( " " + DamlConstants.XMLNS + ":" + DamlConstants.SERVICE_NS + "=" + "\"" +  DamlConstants.SERVICE_NS_URI + "\"" );
			strXmlBuilder.Append( " " + DamlConstants.XMLNS + ":" + DamlConstants.PROCESS_NS + "=" + "\"" +  DamlConstants.PROCESS_NS_URI + "\"" );
			strXmlBuilder.Append( " " + DamlConstants.XMLNS + ":" + DamlConstants.TIME_NS + "=" + "\"" +  DamlConstants.TIME_NS_URI + "\"" );
			strXmlBuilder.Append( ">\n" );
			// Close root tag
			strXmlBuilder.Append( "</" + DamlConstants.RDF_ROOT + ">" );
			
			// Return the root tag
			return strXmlBuilder.ToString();
		}
		
		/// <summary>
		/// Function builds a basic Daml Document Template
		/// </summary>
		/// <param name="bAddOntologyImports">Indicates whether to include the ontology imports</param>
		/// <returns>The basoc Daml Document Template</returns>
		public static XmlDocument BuildDamlDocumentTemplate( bool bAddOntologyImports )
		{
			// Build the basic root tag
			string strDamlDocRoot = DamlContainer.BuildDamlDocRoot();

			// Create an XmlDocument and set the Document Element
			XmlDocument doc = new XmlDocument();

			// Load Xml into XmlDocument
			doc.LoadXml( strDamlDocRoot );
			// Get Document Element, this is our document root node
			XmlNode root = doc.DocumentElement;
			
			// If asked to add ontology imports...
			if( bAddOntologyImports )
			{
				// Create a new node for the ontologies 
				XmlNode ontologyNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_ONTOLOGY, DamlConstants.DAML_NS_URI );
			
				// Add attribute to Ontology node
				XmlAttribute ontologyAttribute = doc.CreateAttribute( DamlConstants.RDF_ABOUT, DamlConstants.RDF_NS_URI );
				// Add ontology attribute
				ontologyNode.Attributes.Append( ontologyAttribute );
				
				// Add daml imports to ontology node

				// Import daml+oil
				XmlNode damlImport = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_IMPORTS, DamlConstants.DAML_NS_URI );
				XmlAttribute damlImportAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
				damlImportAtt.Value = DamlConstants.DAML_NS_URI;
				damlImport.Attributes.Append( damlImportAtt );
				
				// Import time
				XmlNode timeImport = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_IMPORTS, DamlConstants.DAML_NS_URI );
				XmlAttribute timeImportAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
				timeImportAtt.Value = DamlConstants.TIME_NS_URI;
				timeImport.Attributes.Append( timeImportAtt );

				// Import Service
				XmlNode serviceImport = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_IMPORTS, DamlConstants.DAML_NS_URI );
				XmlAttribute serviceImportAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
				serviceImportAtt.Value = DamlConstants.SERVICE_NS_URI;
				serviceImport.Attributes.Append( serviceImportAtt );

				// Import Process
				XmlNode processImport = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_IMPORTS, DamlConstants.DAML_NS_URI );
				XmlAttribute processImportAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
				processImportAtt.Value = DamlConstants.PROCESS_NS_URI;
				processImport.Attributes.Append( processImportAtt );

				// Add Imports to ontology node
				ontologyNode.AppendChild( damlImport );
				ontologyNode.AppendChild( timeImport );
				ontologyNode.AppendChild( serviceImport );
				ontologyNode.AppendChild( processImport );

				// Add the ontology node to our document
				root.AppendChild( ontologyNode );
			}

			// Return the basic daml document template creates
			return doc;
		}
		
		/// <summary>
		/// Function loads an Xml Document from a (possibly large) string 
		/// </summary>
		/// <param name="strXml">String containing document text</param>
		/// <returns>true on successful load
		/// false otherwise</returns>
		public virtual bool LoadXml( string strXml )
		{
			// Quick check for valid input
			if( strXml == null || strXml.Length == 0 )
				throw new ArgumentException( "Parameter is null or empty string", strXml );
			
			bool bStatus = false;

			m_doc.LoadXml( strXml );
			// Move to root element
			XmlNode root = m_doc.DocumentElement;
			// Get attributes of root element
			XmlAttributeCollection attColl = root.Attributes;
			
			// Reset namespace manager, since we will be reloading the namespaces
			if( m_mgr != null )
				m_mgr = null;
			
			m_mgr = new XmlNamespaceManager( m_doc.NameTable );
			
			// Extract all namespaces we can find in document root
			// and add to namespace manager
			for( int i = 0; i < attColl.Count; i++ )
			{
				string strValue = attColl[i].InnerText;

				if( attColl[i].Prefix == DamlConstants.XMLNS )
					m_mgr.AddNamespace( attColl[i].LocalName, strValue );
										
				// Add default namespace (if any) and add to namespace manager
				if( attColl[i].Prefix == "" )
					m_mgr.AddNamespace( DamlConstants.DEFAULT_NS, strValue );
			}
			
			bStatus = true;
		
			// Return the operation status
			return bStatus;
		}

		/// <summary>
		/// Function returns an array of all the ontology imports defined in a 
		/// DAML document. 
		/// </summary>
		/// <returns>A string array of Ontology imports 
		/// (possibly empty if no ontologies have been imported)</returns>
		protected virtual string[] GetOntologyImports()
		{
			// Create array list to store imports
			ArrayList lstImports = new ArrayList();
			// Get document element
			XmlNode root = m_doc.DocumentElement;
			// Get the ontology node
			XmlNode node = root.SelectSingleNode( DamlConstants.DAML_ONTOLOGY, m_mgr );
			
			// If none exists return an empty list
			if( node == null )
				return (string[]) lstImports.ToArray( typeof( System.String ) );
			
			// Get the import nodes
			XmlNodeList lstImportNodes = node.SelectNodes( DamlConstants.DAML_IMPORTS, m_mgr );
			// If none exist return an empty list
			if( lstImportNodes.Count == 0 )
				return (string[]) lstImports.ToArray( typeof( System.String ) );

			// Go thru list of imports and get all rdf:resource attribute values
			// these are the imports
			for( int i = 0; i < lstImportNodes.Count; i++ )
			{
				XmlAttributeCollection attColl = lstImportNodes[i].Attributes;
				foreach( XmlAttribute att in attColl )
				{
					if( att.Name == DamlConstants.RDF_RESOURCE )
						lstImports.Add( att.Value );
				}
			}
			
			// Return list of imports found
			return (string[]) lstImports.ToArray( typeof( System.String ) );
		}// End GetOntologyImports
	}
}
