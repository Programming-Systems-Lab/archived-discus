package psl.discus.javasrc.schemas.treaty;

import java.sql.PreparedStatement;
import java.sql.Connection;
import java.sql.SQLException;
import java.sql.ResultSet;
import java.io.*;

import javax.sql.DataSource;

import psl.discus.javasrc.shared.DAOException;
import psl.discus.javasrc.shared.Util;
import psl.discus.javasrc.shared.FakeDataSource;

/**
 * This class is used to save and fetch Treaty objects from the database
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
            throw new DAOException(e);
        }
        finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }
    }

    public Treaty getTreaty(int treatyid)
        throws DAOException
    {
        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        try {
            con = ds.getConnection();
            String sql = "SELECT treaty FROM treaties WHERE treatyid=?";
            stmt = con.prepareStatement(sql);
            stmt.setInt(1,treatyid);

            rs = stmt.executeQuery();

            if (!rs.next())
                throw new DAOException("no treaties found for treatyid " + treatyid);

            // get bytes and unserialize into treaty
            byte[] bytes = rs.getBytes(1);
            ByteArrayInputStream byteIn = new ByteArrayInputStream(bytes);
            ObjectInputStream in = new ObjectInputStream(byteIn);

            Treaty treaty = (Treaty) in.readObject();
            return treaty;

        } catch (Exception e) {
            throw new DAOException(e);
        }
        finally {
            try { if (rs != null) rs.close(); } catch (SQLException e) { }
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
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
        Treaty treaty = dao.getTreaty(55);

        Util.debug("done");

    }
}
