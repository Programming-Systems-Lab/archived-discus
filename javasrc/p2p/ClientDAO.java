package psl.discus.javasrc.p2p;

import psl.discus.javasrc.shared.*;

import javax.sql.DataSource;

import net.jxta.protocol.PipeAdvertisement;
import net.jxta.document.*;

import java.sql.*;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.util.Vector;

import org.apache.log4j.Logger;

/**
 * Handles database access for the Client class
 * @author matias
 */
public class ClientDAO {
    private static final MimeMediaType xmlMimeMediaType = new MimeMediaType("text/xml");
    private static final Logger logger = Logger.getLogger(ClientDAO.class);

    private DataSource ds;

    public ClientDAO(DataSource ds) {
        this.ds = ds;
    }

    public ServiceSpaceEndpoint addPipeAdvertisement(int serviceSpaceId, PipeAdvertisement pipeAd)
        throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "INSERT INTO ServiceSpaceEndpoints(serviceSpaceId,pipeAd) VALUES(?,?)";
            stmt = con.prepareStatement(sql);

            ByteArrayOutputStream out = new ByteArrayOutputStream();
            pipeAd.getDocument(xmlMimeMediaType).sendToStream(out);

            stmt.setInt(1,serviceSpaceId);
            stmt.setString(2,out.toString());

            stmt.executeUpdate();

            return new ServiceSpaceEndpointImpl(serviceSpaceId, pipeAd);

        } catch (Exception e) {
            throw new DAOException("Could not store pipe advertisement: " + e.getMessage());
        }
        finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }
    }

     public void removePipeAdvertisement(PipeAdvertisement pipeAd)
        throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "DELETE FROM ServiceSpaceEndpoints WHERE pipeAd=?";
            stmt = con.prepareStatement(sql);

            ByteArrayOutputStream out = new ByteArrayOutputStream();
            pipeAd.getDocument(xmlMimeMediaType).sendToStream(out);

            stmt.setString(1,out.toString());

            stmt.executeUpdate();

        } catch (Exception e) {
            throw new DAOException("Could not remove pipe advertisement: " + e.getMessage());
        }
        finally {
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }
    }

    public Vector getServiceSpaceEndpoints()
        throws DAOException {

        Vector ads = new Vector();

        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        try {
            con = ds.getConnection();
            String sql = "SELECT * FROM ServiceSpaceEndpoints";
            stmt = con.prepareStatement(sql);

            rs = stmt.executeQuery();
            while (rs.next()) {
                int serviceSpaceId = rs.getInt("serviceSpaceId");
                String pipeAdString = rs.getString("pipeAd");
                PipeAdvertisement pipeAd = null;

                // instantiate pipead from the string
                try {
                    ByteArrayInputStream in = new ByteArrayInputStream(pipeAdString.getBytes());
                    pipeAd = (PipeAdvertisement)
                            AdvertisementFactory.newAdvertisement(xmlMimeMediaType, in);
                    in.close();
                } catch (Exception e) {
                    logger.warn("failed to read/parse pipe advertisement: " + pipeAdString);
                    continue;
                }

                ads.add(new ServiceSpaceEndpointImpl(serviceSpaceId,pipeAd));

            }

            return ads;

        } catch (SQLException e) {
            throw new DAOException("Could not load advertisements: " + e.getMessage());
        }
        finally {
            try { if (rs != null) rs.close(); } catch (SQLException e) { }
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }


    }

    private class ServiceSpaceEndpointImpl implements ServiceSpaceEndpoint {

        private int serviceSpaceId;
        private PipeAdvertisement pipeAd;

        public ServiceSpaceEndpointImpl(int serviceSpaceId, PipeAdvertisement pipeAd) {
            this.serviceSpaceId = serviceSpaceId;
            this.pipeAd = pipeAd;
        }

        public int getServiceSpaceId() {
            return serviceSpaceId;
        }

        public PipeAdvertisement getPipeAdvertisement() {
            return pipeAd;
        }
    }
}
