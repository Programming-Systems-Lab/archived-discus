using System;

namespace PSL.DISCUS.DAML
{
	/// <summary>
	/// Summary description for InputXPathExprBuilder.
	/// </summary>
	internal sealed class InputXPathExprBuilder:IIOPEXPathExprBuilder
	{
		internal InputXPathExprBuilder()
		{}

		public string BuildExpression( enuIOPESearchBy filter )
		{
			switch( filter )
			{
				case enuIOPESearchBy.PARAM_DESC: return  "/" + DAMLServiceProfile.DEFAULT_NS + ":" + DAMLServiceProfile.INPUT + "/" + DAMLServiceProfile.PROFILE_PARAM_DESC;
												 
				case enuIOPESearchBy.PARAM_NAME: return "/" + DAMLServiceProfile.DEFAULT_NS + ":" + DAMLServiceProfile.INPUT + "/" + DAMLServiceProfile.PROFILE_PARAM_DESC + "[@" + DAMLServiceProfile.RDF_ID + "]" + "/" + DAMLServiceProfile.PROFILE_PARAM_NAME; 
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}

		public string BuildExpression( enuIOPESearchBy filter, string strSearchKey )
		{
			string strXPath = "";

			switch( filter )
			{
				case enuIOPESearchBy.PARAM_DESC: strXPath = BuildExpression( filter );
												 strXPath += "[@" + DAMLServiceProfile.RDF_ID + "='" + strSearchKey + "']";
												 return strXPath;
				
				case enuIOPESearchBy.PARAM_NAME: strXPath = BuildExpression( filter );
												 strXPath += "[." + "='" + strSearchKey + "']";
												 return strXPath;
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}
	}// End - InputXPathExprBuilder

	internal sealed class OutputXPathExprBuilder:IIOPEXPathExprBuilder
	{
		internal OutputXPathExprBuilder()
		{}

		public string BuildExpression( enuIOPESearchBy filter )
		{
			switch( filter )
			{
				case enuIOPESearchBy.PARAM_DESC: return  "/" + DAMLServiceProfile.DEFAULT_NS + ":" + DAMLServiceProfile.OUTPUT + "/" + DAMLServiceProfile.PROFILE_PARAM_DESC;
												 
				case enuIOPESearchBy.PARAM_NAME: return "/" + DAMLServiceProfile.DEFAULT_NS + ":" + DAMLServiceProfile.OUTPUT + "/" + DAMLServiceProfile.PROFILE_PARAM_DESC + "[@" + DAMLServiceProfile.RDF_ID + "]" + "/" + DAMLServiceProfile.PROFILE_PARAM_NAME; 
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}

		public string BuildExpression( enuIOPESearchBy filter, string strSearchKey )
		{
			string strXPath = "";

			switch( filter )
			{
				case enuIOPESearchBy.PARAM_DESC: strXPath = BuildExpression( filter );
					strXPath += "[@" + DAMLServiceProfile.RDF_ID + "='" + strSearchKey + "']";
					return strXPath;
				
				case enuIOPESearchBy.PARAM_NAME: strXPath = BuildExpression( filter );
					strXPath += "[." + "='" + strSearchKey + "']";
					return strXPath;
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}
	}// End OutputXPathExprBuilder

	
	internal sealed class PreconditionXPathExprBuilder:IIOPEXPathExprBuilder
	{
		internal PreconditionXPathExprBuilder()
		{}

		public string BuildExpression( enuIOPESearchBy filter )
		{
			switch( filter )
			{
				case enuIOPESearchBy.COND_DESC: return  "/" + DAMLServiceProfile.DEFAULT_NS + ":" + DAMLServiceProfile.PRECONDITION + "/" + DAMLServiceProfile.PROFILE_CONDITION_DESC;
												 
				case enuIOPESearchBy.COND_NAME: return "/" + DAMLServiceProfile.DEFAULT_NS + ":" + DAMLServiceProfile.PRECONDITION + "/" + DAMLServiceProfile.PROFILE_CONDITION_DESC + "[@" + DAMLServiceProfile.RDF_ID + "]" + "/" + DAMLServiceProfile.PROFILE_CONDITION_NAME; 
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}

		public string BuildExpression( enuIOPESearchBy filter, string strSearchKey )
		{
			string strXPath = "";

			switch( filter )
			{
				case enuIOPESearchBy.COND_DESC: strXPath = BuildExpression( filter );
					strXPath += "[@" + DAMLServiceProfile.RDF_ID + "='" + strSearchKey + "']";
					return strXPath;
				
				case enuIOPESearchBy.COND_NAME: strXPath = BuildExpression( filter );
					strXPath += "[." + "='" + strSearchKey + "']";
					return strXPath;
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}
	}// End PreconditionXPathExprBuilder
	
	internal sealed class EffectXPathExprBuilder:IIOPEXPathExprBuilder
	{
		internal EffectXPathExprBuilder()
		{}

		public string BuildExpression( enuIOPESearchBy filter )
		{
			switch( filter )
			{
				case enuIOPESearchBy.COND_DESC: return  "/" + DAMLServiceProfile.DEFAULT_NS + ":" + DAMLServiceProfile.EFFECT + "/" + DAMLServiceProfile.PROFILE_CONDITION_DESC;
												 
				case enuIOPESearchBy.COND_NAME: return "/" + DAMLServiceProfile.DEFAULT_NS + ":" + DAMLServiceProfile.EFFECT + "/" + DAMLServiceProfile.PROFILE_CONDITION_DESC + "[@" + DAMLServiceProfile.RDF_ID + "]" + "/" + DAMLServiceProfile.PROFILE_CONDITION_NAME; 
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}
		public string BuildExpression( enuIOPESearchBy filter, string strSearchKey )
		{
			string strXPath = "";

			switch( filter )
			{
				case enuIOPESearchBy.COND_DESC: strXPath = BuildExpression( filter );
					strXPath += "[@" + DAMLServiceProfile.RDF_ID + "='" + strSearchKey + "']";
					return strXPath;
				
				case enuIOPESearchBy.COND_NAME: strXPath = BuildExpression( filter );
					strXPath += "[." + "='" + strSearchKey + "']";
					return strXPath;
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}
	}// End EffectXPathExprBuilder
}
