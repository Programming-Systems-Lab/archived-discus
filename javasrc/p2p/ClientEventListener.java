package psl.discus.javasrc.p2p;

/**
 * This interface needs to be implemented by classes that need to receive a response
 * from a client query or receive notification of new known service spaces.
 *
 * Note: because the client calls these methods synchronously, it's important that
 * they return quickly, so as not to hold the client waiting.
 *
 * @author matias
 */
public interface ClientEventListener {

    public void clientResponseEvent(ClientResponseEvent evt);

    public void clientNotificationEvent(ClientNotificationEvent evt);

}
