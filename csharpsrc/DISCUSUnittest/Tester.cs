using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;
using Microsoft.Data.Odbc;
using PSL.DISCUS.Impl.DataAccess;
using PSL.DISCUS.Impl.GateKeeper;
using PSL.DISCUS.Impl.DynamicProxy;
using PSL.DISCUS.Impl.DynamicProxy.Util;
using System.Reflection;
using System.Collections;
//using DISCUSUnittest.localhost;


namespace DISCUSUnittest
{	
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Tester
	{
		/* Register services an known gatekeepers */
		public static void SetupServiceSpace()
		{
			InternalRegistry ireg = new InternalRegistry();
			// BN QuoteBinding Service
			int nServiceID = ireg.RegisterService( "BNQuoteService", "","http://www.xmethods.net/sd/2001/BNQuoteService.wsdl", "http://services.xmethods.net:80/soap/servlet/rpcrouter" );
			if( nServiceID != -1 )
				ireg.RegisterServiceMethod( "BNQuoteService", "getPrice" );
			// XMethodsQuery
			nServiceID = ireg.RegisterService( "XMethodsQuery", "", "http://www.xmethods.net/wsdl/query.wsdl", "http://www.xmethods.net/interfaces/query" );
			if( nServiceID != -1 )
			{
				ireg.RegisterServiceMethod( "XMethodsQuery", "getAllServiceSummaries" );
				ireg.RegisterServiceMethod( "XMethodsQuery", "getServiceNamesByPublisher" );
			}

			// GeoCash
			nServiceID = ireg.RegisterService( "GeoCash", "", "http://64.78.60.122/GeoCash.asmx?WSDL", "http://64.78.60.122/GeoCash.asmx" );
			if( nServiceID != -1 )
				ireg.RegisterServiceMethod( "GeoCash", "GetATMLocations" );

			// Google
			nServiceID = ireg.RegisterService( "GoogleSearchService", "", "http://api.google.com/GoogleSearch.wsdl", "http://api.google.com/search/beta2" );
			if( nServiceID != -1 )
				ireg.RegisterServiceMethod( "GoogleSearchService", "doGoogleSearch" );
			// SecurityManager
			nServiceID = ireg.RegisterService( "SecurityManagerService", "", "http://church.psl.cs.columbia.edu:8080/security/SecurityServices.wsdl", "http://church.psl.cs.columbia.edu:8080/security/jaxrpc/SecurityManagerService" );
			if( nServiceID != -1 )
			{
				ireg.RegisterServiceMethod( "SecurityManagerService", "doRequestCheck" );
				ireg.RegisterServiceMethod( "SecurityManagerService", "signDocument" );
				ireg.RegisterServiceMethod( "SecurityManagerService", "verifyDocument" );
				ireg.RegisterServiceMethod( "SecurityManagerService", "verifyTreaty" );
				ireg.RegisterServiceMethod( "SecurityManagerService", "verifyTreaty2" );
			}
		}
		
		public static void ResetServiceLocations()
		{
			InternalRegistry ireg = new InternalRegistry();
			// BN Quote Binding
			ireg.UpdateServiceLocation( "BNQuoteService", "http://www.xmethods.net/sd/2001/BNQuoteService.wsdl" );
			// XMethodsQuery
			ireg.UpdateServiceLocation( "XMethodsQuery", "http://www.xmethods.net/wsdl/query.wsdl" );
			// GeoCash
			ireg.UpdateServiceLocation( "GeoCash", "http://64.78.60.122/GeoCash.asmx?WSDL" );
			// Google
			ireg.UpdateServiceLocation( "GoogleSearchService", "http://api.google.com/GoogleSearch.wsdl" );
			// SecurityManager
			ireg.UpdateServiceLocation( "SecurityManagerService", "http://church.psl.cs.columbia.edu:8080/security/SecurityServices.wsdl" );
		}

		public static void TestServiceMethods()
		{
			// GateKeeper instance
			GateKeeper g = new GateKeeper();
			//
			//DynamicPxy.PSLGatekeeper1 g = new DynamicPxy.PSLGatekeeper1();
			object objRes = null;
			//InternalRegistry ireg = new InternalRegistry();
			//ireg.UpdateServiceLocation( "SecurityManagerService", "http://church.psl.cs.columbia.edu:8080/security/SecurityServices.wsdl" );


			ExecServiceMethodRequestType e = new ExecServiceMethodRequestType();
			// BNQuoteBinding
			e.TreatyID = -1820085390;//-115276743;
			e.ServiceName = "BNQuoteService";
			e.MethodName = "getPrice";
			e.m_ParamValue.Clear();
			e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>1861005458</string>" );

			objRes = g.ExecuteServiceMethod( e.ToXml() );			

			//objRes = g.EnlistServicesByName( "<?xml version=\"1.0\"?><Treaty xmlns=\"http://localhost/Discus/Schema/Treaty.xsd\"><TreatyID>1000</TreatyID><ClientServiceSpace>myservicespace</ClientServiceSpace><ProviderServiceSpace>providerss</ProviderServiceSpace><ServiceInfo><ServiceName>service</ServiceName><ServiceMethod><MethodName>method</MethodName><Parameter>foo</Parameter><Parameter>bar</Parameter><NumInvokations>1</NumInvokations><Authorized>true</Authorized></ServiceMethod></ServiceInfo></Treaty>" );
			
			// Sql script for service invocation permission table
			// insert into serviceinvokationpermission values(100,'BNQuoteService','getPrice','isbn',100000,'getPrice');
			// insert into serviceinvokationpermission values(100,'XMethodsQuery','getAllServiceSummaries','',100000,'getAllServiceSummaries');
			// insert into serviceinvokationpermission values(100,'XMethodsQuery','getServiceNamesByPublisher','PubName',100000,'getServiceNamesByPublisher');
			// insert into serviceinvokationpermission values(100,'GeoCash','GetATMLocations','Zipcode',100000,'GetATMLocations');

			// XMethodsQuery
			e.ServiceName = "XMethodsQuery";
			e.MethodName = "getAllServiceSummaries";
			e.m_ParamValue.Clear();
			objRes = g.ExecuteServiceMethod( e.ToXml() );
			
			e.MethodName = "getServiceNamesByPublisher";
			e.m_ParamValue.Clear();
			e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>Interdata</string>" );
			objRes = g.ExecuteServiceMethod( e.ToXml() );

			// GeoCash
			e.ServiceName = "GeoCash";
			e.MethodName = "GetATMLocations";
			e.m_ParamValue.Clear();
			e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>10025</string>" );
			objRes = g.ExecuteServiceMethod( e.ToXml() );

			// Google
			
			// SecurityManagerService
			/*e.ServiceName = "SecurityManagerService";
			e.MethodName = "signDocument";
			e.m_Parameter.Clear();
			e.m_Parameter.Add( "<?xml version=\"1.0\" encoding=\"utf-8\"?><ExecServiceMethodRequest><TreatyID>4</TreatyID><ServiceName>GeoCash</ServiceName><MethodName>GetATMLocations</MethodName><Parameter>Simple Document</Parameter></ExecServiceMethodRequest>" );
			objRes = g.ExecuteServiceMethod( e.ToXml() );*/


		}

		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				//SetupServiceSpace();
				//ResetServiceLocations();
				//TestServiceMethods();

				//string strAlpha = "<?xml version=\"1.0\" encoding=\"utf-8\"?><definitions name=\"DemoAlpha\" targetNamespace=\"http://psl.cs.columbia.edu\" xmlns:xlang=\"http://schemas.microsoft.com/bixtalk/xlang\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"><!-- Simple demo using simple xlang constructs --><xlang:behavior><xlang:body><xlang:sequence><xlang:action activation=\"true\" gatekeeper=\"PSLGatekeeper1\" servicename=\"BNQuoteService\" operation=\"getPrice\"><parameter name=\"isbn\"><![CDATA[<?xml version=\"1.0\"?><string>1861005458</string>]]></parameter></xlang:action><xlang:action gatekeeper=\"PSLGatekeeper1\" servicename=\"GeoCash\" operation=\"GetATMLocations\"><parameter name=\"Zipcode\"><![CDATA[<?xml version=\"1.0\"?><string>10025</string>]]></parameter></xlang:action></xlang:sequence></xlang:body></xlang:behavior></definitions>";
				//GateKeeper g = new GateKeeper();
				//string [] arrRes = g.ExecuteAlphaProtocol( strAlpha );

				//string strTreaty = "<?xml version=\"1.0\"?><Treaty xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://localhost/Discus/Schema/Treaty.xsd\"><ServiceInfo><ServiceMethod><Parameter>&lt;?xml version=\"1.0\"?&gt;&lt;string&gt;1861005458&lt;/string&gt;</Parameter><MethodName>getPrice</MethodName></ServiceMethod><ServiceName>BNQuoteService</ServiceName></ServiceInfo><ServiceInfo><ServiceMethod><Parameter>&lt;?xml version=\"1.0\"?&gt;&lt;string&gt;10025&lt;/string&gt;</Parameter><MethodName>GetATMLocations</MethodName></ServiceMethod><ServiceName>GeoCash</ServiceName></ServiceInfo><ServiceInfo><ServiceMethod><MethodName>getAllServiceSummaries</MethodName></ServiceMethod><ServiceName>XMethodsQuery</ServiceName></ServiceInfo><TreatyID>0</TreatyID><ClientServiceSpace>100</ClientServiceSpace><ProviderServiceSpace>PSLGatekeeper1</ProviderServiceSpace></Treaty>";
				//g.EnlistServicesByName( strTreaty );
				
				//PSLGatekeeper2 g = new PSLGatekeeper2();
				//string[] arrRes = g.ExecuteAlphaProtocol( strAlpha ); 



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
