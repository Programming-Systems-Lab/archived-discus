package psl.discus.javasrc.services;

import java.rmi.RemoteException;

import javax.sql.DataSource;

import psl.discus.javasrc.security.SecurityManager;
import psl.discus.javasrc.security.SecurityManagerImpl;
import psl.discus.javasrc.security.SecurityManagerException;

import java.rmi.RemoteException;

/**
 * @author Matias Pelenur
 */
public class SecurityManagerServiceImpl implements SecurityManagerService {

    SecurityManager securityManager;

    public SecurityManagerServiceImpl() {

        // get DataSource
        DataSource ds = null;

        // create SecurityManager instance
        securityManager = new SecurityManagerImpl(ds);

    }


    public String verifyTreaty(String signedTreatyXMLDoc)
        throws RemoteException {

        try {
            return securityManager.verifyTreaty(signedTreatyXMLDoc);
        } catch (SecurityManagerException e) {
            throw new RemoteException("Error", e);
        }
    }
    /*
    public String doRequestCheck(String signedRequestXMLDoc)
        throws RemoteException {

        return securityManager.doRequestCheck(signedRequestXMLDoc);
    }

    public String doResponseCheck(String signedResponseXMLDoc)
        throws RemoteException {

        return securityManager.doResponseCheck(signedResponseXMLDoc);
    }

    public String addServiceSpace(String serviceSpaceXMLDoc)
        throws RemoteException {

        return securityManager.addServiceSpace(serviceSpaceXMLDoc);
    }

    public String removeServiceSpace(int serviceSpaceId)
        throws RemoteException {

        return securityManager.removeServiceSpace(serviceSpaceId);
    }

    public String addPermission(String servicePermissionXMLDoc)
        throws RemoteException {

        return securityManager.addPermission(servicePermissionXMLDoc);
    }

    public String removePermission(String servicePermissionXMLDoc)
        throws RemoteException {

        return securityManager.removePermission(servicePermissionXMLDoc);
    }
    */
}
