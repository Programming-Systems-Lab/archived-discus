package psl.discus.javasrc.p2p;

import net.jxta.protocol.PipeAdvertisement;
import psl.discus.javasrc.security.ServiceSpace;

/**
 * A ServiceSpace endpoint makes available a JXTA PipeAdvertisement for a particular
 * service space. This advertisement can be used to obtain an InputPipe and communicate
 * with that service space.
 * @author matias
 */
public interface ServiceSpaceEndpoint {

    public ServiceSpace getServiceSpace();

    public PipeAdvertisement getPipeAdvertisement();

}
