using System;
using System.CodeDom;

namespace PSL.DISCUS.DynamicProxy
{
	/// <summary>
	/// Summary description for ProxyNameMutator.
	/// </summary>
	public class ProxyNameMutator:ProxyMutator
	{
		private string m_strProxyName = "";
		        		
		public ProxyNameMutator():base()
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

			// Change name
			if( ProxyName.Length == 0 )
				throw new Exception( "ProxyName property not set, set to value other than empty string" );

			if( !m_bProxyClassFound	|| m_nProxyClassLoc == -1 )
				throw new InvalidOperationException( "Cannot perform operation - Proxy class not found" );

			m_cnSpace.Types[m_nProxyClassLoc].Name = ProxyName;
		}
	}
}
