using System;
using PSL.DAML.Interfaces;

namespace PSL.DAML
{
	internal sealed class InputXPathExprBuilder:IIOPEXPathExprBuilder
	{
		/* Constructor */
		internal InputXPathExprBuilder()
		{}

		/* Function */
		public string BuildExpression( enuIOPESearchBy filter )
		{
			switch( filter )
			{
				case enuIOPESearchBy.PARAM_DESC: return  "/" + DamlConstants.DEFAULT_NS + ":" + DamlConstants.INPUT + "/" + DamlConstants.PROFILE_PARAM_DESC;
												 
				case enuIOPESearchBy.PARAM_NAME: return "/" + DamlConstants.DEFAULT_NS + ":" + DamlConstants.INPUT + "/" + DamlConstants.PROFILE_PARAM_DESC + "[@" + DamlConstants.RDF_ID + "]" + "/" + DamlConstants.PROFILE_PARAM_NAME; 
				
				case enuIOPESearchBy.REFERS_TO: return "/" + DamlConstants.DEFAULT_NS + ":" + DamlConstants.INPUT + "/" + DamlConstants.PROFILE_PARAM_DESC + "[@" + DamlConstants.RDF_ID + "]" + "/" + DamlConstants.PROFILE_REFERS_TO; 
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}

		public string BuildExpression( enuIOPESearchBy filter, string strSearchKey )
		{
			string strXPath = "";

			switch( filter )
			{
				case enuIOPESearchBy.PARAM_DESC: strXPath = BuildExpression( filter );
												 strXPath += "[@" + DamlConstants.RDF_ID + "='" + strSearchKey + "']";
												 return strXPath;
				
				case enuIOPESearchBy.PARAM_NAME: strXPath = BuildExpression( filter );
												 strXPath += "[." + "='" + strSearchKey + "']";
												 return strXPath;
				
				case enuIOPESearchBy.REFERS_TO: strXPath = BuildExpression( filter );
					strXPath += "[@" + DamlConstants.RDF_RESOURCE + "='" + strSearchKey + "']";
					return strXPath;
				

				default: throw new ArgumentException( "Invalid filter value" );
			};
		}
	}// End - InputXPathExprBuilder

	internal sealed class OutputXPathExprBuilder:IIOPEXPathExprBuilder
	{
		/* Constructor */
		internal OutputXPathExprBuilder()
		{}

