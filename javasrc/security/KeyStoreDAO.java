package psl.discus.javasrc.security;

import psl.discus.javasrc.shared.*;

import java.sql.*;
import java.io.*;
import java.security.KeyStore;
import java.security.PrivateKey;

import javax.sql.DataSource;

/**
 * Handles loading and saving the KeyStore to the database
 * @author Matias Pelenur
 */
public class KeyStoreDAO {

    public static final String KEYSTORE_TYPE = "JKS";

    private DataSource ds;

    public KeyStoreDAO(DataSource ds) {
        this.ds = ds;
    }

    /**
     * Loads a KeyStore from the database.
     * @param keystore an instance of a KeyStore created with the correct type. This is the keystore
     * that the data will be loaded into.
     */
    public void loadKeyStore(int keystoreid, KeyStore keystore, char[] keyStorePassword)
        throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;
        ResultSet rs = null;

        try {
            con = ds.getConnection();
            String sql = "SELECT keystore FROM KeyStore WHERE keystoreid=?";
            stmt = con.prepareStatement(sql);

            stmt.setInt(1,keystoreid);

            rs = stmt.executeQuery();
            if (!rs.next())
                throw new DAOException("keystore not found");

            // get bytes and unserialize into treaty
            byte[] bytes = rs.getBytes(1);
            ByteArrayInputStream byteIn = new ByteArrayInputStream(bytes);
            try {
                keystore.load(byteIn, keyStorePassword);
            } catch (Exception e) {
                throw new DAOException("Could not load keystore: " + e.getMessage());
            }

        } catch (SQLException e) {
            throw new DAOException(e.getMessage());
        }
        finally {
            try { if (rs != null) rs.close(); } catch (SQLException e) { }
            try { if (stmt != null) stmt.close(); } catch (SQLException e) { }
            try { if (con != null) con.close(); } catch (SQLException e) { }
        }
    }

    /**
     * Stores a new keystore in the database.
     */
    public void storeKeyStore(int keystoreid, KeyStore keystore, char[] keyStorePassword)
        throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "INSERT INTO KeyStore(keystoreid,keystore) VALUES(?,?)";
            stmt = con.prepareStatement(sql);

            stmt.setInt(1,keystoreid);

            ByteArrayOutputStream byteOut = new ByteArrayOutputStream();
            keystore.store(byteOut, keyStorePassword);
            byte[] bytes = byteOut.toByteArray();

            stmt.setBytes(2,bytes);
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
     * Updates (overwrites) an existing keystore in the database with a new keystore
     */
    public void updateKeyStore(int keystoreid, KeyStore keystore, char[] keyStorePassword)
        throws DAOException {

        Connection con = null;
        PreparedStatement stmt = null;

        try {
            con = ds.getConnection();
            String sql = "UPDATE KeyStore SET keystore=? WHERE keystoreid=?";
            stmt = con.prepareStatement(sql);

            ByteArrayOutputStream byteOut = new ByteArrayOutputStream();
            keystore.store(byteOut, keyStorePassword);
            byte[] bytes = byteOut.toByteArray();

            stmt.setBytes(1,bytes);
            stmt.setInt(2,keystoreid);

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
     * Convenience method to store an existing keystore into the database
     */
    public void storeKeyStoreFromFile(String filename, String keyStoreType, char[] keyStorePassword)
        throws DAOException {

        KeyStore keyStore = null;
        try {
            keyStore = KeyStore.getInstance(keyStoreType);
            File keystoreFile = new File(filename);
            if (!keystoreFile.exists())
                throw new DAOException("Could not find keystore at " + keystoreFile.getCanonicalPath());

            FileInputStream fis = new FileInputStream(keystoreFile);
            keyStore.load(fis, keyStorePassword);
            if (keyStore == null)
                throw new DAOException("Could not load keystore");

        } catch (Exception e) {
            throw new DAOException(e.getMessage());
        }

        storeKeyStore(0,keyStore, keyStorePassword);

    }

    // for testing and initial putting keystore file into database
    public static void main(String[] args)
        throws Exception {

        if (args.length < 2) {
            Util.debug("Usage: KeyStoreDAO keystorefile password");
            return;
        }

        KeyStoreDAO dao = new KeyStoreDAO(new FakeDataSource());
        dao.storeKeyStoreFromFile(args[0],"JKS",args[1].toCharArray());

        Util.debug("KeyStore stored in database.");
    }

}
