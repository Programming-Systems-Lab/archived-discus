using System;
using PSL.DAML.Interfaces;

namespace PSL.DAML
{
	/// <summary>
	/// Factory creates XPath expression builders
	/// </summary>
	public sealed class IOPEXPathExprBuilderFactory
	{
		private IOPEXPathExprBuilderFactory()
		{}

		public static IIOPEXPathExprBuilder CreateInstance( enuIOPEType builderType )
		{
			switch( builderType )
			{
				case enuIOPEType.Input: return new InputXPathExprBuilder();

				case enuIOPEType.Output: return new OutputXPathExprBuilder();

				case enuIOPEType.Precondition: return new PreconditionXPathExprBuilder();

				case enuIOPEType.Effect: return new EffectXPathExprBuilder();

				default: throw new ArgumentException( "Invalid/Unrecognized filter type" );
			};
		}
	}
}