		public string BuildExpression( enuIOPESearchBy filter )
		{
			switch( filter )
			{
				case enuIOPESearchBy.PARAM_DESC: return  "/" + DamlConstants.DEFAULT_NS + ":" + DamlConstants.OUTPUT + "/" + DamlConstants.PROFILE_PARAM_DESC;
												 
				case enuIOPESearchBy.PARAM_NAME: return "/" + DamlConstants.DEFAULT_NS + ":" + DamlConstants.OUTPUT + "/" + DamlConstants.PROFILE_PARAM_DESC + "[@" + DamlConstants.RDF_ID + "]" + "/" + DamlConstants.PROFILE_PARAM_NAME; 
				
				case enuIOPESearchBy.REFERS_TO: return "/" + DamlConstants.DEFAULT_NS + ":" + DamlConstants.OUTPUT + "/" + DamlConstants.PROFILE_PARAM_DESC + "[@" + DamlConstants.RDF_ID + "]" + "/" + DamlConstants.PROFILE_REFERS_TO; 
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}

		public string BuildExpression( enuIOPESearchBy filter, string strSearchKey )
		{
			string strXPath = "";

			switch( filter )
			{
				case enuIOPESearchBy.PARAM_DESC: strXPath = BuildExpression( filter );
					strXPath += "[@" + DamlConstants.RDF_ID + "='" + strSearchKey + "']";
					return strXPath;
				
				case enuIOPESearchBy.PARAM_NAME: strXPath = BuildExpression( filter );
					strXPath += "[." + "='" + strSearchKey + "']";
					return strXPath;
				
				case enuIOPESearchBy.REFERS_TO: strXPath = BuildExpression( filter );
					strXPath += "[@" + DamlConstants.RDF_RESOURCE + "='" + strSearchKey + "']";
					return strXPath;
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}
	}// End OutputXPathExprBuilder

	
	internal sealed class PreconditionXPathExprBuilder:IIOPEXPathExprBuilder
	{
		/* Constructor */
		internal PreconditionXPathExprBuilder()
		{}

		public string BuildExpression( enuIOPESearchBy filter )
		{
			switch( filter )
			{
				case enuIOPESearchBy.COND_DESC: return  "/" + DamlConstants.DEFAULT_NS + ":" + DamlConstants.PRECONDITION + "/" + DamlConstants.PROFILE_CONDITION_DESC;
												 
				case enuIOPESearchBy.COND_NAME: return "/" + DamlConstants.DEFAULT_NS + ":" + DamlConstants.PRECONDITION + "/" + DamlConstants.PROFILE_CONDITION_DESC + "[@" + DamlConstants.RDF_ID + "]" + "/" + DamlConstants.PROFILE_CONDITION_NAME; 
				
				case enuIOPESearchBy.REFERS_TO: return "/" + DamlConstants.DEFAULT_NS + ":" + DamlConstants.PRECONDITION + "/" + DamlConstants.PROFILE_CONDITION_DESC + "[@" + DamlConstants.RDF_ID + "]" + "/" + DamlConstants.PROFILE_REFERS_TO; 
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}

		public string BuildExpression( enuIOPESearchBy filter, string strSearchKey )
		{
			string strXPath = "";

			switch( filter )
			{
				case enuIOPESearchBy.COND_DESC: strXPath = BuildExpression( filter );
					strXPath += "[@" + DamlConstants.RDF_ID + "='" + strSearchKey + "']";
					return strXPath;
				
				case enuIOPESearchBy.COND_NAME: strXPath = BuildExpression( filter );
					strXPath += "[." + "='" + strSearchKey + "']";
					return strXPath;
				
				case enuIOPESearchBy.REFERS_TO: strXPath = BuildExpression( filter );
					strXPath += "[@" + DamlConstants.RDF_RESOURCE + "='" + strSearchKey + "']";
					return strXPath;
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}
	}// End PreconditionXPathExprBuilder
	
	internal sealed class EffectXPathExprBuilder:IIOPEXPathExprBuilder
	{
		/* Constructor */
		internal EffectXPathExprBuilder()
		{}

		public string BuildExpression( enuIOPESearchBy filter )
		{
			switch( filter )
			{
				case enuIOPESearchBy.COND_DESC: return  "/" + DamlConstants.DEFAULT_NS + ":" + DamlConstants.EFFECT + "/" + DamlConstants.PROFILE_CONDITION_DESC;
												 
				case enuIOPESearchBy.COND_NAME: return "/" + DamlConstants.DEFAULT_NS + ":" + DamlConstants.EFFECT + "/" + DamlConstants.PROFILE_CONDITION_DESC + "[@" + DamlConstants.RDF_ID + "]" + "/" + DamlConstants.PROFILE_CONDITION_NAME; 
				
				case enuIOPESearchBy.REFERS_TO: return "/" + DamlConstants.DEFAULT_NS + ":" + DamlConstants.EFFECT + "/" + DamlConstants.PROFILE_CONDITION_DESC + "[@" + DamlConstants.RDF_ID + "]" + "/" + DamlConstants.PROFILE_REFERS_TO; 
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}
		public string BuildExpression( enuIOPESearchBy filter, string strSearchKey )
		{
			string strXPath = "";

			switch( filter )
			{
				case enuIOPESearchBy.COND_DESC: strXPath = BuildExpression( filter );
					strXPath += "[@" + DamlConstants.RDF_ID + "='" + strSearchKey + "']";
					return strXPath;
				
				case enuIOPESearchBy.COND_NAME: strXPath = BuildExpression( filter );
					strXPath += "[." + "='" + strSearchKey + "']";
					return strXPath;
				
				case enuIOPESearchBy.REFERS_TO: strXPath = BuildExpression( filter );
					strXPath += "[@" + DamlConstants.RDF_RESOURCE + "='" + strSearchKey + "']";
					return strXPath;
				
				default: throw new ArgumentException( "Invalid filter value" );
			};
		}
	}// End EffectXPathExprBuilder
}
