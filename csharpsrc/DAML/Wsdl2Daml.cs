using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Services;
using System.Web.Services.Description;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.CSharp;
using PSL.DAML;

namespace PSL.DAML.Tools
{
	/// <summary>
	/// Given a wsdl file, build a daml process model consisting of 
	/// daml atomic processes
	/// </summary>
	public class Wsdl2DamlGen
	{
		public const string PROTOCOL = "SOAP";
		public const string AUTO_GEN_NAMESPACE = "Wsdl2Daml";

		public Wsdl2DamlGen()
		{
		}

		public static string GenerateDaml( string strWsdlUrl )
		{
			// Read in Service Description from url
			ServiceDescription svcDesc = ServiceDescription.Read( Wsdl2DamlGen.GetHttpStream( strWsdlUrl ) );	

			ServiceCollection svcColl = svcDesc.Services;

			// Get the services in the service Description collection
			// Expect only one
			// for the first one create a DamlProcess instance (atomic process)
			// for each method in the web service
						
			if( svcColl.Count == 0 )
				return "";

			Service svc = svcColl[0];
						
			// Generate assembly in memory representing web service proxy
			// use reflection on it to get method data, inputs, outputs etc
			// For each method we create a Daml atomic process
			// and add data for its inputs, and outputs at least
			// no provision to add data on preconditions etc.
			// since more information would be needed than we get from
			// the wsdl
			Assembly proxyAssembly = GenerateAssembly( ref svcDesc );
			
			if( proxyAssembly == null )
				throw new Exception( "Error generating in memory web service proxy assembly" );
			
			string strServiceName = Wsdl2DamlGen.AUTO_GEN_NAMESPACE + "." + svc.Name;
			
			DamlProcessModelWriter damlWriter = new DamlProcessModelWriter();

			// Get all the types defined in the assembly
			Type[] arrTypes = proxyAssembly.GetTypes();
			// Get the type representing the web service		
			Type proxyType = proxyAssembly.GetType( strServiceName, true );
			// Ask for all its methods, these are our daml atomic process
			// We only want the public instance methods declared at this type's level
			// we are not interested in any inherited methods
			MethodInfo[] arrMethods = proxyType.GetMethods( BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly );
			
			for( int i = 0; i < arrMethods.Length; i++ )
			{
				// Get the current method 
				MethodInfo currMethod = arrMethods[i];
				// Get the parameters expected by this method
				ParameterInfo[] arrParams = currMethod.GetParameters();
				// Get the return type of this method
				Type returnType = currMethod.ReturnType;

				// Flags whether we ignore or process this method
				bool bIgnoreMethod = false;
				
				// Ignore constructors and async methods
				if( currMethod.IsConstructor || currMethod.ReturnType == typeof(System.IAsyncResult) )
					continue;

				for( int j = 0; j < arrParams.Length; j++ )
				{
					if( arrParams[j].ParameterType == typeof(System.IAsyncResult) )
					{
						bIgnoreMethod = true;
						break;
					}
				}

				if( bIgnoreMethod )
					continue;

				// We do not want any of the async methods
				// filter based on paramters
				// Basically any method that expects or returns
				// System.IAsyncResult
				// neither do we want the constructor(s)
				
				DamlProcess process = new DamlProcess();
				process.Name = currMethod.Name;

				// Get the input and output types and add them to the process
				for( int x = 0; x < arrParams.Length; x++ )
				{
					RdfProperty input = new RdfProperty();

					// Set the name of the input
					input.Name = arrParams[x].Name;
					// Set domain - name of methd
					input.Domain = process.Name;
					// Set subproperty - set as a subProperty of Process Inputs
					input.SubPropertyOf = DamlConstants.PROCESS_INPUT_URI;
					
					// Map .NET type to DamlTypes - strings, decimals, daml+oil#Thing
					input.Range = "http://www.daml.org/2001/03/daml+oil#Thing";

					process.AddInput( input );

					// Create input restriction
					DamlRestriction restriction = new DamlRestriction();
					// Set the cardinality
					restriction.Cardinality = 1;
					// Set the owner
					restriction.Owner = input.Domain;
					// Specify the property to which the restriction applies
					restriction.OnProperty = "#" + input.Name;
					
					// Add restriction to process
					process.AddRestriction( enuIOPEType.Input, new DamlRestriction[] { restriction } );
				}
			
				// Add output to process
				if( returnType.FullName != (typeof(void).FullName) )
				{
					RdfProperty output = new RdfProperty();

					// Set the name of the input
					output.Name = process.Name + "Out";
					// Set domain - name of methd
					output.Domain = process.Name;
					// Set subproperty - set as a subProperty of Process Inputs
					output.SubPropertyOf = DamlConstants.PROCESS_OUTPUT_URI;
					
					// Map .NET type to DamlTypes - strings, decimals, daml+oil#Thing
					output.Range = "http://www.daml.org/2001/03/daml+oil#Thing";

					process.AddOutput( output );
				}
				
				// Add the process to the process model writer
				damlWriter.AddDamlProcess( process );
			}
			
			return damlWriter.ToXml();
		}

