using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Security.Permissions;
using PSL.DISCUS.Impl.GateKeeper;

namespace PSLGatekeeper1
{
	/// <summary>
	/// Summary description for PSLGatekeeper1.
	/// </summary>
	public class PSLGatekeeper1 : System.Web.Services.WebService
	{
		private GateKeeper m_gk;

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public PSLGatekeeper1()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
			m_gk = new GateKeeper();
			m_gk.Name = "PSLGatekeeper1";
			m_gk.TraceOn = true;
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion
		
		[WebMethod]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public string ExecuteServiceMethod( string strXMLExecRequest )
		{
			return m_gk.ExecuteServiceMethod( strXMLExecRequest );
		}

		[WebMethod]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public string[] ExecuteAlphaProtocol( string strAlphaProtocol )
		{
			return m_gk.ExecuteAlphaProtocol( strAlphaProtocol );
		}

		[WebMethod]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.AllFlags)]
		public string EnlistServicesByName( string strXmlTreatyReq )
		{
			return m_gk.EnlistServicesByName( strXmlTreatyReq );
		}
	}
}
