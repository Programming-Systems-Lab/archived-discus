using System;
using System.CodeDom;

namespace PSL.DISCUS.Interfaces.DynamicProxy
{
	/// <summary>
	/// Summary description for IProxyMutate
	/// </summary>
	public interface IProxyMutate
	{
		// Mutate operation context specific
		void Mutate( ref CodeNamespace cnSpace );
	}
}
