package psl.discus.javasrc.security;

import java.util.Vector;

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
public interface ServicePermission {

    /**
     * The group of service spaces (could be just one service space) that this is giving permission to.
     */
    public int getGroupId();

    /**
     * The service that this is giving permission to access.
     */
    public String getServiceName();

    /**
     * All the allowed methods in this permission
     */
    public Vector getMethods();

    /**
     * Tries to find a method with the given name and parameter list.
     * Otherwise, returns null
     * @see MethodPermission
     */
    public MethodPermission getMethod(String methodName, String[] params);

}
