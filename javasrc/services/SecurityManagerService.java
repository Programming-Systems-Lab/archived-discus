package psl.discus.javasrc.services;

import psl.discus.javasrc.security.SecurityManager;
import psl.discus.javasrc.security.SecurityManagerException;

import java.rmi.Remote;
import java.rmi.RemoteException;

/**
 * @author Matias Pelenur
 */
public interface SecurityManagerService extends Remote  {

    /**
     * Assumes treaty document is not signed (for testing)
     */
    public String[] verifyTreaty(String treatyXML)
            throws RemoteException;

    /**
     * When a service space is going to invoke services on another service space, it first
     * creates a treaty for that service space containing a description of the services and
     * methods it wants to access.
     * This method processes a new treaty and returns a treaty containing a new treatyId
     * and the service,method,args entries that are actually authorized for the requesting
     * Service Space.
     *
     * @return an array of 2 strings, where the first string is the status code (0 is OK)
     * and the second string is the content (either treaty XML or error message)
     */
    public String[] verifyTreaty(String treatyXML, boolean signed)
            throws RemoteException;

    /**
     * Signs the given XML document with this service space's private key.
     * @returns an array of two Strings, where the first is a status code (0 is OK)
     * and the second is either the signed XML document or the error message.
     */
    public String[] signDocument(String xml)
        throws RemoteException;

    /**
     * Verifies a signed XML document and returns the document and the id of the signing service space
     * @returns an array three Strings, where the first is a status code (0 is OK),
     * the second element is the signing service space id, and the third is is
     * the given xml document but without the signature, or the error message.
     */
    public String[] verifyDocument(String signedXML)
        throws RemoteException;
}
