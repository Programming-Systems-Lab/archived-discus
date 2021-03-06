using System;
using System.IO;
using PSL.DISCUS.Impl.DataAccess;
using PSL.DISCUS.Impl.GateKeeper;
using System.Collections;

using System.Net;
using System.Net.Sockets;
using PSL.DISCUS.DAML;

namespace DISCUSUnittest
{	
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Tester
	{
		/* Register services an known gatekeepers */
		/* NOTE : The service name MUST match the actual classname [ref: WSDL doc]*/
		
		/*public static void SetupServiceSpace()
		{
			InternalRegistry ireg = new InternalRegistry();
			int nServiceID = -1;
			bool bStatus = false;

			// TODO : weatherConditions --> for BALT-DEMO-2002
			nServiceID = ireg.RegisterService( "WeatherRetriever", "" , "http://www.vbws.com/services/weatherretriever.asmx?WSDL", "http://www.vbws.com/services/weatherretriever.asmx" );
			if( nServiceID != -1 )
			{
				// register methods before you start using them
				bStatus = ireg.RegisterServiceMethod( "WeatherRetriever", "GetTemperature" );
				bStatus = ireg.RegisterServiceMethod( "WeatherRetriever", "GetWeather" );
			}

			// TODO: Traffic conditions in California
			nServiceID = ireg.RegisterService( "CATrafficService", "" , "http://www.xmethods.net/sd/2001/CATrafficService.wsdl", "http://services.xmethods.net:80/soap/servlet/rpcrouter" );
			if( nServiceID != -1 )
			{
				// register methods before you start using them
				bStatus = ireg.RegisterServiceMethod( "CATrafficService", "getTraffic" );
			}

			// TODO: IP2Location Translator
			nServiceID = ireg.RegisterService("GeoPinPoint", "" , "http://ws.serviceobjects.net/gpp/GeoPinPoint.asmx?WSDL ", "http://ws.serviceobjects.net/gpp/GeoPinPoint.asmx" );
			if( nServiceID != -1 )
			{
				// register methods before you start using them
				bStatus = ireg.RegisterServiceMethod( "GeoPinPoint", "GetLocation" );
			}

			// TODO: ZipCode Resolver
			nServiceID = ireg.RegisterService("ZipCodeResolver", "" , "http://webservices.eraserver.net/zipcoderesolver/zipcoderesolver.asmx?WSDL", "http://webservices.eraserver.net/zipcoderesolver/zipcoderesolver.asmx" );
			if( nServiceID != -1 )
			{
				// register methods before you start using them
				bStatus = ireg.RegisterServiceMethod( "ZipCodeResolver", "FullZipCode" );
			}

			// Google Search
			nServiceID = ireg.RegisterService( "GoogleSearchService", "", "http://api.google.com/GoogleSearch.wsdl", "http://api.google.com/search/beta2" );
			if( nServiceID != -1 )
			{
				bStatus = ireg.RegisterServiceMethod( "GoogleSearchService", "doGoogleSearch" );
			}
			
			// GeoCash
			nServiceID = ireg.RegisterService( "GeoCash", "",  "http://ws.serviceobjects.net/gc/GeoCash.asmx?WSDL", "http://ws.serviceobjects.net/gc/GeoCash.asmx" );
			if( nServiceID != -1 )
			{
				bStatus = ireg.RegisterServiceMethod( "GeoCash", "GetATMLocations" );
			}

			// BNQuoteService
			nServiceID = ireg.RegisterService( "BNQuoteService", "", "http://www.xmethods.net/sd/2001/BNQuoteService.wsdl", "http://services.xmethods.net:80/soap/servlet/rpcrouter" );
			if( nServiceID != -1 )
			{
				bStatus = ireg.RegisterServiceMethod( "BNQuoteService", "getPrice" );
			}

			// WorldCup Scores
			nServiceID = ireg.RegisterService( "CupScores", "", "http://64.78.60.122/CupScores.asmx?WSDL", "http://64.78.60.122/CupScores.asmx" );
			if( nServiceID != -1 )
			{
				bStatus = ireg.RegisterServiceMethod( "CupScores", "GetScores" ); 			
			}

			// Randon Neil Finn Lyric Server
			nServiceID = ireg.RegisterService( "finnwordsService", "", "http://www.nickhodge.com/nhodge/finnwords/finnwords.wsdl", "http://www.nickhodge.com/nhodge/finnwords/finnwordssoapengine.php" );
			if( nServiceID != -1 )
			{
				bStatus = ireg.RegisterServiceMethod( "finnwordsService", "getRandomNeilFinnLyric" );
			}

			// Spanish Joke Server, input = "ES"
			nServiceID = ireg.RegisterService( "JokeServer", "", "http://www.xml-webservices.net/services/entretainment/joke_server.asmx?WSDL", "http://www.xml-webservices.net/services/entretainment/joke_server.asmx" );
			if( nServiceID != -1 )
			{
				bStatus = ireg.RegisterServiceMethod( "JokeServer", "JokeToString" );	
			}

			// XMethods Query
			nServiceID = ireg.RegisterService( "XMethodsQuery", "", "http://www.xmethods.net/wsdl/query.wsdl", "http://www.xmethods.net/interfaces/query" );
			if( nServiceID != -1 )
			{
				bStatus = ireg.RegisterServiceMethod( "XMethodsQuery", "getAllServiceSummaries" );
				bStatus = ireg.RegisterServiceMethod( "XMethodsQuery", "getServiceNamesByPublisher" );
			}

			// SecurityManager Service
			nServiceID = ireg.RegisterService( "SecurityManagerService", "", "http://church.psl.cs.columbia.edu:8080/security/SecurityServices.wsdl", "http://church.psl.cs.columbia.edu:8080/security/jaxrpc/SecurityManagerService" );
			if( nServiceID != -1 )
			{
				bStatus = ireg.RegisterServiceMethod( "SecurityManagerService", "doRequestCheck" );
				bStatus = ireg.RegisterServiceMethod( "SecurityManagerService", "signDocument" );
				bStatus = ireg.RegisterServiceMethod( "SecurityManagerService", "verifyDocument" );
				bStatus = ireg.RegisterServiceMethod( "SecurityManagerService", "verifyTreaty" );
				bStatus = ireg.RegisterServiceMethod( "SecurityManagerService", "verifyTreaty2" );
			}
		}
		*/
		public static void ResetServiceLocations()
		{
			InternalRegistry ireg = new InternalRegistry();
			// TODO: weatherConditions
			ireg.UpdateServiceLocation( "WeatherRetriever", "http://www.vbws.com/services/weatherretriever.asmx?WSDL" );
			// Google Search
			ireg.UpdateServiceLocation( "GoogleSearchService", "http://api.google.com/GoogleSearch.wsdl" );
			// GeoCash
			ireg.UpdateServiceLocation( "GeoCash", "http://ws.serviceobjects.net/gc/GeoCash.asmx?WSDL" );
			// BNQuoteService
			ireg.UpdateServiceLocation( "BNQuoteService", "http://www.xmethods.net/sd/2001/BNQuoteService.wsdl" );
			// WorldCup Scores
			ireg.UpdateServiceLocation( "CupScores", "http://64.78.60.122/CupScores.asmx?WSDL" );
			// Randon Neil Finn Lyric Server
			ireg.UpdateServiceLocation( "finnwordsService", "http://www.nickhodge.com/nhodge/finnwords/finnwords.wsdl" );
			// Joke Server
			ireg.UpdateServiceLocation( "JokeServer", "http://www.xml-webservices.net/services/entretainment/joke_server.asmx?WSDL" );
			// XMethods Query
			ireg.UpdateServiceLocation( "XMethodsQuery", "http://www.xmethods.net/wsdl/query.wsdl" );
			// SecurityManager Service
			ireg.UpdateServiceLocation( "SecurityManagerService", "http://church.psl.cs.columbia.edu:8080/security/SecurityServices.wsdl" );
		}

		public static void GenerateAllProxies()
		{
			
			GateKeeper g = new GateKeeper();
			
			// Generate Proxies for all the web services we want to interact with
			// TODO: WeatherConditions
			g.GenerateProxy( "WeatherRetriever", "http://www.vbws.com/services/weatherretriever.asmx?WSDL", "http://www.vbws.com/services/weatherretriever.asmx" );
			// Google Search
			g.GenerateProxy( "GoogleSearchService", "http://api.google.com/GoogleSearch.wsdl", "http://api.google.com/search/beta2" );
			// GeoCash
			g.GenerateProxy( "GeoCash", "http://ws.serviceobjects.net/gc/GeoCash.asmx?WSDL", "http://ws.serviceobjects.net/gc/GeoCash.asmx" );
			// BNQuoteService
			g.GenerateProxy( "BNQuoteService", "http://www.xmethods.net/sd/2001/BNQuoteService.wsdl", "http://services.xmethods.net:80/soap/servlet/rpcrouter" );
			// WorldCup Scores
			g.GenerateProxy( "CupScores", "http://64.78.60.122/CupScores.asmx?WSDL", "http://64.78.60.122/CupScores.asmx" );
			// Randon Neil Finn Lyric Server
			g.GenerateProxy( "finnwordsService", "http://www.nickhodge.com/nhodge/finnwords/finnwords.wsdl", "http://www.nickhodge.com/nhodge/finnwords/finnwordssoapengine.php" );
			// Joke Server
			g.GenerateProxy( "JokeServer", "http://www.xml-webservices.net/services/entretainment/joke_server.asmx?WSDL", "http://www.xml-webservices.net/services/entretainment/joke_server.asmx" );
			// XMethods Query
			g.GenerateProxy( "XMethodsQuery", "http://www.xmethods.net/wsdl/query.wsdl", "http://www.xmethods.net/interfaces/query" );
			// SecurityManager Service
			g.GenerateProxy( "SecurityManagerService", "http://church.psl.cs.columbia.edu:8080/security/SecurityServices.wsdl", "http://church.psl.cs.columbia.edu:8080/security/jaxrpc/SecurityManagerService" );
		}
		
		public static void TestServiceMethods()
		{
			// GateKeeper instance
			GateKeeper g = new GateKeeper();
			g.TraceOn = false;
			object objRes = null;
			//InternalRegistry ireg = new InternalRegistry();
			//ireg.UpdateServiceLocation( "SecurityManagerService", "http://church.psl.cs.columbia.edu:8080/security/SecurityServices.wsdl" );


			// SetupServiceSpace();
			ExecServiceMethodRequestType e = new ExecServiceMethodRequestType();
			
			// TODO: WeatherConditions testing webservice
			e.TreatyID = -1820085390;//-115276743;
			e.ServiceName = "WeatherRetriever";
			e.MethodName = "GetTemperature";
			e.MethodName = "GetWeather";
			e.m_ParamValue.Clear();
			e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>10027</string>" );
			 
			objRes = g.ExecuteServiceMethod( e.ToXml() );
		
			// TODO: Traffic Conditions in Chicago testing webservice
			/* e.TreatyID = -1820085390;//-115276743;
			e.ServiceName = "CATrafficService";
			e.MethodName = "getTraffic";
			e.m_ParamValue.Clear();
			e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>209</string>" );
			
			objRes = g.ExecuteServiceMethod( e.ToXml() );			
			Console.Write(objRes.ToString());

			// TODO: Query for 2 different Highways
			e.m_ParamValue.Clear();
			e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>5</string>" );

			objRes = g.ExecuteServiceMethod( e.ToXml() );			
			Console.Write(objRes.ToString());
			
			
			e.TreatyID = -1820085390;//-115276743;
			e.ServiceName = "GeoPinPoint";
			e.MethodName = "GetLocation";
			e.m_ParamValue.Clear();
			e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>128.59.23.57</string>" );
			e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>0</string>" ); // license key
			

			e.TreatyID = -1820085390;//-115276743;
			e.ServiceName = "ZipCodeResolver";
			e.MethodName = "FullZipCode";
			e.m_ParamValue.Clear();
			e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>0</string>" ); // license key
			e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>500 West 120th Street</string>" );
			e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>New York</string>" );
			e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>NY</string>" );

			objRes = g.ExecuteServiceMethod( e.ToXml() );			
			Console.Write(objRes.ToString());
			
			//objRes = g.EnlistServicesByName( "<?xml version=\"1.0\"?><Treaty xmlns=\"http://localhost/Discus/Schema/Treaty.xsd\"><TreatyID>1000</TreatyID><ClientServiceSpace>myservicespace</ClientServiceSpace><ProviderServiceSpace>providerss</ProviderServiceSpace><ServiceInfo><ServiceName>service</ServiceName><ServiceMethod><MethodName>method</MethodName><Parameter>foo</Parameter><Parameter>bar</Parameter><NumInvokations>1</NumInvokations><Authorized>true</Authorized></ServiceMethod></ServiceInfo></Treaty>" );
			
			// Sql script for service invocation permission table
			// insert into serviceinvokationpermission values(100,'BNQuoteService','getPrice','isbn',100000,'getPrice');
			// insert into serviceinvokationpermission values(100,'XMethodsQuery','getAllServiceSummaries','',100000,'getAllServiceSummaries');
			// insert into serviceinvokationpermission values(100,'XMethodsQuery','getServiceNamesByPublisher','PubName',100000,'getServiceNamesByPublisher');
			// insert into serviceinvokationpermission values(100,'GeoCash','GetATMLocations','Zipcode',100000,'GetATMLocations');
			// GeoCash
			//e.ServiceName = "GeoCash";
			//e.MethodName = "GetATMLocations";
			//e.m_ParamValue.Clear();
			//e.m_ParamValue.Add( "<?xml version=\"1.0\"?><string>10025</string>" );
			//objRes = g.ExecuteServiceMethod( e.ToXml() );

			// Google*/
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				//ResetServiceLocations();
				// TestServiceMethods();
				
				//FileStream fs = new FileStream( "CongoProfile.txt", FileMode.Open );
				FileStream fs = new FileStream( "CongoProcess.txt", FileMode.Open );
				StreamReader s = new StreamReader( fs );
				string strDAML = s.ReadToEnd();

				DAMLProcessModel process = new DAMLProcessModel();
				process.LoadXml( strDAML );
				
				string[] arrProcessNames = process.AtomicProcesses;
				DAMLProcess[] arrAtomicProcesses = new DAMLProcess[arrProcessNames.Length];

				for( int i = 0; i < arrProcessNames.Length; i++ )
				{
					arrAtomicProcesses[i] = process.GetProcessData( arrProcessNames[i], enuProcessType.AtomicProcess );
				}
				
				// Simple execution scenario
				// define IExecuteDAMLProcess
				// single method Execute(DAMLParameters[] arrParams) - hashtable better
				// Parameter map
				// organized by types of inputs

											
				DAMLProcess res = process.GetProcessData( "CongoBuy", enuProcessType.SimpleProcess );
								
				


				
				//DAMLServiceProfile profile = new DAMLServiceProfile();
				// Load DAML from web/file too big to hardcode in source file
				// exceeds 2K compiler limit
				//profile.LoadXml( strDAML );
				
				//string strRes = profile.ProcessModel;
				//string[] arrRes = profile.OntologyImports;
				//strRes = profile.GeographicRadius;
				//strRes = profile.PresentedBy;
				//IOType[] arrInputs = profile.InputParameters;
				//IOType[] arrOutputs = profile.OutputParameters;
				//EPType[] arrPreconds = profile.Preconditions;
				//EPType[] arrEffects = profile.Effects;

				// Valid combinations
				//string strRes = "";
				
				//strRes = profile.IOPERefersTo( enuIOPEType.Input, enuIOPESearchBy.PARAM_NAME, "packagingSelection" );
				//strRes = profile.IOPERefersTo( enuIOPEType.Input, enuIOPESearchBy.PARAM_DESC, "PackagingSelection" );
				
				//IOType i = profile.GetInputByReference( "http://www.daml.org/services/daml-s/2001/10/Congo.daml#packagingSelection" );
				//i = profile.GetOutputByReference( "http://www.daml.org/services/daml-s/2001/10/Congo.daml#createAcctOutput" );
				//EPType k = profile.GetPreconditionByReference( "http://www.daml.org/services/daml-s/2001/10/Congo.daml#congoBuyCreditExistsPrecondition" );
				//k = profile.GetEffectByReference( "http://www.daml.org/services/daml-s/2001/10/Congo.daml#congoBuyEffect" );

				//IOType j = profile.GetInputByDescription( "CreditCardNumber" );
				
				/*
				strRes = profile.IOPERefersTo( enuIOPEType.Input, enuIOPESearchBy.PARAM_DESC, "xyz" );
				strRes = profile.IOPERefersTo( enuIOPEType.Input, enuIOPESearchBy.PARAM_NAME, "xyz" );
				strRes = profile.IOPERefersTo( enuIOPEType.Output, enuIOPESearchBy.PARAM_DESC, "xyz" );
				strRes = profile.IOPERefersTo( enuIOPEType.Output, enuIOPESearchBy.PARAM_NAME, "xyz" );
				strRes = profile.IOPERefersTo( enuIOPEType.Precondition, enuIOPESearchBy.COND_DESC, "xyz" );
				strRes = profile.IOPERefersTo( enuIOPEType.Precondition, enuIOPESearchBy.COND_DESC, "xyz" );
				strRes = profile.IOPERefersTo( enuIOPEType.Effect, enuIOPESearchBy.COND_NAME, "xyz" );
				strRes = profile.IOPERefersTo( enuIOPEType.Effect, enuIOPESearchBy.COND_NAME, "xyz" );
				*/
				// Invalid combinations - should cause an exception to be thrown
				//strRes = profile.IOPERefersTo( enuIOPEType.Input, enuIOPESearchBy.COND_DESC, "xyz" );
				//strRes = profile.IOPERefersTo( enuIOPEType.Input, enuIOPESearchBy.COND_NAME, "xyz" );
				//strRes = profile.IOPERefersTo( enuIOPEType.Output, enuIOPESearchBy.COND_DESC, "xyz" );
				//strRes = profile.IOPERefersTo( enuIOPEType.Output, enuIOPESearchBy.COND_NAME, "xyz" );
				//strRes = profile.IOPERefersTo( enuIOPEType.Precondition, enuIOPESearchBy.PARAM_DESC, "xyz" );
				//strRes = profile.IOPERefersTo( enuIOPEType.Precondition, enuIOPESearchBy.PARAM_NAME, "xyz" );
				//strRes = profile.IOPERefersTo( enuIOPEType.Effect, enuIOPESearchBy.PARAM_DESC, "xyz" );
				//strRes = profile.IOPERefersTo( enuIOPEType.Effect, enuIOPESearchBy.PARAM_NAME, "xyz" );


				
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
