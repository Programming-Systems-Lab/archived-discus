using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using Microsoft.Data.Odbc;
using PSL.DISCUS.Impl.Treaty;
using PSL.DISCUS.Impl.DataAccess;
using PSL.DISCUS.Impl.GateKeeper;


namespace DISCUSUnittest
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Tester
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
			/*	RegServiceDAO s = new RegServiceDAO();
				bool bStatus = false;
				int nServiceID = s.RegisterService( "ServiceMappings.ServiceMap", "C:\\Inetpub\\wwwroot\\ServiceMappings\\bin\\ServiceMappings.dll" );
				bStatus = s.RegisterServiceMethod( "ServiceMappings.ServiceMap", "GetServiceInfo" );
				bStatus = s.RegisterServiceMethod( "ServiceMappings.ServiceMap", "GetNumServiceParams" );
				bStatus = s.RegisterServiceMethod( "ServiceMappings.ServiceMap", "RateService" );
			*/
				GateKeeper g = new GateKeeper();
				Object [] objParams = new Object[1];
				objParams[0] = "service";
				g.ExecuteServiceMethod( 4, "ServiceMappings.ServiceMap", "GetServiceInfo", objParams );
				
				
			}
			catch( System.Exception e )
			{
				string strMsg = e.Message;
			}
		}
	}
}
