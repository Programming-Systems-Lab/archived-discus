using System;

namespace PSL.DISCUS.DataAccess
{
	/// <summary>
	/// Single point of interaction with ServiceSpace data
	/// Allows interaction with RegisteredServices, ServiceMethods
	/// and known ServiceSpaces info
	/// </summary>
	public class InternalRegistry
	{
		private RegServiceDAO m_svcDAO;
		private ServiceSpaceDAO m_svcSpaceDAO;

		public InternalRegistry( string strDBConnect )
		{
			m_svcDAO = new RegServiceDAO( strDBConnect );
			m_svcSpaceDAO = new ServiceSpaceDAO( strDBConnect );
		}
		
		/* RegisteredServices interactions */
		public int RegisterService( string strServiceName, string strServiceNamespace, string strServiceLocation, string strServiceAccessPoint )
		{
			return m_svcDAO.RegisterService( strServiceName, strServiceNamespace, strServiceLocation, strServiceAccessPoint );
		}
		
		public bool UnregisterService( string strServiceName )
		{
			return m_svcDAO.UnRegisterService( strServiceName );
		}

		public bool UpdateServiceAccessPoint( string strServiceName, string strServiceAccessPoint )
		{
			return m_svcDAO.UpdateServiceAccessPoint( strServiceName, strServiceAccessPoint );
		}
		
		public bool UpdateServiceNamespace( string strServiceName, string strServiceNamespace )
		{
			return m_svcDAO.UpdateServiceNamespace( strServiceName, strServiceNamespace );
		}

		public bool UpdateServiceLocation( string strServiceName, string strServiceLocation )
		{
			return m_svcDAO.UpdateServiceLocation( strServiceName, strServiceLocation );
		}

		public bool RegisterServiceMethod( string strServiceName, string strMethod )
		{
			return m_svcDAO.RegisterServiceMethod( strServiceName, strMethod );
		}

		public bool UnregisterServiceMethod( string strServiceName, string strMethod )
		{
			return m_svcDAO.UnregisterServiceMethod( strServiceName, strMethod );
		}

		public bool UpdateServiceMethod( string strServiceName, string strOldMethod, string strNewMethod )
		{
			return m_svcDAO.UpdateServiceMethod( strServiceName, strOldMethod, strNewMethod );
		}

		public int GetServiceID( string strServiceName )
		{
			return m_svcDAO.GetServiceID( strServiceName );
		}

		public string GetServiceAccessPoint( string strServiceName )
		{
			return m_svcDAO.GetServiceAccessPoint( strServiceName );
		}

		public string GetServiceNamespace( string strServiceName )
		{
			return m_svcDAO.GetServiceNamespace( strServiceName );
		}

		public string GetServiceLocation( string strServiceName )
		{
			return m_svcDAO.GetServiceLocation( strServiceName );
		}

		public bool MethodExists( string strServiceName, string strMethod )
		{
			return m_svcDAO.MethodExists( strServiceName, strMethod );
		}

		/* ServiceSpace interactions */
		public bool RegisterGateKeeper( string strGKName, string strGKNamespace, string strGKLocation, string strGKAccessPoint )
		{
			return m_svcSpaceDAO.RegisterGateKeeper( strGKName, strGKNamespace, strGKLocation, strGKAccessPoint );
		}

		public bool UnregisterGateKeeper( string strGKName )
		{
			return m_svcSpaceDAO.UnregisterGateKeeper( strGKName );
		}

		public bool UpdateGateKeeperAccessPoint( string strGKName, string strGKAccessPoint )
		{
			return m_svcSpaceDAO.UpdateGateKeeperAccessPoint( strGKName, strGKAccessPoint );
		}

		public bool UpdateGateKeeperLocation( string strGKName, string strGKLocation )
		{
			return m_svcSpaceDAO.UpdateGateKeeperLocation( strGKName, strGKLocation );
		}

		public bool UpdateGateKeeperNamespace( string strGKName, string strGKNamespace )
		{
			return m_svcSpaceDAO.UpdateGateKeeperNamespace( strGKName, strGKNamespace );
		}

		public string GetGateKeeperAccessPoint( string strGKName )
		{
			return m_svcSpaceDAO.GetGateKeeperAccessPoint( strGKName );
		}

		public string GetGateKeeperLocation( string strGKName )
		{
			return m_svcSpaceDAO.GetGateKeeperLocation( strGKName );
		}

		public string GetGateKeeperNamespace( string strGKName )
		{
			return m_svcSpaceDAO.GetGateKeeperNamespace( strGKName );
		}
	}
}
