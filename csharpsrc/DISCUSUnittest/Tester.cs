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


namespace DISCUSUnittest
{
	class ExecutionRequest
	{
		public ExecutionRequest()
		{
			m_Params = new ArrayList();
			m_bServicedLocally = false;
			m_nTreatyID = -1; // TreatyID exec request assoc with
		}

		public string m_strGKName;
		public string m_strServiceName;
		public string m_strMethod;
		public ArrayList m_Params;
		public int m_nTreatyID;
		public bool m_bServicedLocally;
	};
	
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
				/*InternalRegistry ireg = new InternalRegistry();
				bool bStatus = false;
				int nServiceID = -1;
			
				// Register BookQuotes service
				nServiceID = ireg.RegisterService( "QuoteBinding", "","http://www.xmethods.net/tmodels/BookQuote.wsdl","http://services.xmethods.net:80/soap/servlet/rpcrouter" ); 
				if( nServiceID != -1 )
					bStatus = ireg.RegisterServiceMethod( "QuoteBinding", "getPrice" );

				nServiceID =  ireg.RegisterService( "GeoCash", "", "http://64.78.60.122/GeoCash.asmx?WSDL", "http://64.78.60.122/GeoCash.asmx" );
				if( nServiceID != -1 )
					bStatus = ireg.RegisterServiceMethod( "GeoCash", "GetATMLocations" );
							
				nServiceID = ireg.RegisterService( "XMethodsQuery", "", "http://www.xmethods.net/wsdl/query.wsdl","http://www.xmethods.net/interfaces/query" );
				if( nServiceID != -1 )
					bStatus = ireg.RegisterServiceMethod( "XMethodsQuery","getAllServiceSummaries" );
				
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
				}*/
				
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
				
				// Execute Alpha protocol from string
				Queue execQ = new Queue();
				string strAlpha = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><definitions name=\"DemoAlpha\" targetNamespace=\"http://psl.cs.columbia.edu\" xmlns:xlang=\"http://schemas.microsoft.com/bixtalk/xlang\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"><!-- Simple demo using simple xlang constructs --><xlang:behavior><xlang:body><xlang:sequence><xlang:action activation=\"true\" gatekeeper=\"PSLGateKeeper\" servicename=\"QuoteBinding\" operation=\"getPrice\"><parameter type=\"xs:string\">1861005458</parameter></xlang:action><xlang:action gatekeeper=\"PSLGateKeeper\" servicename=\"GeoCash\" operation=\"GetATMLocations\"><parameter type=\"xs:string\">10025</parameter></xlang:action><xlang:action gatekeeper=\"PSLGateKeeper\" servicename=\"XMethodsQuery\" operation=\"getAllServiceSummaries\"></xlang:action></xlang:sequence></xlang:body></xlang:behavior></definitions>";
				XmlTextReader xt = new XmlTextReader( strAlpha, XmlNodeType.Document, null );

				// Execute an alpha protocol from file
				// XmlTextReader xt = new XmlTextReader( "DemoAlpha.xml" );
				// Add namespaces expected in alpha protocol so our xpath queries work
				XmlNamespaceManager mgr = new XmlNamespaceManager( xt.NameTable );
				mgr.AddNamespace( "xlang", "http://schemas.microsoft.com/bixtalk/xlang" );
				mgr.AddNamespace( "xs", "http://www.w3.org/2001/XMLSchema" );
								
				XmlDocument doc = new XmlDocument();
				doc.Load( xt );
				
				XmlNode root = doc.DocumentElement;
				XmlNode seq = root.SelectSingleNode( "//xlang:sequence", mgr );
				
				// Check whether first child in sequence starts an activation
				// if not sequence is invalid
				
				// Get all children of sequence, these are the actions to be carried out
				if( seq.HasChildNodes )
				{
					XmlNode startNode = seq.FirstChild;
					XmlAttributeCollection attCol = startNode.Attributes;
					XmlNode activation = attCol.GetNamedItem( "activation" );
					if( activation == null || activation.Value.ToLower().CompareTo( "true" ) != 0 )
						return;  //invalid sequence
					
					// Create list of actions
					XmlNodeList lstActions = seq.ChildNodes;
					// For each action create an execution request
					// and add to a queue
					foreach( XmlNode n in lstActions )
					{
						XmlAttributeCollection att = n.Attributes;
						if( att.Count > 0 )
						{
							ExecutionRequest e = new ExecutionRequest();
							XmlNode val = att.GetNamedItem( "gatekeeper" );
							if( val != null )
								e.m_strGKName = val.Value;
							val = att.GetNamedItem( "servicename" );
							if( val != null )
								e.m_strServiceName = val.Value;
							val = att.GetNamedItem( "operation" );
							if( val != null )
								e.m_strMethod = val.Value;
							if( n.HasChildNodes )
							{
								XmlNodeList lstParams = n.ChildNodes;
								foreach( XmlNode p in lstParams )
								{
									XmlAttributeCollection paramAtt = p.Attributes;
									XmlNode type = paramAtt.GetNamedItem( "type" );
									if( type == null )
										return; //all specified params must have a (simeple xsd) type 
									string strType = type.Value;

									// Do mapping from (xsd type) xs:xyz to system type
									if( strType.ToLower().IndexOf( "xs:string" ) != -1 )
									{
										Object obj = p.InnerText;
										e.m_Params.Add( obj );
									}
									if( strType.ToLower().IndexOf( "xs:int" ) != -1 )
									{
										Object obj = Int32.Parse( p.InnerText );
										e.m_Params.Add( obj );
									}
								}
							}
							// Add request to execution queue
							execQ.Enqueue( e );
						}
					}

					// Determine whether requests can be serviced locally
					// Need to create treaty/treaties with all
					// GateKeepers that must be contacted
					// Need to copy execution Q to a list, sort by GKName
					// & create a consolidated treaty to be passed to
					// any external GK requesting use of its services
										
					// Service each execution request
					IEnumerator it = execQ.GetEnumerator();
					while( it.MoveNext() )
					{
						ExecutionRequest e = (ExecutionRequest) it.Current;
						// Get proxy to relevant gatekeeper
						// Assusme g is our proxy object
						// we'd use reflection on proxy to GK
						GateKeeper g = new GateKeeper();
						Object res = null;
						if( e.m_Params.Count > 0 )
							res = g.ExecuteServiceMethod( e.m_nTreatyID, e.m_strServiceName, e.m_strMethod, e.m_Params.ToArray() );
						else res = g.ExecuteServiceMethod( e.m_nTreatyID, e.m_strServiceName, e.m_strMethod, null );

						int y = 9;
						y++;
					}
				}

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
