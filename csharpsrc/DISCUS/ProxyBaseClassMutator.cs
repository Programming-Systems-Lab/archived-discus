using System;
using System.CodeDom;

namespace PSL.DISCUS.DynamicProxy
{
	/// <summary>
	/// Summary description for ProxyBaseClassMutator.
	/// </summary>
	public class ProxyBaseClassMutator:ProxyMutator
	{
		private string m_strBaseClass = "";

        public ProxyBaseClassMutator():base()
		{
		}

		public string BaseClass
		{
			get
			{ return m_strBaseClass; }
			set
			{ 
				if( value == null || value == "" )
					throw new ArgumentException( "Property can't be null/empty string", "NewProxyName" );

				m_strBaseClass = value; 
			}
		}

		public override void Mutate( ref CodeNamespace cnSpace )
		{
			// Call Base class implementation first to do general preperation
			// for mutation
			base.Mutate( ref cnSpace );

			if( m_cnSpace == null )
				throw new Exception( "CodeNamespace is null, either parameter cnSpace is null or base.Mutate(...) not called" );

			// Change name
			if( BaseClass.Length == 0 )
				throw new Exception( "BaseClass property not set, set to value other than empty string" );

			if( !m_bProxyClassFound	|| m_nProxyClassLoc == -1 )
				throw new InvalidOperationException( "Cannot perform operation - Proxy class not found" );

			// Expect single base type reference
			if( m_cnSpace.Types[m_nProxyClassLoc].BaseTypes.Count != 1 )
				throw new InvalidOperationException( "Expect Class to have one base reference type; class actually has " + m_cnSpace.Types[m_nProxyClassLoc].BaseTypes.Count.ToString() + " reference(s)" );

			m_cnSpace.Types[m_nProxyClassLoc].BaseTypes[0].BaseType = BaseClass;
		}
	}
}
