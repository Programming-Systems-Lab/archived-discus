using System;

// DISCUS Dynamic Proxy Package
namespace PSL.DISCUS.Impl.DynamicProxy
{
	/// <summary>
	/// Encapsulates a Dynamic Request to a Web Service
	/// </summary>
	public class DynamicRequest
	{
		private string m_strWSDLFile; // URL to WSDL file
		private string m_strServiceName; // Name of service (class)
		private string m_strDynNamespace; // Namespace to use
		private string m_strFilenameSource; // Code file to generate
		private string m_strProtocol; // Protocol to use (SOAP, HTTP Get or HTTP Post )
		private string m_strProxyPath; // Path where proxy generated
		private string m_strBaseURL;
		public static string DEFAULT_PROXY_NAMESPACE = "DynamicPxy";

		// Request protocols, Default = SOAPProtocol
		public static string SOAPProtocol = "SOAP";
		public static string HTTPGetProtocol = "HTTP GET";
		public static string HTTPPostProtocol = "HTTP POST";

		public DynamicRequest()
		{
			m_strWSDLFile = "";
			m_strServiceName = "";
			m_strDynNamespace = DEFAULT_PROXY_NAMESPACE; // Default namespace
			m_strFilenameSource = "";
			m_strProtocol = DynamicRequest.SOAPProtocol; // Default request protocol
			m_strBaseURL = "";
		}

		
		// Properties (gets and sets)
		public string wsdlFile
		{
			get { return m_strWSDLFile; }
			set { m_strWSDLFile = value; }
		}

		public string baseURL
		{
			get { return m_strBaseURL; }
			set { m_strBaseURL = value; }
		}

		public string serviceName
		{
			get { return m_strServiceName; }
			set { m_strServiceName = value; }
		}
	
		public string dynNamespace
		{
			get { return m_strDynNamespace; }
			set { m_strDynNamespace = value; }
		}
		
		public string filenameSource
		{
			get { return m_strFilenameSource; }
			set { m_strFilenameSource = value; }
		}

		public string proxyPath
		{
			get { return m_strProxyPath; }
			set { m_strProxyPath = value; }
		}

		public string protocol
		{
			get { return m_strProtocol; }
			set { m_strProtocol = value; }
		}
	}// End DynamicRequest
}
