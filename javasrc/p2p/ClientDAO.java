package psl.discus.javasrc.p2p;

import psl.discus.javasrc.shared.*;

import javax.sql.DataSource;

/**
 * Handles database access for the Client class
 * @author matias
 */
public class ClientDAO {

    private DataSource ds;

    public ClientDAO(DataSource ds) {
        this.ds = ds;
    }

    public static void addAdvertisement()
        throws DAOException {

    }
}
