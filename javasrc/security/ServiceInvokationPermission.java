package psl.discus.javasrc.security;

import java.util.Collection;

/**
 * This interface is used to define a certain permission for a service space.
 * For example, this could define a particular service and methods that this
 * service space has permission to access.
 *
 * Note that similarly to Service Space, there are no setter methods, so that
 * instances of objects implementing this interface can be read-only
 *
 * Author: Matias
 * Date: Mar 7, 2002
 * Time: 6:19:20 PM
 */
public interface ServiceInvokationPermission {

    /**
     * The service space that this is giving permission to
     */
    public int getServiceSpaceId();

    /**
     * The service that this is giving permission to access
     */
    public String getServiceName();

    /**
     * If this method is permitted, returns the MethodPermission
     * Otherwise, returns null
     * @see MethodPermission
     */
    public MethodPermission getMethod(String methodName);

}
