
package psl.discus.javasrc.security;

/**
 * This class is used for now only in the p2p and uddi packages, to identify
 * which service space has signed the request being made.
 *
 * @author matias
 */
public class DiscusCredential {

    private int serviceSpaceId;

    public DiscusCredential() {
    }

    public int getServiceSpaceId() {
        return serviceSpaceId;
    }

    public void setServiceSpaceId(int serviceSpaceId) {
        this.serviceSpaceId = serviceSpaceId;
    }


}
