package psl.discus.javasrc.p2p;

import net.jxta.protocol.PipeAdvertisement;

/**
 * A ServiceSpace endpoint makes available a JXTA PipeAdvertisement for a particular
 * service space. This advertisement can be used to obtain an InputPipe and communicate
 * with that service space.
 * @author matias
 */
public interface ServiceSpaceEndpoint {

    public int getServiceSpaceId();
    public PipeAdvertisement getPipeAdvertisement();

    // could also have, for example, getTrustLevel()

}
