package psl.discus.javasrc.shared;

import java.sql.Connection;
import java.sql.SQLException;
import java.io.PrintWriter;

import java.sql.*;
import javax.sql.DataSource;

/**
 * @author Matias Pelenur
 */
public class FakeDataSource implements DataSource {

    public static final String CONNECTION_URL = "jdbc:postgresql://liberty.psl.cs.columbia.edu/discus";
    public static final String DRIVER_CLASS = "org.postgresql.Driver";
    public static final String USERNAME = "alpa";
    public static final String PASSWORD = "discus";

    static {
        try {
            Class.forName(DRIVER_CLASS);
        } catch (ClassNotFoundException e) {
            Util.debug(e);
        }
    }

    public Connection getConnection() throws SQLException {
        return DriverManager.getConnection(CONNECTION_URL,USERNAME,PASSWORD);
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
}
