using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Xml.Serialization;

namespace PSL.DISCUS
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
		
		[System.Xml.Serialization.XmlElementAttribute("ServiceInfo")]
		public ServiceDataType[] m_ServiceInfo;
		
		public TreatyType()
		{
			m_nTreatyID = 0;
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
	}
	
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://localhost/Discus/Schema/Treaty.xsd")]
	public class ServiceDataType 
	{
		public ServiceDataType()
		{
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
		
		[System.Xml.Serialization.XmlElementAttribute("ServiceMethod")]
		public ServiceMethodDataType[] m_ServiceMethod;

		public bool AllServiceMethodsAuthorized()
		{
			if( m_ServiceMethod == null )
				return false;

			for( int i = 0; i < m_ServiceMethod.Length; i++ )
			{
				if( !m_ServiceMethod[i].Authorized )
					return false;
			}
			return true;
		}
    }
	
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://localhost/Discus/Schema/Treaty.xsd")]
	public class ServiceMethodDataType 
	{
		public ServiceMethodDataType()
		{
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
    
		[System.Xml.Serialization.XmlElementAttribute("Parameter")]
		public string[] m_Parameter;
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
