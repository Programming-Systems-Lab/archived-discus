using System;
using System.IO;
using System.Net;

// DISCUS Dynamic Proxy Util Package
namespace PSL.DISCUS.DynamicProxy.Util
{
	/// <summary>
	/// Utility class used in Dynamic Proxy Generation
	/// </summary>
	public abstract class DynProxyUtil
	{
		/*	Function returns the stream associated with a 
		 *  given URL. Used mainly to read in WSDL files
		 *  from the web.
		 *  Input: strURL - URL of interest
		 *  Return: stream associated with strURL (if it can be accessed)
		 */
		public static Stream GetHttpStream( string strURL )
		{
			// Create a web request
			WebRequest objRequest = WebRequest.Create( strURL );
			// Get the response
			Stream objStream = objRequest.GetResponse().GetResponseStream();
			// Return stream
			return objStream;
		}
	}// End DynProxyUtil
}
