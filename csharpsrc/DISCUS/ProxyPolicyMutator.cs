using System;
using System.CodeDom;
using System.Collections;

namespace PSL.DISCUS.DynamicProxy
{
	/// <summary>
	/// Summary description for ProxyPolicyMutator.
	/// </summary>
	public class ProxyPolicyMutator:ProxyMutator
	{
		// Set base class
		public const string PROXY_BASE_CLASS = "Microsoft.WSDK.WSDKClientProtocol";
		// Set compiler params
		public const string PROXY_COMPILER_PARAMS = "Microsoft.WSDK.dll";
		// Set code snippet
		public const string POLICY_CODE_SNIPPET = "if( true )\r\n\t\t\t{ \n\t\t\t\t// Do Something here \r\n\t\t\t}";
		public const string PROXY_WSDK_IMPORT = "Microsoft.WSDK";
		public const string PROXY_WSDK_SECURITY_IMPORT = "Microsoft.WSDK.Security";

		private string m_strProxyName = "";

		public ProxyPolicyMutator():base()
		{
		}

		public string ProxyName
		{
			get
			{ return m_strProxyName; }
			set
			{ 
				if( value == null || value == "" )
					throw new ArgumentException( "Property can't be null/empty string", "NewProxyName" );

				m_strProxyName = value; 
			}
		}

		public override void Mutate( ref CodeNamespace cnSpace )
		{
			// Call Base class implementation first to do general preperation
			// for mutation
			base.Mutate( ref cnSpace );
			
			if( m_cnSpace == null )
				throw new Exception( "CodeNamespace is null, either parameter cnSpace is null or base.Mutate(...) not called" );

			// Add namespaces
			m_cnSpace.Imports.Add( new CodeNamespaceImport(	PROXY_WSDK_IMPORT ) );
			m_cnSpace.Imports.Add( new CodeNamespaceImport(	PROXY_WSDK_SECURITY_IMPORT ) );
			// Add Compiler parameters
			m_lstCompilerParameters.Add( PROXY_COMPILER_PARAMS );

			// Change proxy name if nec
			if( ProxyName.Length > 0 )
			{
				ProxyNameMutator nameMutator = new ProxyNameMutator();
				nameMutator.ProxyName = m_strProxyName;
				nameMutator.Mutate( ref m_cnSpace );
			}
			
			// Change base class
			ProxyBaseClassMutator baseClassMutator = new ProxyBaseClassMutator();
			baseClassMutator.BaseClass = PROXY_BASE_CLASS;
			baseClassMutator.Mutate( ref m_cnSpace );
			
			// Augment proxy methods
			if( POLICY_CODE_SNIPPET.Length > 0 )
			{
				ProxyMethodMutator methodMutator = new ProxyMethodMutator();
				methodMutator.CodeSnippet = POLICY_CODE_SNIPPET;
				methodMutator.Mutate( ref m_cnSpace );
			}
		}
	}
}
