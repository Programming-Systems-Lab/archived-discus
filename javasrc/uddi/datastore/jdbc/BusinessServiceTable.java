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
public class BusinessServiceTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(BusinessServiceTable.class);

  static String dropSQL = null;

  static String createSQL = null;
  static String insertSQL = null;
  static String deleteSQL = null;
  static String selectSQL = null;
  static String selectByBusinessKeySQL = null;
  static String deleteByBusinessKeySQL = null;
  static String verifyOwnershipSQL = null;
  
  static
  {
    // buffer used to build SQL statements
    StringBuffer sql = null;

    // build dropSQL

    dropSQL = "DROP TABLE BUSINESS_SERVICE";



    // build createSQL
    sql = new StringBuffer(100);
    sql.append("CREATE TABLE BUSINESS_SERVICE (");
    sql.append("BUSINESS_KEY VARCHAR(41) NOT NULL,");
    sql.append("SERVICE_KEY VARCHAR(41) NOT NULL,");
    sql.append("LAST_UPDATE TIMESTAMP NOT NULL,");

    sql.append("PRIMARY KEY (SERVICE_KEY),");
    sql.append("FOREIGN KEY (BUSINESS_KEY) REFERENCES BUSINESS_ENTITY (BUSINESS_KEY))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(100);
    sql.append("INSERT INTO BUSINESS_SERVICE (");
    sql.append("BUSINESS_KEY,");
    sql.append("SERVICE_KEY,");

    sql.append("LAST_UPDATE) ");
    sql.append("VALUES (?,?,?)");
    insertSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM BUSINESS_SERVICE ");
    sql.append("WHERE SERVICE_KEY=?");
    deleteSQL = sql.toString();

    // build selectSQL
    sql = new StringBuffer(100);
    sql.append("SELECT ");
    sql.append("BUSINESS_KEY ");
    sql.append("FROM BUSINESS_SERVICE ");
    sql.append("WHERE SERVICE_KEY=?");
    selectSQL = sql.toString();

    // build selectByBusinessKeySQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("SERVICE_KEY ");
    sql.append("FROM BUSINESS_SERVICE ");
    sql.append("WHERE BUSINESS_KEY=?");
    selectByBusinessKeySQL = sql.toString();

    // build deleteByBusinessKeySQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM BUSINESS_SERVICE ");
    sql.append("WHERE BUSINESS_KEY=?");
    deleteByBusinessKeySQL = sql.toString();
    
    // build verifyOwnershipSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("* ");
    sql.append("FROM BUSINESS_ENTITY e, BUSINESS_SERVICE s ");
    sql.append("WHERE e.BUSINESS_KEY = s.BUSINESS_KEY ");
    sql.append("AND s.SERVICE_KEY=? ");
    sql.append("AND e.AUTHORIZED_NAME=?");
    verifyOwnershipSQL = sql.toString();
  }

  /**

   * Drop the BUSINESS_SERVICE table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE BUSINESS_SERVICE: ");

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
   * Create the BUSINESS_SERVICE table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE BUSINESS_SERVICE: ");
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
   * Insert new row into the BUSINESS_ENTITIES table.
   *
   * @param  business object holding values to be inserted
   * @param  JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void insert(BusinessService service,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (service == null)
      throw new JUDDIException("attempt to insert a null BusinessEntity");

    if (service.getBusinessKey() == null)
      throw new JUDDIException("attempt to insert a BusinessService with a null BusinessKey");

    if (service.getServiceKey() == null)
      throw new JUDDIException("attempt to insert a BusinessService with a null ServiceKey");

    PreparedStatement statement = null;
    Timestamp timeStamp = new Timestamp(System.currentTimeMillis());


    try
    {
      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,service.getBusinessKey().toString());
      statement.setString(2,service.getServiceKey().toString());
      statement.setTimestamp(3,timeStamp);


      log.info("insert into BUSINESS_SERVICE table:\n\n\t" + insertSQL +
        "\n\t BUSINESS_KEY=" + service.getBusinessKey().toString() +
        "\n\t SERVICE_KEY=" + service.getServiceKey().toString() +

        "\n\t LAST_UPDATE=" + timeStamp.getTime() + "\n");

      int returnCode = statement.executeUpdate();

      log.info("insert into BUSINESS_SERVICE was successful, return code=" + returnCode);
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
   * Delete row from the BUSINESS_SERVICE table.
   *
   * @param  serviceKey Primary key value
   * @param  JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException
   * @throws java.sql.SQLException
   */
  public static void delete(String serviceKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (serviceKey == null)
      throw new JUDDIException("attempt to insert a BusinessService with a null ServiceKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,serviceKey.toString());

      log.info("delete from BUSINESS_SERVICE table:\n\n\t" + deleteSQL +
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
   * Select one row from the BUSINESS_SERVICE table.
   *
   * @param  primary key value
   * @param  JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static BusinessService select(String serviceKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (serviceKey == null)
      throw new JUDDIException("attempt to select a BusinessService using a null ServiceKey");

    BusinessService service = null;
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,serviceKey.toString());

      log.info("select from BUSINESS_SERVICE table:\n\n\t" + selectSQL +
        "\n\t SERVICE_KEY=" + serviceKey.toString() + "\n");

      resultSet = statement.executeQuery();
      if (resultSet.next())
      {
        service = new BusinessService();
        service.setBusinessKey(resultSet.getString("BUSINESS_KEY"));
        service.setServiceKey(serviceKey);
      }

      if (service != null)
        log.info("select successful, at least one row was found");
      else
        log.info("select executed successfully but no rows were found with SERVICE_KEY=" + serviceKey.toString());

      return service;
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
   * Delete multiple rows from the BUSINESS_SERVICE table that are assigned to
   * the BusinessKey specified.<p>
   *
   * @param  businessKey BusinessKey
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void deleteByBusinessKey(String businessKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (businessKey == null)
      throw new JUDDIException("attempt to delete a BusinessService(s) with a null BusinessKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteByBusinessKeySQL);
      statement.setString(1,businessKey.toString());

      log.info("delet from the BUSINESS_SERVICE table:\n\n\t" + deleteByBusinessKeySQL +
        "\n\t BUSINESS_KEY=" + businessKey.toString() + "\n");

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
   * Select all rows from the business_service table for a given
   * BusinessKey.<p>
   *
   * @param  businessKey BusinessKey
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static Vector selectByBusinessKey(String businessKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (businessKey == null)
      throw new JUDDIException("attempt to select a BusinessService using a null BusinessKey");

    Vector serviceList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
     // create a statement to query with
      statement = connection.prepareStatement(selectByBusinessKeySQL);
      statement.setString(1,businessKey.toString());

      log.info("select from BUSINESS_SERVICE table:\n\n\t" + selectByBusinessKeySQL +
        "\n\t BUSINESS_KEY=" + businessKey.toString() + "\n");

      // execute the statement
      resultSet = statement.executeQuery();

      BusinessService service = null;
      while (resultSet.next())
      {
        service = new BusinessService();
        service.setBusinessKey(businessKey);
        service.setServiceKey(resultSet.getString("SERVICE_KEY"));
        serviceList.add(service);
        service = null;
      }

      log.info("select was successful, rows selected=" + serviceList.size());
      return serviceList;
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
   * Verify that 'authorizedName' has the authority to update or delete 
   * BusinessService identified by the serviceKey parameter
   *
   * @param  serviceKey
   * @param  authorizedName
   * @param  connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static boolean verifyOwnership(String serviceKey,String authorizedName,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if ((serviceKey == null) || (authorizedName == null))
      return false;

    boolean authorized = false;
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      statement = connection.prepareStatement(verifyOwnershipSQL);
      statement.setString(1,serviceKey);
      statement.setString(2,authorizedName);

      log.info("checking ownership of BUSINESS_SERVICE:\n\n\t" + verifyOwnershipSQL +
        "\n\t SERVICE_KEY=" + serviceKey +
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

        // begin a new transaction
        txn.begin(connection);

        // insert a new BusinessEntity
        BusinessEntityTable.insert(business,connection);

        // insert a new BusinessService
        BusinessServiceTable.insert(service,connection);

        // insert another new BusinessService
        service.setServiceKey(UUID.nextID());
        BusinessServiceTable.insert(service,connection);

        // insert one more new BusinessService
        service.setServiceKey(UUID.nextID());
        BusinessServiceTable.insert(service,connection);

        // select a BusinessService object
        service = BusinessServiceTable.select(serviceKey,connection);

        // delete a BusinessService object
        BusinessServiceTable.delete(serviceKey,connection);

        // select a BusinessService object
        service = BusinessServiceTable.select(serviceKey,connection);

        // select a Collection BusinessService objects by BusinessKey
        Vector services = BusinessServiceTable.selectByBusinessKey(businessKey,connection);

        // delete a Collection BusinessService objects by BusinessKey
        BusinessServiceTable.deleteByBusinessKey(businessKey,connection);

        // select a Collection BusinessService objects by BusinessKey
        services = BusinessServiceTable.selectByBusinessKey(businessKey,connection);

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
