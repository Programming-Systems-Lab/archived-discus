package psl.discus.javasrc.p2p;

/**
 * This interface needs to be implemented by classes that need to receive a response
 * from a client query.
 * @author matias
 */
public interface ClientResponseListener {

    public void clientResponseEvent(ClientResponseEvent evt);

}
