package psl.discus.javasrc.security;

import java.security.PublicKey;

/**
 * This interface defines the properties of a Service Space.
 * Note that there are no setter methods. This is because this instances of
 * objects implementing this interface might be ready only, and other methods
 * might be used to change the properties of a Service Space.
 *
 * Author: Matias
 * Date: Mar 7, 2002
 * Time: 6:12:20 PM
 */

public interface ServiceSpace {

    public int getId();
    public String getName();

}
