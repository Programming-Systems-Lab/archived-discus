using System;
using System.Collections;
using System.CodeDom;

namespace PSL.DISCUS.DynamicProxy
{
	/// <summary>
	/// Summary description for ProxyMethodMutator.
	/// </summary>
	public class ProxyMethodMutator:ProxyMutator
	{
		private CodeSnippetStatement m_codeSnippet = new CodeSnippetStatement();
		
		public ProxyMethodMutator():base()
		{
		}

		public string CodeSnippet
		{
			get
			{ return m_codeSnippet.Value; }
			set
			{ m_codeSnippet.Value = value; }
		}

		public override void Mutate( ref CodeNamespace cnSpace )
		{
			// Call Base class implementation first to do general preperation
			// for mutation
			base.Mutate( ref cnSpace );

			if( m_cnSpace == null )
				throw new Exception( "CodeNamespace is null, either parameter cnSpace is null or base.Mutate(...) not called" );
			
			// Create set of statements to insert
			// using CodeSnippets less tedious than coding indiv.
			// statements and codedom elements.
			// Difficult to format however.
			// CodeSnippetStatement codeSnippet = new CodeSnippetStatement();
			// codeSnippet.Value = "if( true )\r\n\t\t\t{ \n\t\t\t\t// Do Something here \r\n\t\t\t}";
			
			if( m_codeSnippet.Value.Length == 0 )
				return;

			// Do insert for each method in the method map
			IDictionaryEnumerator it = m_ProxyMethodMap.GetEnumerator();
			while( it.MoveNext() )
			{
				CodeMemberMethod member = (CodeMemberMethod) m_cnSpace.Types[m_nProxyClassLoc].Members[(int)it.Value];
				member.Statements.Insert( 0, m_codeSnippet );

				int x = 0;
				x++;
			}
		}
	}
}
