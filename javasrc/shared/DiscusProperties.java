package psl.discus.javasrc.shared;

import org.apache.log4j.Logger;

import java.util.Properties;
import java.io.InputStream;

/**
 * Manages properties loaded from the main .properties file
 * @author matias
 */
public final class DiscusProperties {

    private static final Logger logger = Logger.getLogger(DiscusProperties.class);

    public static final String PROPERTIES_FILENAME = "SecurityManager.properties";

    private static String defaultConnectionURL = "jdbc:hsqldb:db/discus-security";
    private static String defaultDriverClass = "org.hsqldb.jdbcDriver";
    private static String defaultUsername = "sa";
    private static String defaultPassword = "";

    private static Properties properties;

    static {

        try {
            InputStream in = FakeDataSource.class.getClassLoader().getResourceAsStream(PROPERTIES_FILENAME);
            if (in != null) {

                properties = new Properties();
                properties.load(in);

                logger.debug("loaded config values from SecurityManager.properties");

            }
        } catch (Exception e) {
            logger.debug("could not load properties: " + e);
        }

        if (properties == null) {
            logger.debug("using default values");
            properties = new Properties();
            properties.setProperty("driverClass", defaultDriverClass);
            properties.setProperty("connectionUrl", defaultConnectionURL);
            properties.setProperty("username", defaultUsername);
            properties.setProperty("password", defaultPassword);
        }

    }

    public static final Properties getProperties() {
        return properties;
    }

    public static final String getProperty(String name) {
        return properties.getProperty(name);
    }


}
