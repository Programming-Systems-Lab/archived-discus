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
import org.uddi4j.datatype.binding.*;
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
public class BindingDescTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(BindingDescTable.class);

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

    dropSQL = "DROP TABLE BINDING_DESCR";



    // build createSQL
    sql = new StringBuffer(150);
    sql.append("CREATE TABLE BINDING_DESCR (");
    sql.append("BINDING_KEY VARCHAR(41) NOT NULL,");
    sql.append("BINDING_DESCR_ID INT NOT NULL,");
    sql.append("LANG_CODE VARCHAR(2) NOT NULL,");
    sql.append("DESCR VARCHAR(255) NOT NULL,");
    sql.append("PRIMARY KEY (BINDING_KEY,BINDING_DESCR_ID),");
    sql.append("FOREIGN KEY (BINDING_KEY) REFERENCES BINDING_TEMPLATE (BINDING_KEY))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(150);
    sql.append("INSERT INTO BINDING_DESCR (");
    sql.append("BINDING_KEY,");
    sql.append("BINDING_DESCR_ID,");
    sql.append("LANG_CODE,");
    sql.append("DESCR) ");
    sql.append("VALUES (?,?,?,?)");
    insertSQL = sql.toString();

    // build selectSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("LANG_CODE,");
    sql.append("DESCR ");
    sql.append("FROM BINDING_DESCR ");
    sql.append("WHERE BINDING_KEY=? ");
    sql.append("ORDER BY BINDING_DESCR_ID");
    selectSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM BINDING_DESCR ");
    sql.append("WHERE BINDING_KEY=?");
    deleteSQL = sql.toString();
  }

  /**

   * Drop the BINDING_DESCR table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE BINDING_DESCR: ");

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
   * Create the BINDING_DESCR table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE BINDING_DESCR: ");
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
   * Insert new row into the BINDING_DESCR table.<p>
   *
   * @param  businessKey String to the BusinessEntity object that owns the Description to be inserted
   * @param  descList Enumeration of Description objects to be inserted
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException
   * @throws java.sql.SQLException
   */
  public static void insert(String bindingKey,Vector descList,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (descList == null)
      throw new JUDDIException("attempt to insert a null Collection of Description instances");

    if (descList.size() == 0)
      return; // everything is valid but no elements to insert

    if (bindingKey == null)
      throw new JUDDIException("attempt to insert a Collection of Description instances using a null BindingKey");

    PreparedStatement statement = null;

    try
    {
      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,bindingKey.toString());

      int descID = 0;
      int listSize = descList.size();
      for (int i=0; i<listSize; i++)
      {
        Description desc = (Description)descList.elementAt(i);

        statement.setInt(2,descID);
        statement.setString(3,desc.getLang());
        statement.setString(4,desc.getText());

        log.info("insert into BINDING_DESCR table:\n\n\t" + insertSQL +
          "\n\t BINDING_KEY=" + bindingKey.toString() +
          "\n\t BINDING_DESCR_ID=" + descID +
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
   * Select all rows from the BINDING_DESCR table for a given BusinessKey.<p>
   *
   * @param  bindingKey String
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static Vector select(String bindingKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (bindingKey == null)
      throw new JUDDIException("attempt to select a Collection of Description instances using a null BindingKey");

    Vector descList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,bindingKey.toString());

      log.info("select from BINDING_DESCR table:\n\n\t" + selectSQL +
        "\n\t BINDING_KEY=" + bindingKey.toString() + "\n");

      // execute the statement
      resultSet = statement.executeQuery();

      while (resultSet.next())
      {
        Description desc = new Description();
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
   * Delete multiple rows from the BINDING_DESCR table that are assigned to the
   * BusinessKey specified.<p>
   *
   * @param  businessKey String
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void delete(String bindingKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (bindingKey == null)
      throw new JUDDIException("attempt to delete a Collection of Description instances using a null BindingKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,bindingKey.toString());

      log.info("delete from BINDING_DESCR table:\n\n\t" + deleteSQL +
        "\n\t BINDING_KEY=" + bindingKey.toString() + "\n");

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
        //BusinessKey businessKey = BusinessKey.createKey();
        String businessKey = UUID.nextID();
        BusinessEntity business = new BusinessEntity();
        business.setBusinessKey(businessKey);
        business.setAuthorizedName("sviens");
        business.setOperator("WebServiceRegistry.com");

        //ServiceKey serviceKey = ServiceKey.createKey();
        String serviceKey = UUID.nextID();
        BusinessService service = new BusinessService();
        service.setServiceKey(serviceKey);
        service.setBusinessKey(businessKey);

        //BindingKey bindingKey = BindingKey.createKey();
        String bindingKey = UUID.nextID();
        BindingTemplate binding = new BindingTemplate();
        binding.setAccessPoint(new AccessPoint("http://www.juddi.org/binding.html","http"));
        binding.setHostingRedirector(null);
        binding.setBindingKey(bindingKey);
        binding.setServiceKey(serviceKey);

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

        // insert a new BindingTemplate
        BindingTemplateTable.insert(binding,connection);

        // insert a Collection of Description objects
        BindingDescTable.insert(bindingKey,descList,connection);

        // select the Collection of Description objects
        descList = BindingDescTable.select(bindingKey,connection);

        // delete the Collection of Description objects
        BindingDescTable.delete(bindingKey,connection);

        // select the Collection of Description objects
        descList = BindingDescTable.select(bindingKey,connection);

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
