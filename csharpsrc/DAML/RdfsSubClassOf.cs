using System;
using System.Collections;
using System.Xml;

namespace PSL.DAML
{
	/// <summary>
	/// RdfsSubClassOf is a specialized version of a DamlClass
	/// </summary>
	public class RdfsSubClassOf:DamlClass
	{
		/// <summary>
		/// Ctor.
		/// </summary>
		public RdfsSubClassOf()
		{
			this.m_damlClassType = enuDamlClassType.rdfsSubClassOf;
		}

		/// <summary>
		/// Property gets/sets the value of the instance
		/// </summary>
		public override string Value
		{
			get
			{ return this.m_strValue; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strValue = value;
			}
		}

		/// <summary>
		/// Procedure resets the instance
		/// </summary>
		public override void Clear()
		{
			this.m_strName = "";
			this.m_strValue = "";
		}

		/// <summary>
		/// Procedure Not Supported on this class
		/// </summary>
		/// <param name="strOption"></param>
		/// <exception cref="NotSupportedException"></exception>"
		public override void AddOption( string strOption )
		{
			throw new NotSupportedException( "Adding Options not supported by RdfSubClassOf class" );
		}

		/// <summary>
		/// Procedure Not Supported on this class
		/// </summary>
		/// <param name="strOption"></param>
		/// <exception cref="NotSupportedException"></exception>"
		public override void RemoveOption( string strOption )
		{
			throw new NotSupportedException( "Removing Options not supported by RdfSubClassOf class" );
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

			// Create rdfsSubClassOf node
			XmlNode damlTypeNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.RDFS_SUBCLASSOF, DamlConstants.RDFS_NS_URI );
			// Create attribute to set the value
			XmlAttribute damlTypeNodeAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
			// Set the value
			damlTypeNodeAtt.Value = this.Value;
			// Append attribute to node
			damlTypeNode.Attributes.Append( damlTypeNodeAtt );
			
			damlClassNode.AppendChild( damlTypeNode );
			root.AppendChild( damlClassNode );
			// Return xml document
			return doc.OuterXml;
		}
	}
}
