/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.datastore.jdbc;

import psl.discus.javasrc.uddi.error.*;
import psl.discus.javasrc.uddi.uuidgen.UUID;

import org.uddi4j.*;
import org.uddi4j.datatype.*;
import org.uddi4j.datatype.binding.*;
import org.uddi4j.datatype.business.*;
import org.uddi4j.datatype.service.*;
import org.uddi4j.util.*;
import org.apache.log4j.Logger;



import java.sql.Connection;

import java.sql.PreparedStatement;

import java.sql.ResultSet;

import java.sql.Statement;

import java.sql.Timestamp;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class BindingTemplateTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(BindingTemplateTable.class);

  static String dropSQL = null;

  static String createSQL = null;
  static String insertSQL = null;
  static String deleteSQL = null;
  static String selectSQL = null;
  static String selectByServiceKeySQL = null;
  static String deleteByServiceKeySQL = null;
  static String deleteByBusinessKeySQL = null;
  static String verifyOwnershipSQL = null;

  static
  {
    // buffer used to build SQL statements
    StringBuffer sql = null;

    // build dropSQL

    dropSQL = "DROP TABLE BINDING_TEMPLATE";



    // build createSQL
    sql = new StringBuffer(100);
    sql.append("CREATE TABLE BINDING_TEMPLATE (");
    sql.append("SERVICE_KEY VARCHAR(41) NOT NULL,");
    sql.append("BINDING_KEY VARCHAR(41) NOT NULL,");
    sql.append("ACCESS_POINT_TYPE VARCHAR(20),");
    sql.append("ACCESS_POINT_URL VARCHAR(255),");
    sql.append("HOSTING_REDIRECTOR VARCHAR(255),");
    sql.append("LAST_UPDATE TIMESTAMP NOT NULL,");

    sql.append("PRIMARY KEY (BINDING_KEY),");
    sql.append("FOREIGN KEY (SERVICE_KEY) REFERENCES BUSINESS_SERVICE (SERVICE_KEY))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(150);
    sql.append("INSERT INTO BINDING_TEMPLATE (");
    sql.append("SERVICE_KEY,");
    sql.append("BINDING_KEY,");
    sql.append("ACCESS_POINT_TYPE,");
    sql.append("ACCESS_POINT_URL,");
    sql.append("HOSTING_REDIRECTOR,");

    sql.append("LAST_UPDATE) ");
    sql.append("VALUES (?,?,?,?,?,?)");
    insertSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM BINDING_TEMPLATE ");
    sql.append("WHERE BINDING_KEY=?");
    deleteSQL = sql.toString();

    // build selectSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("SERVICE_KEY,");
    sql.append("ACCESS_POINT_TYPE,");
    sql.append("ACCESS_POINT_URL,");
    sql.append("HOSTING_REDIRECTOR ");
    sql.append("FROM BINDING_TEMPLATE ");
    sql.append("WHERE BINDING_KEY=?");
    selectSQL = sql.toString();

    // build selectByServiceKeySQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("BINDING_KEY,");
    sql.append("ACCESS_POINT_TYPE,");
    sql.append("ACCESS_POINT_URL,");
    sql.append("HOSTING_REDIRECTOR ");
    sql.append("FROM BINDING_TEMPLATE ");
    sql.append("WHERE SERVICE_KEY=?");
    selectByServiceKeySQL = sql.toString();

    // build deleteByServiceKeySQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM BINDING_TEMPLATE ");
    sql.append("WHERE SERVICE_KEY=?");
    deleteByServiceKeySQL = sql.toString();

    // build verifyOwnershipSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("* ");
    sql.append("FROM BUSINESS_ENTITY e, BUSINESS_SERVICE s, BINDING_TEMPLATE t ");
    sql.append("WHERE s.SERVICE_KEY = t.SERVICE_KEY ");
    sql.append("AND e.BUSINESS_KEY = s.BUSINESS_KEY ");
    sql.append("AND t.BINDING_KEY=? ");
    sql.append("AND e.AUTHORIZED_NAME=?");
    verifyOwnershipSQL = sql.toString();
  }

  /**

   * Drop the BINDING_TEMPLATE table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE BINDING_TEMPLATE: ");

    Statement statement = null;



    try

    {

      statement = connection.createStatement();

      int returnCode = statement.executeUpdate(dropSQL);

      System.out.println("Successful (return code=" + returnCode + ")");

    }

    catch (java.sql.SQLException sqlex)

    {

      System.out.println("Failed (error message="+sqlex.getMessage() + ")\n");

      System.out.println("SQL="+dropSQL);

    }

    finally

    {

      try {

        statement.close();

      }

      catch (Exception e) { } // nothing we can do about this!

    }

  }



  /**
   * Create the BINDING_TEMPLATE table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE BINDING_TEMPLATE: ");
    Statement statement = null;

    try
    {
      statement = connection.createStatement();
      int returnCode = statement.executeUpdate(createSQL);
      System.out.println("Successful (return code=" + returnCode + ")");
    }
    catch (java.sql.SQLException sqlex)
    {
      System.out.println("Failed (error message="+sqlex.getMessage() + ")\n");
      System.out.println("SQL="+createSQL);
    }
    finally
    {
      try {
        statement.close();
      }
      catch (Exception e) { } // nothing we can do about this!
    }
  }

  /**
   * Insert new row into the BINDING_TEMPLATE table.
   *
   * @param  binding Binding Template object holding values to be inserted
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void insert(BindingTemplate binding,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (binding == null)
      throw new JUDDIException("attempt to insert a null BindingTemplate");

    if (binding.getBindingKey() == null)
      throw new JUDDIException("attempt to insert a BindingTemplate with a null BindingKey");

    if (binding.getServiceKey() == null)
      throw new JUDDIException("attempt to insert a BindingTemplate with a null ServiceKey");

    if ((binding.getAccessPoint() == null) && (binding.getHostingRedirector() == null))
      throw new JUDDIException("attempt to insert a BindingTemplate with a null AccessPoint and a null HostingRedirector");

    if ((binding.getAccessPoint() != null) && (binding.getHostingRedirector() != null))
      throw new JUDDIException("attempt to insert a BindingTemplate with both a AccessPoint and a HostingRedirector");

    PreparedStatement statement = null;
    Timestamp timeStamp = new Timestamp(System.currentTimeMillis());


    try
    {
      // pull the raw AccessPoint attributes out (if any)
      String urlType = null;
      String url = null;
      AccessPoint accessPoint = binding.getAccessPoint();
      if (accessPoint != null)
      {
        urlType = accessPoint.getURLType();
        url = accessPoint.getText();
      }

      // pull the raw HostingRedirector attributes out (if any)
      String redirectorKey = null;
      HostingRedirector redirector = binding.getHostingRedirector();
      if (redirector != null)
      {
        if (redirector.getBindingKey() != null)
          redirectorKey = redirector.getBindingKey();
      }

      // prepare and execute the insert
      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,binding.getServiceKey().toString());
      statement.setString(2,binding.getBindingKey().toString());
      statement.setString(3,urlType);
      statement.setString(4,url);
      statement.setString(5,redirectorKey);
      statement.setTimestamp(6,timeStamp);


      log.info("insert into BINDING_TEMPLATE table:\n\n\t" + insertSQL +
        "\n\t SERVICE_KEY=" + binding.getServiceKey().toString() +
        "\n\t BINDING_KEY=" + binding.getBindingKey().toString() +
        "\n\t ACCESS_POINT_TYPE=" + urlType +
        "\n\t ACCESS_POINT_URL=" + url +
        "\n\t HOSTING_REDIRECTOR=" + redirectorKey +

        "\n\t LAST_UPDATE=" + timeStamp.getTime() + "\n");

      int returnCode = statement.executeUpdate();

      log.info("insert was successful, return code=" + returnCode);
    }
    finally
    {
      try {
        statement.close();
      }
      catch (Exception e) { } // nothing we can do about this!
    }
  }

  /**
   * Delete row from the BINDING_TEMPLATE table.
   *
   * @param  primary key value
   * @param  JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void delete(String bindingKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (bindingKey == null)
      throw new JUDDIException("attempt to delete a BindingTemplate with a null BindingKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,bindingKey.toString());

      log.info("delete from BINDING_TEMPLATE table:\n\n\t" + deleteSQL +
        "\n\t BINDING_KEY=" + bindingKey.toString() + "\n");

      // execute the delete
      int returnCode = statement.executeUpdate();

      log.info("delete was successful, rows deleted=" + returnCode);
    }
    finally
    {
      try {
        statement.close();
      }
      catch (Exception e) { } // nothing we can do about this!
    }
  }

  /**
   * Select one row from the BINDING_TEMPLATE table.
   *
   * @param  primary key value
   * @param  JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static BindingTemplate select(String bindingKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (bindingKey == null)
      throw new JUDDIException("attempt to select a BindingTemplate using a null BindingKey");

    BindingTemplate binding = null;
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,bindingKey.toString());

      log.info("select from BINDING_TEMPLATE table:\n\n\t" + selectSQL +
        "\n\t BINDING_KEY=" + bindingKey.toString() + "\n");

      resultSet = statement.executeQuery();
      if (resultSet.next())
      {
        binding = new BindingTemplate();
        binding.setServiceKey(resultSet.getString("SERVICE_KEY"));
        binding.setBindingKey(bindingKey);

        String urlType = resultSet.getString("ACCESS_POINT_TYPE");
        String url = resultSet.getString("ACCESS_POINT_URL");
        if ((urlType != null) && (url != null))
          binding.setAccessPoint(new AccessPoint(urlType,url));

        String redirectorKey = resultSet.getString("HOSTING_REDIRECTOR");
        if (redirectorKey != null)
          binding.setHostingRedirector(new HostingRedirector(redirectorKey));
      }

      if (binding != null)
        log.info("select successful, at least one row was found");
      else
        log.info("select executed successfully but no rows were found with BINDING_KEY=" + bindingKey.toString());

      return binding;
    }
    finally
    {
      try {
        resultSet.close();
        statement.close();
      }
      catch (Exception e) { } // nothing we can do about this!
    }
  }

  /**
   * Select all rows from the business_service table for a given
   * BusinessKey.<p>
   *
   * @param  serviceKey ServiceKey
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static Vector selectByServiceKey(String serviceKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (serviceKey == null)
      throw new JUDDIException("attempt to select a Collection of BindingTemplate instances using a null ServiceKey");

    Vector bindList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectByServiceKeySQL);
      statement.setString(1,serviceKey.toString());

      log.info("select from BINDING_TEMPLATE table:\n\n\t" + selectByServiceKeySQL +
        "\n\t SERVICE_KEY=" + serviceKey.toString() + "\n");

      // execute the statement
      resultSet = statement.executeQuery();

      BindingTemplate binding = null;
      while (resultSet.next())
      {
        binding = new BindingTemplate();
        binding.setServiceKey(serviceKey);
        binding.setBindingKey(resultSet.getString("BINDING_KEY"));

        String urlType = resultSet.getString("ACCESS_POINT_TYPE");
        String url = resultSet.getString("ACCESS_POINT_URL");
        if ((urlType != null) && (url != null))
          binding.setAccessPoint(new AccessPoint(urlType,url));

        String redirectorKey = resultSet.getString("HOSTING_REDIRECTOR");
        if (redirectorKey != null)
          binding.setHostingRedirector(new HostingRedirector(redirectorKey));

        bindList.add(binding);
        binding = null;
      }

      log.info("select was successful, rows selected=" + bindList.size());
      return bindList;
    }
    finally
    {
      try {
        resultSet.close();
        statement.close();
      }
      catch (Exception e) { } // nothing we can do about this!
    }
  }

  /**
   * Delete multiple rows from the BINDING_TEMPLATE table that are assigned to
   * the BusinessKey specified.<p>
   *
   * @param  serviceKey ServiceKey
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void deleteByServiceKey(String serviceKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (serviceKey == null)
      throw new JUDDIException("attempt to delete a Collection of BindingTemplate instances using a null ServiceKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteByServiceKeySQL);
      statement.setString(1,serviceKey.toString());

      log.info("delete from BINDING_TEMPLATE table:\n\n\t" + deleteByServiceKeySQL +
        "\n\t SERVICE_KEY=" + serviceKey.toString() + "\n");

      // execute the delete
      int returnCode = statement.executeUpdate();

      log.info("delete was successful, rows deleted=" + returnCode);
    }
    finally
    {
      try {
        statement.close();
      }
      catch (Exception e) { } // nothing we can do about this!
    }
  }

  /**
   * Verify that 'authorizedName' has the authority to update or delete 
   * BindingTemplate identified by the bindingKey parameter
   *
   * @param  bindingKey
   * @param  authorizedName
   * @param  connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static boolean verifyOwnership(String bindingKey,String authorizedName,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if ((bindingKey == null) || (authorizedName == null))
      return false;

    boolean authorized = false;
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      statement = connection.prepareStatement(verifyOwnershipSQL);
      statement.setString(1,bindingKey);
      statement.setString(2,authorizedName);

      log.info("checking ownership of BINDING_TEMPLATE:\n\n\t" + verifyOwnershipSQL +
        "\n\t BINDNG_KEY=" + bindingKey +
        "\n\t AUTHORIZED_NAME=" + authorizedName + "\n");

      resultSet = statement.executeQuery();
      if (resultSet.next())
        authorized = true;
      
      if (authorized)
        log.info("authorization was successful, a matching row was found");
      else
        log.info("select executed successfully but authorization was unsuccessful");

      return authorized;
    }
    finally
    {
      try {
        resultSet.close();
        statement.close();
      }
      catch (Exception e) { } // nothing we can do about this!
    }
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  // unit test-driver
  public static void main(String[] args)
  {
    psl.discus.javasrc.uddi.util.SysManager.startup();
    ConnectionPool pool = ConnectionPool.getInstance();
    PooledConnection pooledConnection = pool.acquirePooledConnection();
    Connection connection = pooledConnection.getConnection();

    if (pooledConnection == null)
      throw new RuntimeException("PooledConnection is null - cannot continue with test");

    if (connection == null)
      throw new RuntimeException("Connection is null - cannot continue with test");

    test(connection);

    pool.releasePooledConnection(pooledConnection);
    pool.destroy();
    psl.discus.javasrc.uddi.util.SysManager.shutdown();
  }

  // system test-driver
  public static void test(Connection connection)
  {
    Transaction txn = new Transaction();

    if (connection != null)
    {
      try
      {
        String businessKey = UUID.nextID();
        BusinessEntity business = new BusinessEntity();
        business.setBusinessKey(businessKey);
        business.setAuthorizedName("sviens");
        business.setOperator("WebServiceRegistry.com");

        String serviceKey = UUID.nextID();
        BusinessService service = new BusinessService();
        service.setBusinessKey(businessKey);
        service.setServiceKey(serviceKey);

        String bindingKey = UUID.nextID();
        BindingTemplate binding = new BindingTemplate();
        binding.setServiceKey(serviceKey);
        binding.setBindingKey(bindingKey);
        binding.setAccessPoint(new AccessPoint("http://juddi.org/bindingtemplate.html","http"));

        // begin a new transaction
        txn.begin(connection);

        // insert a new BusinessEntity
        BusinessEntityTable.insert(business,connection);

        // insert a new BusinessService
        BusinessServiceTable.insert(service,connection);

        // insert a new BindingTemplate
        BindingTemplateTable.insert(binding,connection);

        // insert another BindingTemplate
        binding.setBindingKey(UUID.nextID());
        BindingTemplateTable.insert(binding,connection);

        // insert one more BindingTemplate
        binding.setBindingKey(UUID.nextID());
        BindingTemplateTable.insert(binding,connection);

        // select a particular BindingTemplate
        binding = BindingTemplateTable.select(bindingKey,connection);

        // delete that BindingTemplate
        BindingTemplateTable.delete(bindingKey,connection);

        // re-select that BindingTemplate
        binding = BindingTemplateTable.select(bindingKey,connection);

        // select a Collection of BindingTemplate objects (by ServiceKey)
        Vector bindingList = BindingTemplateTable.selectByServiceKey(serviceKey,connection);

        // delete a Collection of BindingTemplate objects (by ServiceKey)
        BindingTemplateTable.deleteByServiceKey(serviceKey,connection);

        // re-select a Collection of BindingTemplate objects (by ServiceKey)
        bindingList = BindingTemplateTable.selectByServiceKey(serviceKey,connection);

        // commit the transaction
        txn.commit();
      }
      catch(Exception ex)
      {
        ex.printStackTrace();
        try {
          txn.rollback();
        }
        catch(java.sql.SQLException sqlex) {
          sqlex.printStackTrace();
        }
      }
    }
  }
}
