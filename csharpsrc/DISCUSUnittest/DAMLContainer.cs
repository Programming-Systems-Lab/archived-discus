using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PSL.DISCUS.DAML
{
	/// <summary>
	/// Summary description for DAMLContainer.
	/// </summary>
	public abstract class DAMLContainer
	{
		// Member variables
		protected XmlDocument m_doc;
		protected XmlNamespaceManager m_mgr;
		protected EventLog m_EvtLog;
		
		public DAMLContainer()
		{}


		// Need virtual methods to load from:
		// file/unc path
		// url/uri
		// stream
		// xml string - already implemented

		public virtual bool LoadXml( string strXml )
		{
			bool bStatus = false;

			try
			{
				m_doc.LoadXml( strXml );
				// Move to root element
				XmlNode root = m_doc.DocumentElement;
				// Get attributes of root element
				XmlAttributeCollection attColl = root.Attributes;
				// TODO: Should use PopScope instead??
				if( m_mgr != null )
				{
					//m_mgr.PopScope();
					m_mgr = null;
				}

				m_mgr = new XmlNamespaceManager( m_doc.NameTable );
				//m_mgr.PushScope();

				for( int i = 0; i < attColl.Count; i++ )
				{
					// Extract all namespaces we can find in document root
					// and add to namespace manager
					
					string strValue = attColl[i].InnerText;

					if( attColl[i].Prefix == DAMLConstants.XMLNS )
						m_mgr.AddNamespace( attColl[i].LocalName, strValue );
										 
					// Add default namespace (if any) and add to namespace manager
					if( attColl[i].Prefix == "" )
						m_mgr.AddNamespace( DAMLConstants.DEFAULT_NS, strValue );
				}
				
				bStatus = true;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );	
			}

			return bStatus;
		}

		protected virtual string[] GetOntologyImports()
		{
			ArrayList arrImports = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.DAML_ONTOLOGY, m_mgr );
				
				if( node == null )
					return null;

				XmlNodeList lstImports = node.SelectNodes( DAMLConstants.DAML_IMPORTS, m_mgr );

				if( lstImports.Count == 0 )
					return null;

				// Go thru list of imports and get all rdf:resource attribute values
				// these are the imports
				for( int i = 0; i < lstImports.Count; i++ )
				{
					XmlAttributeCollection attColl = lstImports[i].Attributes;
					foreach( XmlAttribute att in attColl )
					{
						if( att.Name == DAMLConstants.RDF_RESOURCE )
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
