using System;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Web.Services;
using System.Web.Services.Description;
using System.Reflection;
using Microsoft.CSharp;
using System.Collections.Specialized;
using PSL.DISCUS.DynamicProxy.Util;

// DISCUS DynamicProxy package
namespace PSL.DISCUS.DynamicProxy
{
	/// <summary>
	/// Generates web service proxies given a WSDL URL reference
	/// </summary>
	public class ProxyGen
	{
		// Source name used for event logging
		private const string SOURCENAME = "DynamicProxy.ProxyGen";
		private EventLog m_EvtLog;
		private ProxyMutator m_mutator = null;

		// Set mutator
		public ProxyMutator Mutator
		{
			set
			{ m_mutator = value; }
		}

		public void ResetMutator()
		{
			m_mutator = null;
		}

		public bool HasMutator
		{
			get
			{ return m_mutator != null; }
		}

		public ProxyGen()
		{
			// Inititalize event logging facility
			m_EvtLog = new EventLog( "Application" );
			m_EvtLog.Source = SOURCENAME;
		}

		/*	Function generates an assembly based on a dynamic
		 *	request made for a web service.
		 *  Input: req - Dynamic request object
		 *  Return: location of generated assembly containing proxy
		 */ 
		public string GenerateAssembly( DynamicRequest req )
		{
			string strAssemblyLoc = "";

			try
			{
				ServiceDescriptionImporter sdImport = new ServiceDescriptionImporter();
				// Read Service description from WSDL file
				ServiceDescription svcDesc = ServiceDescription.Read( Util.DynProxyUtil.GetHttpStream( req.wsdlFile ) );
				// Set Protocol
				sdImport.ProtocolName = req.protocol;
				// Add service description
				if( req.baseURL.Length > 0 )
					sdImport.AddServiceDescription( svcDesc, null, req.baseURL );
				else sdImport.AddServiceDescription( svcDesc, null, null );
				// Set namespace for generated proxy
				CodeNamespace cnSpace = new CodeNamespace( req.dynNamespace );
				// Create new code compiled unit
				CodeCompileUnit ccUnit = new CodeCompileUnit();
				ServiceDescriptionImportWarnings sdiWarning = sdImport.Import( cnSpace, ccUnit );
				// Pass CodeCOmpileUnit to a System.CodeDom.CodeProvder
				// e.g. Microsoft.CSharp.CSharpCodeProvider to do the 
				// compilation.
				CSharpCodeProvider cdp = new CSharpCodeProvider();
				ICodeGenerator cg = cdp.CreateGenerator();
				ICodeCompiler cc = cdp.CreateCompiler();
				
				// Modify proxy as appropriate
				if( m_mutator != null )
					m_mutator.Mutate( ref cnSpace );

				// Construct paths to source code and assembly
				string strFilenameSource = req.proxyPath + "\\" + req.filenameSource + ".cs";
				string strAssemblyFilename = req.proxyPath + "\\" + req.filenameSource + ".dll";
				// Create an output stream associated with assembly
				StreamWriter sw = new StreamWriter( strFilenameSource );
								
				// Generate the code
				cg.GenerateCodeFromNamespace( cnSpace, sw, null );
				sw.Flush();
				sw.Close();
				
				CompilerParameters cparams = new CompilerParameters( new String[] { "System.dll", "System.Xml.dll", "System.Web.Services.dll" } );
				// Add compiler params from ProxyMytator
				if( m_mutator != null )
					cparams.ReferencedAssemblies.AddRange( m_mutator.CompilerParameters );				
				
				cparams.GenerateExecutable = false;
				cparams.GenerateInMemory = false;
				cparams.MainClass = req.serviceName;
				cparams.OutputAssembly = strAssemblyFilename;
				cparams.IncludeDebugInformation = true;
			
				CompilerResults cr = cc.CompileAssemblyFromFile( cparams, strFilenameSource );
				
				if( cr.Errors.HasErrors )
					throw new Exception( "Compilation failed with errors - see source file: " + strFilenameSource );
				
				// Set location of proxy assembly
				strAssemblyLoc = strAssemblyFilename;
			}
			catch( System.Exception e )
			{
				// Report Error
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strAssemblyLoc;
		}// End GenerateAssembly
	}// End ProxyGen
}
