using System;

namespace PSL.DISCUS
{
	/// <summary>
	/// Summary description for WSDetails.
	/// </summary>
	public class WSDetails
	{
		private string m_strLocation = "";
		private string m_strAccessPoint = "";
		private string m_strNamespace = "";
		private string m_strName = "";
		private string m_strMethod = "";
		private bool m_bMethodExists = false;
				
		public WSDetails()
		{
		}

		public void Reset()
		{
			m_strLocation = "";
			m_strAccessPoint = "";
			m_strNamespace = "";
			m_strName = "";
			m_strMethod = "";
			m_bMethodExists = false;
		}

		// Properties
		public bool IsValid
		{
			get
			{
				return ( m_strLocation.Length != 0 && m_strAccessPoint.Length != 0 );
			}
		}

		public string Location
		{
			get
			{ return m_strLocation; }
			set
			{ m_strLocation = value; }
		}

		public string AccessPoint
		{
			get
			{ return m_strAccessPoint; }
			set
			{ m_strAccessPoint = value; }
		}

		public string Namespace
		{
			get
			{ return m_strNamespace; }
			set
			{ m_strNamespace = value; }
		}

		public string Method
		{
			get
			{ return m_strMethod; }
			set
			{ m_strMethod = value; }
		}

		public string Name
		{
			get
			{ return m_strName; }
			set
			{ m_strName = value; }
		}

		public bool MethodExists
		{
			get
			{ return m_bMethodExists; }
			set
			{ m_bMethodExists = value; }
		}
	}
}
