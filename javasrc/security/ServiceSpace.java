package psl.discus.javasrc.security;

/**
 * Represents a service space on the database
 * @author matias
 */
public interface ServiceSpace {

    /**
     * The unique id this service space is identified by
     * @return the service space's id
     */
    public int getServiceSpaceId();

    /**
     * The name for this service space. This should match the certificate
     * alias for this service space too (currently stored in the keystores table)
     * @return the service space's name or alias
     */
    public String getName();

    /**
     * The trust level associated with this service space.
     * At this point, this is only use in the p2p/uddi module when deciding
     * which service spaces to query for certain services, and not for
     * authorizing incoming requests.
     * @return the trust level
     */
    public int getTrustLevel();
}
