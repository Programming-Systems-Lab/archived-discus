using System;
using System.CodeDom;
using System.Collections;
using PSL.DISCUS.Interfaces.DynamicProxy;

namespace PSL.DISCUS.DynamicProxy
{
	/// <summary>
	/// </summary>
	public abstract class ProxyMutator:IProxyMutate
	{
		protected CodeNamespace m_cnSpace = null;
		protected int m_nProxyClassLoc = -1;
		protected bool m_bProxyClassFound = false;
		protected bool m_bMapCreated = false;
		// Map method names to location declaration[i].Member collection
		protected Hashtable m_ProxyMethodMap = new Hashtable();
		protected ArrayList m_lstCompilerParameters = new ArrayList();
				
		protected ProxyMutator()
		{}
		
		public bool MapCreated
		{
			get
			{ return m_bMapCreated; }
		}

		public void ResetMap()
		{
			if( m_bMapCreated )
			{
				m_nProxyClassLoc = -1;
				m_bProxyClassFound = false;
				m_bMapCreated = false;
				m_ProxyMethodMap.Clear();
				m_lstCompilerParameters.Clear();
			}
		}
		
		public string[] CompilerParameters
		{
			get
			{ return (string[]) m_lstCompilerParameters.ToArray( typeof(System.String) ); }
		}

		public virtual void Mutate( ref CodeNamespace cnSpace )
		{
			if( cnSpace == null )
				throw new ArgumentNullException( "cnSpace", "Can't be null" );

			m_cnSpace = cnSpace;
			if( !m_bMapCreated )
				Create();
		}
		
		public virtual void AddCompilerParameter( string strParam )
		{
			m_lstCompilerParameters.Add( strParam );
		}

		// We only want classes - those with and those without methods
		// so we distinguish complex types (no methods) from
		// our web service class (methods)
		public void Create()
		{
			CodeTypeDeclarationCollection declColl = m_cnSpace.Types;
			if( declColl.Count == 0 )
				return; // No types found in CodeDOM - should we throw exception?

			for( int i = 0; i < declColl.Count; i++ )
			{
				if( m_bProxyClassFound )
					break;
				
				if( !declColl[i].IsClass )
					continue;

				// Once we are dealing with a class, get the class members
				CodeTypeMemberCollection memberColl = declColl[i].Members;
				// If memeber collection empty...
				if( memberColl.Count == 0 )
					continue; // Empty/Shell class - should we throw exception?

				for( int j = 0; j < memberColl.Count; j++ )
				{
					if( memberColl[j] is CodeMemberMethod )
					{
						// Weed out CodeMemberMethod subclasses for now
						// we do not include:
						// constructors
						// typeconstructors or entrypoints e.g Main
						if( ( memberColl[j] is CodeConstructor || memberColl[j] is CodeTypeConstructor ) || memberColl[j] is CodeEntryPointMethod )
							continue;
						
						m_bProxyClassFound = true;
						m_nProxyClassLoc = i;

						// Add the method name and where it is in the member collection
						// to our collection of classes with methods
						m_ProxyMethodMap.Add( ( (CodeMemberMethod) memberColl[j] ).Name, j );
					}
				}// End-for j = 0; j < memberColl.Count
			}// End-for i = 0 i < declColl.Count
			m_bMapCreated = true;
		}// End-Create
	}
}