		private static Assembly GenerateAssembly( ref ServiceDescription svcDesc )
		{
			try
			{
				ServiceDescriptionImporter sdImport = new ServiceDescriptionImporter();
				// Set Protocol
				sdImport.ProtocolName = PROTOCOL;
				sdImport.AddServiceDescription( svcDesc, null, null );
				// Set namespace for generated proxy
				CodeNamespace cnSpace = new CodeNamespace( AUTO_GEN_NAMESPACE );
				// Create new code compiled unit
				CodeCompileUnit ccUnit = new CodeCompileUnit();
				ServiceDescriptionImportWarnings sdiWarning = sdImport.Import( cnSpace, ccUnit );
				// Pass CodeCOmpileUnit to a System.CodeDom.CodeProvder
				// e.g. Microsoft.CSharp.CSharpCodeProvider to do the 
				// compilation.
				CSharpCodeProvider cdp = new CSharpCodeProvider();
				ICodeGenerator cg = cdp.CreateGenerator();
				ICodeCompiler cc = cdp.CreateCompiler();
				
				// Do compilation and proxy generation in memory
				// Create stream writer that writes to a memory stream
				// this will store the source we generate
				MemoryStream sourceStream = new MemoryStream();
				StreamWriter sw = new StreamWriter( sourceStream );	

				// Generate the code and put it in the stream writer
				cg.GenerateCodeFromNamespace( cnSpace, sw, null );
				// Flush the stream writer
				sw.Flush();
								
				TextReader reader = new StreamReader( sourceStream );
				sourceStream.Seek( 0, System.IO.SeekOrigin.Begin );
				string strSource = reader.ReadToEnd();

				CompilerParameters cparams = new CompilerParameters( new String[] { "System.dll", "System.Xml.dll", "System.Web.Services.dll" } );
				
				cparams.GenerateExecutable = false;
				cparams.GenerateInMemory = true;
				cparams.MainClass = svcDesc.Services[0].Name;
				cparams.IncludeDebugInformation = true;
			
				CompilerResults cr = cc.CompileAssemblyFromSource( cparams, strSource );
				Assembly results = cr.CompiledAssembly;
				
				sw.Close();

				// If an empty assembly generated, return null
				if( results.GetTypes().Length == 0 )
					return null;

				return results;
			}
			catch( Exception e )
			{
				string s = e.Message;
			}

			return null;
		}

		public static Stream GetHttpStream( string strURL )
		{
			// Create a web request
			WebRequest objRequest = WebRequest.Create( strURL );
			// Get the response
			Stream objStream = objRequest.GetResponse().GetResponseStream();
			// Return stream
			return objStream;
		}
	}
}
