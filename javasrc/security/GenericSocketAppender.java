package psl.discus.javasrc.security;

import org.apache.log4j.AppenderSkeleton;
import org.apache.log4j.Layout;
import org.apache.log4j.helpers.LogLog;
import org.apache.log4j.spi.LoggingEvent;

import java.io.*;
import java.net.InetAddress;
import java.net.Socket;

/**
 * This class is used together with the log4j package to provide logging to a remote socket
 * (like the one provided by the discus GUI). It borrow *heavily* from the SocketAppender class
 * in org.apache.log4j.net
 * @author matias
 */
public class GenericSocketAppender extends AppenderSkeleton {


    /**
     The default port number of remote logging server (4560).
     */
    static final int DEFAULT_PORT = 8088;

    /**
     The default reconnection delay (30000 milliseconds or 30 seconds).
     */
    static final int DEFAULT_RECONNECTION_DELAY = 30000;

    /**
     We remember host name as String in addition to the resolved
     InetAddress so that it can be returned via getOption().
     */
    String remoteHost;

    InetAddress address;
    int port = DEFAULT_PORT;
    //ObjectOutputStream oos;
    PrintWriter writer;
    int reconnectionDelay = DEFAULT_RECONNECTION_DELAY;
    boolean locationInfo = false;


    String prefix;

    private GenericSocketAppender.Connector connector;

    public GenericSocketAppender() {
    }

    public GenericSocketAppender(Layout layout) {
        this.layout = layout;
    }
    /**
     Connects to remote server at <code>address</code> and <code>port</code>.
     */
    public GenericSocketAppender(InetAddress address, int port) {
        this.address = address;
        this.remoteHost = address.getHostName();
        this.port = port;
        connect(address, port);
    }

    /**
     Connects to remote server at <code>host</code> and <code>port</code>.
     */
    public  GenericSocketAppender(String host, int port) {
        this.port = port;
        this.address = getAddressByName(host);
        this.remoteHost = host;
        connect(address, port);
    }

    /**
     Connect to the specified <b>RemoteHost</b> and <b>Port</b>.
     */
    public  void activateOptions() {
        connect(address, port);
    }

    /**
     Close this appender.
     <p>This will mark the appender as closed and
     call then {@link #cleanUp} method.
     */
    synchronized public void close() {
        if (closed)
            return;

        this.closed = true;
        cleanUp();
    }

    /**
     Drop the connection to the remote host and release the underlying
     connector thread if it has been created
     */
    public void cleanUp() {
        if (writer != null) {
            writer.close();
            writer = null;
        }
        if (connector != null) {
            //LogLog.debug("Interrupting the connector.");
            connector.interrupted = true;
            connector = null;  // allow gc
        }
    }

    void connect(InetAddress address, int port) {
        if (this.address == null)
            return;
        try {
            // First, close the previous connection if any.
            cleanUp();
            //oos = new ObjectOutputStream(new Socket(address, port).getOutputStream());
            writer = new PrintWriter(new OutputStreamWriter(new Socket(address, port).getOutputStream()), true);
        } catch (IOException e) {
            LogLog.error("Could not connect to remote socket server at [" + address.getHostName() + "]: " + e);
            fireConnector();
        }
    }


    public void append(LoggingEvent event) {
        if (event == null)
            return;

        if (address == null) {
            errorHandler.error("No remote host is set for GenericSocketAppender named \"" +
                    this.name + "\".");
            return;
        }

        if (writer != null) {

            if (locationInfo) {
                event.getLocationInformation();
            }

            String msg = (String) event.getMessage();
            writer.println(prefix + msg);

            if (writer.checkError()) {
                writer = null;
                LogLog.warn("Detected problem with connection.");
                if (reconnectionDelay > 0) {
                    fireConnector();
                }
            }
        }
    }

    void fireConnector() {
        if (connector == null) {
            LogLog.debug("Starting a new connector thread.");
            connector = new GenericSocketAppender.Connector();
            connector.setDaemon(true);
            connector.setPriority(Thread.MIN_PRIORITY);
            connector.start();
        }
    }

    static InetAddress getAddressByName(String host) {
        try {
            return InetAddress.getByName(host);
        } catch (Exception e) {
            LogLog.error("Could not find address of [" + host + "]: " + e);
            return null;
        }
    }

    /**
     The GenericSocketAppender does not use a layout. Hence, this method returns
     <code>false</code>.
     */
    public boolean requiresLayout() {
        return false;
    }

    /**
     The <b>RemoteHost</b> option takes a string value which should be
     the host name of the server where a {@link org.apache.log4j.net.SocketNode} is running.
     */
    public  void setRemoteHost(String host) {
        address = getAddressByName(host);
        remoteHost = host;
    }

    /**
     Returns value of the <b>RemoteHost</b> option.
     */
    public  String getRemoteHost() {
        return remoteHost;
    }

    /**
     The <b>Port</b> option takes a positive integer representing
     the port where the server is waiting for connections.
     */
    public  void setPort(int port) {
        this.port = port;
    }

    /**
     Returns value of the <b>Port</b> option.
     */
    public  int getPort() {
        return port;
    }

    /**
     The <b>LocationInfo</b> option takes a boolean value. If true,
     the information sent to the remote host will include location
     information. By default no location information is sent to the server.
     */
    public void setLocationInfo(boolean locationInfo) {
        this.locationInfo = locationInfo;
    }

    /**
     Returns value of the <b>LocationInfo</b> option.
     */
    public  boolean getLocationInfo() {
        return locationInfo;
    }

    /**
     The <b>ReconnectionDelay</b> option takes a positive integer
     representing the number of milliseconds to wait between each
     failed connection attempt to the server. The default value of
     this option is 30000 which corresponds to 30 seconds.

     <p>Setting this option to zero turns off reconnection
     capability.
     */
    public  void setReconnectionDelay(int delay) {
        this.reconnectionDelay = delay;
    }

    /**
     Returns value of the <b>ReconnectionDelay</b> option.
     */
    public int getReconnectionDelay() {
        return reconnectionDelay;
    }

    public String getPrefix() {
        return prefix;
    }

    public void setPrefix(String prefix) {
        this.prefix = prefix;
    }


    /**
     The Connector will reconnect when the server becomes available
     again.  It does this by attempting to open a new connection every
     <code>reconnectionDelay</code> milliseconds.

     <p>It stops trying whenever a connection is established. It will
     restart to try reconnect to the server when previpously open
     connection is droppped.

     @author  Ceki G&uuml;lc&uuml;
     @since 0.8.4
     */
    class Connector extends Thread {

        boolean interrupted = false;

        public void run() {
            Socket socket;
            while (!interrupted) {
                try {
                    sleep(reconnectionDelay);
                    LogLog.debug("Attempting connection to " + address.getHostName());
                    socket = new Socket(address, port);
                    synchronized (this) {
                        //oos = new ObjectOutputStream(socket.getOutputStream());
                        writer = new PrintWriter(new OutputStreamWriter(socket.getOutputStream()),true);
                        connector = null;
                        break;
                    }
                } catch (InterruptedException e) {
                    LogLog.debug("Connector interrupted. Leaving loop.");
                    return;
                } catch (java.net.ConnectException e) {
                    LogLog.debug("Remote host " + address.getHostName()
                            + " refused connection.");
                } catch (IOException e) {
                    LogLog.debug("Could not connect to " + address.getHostName() +
                            ". Exception is " + e);
                }
            }
            //LogLog.debug("Exiting Connector.run() method.");
        }

        /**
         public
         void finalize() {
         LogLog.debug("Connector finalize() has been called.");
         }
         */
    }

}

