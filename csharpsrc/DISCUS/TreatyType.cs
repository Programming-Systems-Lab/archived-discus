using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using PSL.DISCUS.Interfaces;
using PSL.DISCUS.Impl;


namespace PSL.DISCUS.Impl.Treaty
{
	/// <summary>
	/// Summary description for Treaty.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://localhost/Discus/Schema/Treaty.xsd")]
	[System.Xml.Serialization.XmlRootAttribute("Treaty", Namespace="http://localhost/Discus/Schema/Treaty.xsd", IsNullable=false)]
	public class TreatyType:IComparable
	{
		[System.Xml.Serialization.XmlElementAttribute("TreatyID")]
		public int m_nTreatyID = 0;
		
		[System.Xml.Serialization.XmlElementAttribute("ClientServiceSpace")]
		public string m_strClientServiceSpace="";

		[System.Xml.Serialization.XmlElementAttribute("ProviderServiceSpace")]
		public string m_strProviderServiceSpace="";

		[System.Xml.Serialization.XmlElementAttribute("ServiceInfo")]
		public ServiceDataType[] m_arrServiceInfo;

		public TreatyType()
		{
		}

		public static TreatyType BuildTreatyFromXML( string strXMLTreaty ) 
		{
			// Clean up XML string a bit
			int nIndex = strXMLTreaty.IndexOf( "<TreatyID>" );
			strXMLTreaty = strXMLTreaty.Substring( nIndex );
			strXMLTreaty = "<Treaty>" + strXMLTreaty;

			// Create an empty Treaty
			TreatyType treaty = new TreatyType();
			// Validate Treaty against schema, method throws 
			// System.Xml.XmlException, System.Exception
			ValidateTreaty( strXMLTreaty );
			// After validation build treaty using xpath
			XmlDocument doc = new XmlDocument();
			// Load the document using the XML Treaty
			doc.LoadXml( strXMLTreaty );			
				
			// Create root node based on XML document
			XmlNode root = doc.DocumentElement;
			XmlNode ProviderSSNode = root.SelectSingleNode( "ProviderServiceSpace" );
			// Fill in Provider Service Space
			treaty.m_strProviderServiceSpace = ProviderSSNode.InnerText;
			XmlNode ClientSSNode = root.SelectSingleNode( "ClientServiceSpace" );
			// Fill in Client Service Space
			treaty.m_strClientServiceSpace = ClientSSNode.InnerText;
				
			// Get list of ServceInfo nodes
			XmlNodeList ServiceInfoNodeList = root.SelectNodes( "ServiceInfo" );
			treaty.m_arrServiceInfo = new ServiceDataType[ServiceInfoNodeList.Count];
			int i = 0;

			foreach( XmlNode svcInfo in ServiceInfoNodeList )
			{
				treaty.m_arrServiceInfo[i] = new ServiceDataType();
				treaty.m_arrServiceInfo[i].m_strServiceName = svcInfo.SelectSingleNode( "ServiceName" ).InnerText;
				// Get list of service methods
				XmlNodeList ServiceMethodsNodeList = svcInfo.SelectNodes( "ServiceMethods" );
				treaty.m_arrServiceInfo[i].m_arrServiceMethods = new ServiceMethodDataType[ServiceMethodsNodeList.Count];
				int j = 0;
				
				foreach( XmlNode method in ServiceMethodsNodeList )
				{
					treaty.m_arrServiceInfo[i].m_arrServiceMethods[j] = new ServiceMethodDataType();
					treaty.m_arrServiceInfo[i].m_arrServiceMethods[j].m_strMethodName = method.SelectSingleNode( "MethodName" ).InnerText;
					XmlNodeList ParameterNodeList = method.SelectNodes( "Parameter" );
					treaty.m_arrServiceInfo[i].m_arrServiceMethods[j].m_arrParameter = new String[ParameterNodeList.Count];
					int k = 0;
				
					foreach( XmlNode p in ParameterNodeList )
					{
						treaty.m_arrServiceInfo[i].m_arrServiceMethods[j].m_arrParameter[k] = p.InnerText;
						k++;
					}
					j++;
				}
				i++;
			}
			return treaty;
		}

		public int CompareTo( Object obj )
		{
			TreatyType temp = obj as TreatyType;
			return m_nTreatyID.CompareTo( temp.m_nTreatyID );
		}

		public int FindService( string strServiceName )
		{
			int nRetVal = -1;

			for( int i = 0; i < m_arrServiceInfo.Length; i++ )
			{
				if( m_arrServiceInfo[i] != null )
				{
					if( m_arrServiceInfo[i].m_strServiceName.ToLower() == strServiceName.ToLower() )
					{
						nRetVal = i;
						break;
					}
				}
			}
			return nRetVal;
		}

