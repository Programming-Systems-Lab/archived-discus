using System;

namespace PSL.DISCUS
{
	/// <summary>
	/// To generate a web service proxy we need:
	/// proxy class name
	/// proxy location (WSDL)
	/// proxy accesspoint url
	/// proxy cache location - where generated assemblies go
	/// </summary>
	public class WSProxyGenContext:TaskContext
	{
		private string m_strName = "";
		private string m_strWsdlUrl = "";
		private string m_strAccessPointUrl = "";
		private string m_strProxyCache = "";

		// optional proxy mutator object
		
		public WSProxyGenContext()
		{
		}

		public string Name
		{
			get
			{ return m_strName; }
			set
			{ 
				lock( m_strName )
				{
					m_strName = value; 
				}
			}
		}

		public string WsdlUrl
		{
			get
			{ return m_strWsdlUrl; }
			set
			{
				lock( m_strWsdlUrl )
				{
					m_strWsdlUrl = value;
				}
			}
		}

		public string AccessPointUrl
		{
			get
			{ return m_strAccessPointUrl; }
			set
			{
				lock( m_strAccessPointUrl )
				{
					m_strAccessPointUrl = value;
				}
			}
		}

		public string ProxyCache
		{
			get
			{ return m_strProxyCache; }
			set
			{
				lock( m_strProxyCache )
				{
					m_strProxyCache = value;
				}
			}
		}
	}
}
