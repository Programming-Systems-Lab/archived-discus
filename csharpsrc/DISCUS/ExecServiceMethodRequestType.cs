using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

namespace PSL.DISCUS.Impl.GateKeeper
{
	/// <summary>
	/// Summary description for ExecServiceMethodRequestType.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.psl.cs.columbia.edu/Discus/ExecServiceMethodRequest.xsd")]
	[System.Xml.Serialization.XmlRootAttribute("ExecServiceMethodRequest", Namespace="http://www.psl.cs.columbia.edu/Discus/ExecServiceMethodRequest.xsd", IsNullable=false)]
	public class ExecServiceMethodRequestType
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
		private string m_strServiceName;
		[XmlElementAttribute("ServiceName")]
		public string ServiceName
		{
			get
			{ return m_strServiceName; }
			set
			{ m_strServiceName = value; }
		}
		
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

		public ExecServiceMethodRequestType()
		{
			m_Parameter = new ArrayList();
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
			
			ExecServiceMethodRequestType temp = objInst as ExecServiceMethodRequestType;
			
			// Copy instance variables
			TreatyID = temp.TreatyID;
			ServiceName = temp.ServiceName;
			MethodName = temp.MethodName;
			m_Parameter.Clear();
			
			for( int i = 0; i < temp.m_Parameter.Count; i++ )
				m_Parameter.Add( temp.m_Parameter[i] );
		}
	}
}
