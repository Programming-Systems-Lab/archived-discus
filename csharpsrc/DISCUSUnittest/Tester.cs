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
				
				// Exec request with input paramters 
				// XMethodsQuery - getServiceNamesByPublisher
				// string strInput = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ExecServiceMethodRequest><TreatyID>4</TreatyID><ServiceName>XMethodQuery</ServiceName><MethodName>getServiceNamesByPublisher</MethodName><Parameter><![CDATA[<?xml version=\"1.0\"?><string>Interdata</string>]]></Parameter></ExecServiceMethodRequest>";
				// Exec request no input parameters
				// XMethodsQuery - getAllServiceSummaries
				// string strInput = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ExecServiceMethodRequest><TreatyID>4</TreatyID><ServiceName>XMethodsQuery</ServiceName><MethodName>getAllServiceSummaries</MethodName></ExecServiceMethodRequest>";
				// QuoteBindingService
				// string strInput = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ExecServiceMethodRequest><TreatyID>4</TreatyID><ServiceName>QuoteBinding</ServiceName><MethodName>getPrice</MethodName><Parameter><![CDATA[<?xml version=\"1.0\"?><string>1861005458</string>]]></Parameter></ExecServiceMethodRequest>";
				// ATMLocations service
				string strInput = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ExecServiceMethodRequest><TreatyID>4</TreatyID><ServiceName>GeoCash</ServiceName><MethodName>GetATMLocations</MethodName><Parameter><![CDATA[<?xml version=\"1.0\"?><string>10025</string>]]></Parameter></ExecServiceMethodRequest>";

				GateKeeper g = new GateKeeper();
				string strReturn = g.ExecuteServiceMethod( strInput );

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
