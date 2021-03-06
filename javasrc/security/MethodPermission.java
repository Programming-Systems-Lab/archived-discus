package psl.discus.javasrc.security;

import java.util.Collection;

/**
 * This interface is used to define access permission for methods and arguments
 *
 * Author: Matias
 * Date: Mar 7, 2002
 * Time: 6:31:03 PM
 */
public interface MethodPermission {

    /**
     * A unique identifier for this permission
     */
    public int getPermissionId();

    public String getMethodName();

    public Collection getParams();

    public int getNumberInvokations();

    /**
     * The real method name for this method call, given back in the treaty
     */
    public String getMethodImplementation();

}
