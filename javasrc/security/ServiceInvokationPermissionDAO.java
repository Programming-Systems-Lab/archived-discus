package psl.discus.javasrc.security;

import java.sql.*;
import java.util.*;

import javax.sql.DataSource;

import psl.discus.javasrc.shared.DAOException;
import psl.discus.javasrc.shared.FakeDataSource;
import psl.discus.javasrc.shared.Util;

/**
 * Author: Matias
 * Date: Mar 19, 2002
 * Time: 5:11:17 PM
 */
public class ServiceInvokationPermissionDAO {

    DataSource ds;

    ServiceInvokationPermissionDAO(DataSource ds) {
        this.ds = ds;
    }


    public void addPermission(int clientServiceSpaceId, String serviceName,
                              String methodName, String paramString, int numInvokations, String methodImplementation)
            throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "INSERT INTO ServiceInvokationPermission(clientServiceSpaceId,serviceName,methodName," +
                         "params, numInvokations,methodImplementation) VALUES(?,?,?,?,?)";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1, clientServiceSpaceId);
            stmt.setString(2, serviceName);
            stmt.setString(3, methodName);
            stmt.setString(4, paramString);
            stmt.setInt(5,numInvokations);
            stmt.setString(6,methodImplementation);

            int result = stmt.executeUpdate();

        } catch (SQLException e) {
            throw new DAOException(e);
        } finally {
            try {
                if (stmt != null) stmt.close();
                if (con != null) con.close();
            }
            catch (SQLException e) { }
        }
    }

    public void removePermissionForMethod(int clientServiceSpaceId, String serviceName, String methodName)
            throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "DELETE FROM ServiceInvokationPermission " +
                         "WHERE clientServiceSpaceId=? AND serviceName=? AND methodName=?";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1,clientServiceSpaceId);
            stmt.setString(2,serviceName);
            stmt.setString(3,methodName);

            int result = stmt.executeUpdate();

        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try {
                if (stmt != null) stmt.close();
                if (con != null) con.close();
            }
            catch (SQLException e) { }
        }

    }


    public ServiceInvokationPermission getPermissions(int clientServiceSpaceId, String serviceName)
            throws DAOException {

        ServiceInvokationPermissionImpl permission = new ServiceInvokationPermissionImpl(clientServiceSpaceId, serviceName);
        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        try {
            con = ds.getConnection();
            String sql = "SELECT methodName, params, numinvokations, methodImplementation FROM ServiceInvokationPermission " +
                         "WHERE clientServiceSpaceId=? AND serviceName=?";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1, clientServiceSpaceId);
            stmt.setString(2, serviceName);

            rs = stmt.executeQuery();
            while (rs.next()) {
                String methodName = rs.getString("methodName");
                String params = rs.getString("params");
                int numInvokations = rs.getInt("numinvokations");
                String methodImplementation = rs.getString("methodImplementation");
                MethodPermission mp = new MethodPermissionImpl(methodName, params, numInvokations, methodImplementation);
                permission.addMethodPermission(mp);
            }
        } catch (SQLException e) {
            throw new DAOException(e);
        } finally {
            try {
                if (rs != null) rs.close();
                if (stmt != null) stmt.close();
                if (con != null) con.close();
            }
            catch (SQLException e) { }
        }

        return permission;


    }

    /**
     * An implementation of ServiceInvokationPermission. Note that this class is private, because
     * only the ServiceInvokationPermissionDAO class is allowed to create instances of it.
     * It holds the serviceName and a Vector of MethodPermission objects.
     */
    private class ServiceInvokationPermissionImpl implements ServiceInvokationPermission {

        private int serviceSpaceId;
        private String serviceName;
        private Vector allowedMethods;

        private ServiceInvokationPermissionImpl(int serviceSpaceId, String serviceName) {
            this.serviceSpaceId = serviceSpaceId;
            this.serviceName = serviceName;
            allowedMethods = new Vector();
        }

        private void addMethodPermission(MethodPermission mp) {
            allowedMethods.add(mp);
        }

        public int getServiceSpaceId() {
            return serviceSpaceId;
        }

        public String getServiceName() {
            return serviceName;
        }

        /**
         * MethodsPermissions are uniquely identified by a method name and the parameter list.
         * However, because we don't know if the parameter list we get is going to be exactly
         * in the order as is stored in the database, we can't really use a hashtable to store
         * the method permissions. Instead, we use a Vector, and use a special MethodPermissionFinder
         * class to find a method permission (see below), which checks that the method name
         * is the same and that the parameter list is the same (even if it is in a different order).
         *
         * @returns the MethodPermission, or null if there is no such method permission.
         */
        public MethodPermission getMethod(String methodName, String[] params) {

            MethodPermissionFinder finder = new MethodPermissionFinder(methodName, params);
            int index = allowedMethods.indexOf(finder);
            if (index == -1)
                return null;
            else
                return (MethodPermission) allowedMethods.elementAt(index);
        }
    }

    private class MethodPermissionImpl implements MethodPermission {

        private String methodName;
        private Vector params;
        private int numInvokations;
        private String methodImplementation;

        public MethodPermissionImpl(String methodName, String paramString, int numInvokations, String methodImplementation) {
            this.methodName = methodName;
            this.numInvokations = numInvokations;
            params = new Vector();
            StringTokenizer st = new StringTokenizer(paramString, ",");
            while (st.hasMoreTokens()) {
                params.add(st.nextToken());
            }
            this.methodImplementation = methodImplementation;

        }

        public String getMethodName() {
            return methodName;
        }

        public Collection getParams() {
            return params;
        }

        public int getNumberInvokations() {
            return numInvokations;
        }

        public String getMethodImplementation() {
            return methodImplementation;
        }
    }

    /**
     * This class is uses exclusively to find MethodPermissions in the methods Vector of the
     * ServiceInvokationPermissionImpl.
     */
    private class MethodPermissionFinder {
        String methodName;
        String[] params;

        public MethodPermissionFinder(String methodName, String[] params) {
            this.methodName = methodName;
            this.params = params;
        }

        public boolean equals(Object o) {
            if (!(o instanceof MethodPermission))
                return false;

            MethodPermission mp = (MethodPermission) o;
            // first we check that the name matches, and then the params
            if (!mp.getMethodName().equals(methodName))
                return false;
            else if (!mp.getParams().containsAll(Arrays.asList(params)))
                return false;
            else
                return true;
        }
    }

    // for testing only
    public static void main (String[] args)
        throws Exception {

        FakeDataSource ds = new FakeDataSource();
        ServiceInvokationPermissionDAO dao = new ServiceInvokationPermissionDAO(ds);

        //dao.addPermission(100,"service","method","foo,bar",1);
        ServiceInvokationPermission perm = dao.getPermissions(100,"service");

        Util.debug("done");

    }
}

