using System;

namespace PSL.DISCUS.Interfaces
{
	/// <summary>
	/// IResourceAcquire interface
	/// </summary>
	public interface IResourceAcquire
	{
		// Returns an XML treaty as a response
		string EnlistServicesByName( string strXMLTreatyReq );
		// Returns either true or false based on the service requested and the credentials
		bool RequestServiceByName( string strServiceName );
		// Revoke a treaty
		void RevokeTreaty( int nTreatyID );
		// Dissolve a treaty
		void DissolveTreaty( int nTreatyID );
	}
}
