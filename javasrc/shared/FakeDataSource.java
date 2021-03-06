package psl.discus.javasrc.shared;

import psl.discus.javasrc.shared.DAOException;

import javax.naming.Context;
import javax.naming.InitialContext;
import javax.sql.DataSource;
import java.io.*;
import java.sql.*;
import java.util.Properties;
import java.util.Map;
import java.net.URL;

import org.apache.log4j.Logger;

/**
 * This class is purely used to facilitate testing. It fakes a DataSource connection to the
 * discus development database.
 * @author Matias Pelenur
 */
public class FakeDataSource implements DataSource {

    private static final Logger logger = Logger.getLogger(FakeDataSource.class);

    private String driverClass;
    private String connectionUrl;
    private String username;
    private String password;

    private boolean persistConnection;  // if true, only one Connection will be used for all calls
    private PersistentConnection persistentConnection;

    public FakeDataSource() {
        this(DiscusProperties.getProperties());
    }

    public FakeDataSource(Properties props) {
        this(   props.getProperty("driverClass"),
                props.getProperty("connectionUrl"),
                props.getProperty("username"),
                props.getProperty("password"));
    }

    public FakeDataSource(String driverClass, String connectionUrl, String username, String password) {
        this.connectionUrl = connectionUrl;
        this.username = username;
        this.password = password;

        try {
            Class.forName(driverClass);
        } catch (ClassNotFoundException e) {
            logger.fatal(e);
        }
    }

    public void setPersistConnection(boolean persistConnection)
        throws DAOException {

        try {
            if (persistConnection && !this.persistConnection) {
                persistentConnection = new PersistentConnection(getConnection());
            }
            else if (!persistConnection && this.persistConnection) {
                persistentConnection.reallyClose();
            }
        } catch (SQLException e) {
            throw new DAOException(e);
        }

        this.persistConnection = persistConnection;
    }

    public Connection getConnection() throws SQLException {
        if (!persistConnection) {
            return DriverManager.getConnection(connectionUrl, username, password);
        }
        else {
            return persistentConnection;
        }
    }

    public Connection getConnection(String s, String s1) throws SQLException {
        return null;
    }

    public PrintWriter getLogWriter() throws SQLException {
        return null;
    }

    public int getLoginTimeout() throws SQLException {
        return 0;
    }

    public void setLogWriter(PrintWriter writer) throws SQLException {
    }

    public void setLoginTimeout(int i) throws SQLException {
    }

    public void testConnection()
        throws SQLException {

        Connection con = null;
        try {
            con = getConnection();
        }
        finally {
            try { if (con != null) con.close(); } catch (SQLException e) {}
        }
    }


    /**
     * This class is kind of a big hack... But, to be able to do persisten connections,
     * that is a connection that does not close, we need to override the close() method so as
     * not to close the connection. However, since we don't know what class will be implementing
     * Connection, we need to instead wrap it with our PersistentConnection class, which just delegates
     * every method call except close() to the real implementation
     */
    private class PersistentConnection implements Connection {

        private Connection realConnection;

        public PersistentConnection(Connection realConnection) {
            this.realConnection = realConnection;
        }

        public Statement createStatement() throws SQLException {
            return realConnection.createStatement();
        }

        public PreparedStatement prepareStatement(String sql)
                throws SQLException {
            return realConnection.prepareStatement(sql);
        }

        public CallableStatement prepareCall(String sql) throws SQLException {
            return realConnection.prepareCall(sql);
        }

        public String nativeSQL(String sql) throws SQLException {
            return realConnection.nativeSQL(sql);
        }

        public void setAutoCommit(boolean autoCommit) throws SQLException {
            realConnection.setAutoCommit(autoCommit);
        }

        public boolean getAutoCommit() throws SQLException {
            return realConnection.getAutoCommit();
        }

        public void commit() throws SQLException {
            realConnection.commit();
        }

        public void rollback() throws SQLException {
            realConnection.rollback();
        }

        public void close() throws SQLException {
            // do nothing
        }

        public void reallyClose() throws SQLException {
            // this will actually close the underlying connection
            realConnection.close();
        }

        public boolean isClosed() throws SQLException {
            return false;
        }

        public DatabaseMetaData getMetaData() throws SQLException {
            return realConnection.getMetaData();
        }

        public void setReadOnly(boolean readOnly) throws SQLException {
            realConnection.setReadOnly(readOnly);
        }

        public boolean isReadOnly() throws SQLException {
            return realConnection.isReadOnly();
        }

        public void setCatalog(String catalog) throws SQLException {
            realConnection.setCatalog(catalog);
        }

        public String getCatalog() throws SQLException {
            return realConnection.getCatalog();
        }

        public void setTransactionIsolation(int level) throws SQLException {
            realConnection.setTransactionIsolation(level);
        }

        public int getTransactionIsolation() throws SQLException {
            return realConnection.getTransactionIsolation();
        }

        public SQLWarning getWarnings() throws SQLException {
            return realConnection.getWarnings();
        }

        public void clearWarnings() throws SQLException {
            realConnection.clearWarnings();
        }

        public Statement createStatement(int resultSetType, int resultSetConcurrency)
                throws SQLException {
            return realConnection.createStatement(resultSetType, resultSetConcurrency);
        }

        public PreparedStatement prepareStatement(String sql, int resultSetType,
                                                  int resultSetConcurrency)
                throws SQLException {
            return realConnection.prepareStatement(sql, resultSetType, resultSetConcurrency);
        }

        public CallableStatement prepareCall(String sql, int resultSetType,
                                             int resultSetConcurrency) throws SQLException {
            return realConnection.prepareCall(sql, resultSetType, resultSetConcurrency);
        }

        public Map getTypeMap() throws SQLException {
            return realConnection.getTypeMap();
        }

        public void setTypeMap(Map map) throws SQLException {
            realConnection.setTypeMap(map);
        }
    }
}
