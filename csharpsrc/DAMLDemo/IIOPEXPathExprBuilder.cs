using System;

namespace PSL.DISCUS.DAML
{
	/// <summary>
	/// Interface implemented by classes used to generart XPath expressions
	/// for searching DAMLServiceProfile documents for inputs, outputs, preconditiond
	/// and effects.
	/// </summary>
	public interface IIOPEXPathExprBuilder
	{
		string BuildExpression( enuIOPESearchBy filter );
		string BuildExpression( enuIOPESearchBy filter, string strSearchKey );
	}
}
