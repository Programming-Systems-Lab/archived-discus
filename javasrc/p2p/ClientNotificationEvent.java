package psl.discus.javasrc.p2p;

import org.w3c.dom.Element;

import java.util.EventObject;

import psl.discus.javasrc.security.ServiceSpace;

/**
 * Encapsulates information about a client notification, sent when the client
 * finds a new discus p2p server peer
 * @author matias
 */
public class ClientNotificationEvent extends EventObject {

    ServiceSpaceEndpoint serviceSpaceEndpoint;

    public ClientNotificationEvent(Object source, ServiceSpaceEndpoint serviceSpace) {
        super(source);
        this.serviceSpaceEndpoint = serviceSpace;
    }

    public ServiceSpaceEndpoint getServiceSpaceEndpoint() {
        return serviceSpaceEndpoint;
    }

}
