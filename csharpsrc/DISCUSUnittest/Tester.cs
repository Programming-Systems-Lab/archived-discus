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
			
				// Register BookQuotes service
				nServiceID = s.RegisterService( "QuoteBinding", "","http://www.xmethods.net/tmodels/BookQuote.wsdl","http://services.xmethods.net:80/soap/servlet/rpcrouter" ); 
				if( nServiceID != -1 )
					bStatus = s.RegisterServiceMethod( "QuoteBinding", "getPrice" );

				nServiceID =  s.RegisterService( "GeoCash", "", "http://64.78.60.122/GeoCash.asmx?WSDL", "http://64.78.60.122/GeoCash.asmx" );
				if( nServiceID != -1 )
					bStatus = s.RegisterServiceMethod( "GeoCash", "GetATMLocations" );
							
				nServiceID = s.RegisterService( "XMethodsQuery", "", "http://www.xmethods.net/wsdl/query.wsdl","http://www.xmethods.net/interfaces/query" );
				if( nServiceID != -1 )
					bStatus = s.RegisterServiceMethod( "XMethodsQuery","getAllServiceSummaries" );
				
				GateKeeper g = new GateKeeper();
				Object [] objParams = new Object[1];
				objParams[0] = "1861005458";
				float fltPrice = (float) g.ExecuteServiceMethod( 4, "QuoteBinding", "getPrice", objParams );
				  
				Console.Write( "ISBN: " );
				Console.Write( (string) objParams[0] );
				Console.Write( " Price = " );
				Console.WriteLine( fltPrice );
				Console.Write( "\n" );

				objParams[0] = "10025";
				string strLocations = (string) g.ExecuteServiceMethod( 4, "GeoCash", "GetATMLocations", objParams );
				Console.Write( "ATM Locations: " );
				Console.WriteLine( strLocations );
				Console.Write( "\n" );

				object res = g.ExecuteServiceMethod( 4, "XMethodsQuery", "getAllServiceSummaries", null );
				System.Array arrRes = (System.Array) res;

				for( int i = 0; i < arrRes.Length; i++ )
				{
					Type t = arrRes.GetValue(i).GetType();
					
					FieldInfo fName = t.GetField( "name" );
					FieldInfo fid = t.GetField( "id" );
					FieldInfo fDesc = t.GetField( "shortDescription" );
					FieldInfo fWSDL = t.GetField( "wsdlURL" );
					FieldInfo fPub = t.GetField( "publisherID" );

					Console.WriteLine( "Service name: " + (string) fName.GetValue( arrRes.GetValue(i) ) );
					Console.WriteLine( "Service ID: " + (string) fid.GetValue( arrRes.GetValue(i) ) );
					Console.WriteLine( "Desc: " + (string) fDesc.GetValue( arrRes.GetValue(i) ) );
					Console.WriteLine( "WSDL: " + (string) fWSDL.GetValue( arrRes.GetValue(i) ) );
					Console.WriteLine( "PubID: " + (string) fPub.GetValue( arrRes.GetValue(i) ) );
					Console.Write( "\n" );
				}
				
				/*
				 * Currently using services implemented/hosted on Apache, GLUE and .NET
				 * need to add services implemented using Delphi and hosted on WASP/Tomcat
				 * Need to add added reflection code.
				 * 
				 * 1) Facilitate extreme use of reflection on returned types
				 * 2) Create web service wrapper for gatekeeper
				 * 3) Integrate the security manager implementation with the gatekeeper
				 * 
				 * */
			}
			catch( System.Exception e )
			{
				string strMsg = e.Message;
			}
		}
	}
}
