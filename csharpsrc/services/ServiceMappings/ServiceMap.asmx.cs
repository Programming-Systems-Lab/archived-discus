using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;

namespace ServiceMappings
{
	/// <summary>
	/// Summary description for ServiceMap.
	/// </summary>
	public class ServiceMap : System.Web.Services.WebService
	{
		public ServiceMap()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
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
		public string GetServiceInfo( string strServiceName )
		{
			string strRetVal = "";

			strRetVal += "<ServiceMap><ServiceName>ServiceMap</ServiceName><ServiceMethods><MethodData><MethodName>GetServiceInfo</MethodName><Parameter>strServiceName</Parameter></MethodData></ServiceMethods></ServiceMap>";
            
			return strRetVal;
		}

		[WebMethod]
		public int GetNumServiceParams( string strServiceName )
		{
			return 5;
		}

		[WebMethod]
		public int RateService( int nRating )
		{
			int nLocalRating = nRating;
			return 0;
		}
	}
}
