package psl.discus.javasrc.security;

/**
 * Represents a service in the database
 * NOTE: the services table is administed by the GateKeeper, not by the Security Manager
 * This interface is used just by the SecurityGUI when it gets the available service names
 * @author matias
 */
public interface Service {

    public int getServiceId();
    public String getServiceName();

}
