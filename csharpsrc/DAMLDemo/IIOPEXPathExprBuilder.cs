using System;

namespace PSL.DISCUS.DAML
{
	/// <summary>
	/// Summary description for IIOPEXPathExprBuilder.
	/// </summary>
	public interface IIOPEXPathExprBuilder
	{
		string BuildExpression( enuIOPESearchBy filter );
		string BuildExpression( enuIOPESearchBy filter, string strSearchKey );
	}
}
