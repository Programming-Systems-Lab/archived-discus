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
import java.sql.ResultSet;
import java.sql.PreparedStatement;
import java.sql.Statement;
import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class ServiceCategoryTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(ServiceCategoryTable.class);

  static String dropSQL = null;

  static String createSQL = null;
  static String insertSQL = null;
  static String selectSQL = null;
  static String deleteSQL = null;

  static
  {
    // buffer used to build SQL statements
    StringBuffer sql = null;

    // build dropSQL

    dropSQL = "DROP TABLE SERVICE_CATEGORY";



    // build createSQL
    sql = new StringBuffer(150);
    sql.append("CREATE TABLE SERVICE_CATEGORY (");
    sql.append("SERVICE_KEY VARCHAR(41) NOT NULL,");
    sql.append("CATEGORY_ID INT NOT NULL,");
    sql.append("TMODEL_KEY_REF VARCHAR(41),");
    sql.append("KEY_NAME VARCHAR(255),");
    sql.append("KEY_VALUE VARCHAR(255) NOT NULL,");
    sql.append("PRIMARY KEY (SERVICE_KEY,CATEGORY_ID),");
    sql.append("FOREIGN KEY (SERVICE_KEY) REFERENCES BUSINESS_SERVICE (SERVICE_KEY))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(150);
    sql.append("INSERT INTO SERVICE_CATEGORY (");
    sql.append("SERVICE_KEY,");
    sql.append("CATEGORY_ID,");
    sql.append("TMODEL_KEY_REF,");
    sql.append("KEY_NAME,");
    sql.append("KEY_VALUE) ");
    sql.append("VALUES (?,?,?,?,?)");
    insertSQL = sql.toString();

    // build selectSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("TMODEL_KEY_REF,");
    sql.append("KEY_NAME,");
    sql.append("KEY_VALUE ");
    sql.append("FROM SERVICE_CATEGORY ");
    sql.append("WHERE SERVICE_KEY=? ");
    sql.append("ORDER BY CATEGORY_ID");
    selectSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM SERVICE_CATEGORY ");
    sql.append("WHERE SERVICE_KEY=?");
    deleteSQL = sql.toString();
  }

  /**

   * Drop the SERVICE_CATEGORY table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE SERVICE_CATEGORY: ");

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
   * Create the SERVICE_CATEGORY table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE SERVICE_CATEGORY: ");
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
   * Insert new row into the SERVICE_CATEGORY table.<p>
   *
   * @param  serviceKey String to the parent ServiceEntity object.
   * @param  refs A Vector of KeyedReference instances to insert.
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void insert(String serviceKey,Vector keyRefs,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (keyRefs == null)
      throw new JUDDIException("attempt to insert a null Collection of KeyedReference instances");

    if (serviceKey == null)
      throw new JUDDIException("attempt to insert a Collection of KeyedReference instances using a null ServiceKey");

    PreparedStatement statement = null;

    try
    {
      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,serviceKey.toString());

      int categoryID = 0;
      int listSize = keyRefs.size();
      for (int i=0; i<listSize; i++)
      {
        KeyedReference keyRef = (KeyedReference)keyRefs.elementAt(i);

        // extract values to insert
        String tModelKeyValue = null;
        if (keyRef.getTModelKey() != null)
          tModelKeyValue = keyRef.getTModelKey().toString();

        // set the values
        statement.setInt(2,categoryID);
        statement.setString(3,tModelKeyValue);
        statement.setString(4,keyRef.getKeyName());
        statement.setString(5,keyRef.getKeyValue());

        log.info("insert into SERVICE_CATEGORY table:\n\n\t" + insertSQL +
          "\n\t BUSINESS_KEY=" + serviceKey.toString() +
          "\n\t CATEGORY_ID=" + categoryID +
          "\n\t TMODEL_KEY_REF=" + tModelKeyValue +
          "\n\t KEY_NAME=" + keyRef.getKeyName() +
          "\n\t KEY_VALUE=" + keyRef.getKeyValue() + "\n");

        // insert!
        int returnCode = statement.executeUpdate();

        log.info("insert was successful, return code=" + returnCode);
        categoryID++;
      }
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
   * Select all rows from the SERVICE_CATEGORY table for a given ServiceKey.<p>
   *
   * @param  serviceKey String
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static Vector select(String serviceKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (serviceKey == null)
      throw new JUDDIException("attempt to select multiple KeyedReference instances using a null ServiceKey");

    Vector keyRefList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,serviceKey.toString());

      log.info("select from SERVICE_CATEGORY table:\n\n\t" + selectSQL +
        "\n\t SERVICE_KEY=" + serviceKey.toString() + "\n");

      // execute the statement
      resultSet = statement.executeQuery();

      KeyedReference keyRef = null;
      while (resultSet.next())
      {
        keyRef = new KeyedReference();
        keyRef.setTModelKey(resultSet.getString("TMODEL_KEY_REF"));
        keyRef.setKeyName(resultSet.getString("KEY_NAME"));
        keyRef.setKeyValue(resultSet.getString("KEY_VALUE"));
        keyRefList.add(keyRef);
        keyRef = null;
      }

      log.info("select was successful, rows selected=" + keyRefList.size());
      return keyRefList;
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
   * Delete multiple rows from the SERVICE_CATEGORY table that are assigned to the
   * ServiceKey specified.<p>
   *
   * @param  serviceKey String
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void delete(String serviceKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (serviceKey == null)
      throw new JUDDIException("attempt to delete multiple KeyedReference instances using a null ServiceKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,serviceKey.toString());

      log.info("delete from SERVICE_CATEGORY table:\n\n\t" + deleteSQL +
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
        service.setServiceKey(serviceKey);
        service.setBusinessKey(businessKey);

        Vector keyRefs = new Vector();
        keyRefs.add(new KeyedReference(UUID.nextID(),"blah, blah, blah"));
        keyRefs.add(new KeyedReference(UUID.nextID(),"Yadda, Yadda, Yadda"));
        keyRefs.add(new KeyedReference(UUID.nextID(),"WhoobWhoobWhoobWhoob"));

        // begin a new transaction
        txn.begin(connection);

        // insert a new BusinessEntity
        BusinessEntityTable.insert(business,connection);

        // insert a new BusinessService
        BusinessServiceTable.insert(service,connection);

        // insert a Collection of new Category KeyedReference objects
        ServiceCategoryTable.insert(serviceKey,keyRefs,connection);

        // insert another new BusinessService
        service.setServiceKey(UUID.nextID());
        BusinessServiceTable.insert(service,connection);

        // insert another Collection of new Category KeyedReference objects
        ServiceCategoryTable.insert(service.getServiceKey(),keyRefs,connection);

        // select a Collection of Category KeyedReference objects
        keyRefs = ServiceCategoryTable.select(serviceKey,connection);

        // delete a Collection of Category KeyedReference objects
        ServiceCategoryTable.delete(serviceKey,connection);

        // re-select a Collection of Category KeyedReference objects
        keyRefs = ServiceCategoryTable.select(serviceKey,connection);

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
