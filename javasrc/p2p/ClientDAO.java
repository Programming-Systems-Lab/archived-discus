package psl.discus.javasrc.p2p;

import psl.discus.javasrc.shared.*;
import psl.discus.javasrc.security.ServiceSpace;
import psl.discus.javasrc.security.ServiceSpaceDAO;

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

    public ServiceSpaceEndpoint addPipeAdvertisement(ServiceSpace serviceSpace, PipeAdvertisement pipeAd)
        throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "INSERT INTO ServiceSpaceEndpoints(serviceSpaceId,pipeAd) VALUES(?,?)";
            stmt = con.prepareStatement(sql);

            ByteArrayOutputStream out = new ByteArrayOutputStream();
            pipeAd.getDocument(xmlMimeMediaType).sendToStream(out);

            stmt.setInt(1,serviceSpace.getServiceSpaceId());
            stmt.setString(2,out.toString());

            stmt.executeUpdate();

            return new ServiceSpaceEndpointImpl(serviceSpace, pipeAd);

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
            String sql = "SELECT sse.pipeAd, ss.* FROM ServiceSpaceEndpoints sse, ServiceSpaces ss " +
                         "WHERE sse.serviceSpaceId = ss.serviceSpaceId";
            stmt = con.prepareStatement(sql);

            rs = stmt.executeQuery();
            while (rs.next()) {
                int serviceSpaceId = rs.getInt("ss.serviceSpaceId");
                String serviceSpaceName = rs.getString("serviceSpaceName");
                int trustLevel = rs.getInt("trustLevel");
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

                ServiceSpace serviceSpace =
                        new ServiceSpaceDAO.ServiceSpaceImpl(serviceSpaceId,serviceSpaceName,trustLevel);
                ads.add(new ServiceSpaceEndpointImpl(serviceSpace,pipeAd));

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

        private ServiceSpace serviceSpace;
        private PipeAdvertisement pipeAd;

        public ServiceSpaceEndpointImpl(ServiceSpace serviceSpace, PipeAdvertisement pipeAd) {
            this.serviceSpace = serviceSpace;
            this.pipeAd = pipeAd;
        }

        public ServiceSpace getServiceSpace() {
            return serviceSpace;
        }

        public PipeAdvertisement getPipeAdvertisement() {
            return pipeAd;
        }
    }
}
