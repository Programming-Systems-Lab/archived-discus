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
    private ClientEventListener responseListener;
        // the object that is listening for a response on this query

    private String name;            // an optional name for this query

    public ClientQuery(Element queryElement, ClientEventListener responseListener) {
        this.queryElement = queryElement;
        this.responseListener = responseListener;
    }

    public Element getQueryElement() {
        return queryElement;
    }

    public ClientEventListener getResponseListener() {
        return responseListener;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String toString() {
        return "\"" + name + "\"";
    }

}
