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
using PSL.DISCUS.Impl.DynamicProxy;
using PSL.DISCUS.Impl.DynamicProxy.Util;
using System.Reflection;


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
				RegServiceDAO s = new RegServiceDAO();
				bool bStatus = false;
				int nServiceID = -1;
				/* nServiceID = s.RegisterService( "ServiceMap", "ServiceMappings", "C:\\Inetpub\\wwwroot\\ServiceMappings\\bin\\ServiceMappings.dll", "" );
				if( nServiceID != -1 )
				{
					bStatus = s.RegisterServiceMethod( "ServiceMap", "GetServiceInfo" );
					bStatus = s.RegisterServiceMethod( "ServiceMap", "GetNumServiceParams" );
					bStatus = s.RegisterServiceMethod( "ServiceMap", "RateService" );
				}
			
				// If Service location changed to WSDL ref then Service Access Point SHOULD be updated (in case BASE URL not included in WSDL)
				bStatus = s.UpdateServiceLocation( "ServiceMap", "http://localhost/ServiceMappings/ServiceMap.asmx?WSDL" );
				bStatus = s.UpdateServiceAccessPoint( "ServiceMap", "http://localhost/ServiceMappings/ServiceMap.asmx" );
				
				GateKeeper g = new GateKeeper();
				Object [] objParams = new Object[1];
				objParams[0] = "service";
				string strResults = (string) g.ExecuteServiceMethod( 4, "ServiceMap", "GetServiceInfo", objParams );
			
				/*DynamicRequest req = new DynamicRequest();
				req.dynNamespace = "dynProxy";
				req.serviceName = "ServiceMap";
				req.proxyPath = "C:";
				req.wsdlFile = "http://localhost/ServiceMappings/ServiceMap.asmx?wsdl";
				req.filenameSource = "ServiceMap";
				ProxyGen pxyGen = new ProxyGen();
				string strLoc = pxyGen.GenerateAssembly( req );
				
				object objResult = null;
				object [] parameters = new Object[1];
				parameters[0] = "1861005458";
				Assembly a = Assembly.LoadFrom( strLoc );
				Type tDyn = a.GetType( req.dynNamespace + "." + req.serviceName );
				// Create an instance of the type
				Object obj = Activator.CreateInstance( tDyn );
				// Invoke method
				objResult = tDyn.InvokeMember( "getPrice", 
					BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
					null,
					obj,
					parameters ); 
				*/			
				
				/*QuoteBinding b = new QuoteBinding();
				b.Url = "http://services.xmethods.net:80/soap/servlet/rpcrouter";
				float fltPrice = b.getPrice( "1861005458" );*/

				// Register BookQuotes service
				nServiceID = s.RegisterService( "QuoteBinding", "","http://www.xmethods.net/tmodels/BookQuote.wsdl","http://services.xmethods.net:80/soap/servlet/rpcrouter" ); 
				if( nServiceID != -1 )
					bStatus = s.RegisterServiceMethod( "QuoteBinding", "getPrice" );

				GateKeeper g = new GateKeeper();
				Object [] objParams = new Object[1];
				objParams[0] = "1861005458";
				float fltPrice = (float) g.ExecuteServiceMethod( 4, "QuoteBinding", "getPrice", objParams );
			

				int x = 0;
				x++;
			}
			catch( System.Exception e )
			{
				string strMsg = e.Message;
			}
		}
	}
}
