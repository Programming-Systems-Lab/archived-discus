package psl.discus.javasrc.p2p;

import org.w3c.dom.Element;

import java.util.EventObject;

import psl.discus.javasrc.security.ServiceSpace;

/**
 * Encapsulates information about a client response -- that is, when the client gets a response
 * message from another peer.
 * @author matias
 */
public class ClientResponseEvent extends EventObject{

    private ClientQuery sourceQuery;
    private Element response;
    private ServiceSpace serviceSpace;  // the service space that sent this response

    public ClientResponseEvent(Object source, ClientQuery sourceQuery, Element response, ServiceSpace serviceSpace) {
        super(source);
        this.sourceQuery = sourceQuery;
        this.response = response;
        this.serviceSpace = serviceSpace;
    }

    /**
     * The query that this response corresponds to, to let the caller track queries and responses
     * @return the original ClientQuery object
     */
    public ClientQuery getSourceQuery() {
        return sourceQuery;
    }

    /**
     * The actual response from the peer.
     * @return an XML Element that contains as child the response(s) from the peer
     */
    public Element getResponse() {
        return response;
    }

    /**
     * The service space that sent this response (there might be multiple responses for a query)
     * @return the ServiceSpace object
     */
    public ServiceSpace getServiceSpace() {
        return serviceSpace;
    }




}
