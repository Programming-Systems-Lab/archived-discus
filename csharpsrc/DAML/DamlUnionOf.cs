using System;
using System.Xml;
using System.Collections;

namespace PSL.DAML
{
	/// <summary>
	/// DamlUnionOf is a specialized version of a DamlClass
	/// </summary>
	public class DamlUnionOf:DamlClass
	{
		/// <summary>
		/// Ctor.
		/// </summary>
		public DamlUnionOf()
		{
			this.m_damlClassType = enuDamlClassType.damlUnionOf;
		}

		/// <summary>
		/// Function converts an instance to and xml string
		/// </summary>
		/// <returns>The xml string representing the instance</returns>
		public override string ToXml()
		{
			XmlDocument doc = DamlContainer.BuildDamlDocumentTemplate( false );
			XmlNode root = doc.DocumentElement;

			// Create the document root
			XmlNode damlClassNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_CLASS, DamlConstants.DAML_NS_URI );
			// Create an attribute to hold the name of the damlClass
			XmlAttribute damlClassAtt = doc.CreateAttribute( DamlConstants.RDF_ID, DamlConstants.RDF_NS_URI );
			// Set attribute value
			damlClassAtt.Value = this.m_strName;
			// Append attribute to damlClass node
			damlClassNode.Attributes.Append( damlClassAtt );

			// Create daml:UnionOf node
			XmlNode damlTypeNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_UNION_OF, DamlConstants.DAML_NS_URI );
			// Create attribute to set the parseType
			XmlAttribute damlTypeNodeAtt = doc.CreateAttribute( DamlConstants.RDF_PARSE_TYPE, DamlConstants.RDF_NS_URI );
			// Set attribute value
			switch( this.m_parseType )
			{
				case enuRdfParseType.damlCollection:
					damlTypeNodeAtt.Value = DamlConstants.DAML_COLLECTION;
					break;

				default: throw new Exception( "Unknown rdfParseType" );
			}
			// Append attribute to node
			damlTypeNode.Attributes.Append( damlTypeNodeAtt );

			// Scroll thru the options creating a daml:Class rdf:about node
			// adding each one to the damlTypeNode
			IDictionaryEnumerator it = this.m_options.GetEnumerator();

			while( it.MoveNext() )
			{
				// Create node
				XmlNode rdfAboutNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_CLASS, DamlConstants.DAML_NS_URI );
				// Create attribute
				XmlAttribute rdfAboutAtt = doc.CreateAttribute( DamlConstants.RDF_ABOUT, DamlConstants.RDF_NS_URI );
				// Set attribute value
				rdfAboutAtt.Value = (string) it.Key;
				// Add attribute to node
				rdfAboutNode.Attributes.Append( rdfAboutAtt );

				// Add node to the damlTypeNode
				damlTypeNode.AppendChild( rdfAboutNode );

			}

			damlClassNode.AppendChild( damlTypeNode );
			root.AppendChild( damlClassNode );
			// Return xml string
			return doc.OuterXml;
		}	
	}
}
