package psl.discus.javasrc.p2p;

import org.w3c.dom.Element;

/**
 * This class is used to send queries via the Client class.
 * The class sending the query can keep track of the responses by checking what
 * query a response is tied to.
 *
 * Note that this class could be extended, if necessary, to provide more information
 * to the caller when the query is replied to (since this same object is returned with the response)
 *
 * @author matias
 */
public class ClientQuery {

    private Element queryElement;    // the XML element holding the actual query
    private ClientResponseListener responseListener;
        // the object that is listening for a response on this query

    public ClientQuery(Element queryElement, ClientResponseListener responseListener) {
        this.queryElement = queryElement;
        this.responseListener = responseListener;
    }

    public Element getQueryElement() {
        return queryElement;
    }

    public ClientResponseListener getResponseListener() {
        return responseListener;
    }


}