package psl.discus.javasrc.security;

import java.sql.PreparedStatement;
import java.sql.Connection;
import java.sql.SQLException;
import java.sql.ResultSet;
import java.io.*;
import java.util.Date;

import javax.sql.DataSource;

import psl.discus.javasrc.shared.DAOException;
import psl.discus.javasrc.shared.Util;
import psl.discus.javasrc.shared.FakeDataSource;
import psl.discus.javasrc.schemas.treaty.Treaty;

/**
 * This class is used to save and retrieve Treaty objects from the database
 * The treaties are stored in serialized form as a byte array.
 *
 * Author: Matias
 * Date: Mar 20, 2002
 * Time: 3:02:50 PM
 */
public class TreatyDAO {

    DataSource ds;

    public TreatyDAO(DataSource ds) {
        this.ds = ds;
    }

    /**
     * Adds a new treaty to the database, setting the status to 0 (current)
     */
    public void addTreaty(Treaty treaty)
        throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "INSERT INTO treaties(treatyid,treaty) VALUES(?,?)";
            stmt = con.prepareStatement(sql);

            stmt.setInt(1,treaty.getTreatyID());

            // serialize treaty object and put into a BLOB
            ByteArrayOutputStream byteOut = new ByteArrayOutputStream();
            ObjectOutputStream out = new ObjectOutputStream(byteOut);
            out.writeObject(treaty);
            byte[] bytes = byteOut.toByteArray();
            stmt.setBytes(2,bytes);

            int result = stmt.executeUpdate();

        } catch (Exception e) {
            throw new DAOException(e.getMessage());
        }
        finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }
    }

    /**
     * Gets treaty data for a specified treatyid.
     */
    public TreatyData getTreaty(int treatyid)
        throws DAOException
    {
        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        try {
            con = ds.getConnection();
            String sql = "SELECT * FROM treaties WHERE treatyid=?";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1,treatyid);

            rs = stmt.executeQuery();

            if (!rs.next())
                throw new DAOException("No treaty found for treatyid " + treatyid);

            // get bytes and unserialize treaty data into treaty
            byte[] bytes = rs.getBytes("treaty");
            ByteArrayInputStream byteIn = new ByteArrayInputStream(bytes);
            ObjectInputStream in = new ObjectInputStream(byteIn);
            Treaty treaty = (Treaty) in.readObject();

            int status = rs.getInt("status");
            Date created = rs.getDate("createdate");
            TreatyData treatyData = new TreatyDataImpl(treatyid, treaty, status, created);
            return treatyData;

        } catch (Exception e) {
            throw new DAOException(e.getMessage());
        }
        finally {
            try { if (rs != null) rs.close(); } catch (SQLException e) { }
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }


    }

    /**
     * Updates treaty information, re-writting the given Treaty to the database.
     */
    public void updateTreaty(Treaty treaty)
        throws DAOException
    {
        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "UPDATE treaties SET treaty=? WHERE treatyid=?";
            stmt = con.prepareStatement(sql);

            // serialize treaty object and put into a BLOB
            ByteArrayOutputStream byteOut = new ByteArrayOutputStream();
            ObjectOutputStream out = new ObjectOutputStream(byteOut);
            out.writeObject(treaty);
            byte[] bytes = byteOut.toByteArray();
            stmt.setBytes(1,bytes);
            stmt.setInt(2,treaty.getTreatyID());

            stmt.executeUpdate();

        } catch (Exception e) {
            throw new DAOException(e.getMessage());
        }
        finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }
    }

    /**
     * This method only modifies a treaty's status. Use for example when a treaty gets revoked.
     */
    public void updateTreatyStatus(int treatyid, int status)
        throws DAOException
    {
        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "UPDATE treaties SET status=? WHERE treatyid=?";
            stmt = con.prepareStatement(sql);

            stmt.setInt(1,status);
            stmt.setInt(2,treatyid);

            stmt.executeUpdate();

        } catch (Exception e) {
            throw new DAOException(e.getMessage());
        }
        finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }
    }

    /**
     * The private implementation of a TreatyData object
     */
    private class TreatyDataImpl implements TreatyData {

        private int treatyid;
        private Treaty treaty;
        private int status;
        private Date created;

        public TreatyDataImpl(int treatyid, Treaty treaty, int status, Date created) {
            this.treatyid = treatyid;
            this.treaty = treaty;
            this.status = status;
            this.created = created;
        }

        public int getTreatyId() {
            return treatyid;
        }

        public Treaty getTreaty() {
            return treaty;
        }

        public int getStatus() {
            return status;
        }

        public Date getCreatedDate() {
            return created;
        }
    }


    // for testing
    public static void main(String args[])
        throws Exception {

        /*Treaty treaty = new Treaty();
        treaty.setTreatyID((int)(100*Math.random()));
        treaty.setClientServiceSpace("clientSS");
        treaty.setProviderServiceSpace("providerSS");
        ServiceInfo si = new ServiceInfo();
        si.setServiceName("servicename");
        si.addServiceMethod(new ServiceMethod());
        treaty.addServiceInfo(si);
        */
        FakeDataSource ds = new FakeDataSource();
        TreatyDAO dao = new TreatyDAO(ds);

        //dao.addTreaty(treaty);
        Treaty treaty = dao.getTreaty(55).getTreaty();

        Util.debug("done");

    }
}
