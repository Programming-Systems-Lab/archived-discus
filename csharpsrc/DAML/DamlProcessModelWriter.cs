using System;
using System.Xml;
using System.Collections;

namespace PSL.DAML
{
	/// <summary>
	/// Summary description for DamlProcessModelWriter.
	/// </summary>
	public class DamlProcessModelWriter
	{
		private Hashtable m_processMap = new Hashtable();
		private Hashtable m_dataTypeDefinitionMap = new Hashtable();

		public DamlProcessModelWriter()
		{
		}

		public void AddDamlProcess( DamlProcess process )
		{
			if( process == null )
				throw new ArgumentNullException( "process", "DamlProcess parameter cannot be null" );

			if( process.Name == null || process.Name.Length == 0 )
				throw new ArgumentException( "DamlProcess name must be set", "process" );

			if( !this.m_processMap.ContainsKey( process.Name ) )
				this.m_processMap.Add( process.Name, process );
		}

		public void RemoveProcess( string strProcessName )
		{
			if( strProcessName == null || strProcessName.Length == 0 )
				throw new ArgumentException( "Process name cannot be null or empty string", "strProcessName" );

			this.m_processMap.Remove( strProcessName );
		}

		public string ToXml()
		{
			// Get a document template
			XmlDocument doc = DamlContainer.BuildDamlDocumentTemplate( true );

			// Get the document element (document root)
			XmlNode root = doc.DocumentElement;

			// DamlProcessWriter may have to control the datatypes
			// being written out by each DamlProcess, multiple processes in
			// a proces model may share data types, we only need one
			// data type definition
			
			IDictionaryEnumerator it = this.m_processMap.GetEnumerator();

			while( it.MoveNext() )
			{
				DamlProcess proc = (DamlProcess) it.Value;

				XmlDocument processDoc = new XmlDocument();
				// DO NOT add type defintions to xml document we create
				// the ProcessModel should keep track of all the data types
				// in each process to ensure each type definition gets written
				// only once since multiple process in a process model may share
				// data types an as such could cause duplicate definitions 
				// to be written out in the process model xml document
				processDoc.LoadXml( proc.ToXml( false ) );
				
				XmlNode processDocRoot = processDoc.DocumentElement;

				XmlNodeList lstChildren = processDocRoot.ChildNodes;

				foreach( XmlNode child in lstChildren )
				{
					// Add every child to our document root except the ontology imports
					// since we already have these from the DamlDocumentTemplate
					if( child.Name != DamlConstants.DAML_ONTOLOGY )
					{
						XmlNode temp = doc.ImportNode( child, true );
						root.AppendChild( temp );
					}
				}

				// Get the data types we have not seen as yet
				DamlTypeDefinition[] arrDefinitions = proc.TypeDefinitions;
				for( int i = 0; i < arrDefinitions.Length; i++ )
				{
					if( !this.m_dataTypeDefinitionMap.ContainsKey( arrDefinitions[i].Name ) )
						this.m_dataTypeDefinitionMap.Add( arrDefinitions[i].Name, arrDefinitions[i] );
				}
			}

			// Write data type map last
			IDictionaryEnumerator typeIt = this.m_dataTypeDefinitionMap.GetEnumerator();

			while( typeIt.MoveNext() )
			{
				DamlTypeDefinition definition = (DamlTypeDefinition) typeIt.Value;
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

			return doc.OuterXml;
		}
	}
}
