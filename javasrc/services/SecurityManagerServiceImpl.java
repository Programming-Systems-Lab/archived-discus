package psl.discus.javasrc.services;

import java.rmi.RemoteException;

import javax.sql.DataSource;

import psl.discus.javasrc.security.*;
import psl.discus.javasrc.security.SecurityManager;

import psl.discus.javasrc.shared.FakeDataSource;
import psl.discus.javasrc.shared.Util;

import java.rmi.RemoteException;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;

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

    public String[] verifyTreaty(String treatyXML, boolean signed)
        throws RemoteException {

        if (treatyXML == null)
            throw new RemoteException("treatyXML parameter is null");

        // HACK: there seems to be a bug in the JWSDP that converts newlines to spaces in XML tags...
        // convert back to newlines!
        treatyXML = Util.replaceString(treatyXML,"> <", ">\n<");

        try {
            return securityManager.verifyTreaty(treatyXML,signed);
        } catch (SecurityManagerException e) {
            throw new RemoteException("Error", e);
        }
    }

    public String[] doRequestCheck(String requestXML, boolean signed)
        throws RemoteException {

        if (requestXML == null)
            throw new RemoteException("xml parameter is null");

        // HACK: there seems to be a bug in the JWSDP that converts newlines to spaces in XML tags...
        // convert back to newlines!
        requestXML = Util.replaceString(requestXML,"> <", ">\n<");

        try {
            return securityManager.doRequestCheck(requestXML, signed);
        } catch (SecurityManagerException e) {
            throw new RemoteException("Error", e);
        }
    }

    public String[] signDocument(String xml)
            throws RemoteException {

        if (xml == null)
           throw new RemoteException("xml parameter is null");

        // HACK: there seems to be a bug in the JWSDP that converts newlines to spaces in XML tags...
        // convert back to newlines!
        xml = Util.replaceString(xml,"> <", ">\n<");

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

        // HACK: there seems to be a bug in the JWSDP that converts newlines to spaces in XML tags...
        // convert back to newlines!
        signedXML = Util.replaceString(signedXML,"> <", ">\n<");

        try {
            return signatureManager.verifyDocument(signedXML);
        } catch (SignatureManagerException e) {
            throw new RemoteException("Error", e);
        }
    }



    /*
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
