using System;

// DISCUS Dynamic Proxy Package
namespace PSL.DISCUS.DynamicProxy
{
	/// <summary>
	/// Encapsulates a Dynamic Request to a Web Service
	/// </summary>
	public class DynamicRequest
	{
		private string m_strWSDLFile = ""; // URL to WSDL file
		private string m_strServiceName = ""; // Name of service (class)
		private string m_strDynNamespace = DEFAULT_PROXY_NAMESPACE; // Namespace to use
		private string m_strFilenameSource = ""; // Code file to generate
		private string m_strProtocol = SOAPProtocol; // Protocol to use (SOAP, HTTP Get or HTTP Post )
		private string m_strProxyPath = ""; // Path where proxy generated
		
		public const string DEFAULT_PROXY_NAMESPACE = "DynamicPxy";
		// Request protocols, Default = SOAPProtocol
		public const string SOAPProtocol = "SOAP";
		public const string HTTPGetProtocol = "HTTP GET";
		public const string HTTPPostProtocol = "HTTP POST";

		public DynamicRequest()
		{
		}
		
		// Properties (gets and sets)
		public string WsdlFile
		{
			get { return m_strWSDLFile; }
			set { m_strWSDLFile = value; }
		}

		public string ServiceName
		{
			get { return m_strServiceName; }
			set { m_strServiceName = value; }
		}
	
		public string DynNamespace
		{
			get { return m_strDynNamespace; }
			set { m_strDynNamespace = value; }
		}
		
		public string FilenameSource
		{
			get { return m_strFilenameSource; }
			set { m_strFilenameSource = value; }
		}

		public string ProxyPath
		{
			get { return m_strProxyPath; }
			set { m_strProxyPath = value; }
		}

		public string Protocol
		{
			get { return m_strProtocol; }
			set { m_strProtocol = value; }
		}
	}// End DynamicRequest
}
