package psl.discus.javasrc.security;

import psl.discus.javasrc.shared.*;

import java.sql.*;
import java.util.*;

import javax.sql.DataSource;

import org.apache.log4j.Logger;

/**
 * Author: Matias
 * Date: Mar 19, 2002
 * Time: 5:11:17 PM
 */
public class ServicePermissionDAO {

    private static final Logger logger = Logger.getLogger(ServicePermissionDAO.class);

    DataSource ds;

    public ServicePermissionDAO(DataSource ds) {
        this.ds = ds;
    }


    public void addPermission(int groupId, String serviceName,
                              String methodName, String paramString, int numInvokations, String methodImplementation)
            throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "INSERT INTO ServicePermissions(groupId,serviceName,methodName," +
                         "params, numInvokations,methodImplementation) VALUES(?,?,?,?,?,?)";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1, groupId);
            stmt.setString(2, serviceName);
            stmt.setString(3, methodName);
            stmt.setString(4, paramString);
            stmt.setInt(5,numInvokations);
            stmt.setString(6,methodImplementation);

            stmt.executeUpdate();

        } catch (SQLException e) {
            throw new DAOException(e);
        } finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }
    }

    public void modifyPermission(int permissionid, int groupId, String serviceName,
                              String methodName, String paramString, int numInvokations, String methodImplementation)
        throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "UPDATE ServicePermissions SET groupId=?,serviceName=?,methodName=?," +
                         "params=?, numInvokations=?,methodImplementation=? WHERE permissionId=?";
            stmt = con.prepareStatement(sql);

            stmt.setInt(1, groupId);
            stmt.setString(2, serviceName);
            stmt.setString(3, methodName);
            stmt.setString(4, paramString);
            stmt.setInt(5,numInvokations);
            stmt.setString(6,methodImplementation);
            stmt.setInt(7,permissionid);

            stmt.executeUpdate();

        } catch (SQLException e) {
            throw new DAOException(e);
        } finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }

    }

    public void removePermissionForMethod(int permissionId)
            throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "DELETE FROM ServicePermissions WHERE permissionId=?";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1,permissionId);

            stmt.executeUpdate();

        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }

    }


    public ServicePermission getPermissions(int clientServiceSpaceId, String serviceName)
            throws DAOException {

        // we set 0 as the group id, since a permission for a particular service space can span
        // multiple groups. since this method requests permission for a particular service space anyway,
        // it shouldn't cause many problems
        ServiceInvokationPermissionImpl permission = new ServiceInvokationPermissionImpl(0, serviceName);

        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        try {
            con = ds.getConnection();
            String sql = "SELECT permissionId, methodName, params, numinvokations, methodImplementation FROM " +
                         "ServicePermissions sp, ServiceSpaceGroups sg " +
                         "WHERE serviceName=? AND sp.groupid=sg.groupid AND sg.servicespaceid=? ";
            stmt = con.prepareStatement(sql);
            stmt.setString(1, serviceName);
            stmt.setInt(2, clientServiceSpaceId);

            rs = stmt.executeQuery();
            while (rs.next()) {
                int permissionId = rs.getInt("permissionId");
                String methodName = rs.getString("methodName");
                String params = rs.getString("params");
                int numInvokations = rs.getInt("numinvokations");
                String methodImplementation = rs.getString("methodImplementation");
                MethodPermission mp =
                        new MethodPermissionImpl(permissionId, methodName, params, numInvokations, methodImplementation);
                permission.addMethodPermission(mp);
            }
        } catch (SQLException e) {
            throw new DAOException(e);
        } finally {
            try { if (rs != null) rs.close(); } catch (SQLException e) { }
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }

        return permission;
}

    public Vector getAllPermissions() throws DAOException {
        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        Vector servicePermissions = new Vector();

        try {
            con = ds.getConnection();
            String sql = "SELECT * FROM ServicePermissions ORDER BY groupid, serviceName";
            stmt = con.prepareStatement(sql);

            ServiceInvokationPermissionImpl permission = null;

            int currentGroup = -1;
            String currentService = "";
            rs = stmt.executeQuery();
            while (rs.next()) {
                int groupid = rs.getInt("groupid");
                String service = rs.getString("serviceName");
                if (currentGroup != groupid || !service.equals(currentService)) {
                    // save current and start a new ServicePermission object
                    if (permission != null) {
                        servicePermissions.add(permission);
                    }

                    permission = new ServiceInvokationPermissionImpl(groupid, service);
                    currentGroup = groupid;
                    currentService = service;
                }

                if (permission != null) {  // add this method permission to the current servicepermission
                    int permissionId = rs.getInt("permissionId");
                    String methodName = rs.getString("methodName");
                    String params = rs.getString("params");
                    int numInvokations = rs.getInt("numinvokations");
                    String methodImplementation = rs.getString("methodImplementation");
                    MethodPermission mp =
                            new MethodPermissionImpl(permissionId, methodName, params, numInvokations, methodImplementation);
                    permission.addMethodPermission(mp);
                }
            }

            if (permission != null)
                servicePermissions.add(permission);     // last one, since rs.next() will have been false

        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try { if (rs != null) rs.close(); } catch (SQLException e) { }
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }

        return servicePermissions;

    }


    /**
     * Returns a list of the registered service in the current service space
     * NOTE: this is retrieved from the registered_services table, which is administered
     * by the GateKeeper
     * @return a Vector of Service objects
     * @throws psl.discus.javasrc.shared.DAOException
     */
    public Vector getRegisteredServices() throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        Vector services = new Vector();

        try {
            con = ds.getConnection();
            String sql = "SELECT rs_service_id, rs_servicename FROM registered_services ORDER BY rs_servicename";
            stmt = con.prepareStatement(sql);

            rs = stmt.executeQuery();
            while (rs.next()) {
                Service s = new ServiceImpl(rs.getInt("rs_service_id"),rs.getString("rs_servicename"));
                services.add(s);
            }

        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try { if (rs != null) rs.close(); } catch (SQLException e) { }
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }

        return services;
    }

    /**
     * Returns a list of the registered method names for a certain service
     * NOTE: this is retrieved from the registered_services table, which is administered
     * by the GateKeeper
     * @return a Vector of Service objects
     * @throws psl.discus.javasrc.shared.DAOException
     */
    public Vector getMethodNames(int serviceId) throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        Vector methods = new Vector();

        try {
            con = ds.getConnection();
            String sql = "SELECT sm_methodname FROM service_methods " +
                         "WHERE rs_service_id=? ORDER BY sm_methodname";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1,serviceId);

            rs = stmt.executeQuery();
            while (rs.next()) {
                methods.add(rs.getString("sm_methodname"));
            }

        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try { if (rs != null) rs.close(); } catch (SQLException e) { }
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }

        return methods;
    }

    /**
     * An implementation of ServicePermission. Note that this class is private, because
     * only the ServicePermissionDAO class is allowed to create instances of it.
     * It holds the serviceName and a Vector of MethodPermission objects.
     */
    private class ServiceInvokationPermissionImpl implements ServicePermission {

        private int groupId;
        private String serviceName;
        private Vector allowedMethods;

        private ServiceInvokationPermissionImpl(int groupId, String serviceName) {
            this.groupId = groupId;
            this.serviceName = serviceName;
            allowedMethods = new Vector();
        }

        private void addMethodPermission(MethodPermission mp) {
            allowedMethods.add(mp);
        }

        public int getGroupId() {
            return groupId;
        }

        public String getServiceName() {
            return serviceName;
        }

        public Vector getMethods() {
            return allowedMethods;
        }

        /**
         * MethodsPermissions are uniquely identified by a method name and the parameter list.
         * However, because we don't know if the parameter list we get is going to be exactly
         * in the order as is stored in the database, we can't really use a hashtable to store
         * the method permissions. Instead, we use a Vector, and use a special MethodPermissionFinder
         * class to find a method permission (see below), which checks that the method name
         * is the same and that the parameter list is the same (even if it is in a different order).
         *
         * @return the MethodPermission, or null if there is no such method permission.
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

        private int permissionId;
        private String methodName;
        private Vector params;
        private int numInvokations;
        private String methodImplementation;

        public MethodPermissionImpl(int permissionId, String methodName, String paramString, int numInvokations, String methodImplementation) {
            this.permissionId = permissionId;
            this.methodName = methodName;
            this.numInvokations = numInvokations;
            params = new Vector();
            StringTokenizer st = new StringTokenizer(paramString, ", ");
            while (st.hasMoreTokens()) {
                params.add(st.nextToken());
            }
            this.methodImplementation = methodImplementation;

        }

        public int getPermissionId() {
            return permissionId;
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

    private class ServiceImpl implements Service {
        int serviceId;
        String serviceName;

        public ServiceImpl(int serviceId, String serviceName) {
            this.serviceId = serviceId;
            this.serviceName = serviceName;
        }

        public int getServiceId() {
            return serviceId;
        }

        public String getServiceName() {
            return serviceName;
        }

        public String toString() {
            return serviceName;
        }
    }

    // for testing only
    public static void main (String[] args)
        throws Exception {

        FakeDataSource ds = new FakeDataSource();
        ServicePermissionDAO dao = new ServicePermissionDAO(ds);

        //dao.addPermission(100,"service","method","foo,bar",1);
        ServicePermission perm = dao.getPermissions(100,"service");

        logger.debug("done");

    }


}

