using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PSL.DISCUS.DAML
{
	public enum enuProcessType
	{
		AtomicProcess,
		CompositeProcess,
		SimpleProcess
	};
	
	/// <summary>
	/// Summary description for DAMLProcessModel.
	/// </summary>
	public class DAMLProcessModel:DAMLContainer
	{
		public DAMLProcessModel()
		{
			// Init inherited members
			m_EvtLog = new EventLog( "Application" );
			m_EvtLog.Source = "DAMLProcessModel";
			m_doc = new XmlDocument();
			m_mgr = null;
		}

		public String[] AtomicProcesses
		{
			get
			{ return GetProcesses( enuProcessType.AtomicProcess ); }
		}

		public String[] CompositeProcesses
		{
			get
			{ return GetProcesses( enuProcessType.CompositeProcess ); }
		}
		
		public String[] SimpleProcesses
		{
			get
			{ return GetProcesses( enuProcessType.SimpleProcess ); }
		}

		public string[] AllProcesses
		{
			get
			{
				ArrayList arrProcesses = new ArrayList();
				
				string[] arrAtomic = AtomicProcesses;
				string[] arrComposite = CompositeProcesses;
				string[] arrSimple = SimpleProcesses;
				
				// Add all to our collection
				arrProcesses.AddRange( arrAtomic );
				arrProcesses.AddRange( arrComposite );
				arrProcesses.AddRange( arrSimple );

				return (string[]) arrProcesses.ToArray( typeof( System.String ) );
			}
		}
		
		public string[] GetInputsOfNamedProcess( string strProcessName )
		{
			ArrayList arrInputs = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				string strURI = "";

				foreach( string prefix in m_mgr )
				{
					if( prefix == DAMLConstants.PROCESS_NS )
					{
						strURI = m_mgr.LookupNamespace( prefix );
						break;
					}
				}
	
				strURI += DAMLConstants.INPUT;
				
				string strXPath = DAMLConstants.RDF_PROPERTY + "/" + DAMLConstants.RDFS_SUBPROPERTYOF + "[@" + DAMLConstants.RDF_RESOURCE + "='" + strURI + "']";
				
				XmlNodeList lstNodes = root.SelectNodes( strXPath, m_mgr );

				foreach( XmlNode node in lstNodes )
				{
					XmlNode domainNode = node.NextSibling;
					
					string strDomain = domainNode.Attributes[DAMLConstants.RDF_RESOURCE].Value;
					
					if( strProcessName.CompareTo( strDomain.Trim( new Char[] { '#' } ) ) == 0 )
					{
						string strTemp = domainNode.ParentNode.Attributes[DAMLConstants.RDF_ID].Value;
						arrInputs.Add( strTemp );
					}
				}
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			
			return (string[]) arrInputs.ToArray( typeof( System.String ) );
			
		}
		
		private string[] GetProcesses( enuProcessType processType )
		{
			ArrayList arrProcess = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				string strProcessURI = "";

				foreach( string prefix in m_mgr )
				{
					if( prefix == DAMLConstants.PROCESS_NS )
					{
						strProcessURI = m_mgr.LookupNamespace( prefix );
						break;
					}
				}
				
				string strProcessType = "";

				switch( processType )
				{
					case enuProcessType.AtomicProcess: strProcessType = DAMLConstants.DAML_ATOMIC_PROCESS;
													   break;

					case enuProcessType.CompositeProcess: strProcessType = DAMLConstants.DAML_COMPLEX_PROCESS;
														break;

					case enuProcessType.SimpleProcess: strProcessType = DAMLConstants.DAML_SIMPLE_PROCESS;
													   break;
				};
				
				string strXPath = DAMLConstants.DAML_CLASS + "/" + DAMLConstants.RDFS_SUBCLASSOF + "[@" + DAMLConstants.RDF_RESOURCE + "='" + strProcessURI + strProcessType + "']";

				XmlNodeList lstProcess = root.SelectNodes( strXPath, m_mgr );
			
				foreach( XmlNode processNode in lstProcess )
				{
					// Move up to parent
					XmlNode parentNode = processNode.ParentNode;

					string strValue = parentNode.Attributes[DAMLConstants.RDF_ID].Value;
					
					if( strValue.Length > 0 )
						arrProcess.Add( strValue );
					
				}
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			
			return (string[]) arrProcess.ToArray( typeof( System.String ) );
		}
	}
}
