package psl.discus.javasrc.security;

import java.sql.*;
import java.util.*;

import javax.sql.DataSource;

import psl.discus.javasrc.shared.DAOException;

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


    public void addPermission(ServiceSpace clientServiceSpace, String serviceName, String methodName, String paramString)
            throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "INSERT INTO ServiceInvokationPermission(clientServiceSpaceId,serviceName,methodName,params) " +
                         "VALUES(?,?,?,?)";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1, clientServiceSpace.getId());
            stmt.setString(2, serviceName);
            stmt.setString(3, methodName);
            stmt.setString(4, paramString);

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

    public void removePermissionForMethod(ServiceSpace clientServiceSpace, String serviceName, String methodName)
            throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "DELETE FROM ServiceInvokationPermission " +
                         "WHERE clientServiceSpaceId=? AND serviceName=? AND methodName=?";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1,clientServiceSpace.getId());
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


    public ServiceInvokationPermission getPermissions(ServiceSpace clientServiceSpace, String serviceName)
            throws DAOException {

        ServiceInvokationPermissionImpl permission = new ServiceInvokationPermissionImpl(clientServiceSpace.getId(), serviceName);
        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        try {
            con = ds.getConnection();
            String sql = "SELECT methodName, params FROM ServiceInvokationPermission WHERE clientServiceSpaceId=? AND serviceName=?";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1, clientServiceSpace.getId());
            stmt.setString(2, serviceName);

            rs = stmt.executeQuery();
            while (rs.next()) {
                String methodName = rs.getString("methodName");
                String params = rs.getString("params");
                MethodPermission mp = new MethodPermissionImpl(methodName, params);
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


    private class ServiceInvokationPermissionImpl implements ServiceInvokationPermission {

        private int serviceSpaceId;
        private String serviceName;
        private Hashtable allowedMethods;

        private ServiceInvokationPermissionImpl(int serviceSpaceId, String serviceName) {
            this.serviceSpaceId = serviceSpaceId;
            this.serviceName = serviceName;
            allowedMethods = new Hashtable();
        }

        private void addMethodPermission(MethodPermission mp) {
            allowedMethods.put(mp.getMethodName(), mp);
        }

        public int getServiceSpaceId() {
            return serviceSpaceId;
        }

        public String getServiceName() {
            return serviceName;
        }

        public MethodPermission getMethod(String methodName) {
            return (MethodPermission) allowedMethods.get(methodName);
        }
    }

    private class MethodPermissionImpl implements MethodPermission {

        private String methodName;
        private Vector params;

        public MethodPermissionImpl(String methodName, String paramString) {
            this.methodName = methodName;
            params = new Vector();
            StringTokenizer st = new StringTokenizer(paramString, ",");
            while (st.hasMoreTokens()) {
                params.add(st.nextToken());
            }

        }

        public String getMethodName() {
            return methodName;
        }

        public Collection getParams() {
            return params;
        }
    }

}

