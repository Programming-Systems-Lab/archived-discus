using System;
using System.Collections;
using System.Xml;

namespace PSL.DAML
{
	/// <summary>
	/// RdfProperty class
	/// </summary>
	public class RdfProperty:DamlTypeDefinition
	{
		private string m_strSubPropertyOf = "";
		private string m_strDomain = "";
		private string m_strRange = "";
		private string m_strSameValueAs = "";

		/// <summary>
		/// Ctor.
		/// </summary>
		public RdfProperty()
		{
			this.m_damlType = enuDamlType.rdfProperty;
		}

		/// <summary>
		/// Property gets/sets the SubPropertyOf value of the instance
		/// </summary>
		public string SubPropertyOf
		{
			get
			{ return m_strSubPropertyOf; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;

				m_strSubPropertyOf = value; 
			}
		}

		/// <summary>
		/// Property gets/sets the Domain value of the instance
		/// </summary>
		public string Domain
		{
			get
			{ return m_strDomain; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;
				
				m_strDomain = value; 
			}
		}

		/// <summary>
		/// Property gets/sets the Range value of the instance
		/// </summary>
		public string Range
		{
			get
			{ return m_strRange; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;
				
				m_strRange = value; 
			}
		}

		/// <summary>
		/// Property gets/sets the SameValueAs value of the instance
		/// </summary>
		public string SameValueAs
		{
			get
			{ return m_strSameValueAs; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;
				
				m_strSameValueAs = value; 
			}
		}

		/// <summary>
		/// Procedure resets the instance
		/// </summary>
		public override void Clear()
		{
			this.m_strName = "";
						
			this.m_strDomain = "";
			this.m_strRange = "";
			this.m_strSameValueAs = "";
			this.m_strSubPropertyOf = "";
		}

		/// <summary>
		/// Function converts an instance to and xml string
		/// </summary>
		/// <returns>The xml string representing the instance</returns>
		public override string ToXml()
		{
			XmlDocument doc = DamlContainer.BuildDamlDocumentTemplate( false );
			XmlNode root = doc.DocumentElement;

			// Create rdfPropertyNode
			XmlNode rdfPropertyNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.RDF_PROPERTY, DamlConstants.RDF_NS_URI );
			// Create attribute
			XmlAttribute rdfPropertyAtt = doc.CreateAttribute( DamlConstants.RDF_ID, DamlConstants.RDF_NS_URI );
			// Set attribute value
			rdfPropertyAtt.Value = this.m_strName;
			// Add attribute to node
			rdfPropertyNode.Attributes.Append( rdfPropertyAtt );

			// Create node for subPropertyOf (only if it has been set)
			if( this.m_strSubPropertyOf.Length > 0 )
			{
				XmlNode subPropertyOfNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.RDFS_SUBPROPERTYOF, DamlConstants.RDFS_NS_URI );
				// Create attribute
				XmlAttribute subPropertyOfAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
				// Set attribute value
				subPropertyOfAtt.Value = this.m_strSubPropertyOf;
				// Add attribute to node
				subPropertyOfNode.Attributes.Append( subPropertyOfAtt );

				rdfPropertyNode.AppendChild( subPropertyOfNode );
			}
			
			// Create node for domain
			XmlNode domainNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.RDFS_DOMAIN, DamlConstants.RDFS_NS_URI );
			// Create attribute
			XmlAttribute domainAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
			// Set attribute value
			domainAtt.Value = this.m_strDomain.StartsWith( "#" ) ? this.m_strDomain : "#" + this.m_strDomain;
			// Add attribute to node
			domainNode.Attributes.Append( domainAtt );

			rdfPropertyNode.AppendChild( domainNode );

			// Create node for range
			XmlNode rangeNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.RDFS_RANGE, DamlConstants.RDFS_NS_URI );
			// Create attribute
			XmlAttribute rangeAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
			// Set attribute value
			rangeAtt.Value = this.m_strRange;
			// Add attribute to node
			rangeNode.Attributes.Append( rangeAtt );

			rdfPropertyNode.AppendChild( rangeNode );

			// Create node for sameValueAs (only if it has been set)
			if( this.m_strSameValueAs.Length > 0 )
			{
				XmlNode sameValueAsNode = doc.CreateNode( XmlNodeType.Element, DamlConstants.DAML_SAMEVALUESAS, DamlConstants.DAML_NS_URI );
				// Create attribute
				XmlAttribute sameValueAsAtt = doc.CreateAttribute( DamlConstants.RDF_RESOURCE, DamlConstants.RDF_NS_URI );
				// Set attribute value
				sameValueAsAtt.Value = this.m_strSameValueAs;
				// Add attribute to node
				sameValueAsNode.Attributes.Append( sameValueAsAtt );

				rdfPropertyNode.AppendChild( sameValueAsNode );
			}

			root.AppendChild( rdfPropertyNode );
			// Return xml string
			return doc.OuterXml;
		}
	}
}
