using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Xml.Serialization;

namespace PSL.DISCUS.Impl.GateKeeper
{
	/// <summary>
	/// Summary description for TreatyType.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://localhost/Discus/Schema/Treaty.xsd")]
	[System.Xml.Serialization.XmlRootAttribute("Treaty", Namespace="http://localhost/Discus/Schema/Treaty.xsd", IsNullable=false)]
	public class TreatyType
	{
		private int m_nTreatyID = 0;
		[XmlElementAttribute("TreatyID")]
		[System.ComponentModel.DefaultValueAttribute(0)]
		public int TreatyID
		{
			get
			{ return m_nTreatyID; }

			set
			{ m_nTreatyID = value; }
		}

		private string m_strClientServiceSpace;
		[XmlElementAttribute("ClientServiceSpace")]
		public string ClientServiceSpace
		{
			get
			{ return m_strClientServiceSpace; }
			set
			{ m_strClientServiceSpace = value; }
		}

		private string m_strProviderServiceSpace;
		[XmlElementAttribute("ProviderServiceSpace")]
		public string ProviderServiceSpace
		{
			get
			{ return m_strProviderServiceSpace; }
			set
			{ m_strProviderServiceSpace = value; }
		}
		[XmlArrayAttribute("ServiceInfo")]
		[XmlArrayItem(typeof(ServiceDataType))]
		public ArrayList m_ServiceInfo;
	
		public TreatyType()
		{
			m_ServiceInfo = new ArrayList();
		}
		
		public string ToXml()
		{
			string strXml = "";
			try
			{
				// Create a new serializer
				XmlSerializer ser = new XmlSerializer( this.GetType() );
				// Create a memory stream
				MemoryStream ms = new MemoryStream();
				// Serialize to stream ms
				ser.Serialize( ms, this );
				// Goto start of stream
				ms.Seek( 0, System.IO.SeekOrigin.Begin );
				// Create a stream reader
				TextReader reader = new StreamReader( ms );
				// Read entire stream, this is our return value
				strXml = reader.ReadToEnd();
				// Close reader
				reader.Close();
				// Close stream
				ms.Close();
			}
			catch( System.Exception e )
			{
				string strTemp = e.Message;
			}
			return strXml;
		}

		public void BuildFromXml( string strXml )
		{
			if( strXml.Length == 0 )
				return;

			// Create new XmlSerializer
			XmlSerializer ser = new XmlSerializer( this.GetType() );
			// Re-construct object
			System.Xml.XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read(); 
			// Deserialize
			Object objInst = ser.Deserialize( xt );
			
			TreatyType temp = objInst as TreatyType;
			TreatyID = temp.TreatyID;
			ClientServiceSpace = temp.ClientServiceSpace;
			ProviderServiceSpace = temp.ProviderServiceSpace;
			m_ServiceInfo.Clear();

			for( int i = 0; i < temp.m_ServiceInfo.Count; i++ )
			{
				m_ServiceInfo.Add( temp.m_ServiceInfo[i] );
			}
			
		}
	}

	[Serializable]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://localhost/Discus/Schema/Treaty.xsd")]
	public class ServiceDataType 
	{
		public ServiceDataType()
		{
			m_ServiceMethod = new ArrayList();
		}
		
		/// <remarks/>
		private string m_strServiceName;
		[XmlElementAttribute("ServiceName")]
		public string ServiceName
		{
			get
			{ return m_strServiceName; }
			set
			{ m_strServiceName = value; }
		}
		[XmlArrayAttribute("ServiceMethod")]
		[XmlArrayItem(typeof(ServiceMethodDataType))]
		public ArrayList m_ServiceMethod;
    }
	
	[Serializable]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://localhost/Discus/Schema/Treaty.xsd")]
	public class ServiceMethodDataType 
	{
		public ServiceMethodDataType()
		{
			m_Parameter = new ArrayList();
		}
		
		/// <remarks/>
		private string m_strMethodName;
		[XmlElementAttribute("MethodName")]
		public string MethodName
		{
			get
			{ return m_strMethodName; }
			set
			{ m_strMethodName = value; }
		}
    
		[XmlArrayAttribute("Parameter")]
		[XmlArrayItem(typeof(string))]
		public ArrayList m_Parameter;
		private int m_nNumInvokations = 1;
		[XmlElementAttribute("NumInvokations")]
		[System.ComponentModel.DefaultValueAttribute(1)]
		public int NumInvokations
		{	
			get
			{ return m_nNumInvokations; }
			set
			{ m_nNumInvokations = value; }
		}
		
		private bool m_bAuthorized = false;
		[XmlElementAttribute("Authorized")]
		[System.ComponentModel.DefaultValueAttribute(false)]
		public bool Authorized
		{
			get
			{ return m_bAuthorized; }
			set
			{ m_bAuthorized = value; }
		}
		
		private string m_strMethodImplementation;
		public string MethodImplementation
		{
			get
			{ return m_strMethodImplementation; }
			set
			{ m_strMethodImplementation = value; }
		}
	}
}
