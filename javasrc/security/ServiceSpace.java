package psl.discus.javasrc.security;

/**
 * Represents a service space on the database
 * @author matias
 */
public interface ServiceSpace {

    public int getServiceSpaceId();

    // In the future this could also hold the service space's certificate (public key)
    // as well as description, etc

}
