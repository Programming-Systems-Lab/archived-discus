package psl.discus.javasrc.security;

import psl.discus.javasrc.schemas.treaty.Treaty;

/**
 * This interface is used to represent data about an instance of a Treaty.
 * In particular, it contains the actual Treaty object (deserialized from XML)
 * and the status of the treaty (current, revoked, etc)
 * @author Matias Pelenur
 */
public interface TreatyData {

    public static final int STATUS_CURRENT = 0;
    public static final int STATUS_REVOKED = -1;

    /**
     * Get the id of this treaty
     */
    public int getTreatyId();

    /**
     * Get the actual treaty object
     */
    public Treaty getTreaty();

    /**
     * Get the current status of this treaty
     */
    public int getStatus();

}
