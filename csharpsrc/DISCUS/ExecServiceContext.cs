using System;
using System.Threading;
using System.Collections;

namespace PSL.DISCUS
{
	/// <summary>
	/// Description for WSInvokeTaskContext.
	/// To invoke a WS proxy one needs:
	/// The location of the service - assembly to load
	/// The service name - type to create
	/// The proxy url - in the event proxy generated without one
	/// The name of the method to execute
	/// Array of XML parameters to deserialize and pass for method invocation
	/// </summary>
	public sealed class ExecServiceContext:TaskContext
	{
		private string m_strAssembly = "";
		private string m_strServiceName = "";
		private string m_strAccessPointUrl = "";
		private string m_strMethodName = "";
		private ArrayList m_params = new ArrayList();
		
		public ExecServiceContext()
		{
		}

		public void Reset()
		{
			lock( this )
			{
				m_strAssembly = "";
				m_strServiceName = "";
				m_strAccessPointUrl = "";
				m_strMethodName = "";
				m_params = new ArrayList();
			}
		}
		
		// Properties
		public string Assembly
		{
			get
			{ return m_strAssembly; }
			set
			{ 
				lock( m_strAssembly )
				{
					m_strAssembly = value; 
				}
			}
		}
		
		public string ServiceName
		{
			get
			{ return m_strServiceName; }
			set
			{ 
				lock( m_strServiceName )
				{
					m_strServiceName = value; 
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

		public string MethodName
		{
			get
			{ return m_strMethodName; }
			set
			{ 
				lock( m_strMethodName )
				{
					m_strMethodName = value; 
				}
			}
		}

		public ArrayList Parameters
		{
			get
			{ return m_params; }
			set
			{ 
				lock( m_params.SyncRoot )
				{
					m_params = value; 
				}
			}
		}
	}
}
