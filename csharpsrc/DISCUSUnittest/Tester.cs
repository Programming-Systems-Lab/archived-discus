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
using System.Collections;
using DynamicPxy;


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
				//InternalRegistry ireg = new InternalRegistry();
				//ireg.RegisterServiceMethod( "XMethodsQuery", "getServiceNamesByPublisher" );
				//ireg.UpdateGateKeeperLocation( "PSLGatekeeper1", "http://localhost/PSLGatekeeper1/PSLGatekeeper1.asmx?WSDL" );

				// Exec request with input paramters 
				// XMethodsQuery - getServiceNamesByPublisher
				// string strInput = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ExecServiceMethodRequest><TreatyID>4</TreatyID><ServiceName>XMethodsQuery</ServiceName><MethodName>getServiceNamesByPublisher</MethodName><Parameter><![CDATA[<?xml version=\"1.0\"?><string>Interdata</string>]]></Parameter></ExecServiceMethodRequest>";
				// Exec request no input parameters
				// XMethodsQuery - getAllServiceSummaries
				// string strInput = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ExecServiceMethodRequest><TreatyID>4</TreatyID><ServiceName>XMethodsQuery</ServiceName><MethodName>getAllServiceSummaries</MethodName></ExecServiceMethodRequest>";
				// QuoteBindingService
				// string strInput = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ExecServiceMethodRequest><TreatyID>4</TreatyID><ServiceName>QuoteBinding</ServiceName><MethodName>getPrice</MethodName><Parameter><![CDATA[<?xml version=\"1.0\"?><string>1861005458</string>]]></Parameter></ExecServiceMethodRequest>";
				// ATMLocations service
				// string strInput = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ExecServiceMethodRequest><TreatyID>4</TreatyID><ServiceName>GeoCash</ServiceName><MethodName>GetATMLocations</MethodName><Parameter><![CDATA[<?xml version=\"1.0\"?><string>10025</string>]]></Parameter></ExecServiceMethodRequest>";

				//GateKeeper g = new GateKeeper();
				//string strReturn = g.ExecuteServiceMethod( strInput );

				//XmlDocument doc = new XmlDocument();
				//doc.LoadXml( strReturn );

				//XmlNode root = doc.DocumentElement;
				
				//string strData = root.InnerText;
				
				//string strAlpha = "<?xml version=\"1.0\" encoding=\"utf-8\"?><definitions name=\"DemoAlpha\" targetNamespace=\"http://psl.cs.columbia.edu\" xmlns:xlang=\"http://schemas.microsoft.com/bixtalk/xlang\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"><!-- Simple demo using simple xlang constructs --><xlang:behavior><xlang:body><xlang:sequence><xlang:action activation=\"true\" gatekeeper=\"PSLGatekeeper1\" servicename=\"QuoteBinding\" operation=\"getPrice\"><parameter><![CDATA[<?xml version=\"1.0\"?><string>1861005458</string>]]></parameter></xlang:action><xlang:action gatekeeper=\"PSLGatekeeper1\" servicename=\"GeoCash\" operation=\"GetATMLocations\"><parameter><![CDATA[<?xml version=\"1.0\"?><string>10025</string>]]></parameter></xlang:action><xlang:action gatekeeper=\"PSLGatekeeper1\" servicename=\"XMethodsQuery\" operation=\"getAllServiceSummaries\"></xlang:action></xlang:sequence></xlang:body></xlang:behavior></definitions>";
				//GateKeeper g = new GateKeeper();
				//string [] arrRes = g.ExecuteAlphaProtocol( strAlpha );
				
				/*ProxyGen pGen = new ProxyGen();
				DynamicRequest req = new DynamicRequest();
				req.baseURL = "http://localhost/PSLGatekeeper2/PSLGatekeeper2.asmx";
				req.filenameSource = "PSLGatekeeper2";
				req.serviceName = req.filenameSource;
				req.wsdlFile = "http://localhost/PSLGatekeeper2/PSLGatekeeper2.asmx?WSDL";
				req.proxyPath = "C:\\temp\\pxyCache";

				string strAssembly = pGen.GenerateAssembly( req );
				*/
				/*
				PSLGatekeeper2 g = new PSLGatekeeper2();
				string[] arrRes = g.ExecuteAlphaProtocol( strAlpha );
				*/
				
				/*ProxyGen pGen = new ProxyGen();
				DynamicRequest req = new DynamicRequest();
				req.baseURL = "http://church.psl.cs.columbia.edu:8080/security/jaxrpc/SecurityManagerService";
				req.filenameSource = "SecurityManagerService";
				req.serviceName = req.filenameSource;
				req.wsdlFile = "http://church.psl.cs.columbia.edu:8080/security/SecurityServices.wsdl";
				req.proxyPath = "C:\\temp\\pxyCache";
				
				string strAssembly = pGen.GenerateAssembly( req );*/

				/*InternalRegistry ireg = new InternalRegistry();
				int nServiceID = ireg.RegisterService( "SecurityManagerService", "", "http://church.psl.cs.columbia.edu:8080/security/SecurityServices.wsdl", "http://church.psl.cs.columbia.edu:8080/security/jaxrpc/SecurityManagerService" ); 
				if( nServiceID != -1 )
					ireg.RegisterServiceMethod( "SecurityManagerService", "verifyTreaty" );
				*/
				
				//string strTreaty = "<?xml version=\"1.0\" ?><Treaty xmlns=\"http://localhost/Discus/Schema/Treaty.xsd\"><TreatyID>1000</TreatyID><ClientServiceSpace>myservicespace</ClientServiceSpace><ProviderServiceSpace>providerss</ProviderServiceSpace><ServiceInfo><ServiceName>service</ServiceName><ServiceMethod><MethodName>method</MethodName><Parameter>foo</Parameter><Parameter>bar</Parameter><NumInvokations>1</NumInvokations><Authorized>true</Authorized></ServiceMethod></ServiceInfo></Treaty>";
				string strInput = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ExecServiceMethodRequest><TreatyID>1000</TreatyID><ServiceName>SecurityManagerService</ServiceName><MethodName>verifyTreaty</MethodName><Parameter><![CDATA[<?xml version=\"1.0\"?><string><?xml version=\"1.0\"?><Treaty xmlns=\"http://localhost/Discus/Schema/Treaty.xsd\"><TreatyID>1000</TreatyID><ClientServiceSpace>myservicespace</ClientServiceSpace><ProviderServiceSpace>providerss</ProviderServiceSpace><ServiceInfo><ServiceName>service</ServiceName><ServiceMethod><MethodName>method</MethodName><Parameter>foo</Parameter><Parameter>bar</Parameter><NumInvokations>1</NumInvokations><Authorized>true</Authorized></ServiceMethod></ServiceInfo></Treaty></string>]]></Parameter></ExecServiceMethodRequest>";
				GateKeeper g = new GateKeeper();
				string strRes = g.ExecuteServiceMethod( strInput );

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
