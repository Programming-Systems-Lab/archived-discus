package psl.discus.javasrc.services;

import java.rmi.RemoteException;

import javax.sql.DataSource;

import psl.discus.javasrc.security.*;
import psl.discus.javasrc.security.SecurityManager;

import psl.discus.javasrc.shared.FakeDataSource;
import psl.discus.javasrc.shared.Util;

import java.rmi.RemoteException;

/**
 * @author Matias Pelenur
 */
public class SecurityManagerServiceImpl implements SecurityManagerService {

    SecurityManager securityManager;
    SignatureManager signatureManager;

    public SecurityManagerServiceImpl()
        throws RemoteException {

        Util.debug("Initializing SecurityManagerService...");

        // get DataSource
        DataSource ds = new FakeDataSource();

        // create SignatureManager
        try {
            signatureManager = new SignatureManager();
        }
        catch (Exception e) {
            throw new RemoteException("Could not initialize SignatureManager: " + e);
        }

        // create SecurityManager instance
        try {
            securityManager = new SecurityManagerImpl(ds,signatureManager);
        } catch (SecurityManagerException e) {
            throw new RemoteException("Could not initialize SecurityManager: " + e);
        }

    }

    /**
     * For testing, assumes treatyXMl is NOT signed
     */
    public String verifyTreaty(String treatyXML)
        throws RemoteException {

        return verifyTreaty(treatyXML, false);

    }

    public String verifyTreaty(String treatyXML, boolean signed)
        throws RemoteException {

        if (treatyXML == null)
            throw new RemoteException("treatyXML parameter is null");

        try {
            return securityManager.verifyTreaty(treatyXML,signed);
        } catch (SecurityManagerException e) {
            throw new RemoteException("Error", e);
        }
    }

    public String signDocument(String xml)
            throws RemoteException {

        if (xml == null)
           throw new RemoteException("xml parameter is null");

        try {
            return signatureManager.signDocument(xml);
        } catch (SignatureManagerException e) {
            throw new RemoteException("Error", e);
        }

    }

    public String[] verifyDocument(String signedXML)
            throws RemoteException {

        if (signedXML == null)
           throw new RemoteException("xml parameter is null");

        try {
            return signatureManager.verifyDocument(signedXML);
        } catch (SignatureManagerException e) {
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
