package psl.discus.javasrc.security;

import psl.discus.javasrc.shared.DAOException;

import java.sql.*;
import java.util.*;

import javax.sql.DataSource;

public class ServiceSpaceDAO {

    DataSource ds;

    public ServiceSpaceDAO(DataSource ds) {
        this.ds = ds;
    }

    public void addServiceSpace(String serviceSpaceName) throws DAOException {
        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "INSERT INTO ServiceSpaces(serviceSpaceName) VALUES(?)";
            stmt = con.prepareStatement(sql);
            stmt.setString(1,serviceSpaceName);

            stmt.executeUpdate();

        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }
    }

    public ServiceSpace getServiceSpaceByName(String serviceSpaceName) throws DAOException {
        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        ServiceSpace ss = null;

        try {
            con = ds.getConnection();
            String sql = "SELECT * FROM ServiceSpaces WHERE serviceSpaceName=?";
            stmt = con.prepareStatement(sql);

            stmt.setString(1,serviceSpaceName);

            rs = stmt.executeQuery();
            if (!rs.next()) {
                throw new DAOException("ServiceSpace with name " + serviceSpaceName + " not found.");
            }

            ss = new ServiceSpaceImpl(rs.getInt("serviceSpaceId"), serviceSpaceName,
                                      rs.getInt("trustLevel"));

        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try { if (rs != null) rs.close(); } catch (SQLException e) { }
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }

        return ss;
    }

    public Vector getGroups() throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        Vector v = new Vector();

        try {
            con = ds.getConnection();
            String sql = "SELECT * FROM Groups";
            stmt = con.prepareStatement(sql);

            rs = stmt.executeQuery();
            while (rs.next()) {
                GroupImpl g = new GroupImpl(rs.getInt("groupid"), rs.getString("groupname"));
                v.add(g);
            }
        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try { if (rs != null) rs.close(); } catch (SQLException e) { }
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }

        return v;
    }

    public Vector getServiceSpacesInGroup(int groupId) throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        Vector v = new Vector();

        try {
            con = ds.getConnection();
            String sql = "SELECT ssg.serviceSpaceId, ss.serviceSpaceName, ss.trustLevel " +
                    "FROM ServiceSpaceGroups ssg, ServiceSpaces ss WHERE groupid=?";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1,groupId);

            rs = stmt.executeQuery();
            while (rs.next()) {
                ServiceSpaceImpl ss = new ServiceSpaceImpl(rs.getInt("servicespaceid"),
                                                           rs.getString("serviceSpaceName"),
                                                           rs.getInt("trustLevel"));
                v.add(ss);
            }
        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try { if (rs != null) rs.close(); } catch (SQLException e) { }
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }

        return v;

    }

    public void addGroup(String groupName) throws DAOException {
        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "INSERT INTO Groups(groupname) VALUES(?)";
            stmt = con.prepareStatement(sql);
            stmt.setString(1,groupName);

            stmt.executeUpdate();

        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }
    }

    public void addServiceSpaceToGroup(int serviceSpaceId, int groupId)
        throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "INSERT INTO ServiceSpaceGroups(groupid,servicespaceid) VALUES(?,?)";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1,groupId);
            stmt.setInt(2,serviceSpaceId);

            stmt.executeUpdate();

        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }

    }

    public void removeServiceSpaceFromGroup(int serviceSpaceId, int groupId)
        throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "DELETE FROM ServiceSpaceGroups WHERE groupid=? AND servicespaceid=?";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1,groupId);
            stmt.setInt(2,serviceSpaceId);

            stmt.executeUpdate();

        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }

    }

    public void removeGroup(int groupId)
        throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "DELETE FROM Groups WHERE groupid=?";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1,groupId);

            stmt.executeUpdate();

        } catch (SQLException e) {
            throw new DAOException(e);
        }
        finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }

    }

    private class GroupImpl implements Group {

        private int groupId;
        private String name;

        public GroupImpl(int groupId, String name) {
            this.groupId = groupId;
            this.name = name;
        }

        public int getGroupId() {
            return groupId;
        }

        public String getName() {
            return name;
        }

        public String toString() {
            return name;
        }
    }

    public static class ServiceSpaceImpl implements ServiceSpace {

        int serviceSpaceId;
        String name;
        int trustLevel;

        public ServiceSpaceImpl(int serviceSpaceId, String name, int trustLevel) {
            this.serviceSpaceId = serviceSpaceId;
            this.name = name;
            this.trustLevel = trustLevel;
        }

        public int getServiceSpaceId() {
            return serviceSpaceId;
        }

        public String getName() {
            return name;
        }

        public int getTrustLevel() {
            return trustLevel;
        }

        public String toString() {
            return "Service Space " + serviceSpaceId +
                    " (id=" + serviceSpaceId + ", trustlevel=" + trustLevel + ")";
        }
    }
}
