/* * jUDDI - An open source Java implementation of UDDI v2.0 * http://juddi.org/ * * Copyright (c) 2002, Steve Viens and contributors * All rights reserved. */package psl.discus.javasrc.uddi.datastore.jdbc;import psl.discus.javasrc.uddi.util.Config;import org.apache.log4j.Logger;import java.sql.Connection;import java.sql.DriverManager;import java.util.Vector;/** * @author  Steve Viens * @version 0.6 */public class ConnectionPool{  // private reference to the jUDDI logger  private static Logger log = Logger.getLogger(ConnectionPool.class);  // private referenct to singleton jUDDI connection pool  private static ConnectionPool connectionPool = null;  // private default values for connections and connection pooling  private static final String defaultDriver         = "org.hsqldb.jdbcDriver";  private static final String defaultURL            = "jdbc:hsqldb:" + Config.getHomeDir() + "/hsql/juddidb";  private static final String defaultUser           = "sa";  private static final String defaultPassword       = "";  private static final int    defaultMinConnections = 10;  private static final int    defaultMaxConnections = 50;  private static final long   defaultMaxWaitTime    = 10000;  // 10 seconds  private static final long   defaultRetryInterval  = 1000;   // 1 second  private static final long   defaultVerifyInterval = 5000;   // 5 seconds  // connection pool instance variables  private Vector  pool;  private int     slack;  private String  driver;  private String  url;  private String  user;  private String  password;  private int     minConnections;  private int     maxConnections;  private long    maxWaitTime;  private long    retryInterval;  private long    verifyInterval;  /**   *   */  public static ConnectionPool getInstance()  {    if (connectionPool == null)      createInstance();    return connectionPool;  }  /**   *   */  private static synchronized void createInstance()  {    if (connectionPool == null)      connectionPool = new ConnectionPool();  }  /**   * Private default constructor handles initialization of the ConnectionPool   * instance. The initialization is limited to setting up a pool of connections   * that will be handed out to the threads using this class. Made private   * cause we don't want this constructor to be called from any method except   * it's own 'createInstance' method.   *   * @throws RuntimeException   */  private ConnectionPool()  {    String configDriver =      Config.getProperty("juddi.datastore.jdbcDriver");    String configURL =      Config.getProperty("juddi.datastore.jdbcURL");    String configUser =      Config.getProperty("juddi.datastore.jdbcUser");    String configPassword =      Config.getProperty("juddi.datastore.jdbcPassword");    Integer configMinConnections =      Config.getPropertyInteger("juddi.datastore.jdbcMinConnections");    Integer configMaxConnections =      Config.getPropertyInteger("juddi.datastore.jdbcMaxConnections");    Integer configMaxWaitTime =      Config.getPropertyInteger("juddi.datastore.jdbcMaxWaitTime");    Integer configRetryInterval =      Config.getPropertyInteger("juddi.datastore.jdbcRetryInterval");    Integer configVerifyInterval =      Config.getPropertyInteger("juddi.datastore.jdbcVerifyInterval");    this.driver = (configDriver == null)      ? defaultDriver      : configDriver;    this.url = (configURL == null)      ? defaultURL      : configURL;    this.user = (configUser == null)      ? defaultUser      : configUser;    this.password = (configPassword == null)      ? defaultPassword      : configPassword;    this.minConnections = (configMinConnections == null)      ? defaultMinConnections      : configMinConnections.intValue();    this.maxConnections = (configMaxConnections == null)      ? defaultMaxConnections      : configMaxConnections.intValue();    this.maxWaitTime = (configMaxWaitTime == null)      ? defaultMaxWaitTime      : configMaxWaitTime.longValue();    this.retryInterval = (configRetryInterval == null)      ? defaultRetryInterval      : configRetryInterval.longValue();    this.verifyInterval = (configVerifyInterval == null)      ? defaultVerifyInterval      : configVerifyInterval.longValue();    this.minConnections = (minConnections < 0)      ? defaultMinConnections      : minConnections;    this.maxConnections = (maxConnections < 0)      ? defaultMaxConnections      : maxConnections;    this.maxConnections = (maxConnections < minConnections)      ? minConnections      : maxConnections;    this.driver = (configDriver == null)      ? defaultDriver      : configDriver;    this.url = (configURL == null)      ? defaultURL      : configURL;    this.user = (configUser == null)      ? defaultUser      : configUser;    this.password = (configPassword == null)      ? defaultPassword      : configPassword;    log.info("Database Driver:     " + this.driver);    log.info("Database URL:        " + this.url);    log.info("Database User:       " + this.user);    log.info("Database Password:   " + this.password);    log.info("Minimum Connections: " + this.minConnections);    log.info("Maximum Connections: " + this.maxConnections);    log.info("Maximum Wait Time:   " + this.maxWaitTime);    log.info("Retry Interval:      " + this.retryInterval);    log.info("Verify Interval:     " + this.verifyInterval);    log.info("Creating " + minConnections + " pooled connection(s) of a possible " + maxConnections);    pool = new Vector(maxConnections);    for (int i=0; i<minConnections; i++)    {      PooledConnection connection = createPooledConnection();      if (connection != null)      {        pool.add(connection);      }      else      {        // may have had db error just occur - verify previous connections:        int j;        for (j=0; j<pool.size(); j++)        {          boolean connectionClosed = false;          connection = (PooledConnection)pool.elementAt(j);          connectionClosed = connection.isClosed();          if (connectionClosed)            connection.setConnection(null);        }        // remove any bad connections, reverse order to avoid possible side effects..        if (pool.size() > 0)        {          for ( ; j>=0; j--)          {            connection = (PooledConnection)pool.elementAt(j);            if (connection.getConnection() == null)              pool.remove(j);          }        }      }    }    slack = maxConnections - pool.size();  }  /**   * Returns a PooledConnection from the connection pool. If no connection   * is available ("maxConnections" is reached) and there is pool slack   * (initially only allocate "initialConnections"), we make another   * connection. If there is no connection available and no pool slack we   * wait for up to "maxWaitTime" milliseconds for one "etryInterval"   * milliseconds.   *   * @return an instance of <code>PooledConnection</code> from the pool.   */  public PooledConnection acquirePooledConnection()    throws RuntimeException  {    PooledConnection connection = null;    synchronized(pool) // might hang around here waiting for the lock..    {      long endTime = 0;      // what are our events?      // 1. A connection is returned to the pool.      // 2. A connection is identified as bad and the pool slack is incremented      // avoid the system time calls for the 99.9% of time they're unnecessary      do      {        // try to return an existing connection, only        // take one, spread the load across threads...        connection = null;        if (!pool.isEmpty())          connection = (PooledConnection)pool.remove(0);        if (connection != null)        {          log.debug("allocated connection from pool");          break; // slight efficiencies..        }        // 99.9% of the time we don't want to go past this point        // if pool was empty, got any room for growth?.. if pool was not        // empty but the connection was bad we'll be trying to re-acquire        // one here...        if (slack > 0)        {          connection = createPooledConnection();          if (connection != null)          {            // success! - decrement the slack count            slack--;            log.debug("create connection successfull, slack " +              "decremented to " + slack);            break;          }        } // end if got slack        // not much success that time through, wait a bit longer if we can..        if (endTime == 0) // first time in          endTime = System.currentTimeMillis() + maxWaitTime;        long waitTime = endTime - System.currentTimeMillis();        if (waitTime > retryInterval)          waitTime = retryInterval;        if (waitTime > 0)        {          log.debug("pool is empty, no slack/no connect success, continue " +            "to wait " + waitTime + " milliseconds..");          try          {            pool.wait(waitTime);          }          catch(InterruptedException ie) {}        }        // arrive here with waitTime expired or given        // control of the critical section for retry      }      while (System.currentTimeMillis() < endTime && connection == null);    } // end synchronized block    if (connection != null)      log.debug("returning a good connection, slack=" + slack);    else    {      log.error("unable to return a good connection, slack=" + slack);      throw new RuntimeException("unable to return a good connection, slack=" + slack);    }    return connection;  }  /**   * Returns a single pooled connection to the pool that was created when   * the class was initialized. When a connection is returned to the pool   * we examine the time since its last verified time and if it exceeds   * "verifyInterval" milliseconds we verify that the connection is   * good. If it's not good we don't add it back to the pool but instead   * increment the pool slack so another connection can be made.   *   * @param db  The database manager to be returned to the pool for use   *            by other threads.   */  public void releasePooledConnection(PooledConnection connection)  {    if (connection != null)    {      // verify that our connection is ok      boolean connectionClosed = false;      if (connection.getTimeLastVerified() + verifyInterval < System.currentTimeMillis())      {        connectionClosed = connection.isClosed();        connection.setTimeLastVerified(System.currentTimeMillis());      }      else        connectionClosed = false;      synchronized(pool)      {        if (connectionClosed)          slack++; // we thought it was good but it isn't!        else          pool.add(connection);        // increased slack pool space is a noteworthy event        pool.notifyAll();      } // end synchronized block      log.debug("returned connection to pool");    }    else    {      log.error("Released a null connection");    }  }  /**   * Iterate through the pool Vector of Databases removing each   * Database object and closing it's JDBC connection if neccessary.   */  protected void destroy()  {    log.info("Destroying jUDDI connection pool; " +      slack + " pool slots remaining and " +      pool.size() + " connections accounted for");    synchronized(pool)    {      while(pool.isEmpty() == false)      {        PooledConnection connection = (PooledConnection)pool.remove(0);        if (connection != null)        {          try          {            connection.getConnection().close();          }          catch(java.sql.SQLException e) { /* whatever */ }          connection = null;        }      }      slack = maxConnections;    }  }  /**   *   */  private PooledConnection createPooledConnection()  {    Connection connection = null;    try    {      Class.forName(driver);      connection = (Connection)DriverManager.getConnection(url,user,password);      connection.setAutoCommit(false);    }    catch(Exception e)    {      log.warn("error connecting to database: " + e.getMessage());    }    return new PooledConnection(connection);  }  /**   *   */  public int getCount()  {    if (pool!=null)      return this.pool.size();    else      return -1;  }  /**   *   */  public int getSlack()  {    return this.slack;  }  /**   *   */  public String getDriver()  {    return this.driver;  }  /**   *   */  public String getURL()  {    return this.url;  }  /**   *   */  public String getUser()  {    return this.user;  }  /**   *   */  public String getPassword()  {    return this.password;  }  /**   *   */  public int getMinConnections()  {    return this.minConnections;  }  /**   *   */  public int getMaxConnections()  {    return this.maxConnections;  }  /**   *   */  public long getMaxWaitTime()  {    return this.maxWaitTime;  }  /**   *   */  public long getRetryInterval()  {    return this.retryInterval;  }  /**   *   */  public long getVerifyInterval()  {    return this.verifyInterval;  }}