		public string ToWFXML() // returns well formed XML doc
		{
			string strRetVal = "";
			
			MemoryStream ms = new MemoryStream();
			XmlSerializer ser = new XmlSerializer(typeof(TreatyType));
			ser.Serialize( ms, this );
			ms.Seek( 0, System.IO.SeekOrigin.Begin );
			
			TextReader tr = new StreamReader( ms );
			strRetVal = tr.ReadToEnd();
			
			tr.Close();
			ms.Close();
	
			return strRetVal;
		}
		
		public string ToSimpleXML()
		{
			string strRetVal = ToWFXML();
			
			// Clean up XML string a bit
			int nIndex = strRetVal.IndexOf( "<TreatyID>" );
			strRetVal = strRetVal.Substring( nIndex );
			strRetVal = "<Treaty>" + strRetVal;

			return strRetVal;
		}

		public static bool ValidateTreaty( string strXMLTreaty )
		{
			bool bRetVal = false;
			
			try
			{
				// Create Validating XML reader
				XmlValidatingReader vr = new XmlValidatingReader( strXMLTreaty, XmlNodeType.Document, null );
				// Add Treaty schema to Schema collection of validating XML reader
				vr.Schemas.Add("http://localhost/Discus/Schema/Treaty.xsd", "http://localhost/Discus/Schema/Treaty.xsd");
				// Set validation type to Schema
				vr.ValidationType = ValidationType.Schema;
					
				// Read and validate XML against schema
				while( vr.Read() )
				{}
				// Close reader
				vr.Close();
				bRetVal = true;
			}
			catch( System.Xml.XmlException e ) // re-throw exception
			{
				throw new System.Xml.XmlException( e.Message, e );
			}
			catch( System.Exception e ) // re-throw exception
			{
				throw new System.Exception( e.Message, e );
			}

			return bRetVal;
		}
	}// End class def Treaty
	
	[Serializable]
	[System.Xml.Serialization.SoapTypeAttribute(Namespace="http://localhost/Discus/Schema/Treaty.xsd")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://localhost/Discus/Schema/Treaty.xsd")]
	public class ServiceDataType:IComparable
	{
		[System.Xml.Serialization.SoapElementAttribute("ServiceName")]
		[System.Xml.Serialization.XmlElementAttribute("ServiceName")]
		public string m_strServiceName;
    
		[System.Xml.Serialization.SoapElementAttribute("ServiceMethods")]
		[System.Xml.Serialization.XmlElementAttribute("ServiceMethods")]
		public ServiceMethodDataType[] m_arrServiceMethods;

		public ServiceDataType()
		{
		}

		public int CompareTo( Object obj )
		{
			ServiceDataType temp = obj as ServiceDataType;
			return m_strServiceName.ToLower().CompareTo( temp.m_strServiceName.ToLower() );
		}

		public int FindMethod( string strMethodName )
		{
			int nRetVal = -1;

			for( int i = 0; i < m_arrServiceMethods.Length; i++ )
			{
				if( m_arrServiceMethods[i] != null )
				{
					if( m_arrServiceMethods[i].m_strMethodName.ToLower() == strMethodName.ToLower() )
					{
						nRetVal = i;
						break;
					}
				}
			}
			return nRetVal;
		}
	}
	
	[Serializable]
	[System.Xml.Serialization.SoapTypeAttribute(Namespace="http://localhost/Discus/Schema/Treaty.xsd")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://localhost/Discus/Schema/Treaty.xsd")]
	public class ServiceMethodDataType:IComparable
	{
		[System.Xml.Serialization.SoapElementAttribute("MethodName")]
		[System.Xml.Serialization.XmlElementAttribute("MethodName")]
		public string m_strMethodName;
    
		[System.Xml.Serialization.SoapElementAttribute("Parameter")]
		[System.Xml.Serialization.XmlElementAttribute("Parameter")]
		public string[] m_arrParameter;
		
		[System.Xml.Serialization.SoapElementAttribute("Authorized")]
		[System.Xml.Serialization.XmlElementAttribute("Authorized")]
		public bool m_bAuthorized = false;

		public ServiceMethodDataType()
		{
		}

		public int CompareTo( Object obj )
		{
			ServiceMethodDataType temp = obj as ServiceMethodDataType;
			return m_strMethodName.ToLower().CompareTo( temp.m_strMethodName.ToLower() );
		}

		public int FindParameter( string strParamName )
		{
			int nRetVal = -1;
			
			for( int i = 0; i < m_arrParameter.Length; i++ )
			{
				if( m_arrParameter[i] != null )
				{
					if( m_arrParameter[i].ToLower() == strParamName.ToLower() )
					{
						nRetVal = i;
						break;
					}
				}
			}
			return nRetVal;
		}

		public bool IsAuthorized()
		{
			return m_bAuthorized;
		}
	}
}
