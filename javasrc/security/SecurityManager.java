package psl.discus.javasrc.security;

import java.security.PrivateKey;
import java.security.PublicKey;
import java.util.Collection;

import org.w3c.dom.Document;

/**
 * This interface defines methods to
 * - encrypt, decrypt, sign, and verify XML messages
 * - manage a ServiceSpace's security matrix
 * - doing security checks on requests
 *
 * Author: Matias
 * Date: Mar 6, 2002
 * Time: 9:58:20 PM
 */
public interface SecurityManager {

    public static final String STATUS_OK = "0";
    public static final String STATUS_ERROR = "-1";



    /*-----------------------------------------------------------------------------------------------*/
    /* Methods to perform security checks */

    /**
     * When a service space is going to invoke services on another service space, it first
     * creates a treaty for that service space containing a description of the services and
     * methods it wants to access.
     *
     * This method processes a new treaty and returns a treaty containing a new treatyId
     * and the service,method,args entries that are actually authorized for the requesting
     * Service Space.
     *
     * @returns an array of 2 strings, where the first string is the status code (0 is OK)
     * and the second string is the content (either treaty XML or error message)
     */
    public String[] verifyTreaty(String treatyXML, boolean signed);

    /**
     * Checks if a requesting service space has permission to invoke a certain method
     * with certain arguments on a specified service, according to the treaty that the
     * service space created initially.
     * @returns an array of 2 strings, where the first string is the status code (0 is OK)
     * and the second string is the error message, if any
     */
    public String[] doRequestCheck(String requestXML, boolean signed);


    /**
     * Revokes a treaty. Returns STATUS_OK if revoked OK, or STATUS_ERROR and an error message
     * if the operation failed.
     */
    public String[] revokeTreaty(int treatyid);

    /*-----------------------------------------------------------------------------------------------*/
    /* Methods to modify the security matrix for this service space */

    public String addServiceSpace(String serviceSpaceXMLDoc)
            throws SecurityManagerException;

    public String removeServiceSpace(int serviceSpaceId)
            throws SecurityManagerException;

    /**
     * Adds a permission to the security matrix for a certain service space.
     * @see ServiceInvokationPermission
     */
    public String addPermission(String servicePermissionXMLDoc)
            throws SecurityManagerException;

    public String removePermission(String servicePermissionXMLDoc)
            throws SecurityManagerException;

    /*-----------------------------------------------------------------------------------------------*/
    /* Methods for encrypting, decrypting, signing and verifying messages */
    /* REMOVED: this methods probably won't be publicly accessible

    public String signMessage(String msg, int recipientServiceSpace);
    public String encryptMessage(String msg, int recipientServiceSpace);
    public String encryptAndSignMessage(String msg, int recipientServiceSpace);

    public String verifyMessage(String msg, int senderServiceSpace);
    public String decryptMessage(String msg, int senderServiceSpace);
    public String verifyAndDecryptMessage(String msg, int senderServiceSpace);
    */

}
