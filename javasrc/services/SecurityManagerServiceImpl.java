package psl.discus.javasrc.services;

import java.rmi.RemoteException;

import javax.sql.DataSource;
import javax.naming.InitialContext;
import javax.naming.NamingException;
import javax.naming.Context;

import psl.discus.javasrc.security.*;
import psl.discus.javasrc.security.SecurityManager;

import psl.discus.javasrc.security.FakeDataSource;
import psl.discus.javasrc.security.Util;

import java.rmi.RemoteException;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;

/**
 * @author Matias Pelenur
 */
public class SecurityManagerServiceImpl implements SecurityManagerService {

    public static final String DISCUS_DB_JNDI_NAME = "jdbc/SecurityManagerDB";

    private SecurityManager securityManager;
    private SignatureManager signatureManager;

    public SecurityManagerServiceImpl()
        throws RemoteException {

        Util.debug("Initializing SecurityManagerService...");

        // For testing: [removed]
        // DataSource ds = new FakeDataSource();

        // get datasource -- it should be defined in the server and web.xml configuration
        DataSource ds  = null;

        try {
            Context initCtx = new InitialContext();
            Context envCtx = (Context) initCtx.lookup("java:comp/env");
            ds = (DataSource) envCtx.lookup (DISCUS_DB_JNDI_NAME);
        } catch (NamingException e) {
            throw new RemoteException("Could not find datasource: " + e);
        }

        // create SignatureManager
        try {
            signatureManager = new SignatureManagerImpl(ds);
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

        return securityManager.verifyTreaty(treatyXML,signed);

    }

    public String[] doRequestCheck(String requestXML, boolean signed)
        throws RemoteException {

        if (requestXML == null)
            throw new RemoteException("xml parameter is null");

        // HACK: there seems to be a bug in the JWSDP that converts newlines to spaces in XML tags...
        // convert back to newlines!
        requestXML = Util.replaceString(requestXML,"> <", ">\n<");

        return securityManager.doRequestCheck(requestXML, signed);

    }

    /**
     * Revokes a treaty. Returns STATUS_OK if revoked OK, or STATUS_ERROR and an error message
     * if the operation failed.
     */
    public String[] revokeTreaty(int treatyid)
        throws RemoteException {

        return securityManager.revokeTreaty(treatyid);

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

}
