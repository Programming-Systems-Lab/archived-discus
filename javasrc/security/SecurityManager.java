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

    /*-----------------------------------------------------------------------------------------------*/
    /* Methods for encrypting, decrypting, signing and verifying messages */
    public void signMessage(Document msg, ServiceSpace recipient, PrivateKey signer);
    public void encryptMessage(Document msg, ServiceSpace recipient, PrivateKey signer);
    public void encryptAndSignMessage(Document msg, ServiceSpace recipient, PrivateKey signer);

    public void verifyMessage(Document msg, ServiceSpace sender);
    public void decryptMessage(Document msg, ServiceSpace sender);
    public void verifyAndDecryptMessage(Document msg, ServiceSpace sender);


    /*-----------------------------------------------------------------------------------------------*/
    /* Methods to perform security checks */

    /**
     * When a service space is going to invoke services on another service space, it first
     * creates a treaty for that service space containing a description of the services and
     * methods it wants to access.
     * This method processes a new treaty and returns a treaty containing a new treatyId
     * and the service,method,args entries that are actually authorized for the requesting
     * Service Space.
     *
     * @returns an XML Document conforming to the treaty schema
     */
    public Document verifyTreaty(ServiceSpace requester, Document treaty)
        throws SecurityManagerException;

    /**
     * Checks if a requesting service space has permission to invoke a certain method
     * with certain arguments on a specified service, according to the treaty that the
     * service space created initialliy
     */
    public void doRequestCheck(ServiceSpace requester, String treatyId,
                               String service, String method, Collection args)
        throws SecurityManagerException;

    /**
     * Checks if the contents of the response to a request is allowed -- in particular, if all the
     * arguments in the response are allowed for the requesting service space and the treaty
     */
    public void doResponseCheck(ServiceSpace requester, String treatyId,
                                String service, String method, Collection args)
        throws SecurityManagerException;

    /*-----------------------------------------------------------------------------------------------*/
    /* Methods to modify the security matrix for this service space */

    public void addServiceSpace(ServiceSpace ss, int trustLevel);
    public void removeServiceSpace(ServiceSpace ss);
    public ServiceSpace getServiceSpaceByPublicKey(PublicKey pk);
    public ServiceSpace getServiceSpaceByName(String name);

    /**
     * Adds a permission to the security matrix for a certain service space.
     * @see ServiceInvokationPermission
     */
    public void addPermission(ServiceSpace ss, ServiceInvokationPermission p);
    public void removePermission(ServiceSpace ss, ServiceInvokationPermission p);

}
