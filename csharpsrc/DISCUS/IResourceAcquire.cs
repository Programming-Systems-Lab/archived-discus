using System;

namespace PSL.DISCUS.Interfaces
{
	/// <summary>
	/// Summary description for IResourceAcquire.
	/// </summary>
	public interface IResourceAcquire
	{
		// Returns an XML treaty as a response
		string EnlistServicesByName( string strXMLTreatyReq, string strXMLCredentials );
		// Returns either true or false based on the service requested and the credentials
		bool RequestServiceByName( string strServiceName, string strXMLCredentials );
		// Revoke a treaty
		void RevokeTreaty( int nTreatyID );
		// Dissolve a treaty
		void DissolveTreaty( int nTreatyID );
	}
}
