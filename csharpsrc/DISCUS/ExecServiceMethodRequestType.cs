using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

namespace PSL.DISCUS
{
	/// <summary>
	/// ExecServiceMethodRequestType encapsulates a request for
	/// ServiceMethod execution
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
		public ArrayList m_ParamValue;

		[XmlIgnoreAttribute]
		public ArrayList m_ParamName;
		
		public ExecServiceMethodRequestType()
		{
			m_ParamValue = new ArrayList();
			m_ParamName = new ArrayList();
		}

		/* Function converts this instance to Xml using the
		 * XmlSerializer.
		 * Inputs: none
		 * Return values: the Xml serialization of the instance
		 */
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
		}//End ToXml
	}
}
