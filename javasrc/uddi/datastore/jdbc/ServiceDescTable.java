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
import org.uddi4j.datatype.Description;
import org.uddi4j.datatype.business.BusinessEntity;
import org.uddi4j.datatype.service.BusinessService;
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
public class ServiceDescTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(ServiceDescTable.class);

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

    dropSQL = "DROP TABLE SERVICE_DESCR";



    // build createSQL
    sql = new StringBuffer(150);
    sql.append("CREATE TABLE SERVICE_DESCR (");
    sql.append("SERVICE_KEY VARCHAR(41) NOT NULL,");
    sql.append("SERVICE_DESCR_ID INT NOT NULL,");
    sql.append("LANG_CODE VARCHAR(2) NOT NULL,");
    sql.append("DESCR VARCHAR(255) NOT NULL,");
    sql.append("PRIMARY KEY (SERVICE_KEY,SERVICE_DESCR_ID),");
    sql.append("FOREIGN KEY (SERVICE_KEY) REFERENCES BUSINESS_SERVICE (SERVICE_KEY))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(150);
    sql.append("INSERT INTO SERVICE_DESCR (");
    sql.append("SERVICE_KEY,");
    sql.append("SERVICE_DESCR_ID,");
    sql.append("LANG_CODE,");
    sql.append("DESCR) ");
    sql.append("VALUES (?,?,?,?)");
    insertSQL = sql.toString();

    // build selectSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("LANG_CODE,");
    sql.append("DESCR ");
    sql.append("FROM SERVICE_DESCR ");
    sql.append("WHERE SERVICE_KEY=? ");
    sql.append("ORDER BY SERVICE_DESCR_ID");
    selectSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM SERVICE_DESCR ");
    sql.append("WHERE SERVICE_KEY=?");
    deleteSQL = sql.toString();
  }

  /**

   * Drop the SERVICE_DESCR table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE SERVICE_DESCR: ");

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
   * Create the SERVICE_DESCR table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE SERVICE_DESCR: ");
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
   * Insert new row into the SERVICE_DESCR table.<p>
   *
   * @param  businessKey String to the BusinessEntity object that owns the Description to be inserted
   * @param  descriptions Vector of Description objects holding values to be inserted
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void insert(String serviceKey,Vector descList,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (descList == null)
      throw new JUDDIException("attempt to insert a null Collection of Description instances");

    if (descList.size() == 0)
      return; // everything is valid but no elements to insert

    if (serviceKey == null)
      throw new JUDDIException("attempt to insert a Collection of Description instances using a null ServiceKey");

    PreparedStatement statement = null;

    try
    {
      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,serviceKey.toString());

      int descID = 0;
      int listSize = descList.size();
      for (int i=0; i<listSize; i++)
      {
        Description desc = (Description)descList.elementAt(i);

        statement.setInt(2,descID);
        statement.setString(3,desc.getLang());
        statement.setString(4,desc.getText());

        log.info("insert into SERVICE_DESCR table:\n\n\t" + insertSQL +
          "\n\t SERVICE_KEY=" + serviceKey.toString() +
          "\n\t SERVICE_DESCR_ID=" + descID +
          "\n\t LANG_CODE=" + desc.getLang() +
          "\n\t DESCR=" + desc.getText() + "\n");

        int returnCode = statement.executeUpdate();

        log.info("insert was successful, return code=" + returnCode);
        descID++;
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
   * Select all rows from the SERVICE_DESCR table for a given BusinessKey.<p>
   *
   * @param  serviceKey String
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static Vector select(String serviceKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (serviceKey == null)
      throw new JUDDIException("attempt to select a Collection of Description instances using a null ServiceKey");

    Vector descList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,serviceKey.toString());

      log.info("select from SERVICE_DESCR table:\n\n\t" + selectSQL +
        "\n\t SERVICE_KEY=" + serviceKey.toString() + "\n");

      // execute the statement
      resultSet = statement.executeQuery();

      Description desc = null;
      while (resultSet.next())
      {
        desc = new Description();
        desc.setLang(resultSet.getString("LANG_CODE"));
        desc.setText(resultSet.getString("DESCR"));
        descList.add(desc);
        desc = null;
      }

      log.info("select was successful, rows selected=" + descList.size());
      return descList;
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
   * Delete multiple rows from the SERVICE_DESCR table that are assigned to the
   * BusinessKey specified.<p>
   *
   * @param  businessKey String
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void delete(String serviceKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (serviceKey == null)
      throw new JUDDIException("attempt to select a Collection of Description instances using a null ServiceKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,serviceKey.toString());

      log.info("delete from SERVICE_DESCR table:\n\n\t" + deleteSQL +
        "\n\t SERVICE_KEY=" + serviceKey.toString() + "\n");

      // execute the delete
      int returnCode = statement.executeUpdate();

      log.info("delete was successful, rows selected=" + returnCode);
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

        Vector descList = new Vector();
        descList.add(new Description("blah, blah, blah","en"));
        descList.add(new Description("Yadda, Yadda, Yadda","it"));
        descList.add(new Description("WhoobWhoobWhoobWhoob","cy"));
        descList.add(new Description("Haachachachacha","km"));

        // begin a new transaction
        txn.begin(connection);

        // insert a new BusinessEntity
        BusinessEntityTable.insert(business,connection);

        // insert a new BusinessService
        BusinessServiceTable.insert(service,connection);

        // insert a Collection of Description objects
        ServiceDescTable.insert(serviceKey,descList,connection);

        // select the Collection of Description objects
        descList = ServiceDescTable.select(serviceKey,connection);

        // delete the Collection of Description objects
        ServiceDescTable.delete(serviceKey,connection);

        // re-select the Collection of Description objects
        descList = ServiceDescTable.select(serviceKey,connection);

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
