package psl.discus.javasrc.p2p;

import org.w3c.dom.Element;

import java.util.EventObject;

/**
 * Encapsulates information about a client response -- that is, when the client gets a response
 * message from another peer.
 * @author matias
 */
public class ClientResponseEvent extends EventObject{

    private ClientQuery sourceQuery;
    private Element response;

    public ClientResponseEvent(Object source, ClientQuery sourceQuery, Element response) {
        super(source);
        this.sourceQuery = sourceQuery;
        this.response = response;
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



}
