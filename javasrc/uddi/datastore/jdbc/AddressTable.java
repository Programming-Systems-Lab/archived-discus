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
public class AddressTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(AddressTable.class);

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

    dropSQL = "DROP TABLE ADDRESS";



    // build createSQL
    sql = new StringBuffer(150);
    sql.append("CREATE TABLE ADDRESS (");
    sql.append("BUSINESS_KEY VARCHAR(41) NOT NULL,");
    sql.append("CONTACT_ID INT NOT NULL,");
    sql.append("ADDRESS_ID INT NOT NULL,");
    sql.append("USE_TYPE VARCHAR(255),");
    sql.append("SORT_CODE VARCHAR(10),");
    sql.append("TMODEL_KEY VARCHAR(41),");
    sql.append("PRIMARY KEY (BUSINESS_KEY,CONTACT_ID,ADDRESS_ID),");
    sql.append("FOREIGN KEY (BUSINESS_KEY,CONTACT_ID) REFERENCES CONTACT (BUSINESS_KEY,CONTACT_ID))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(150);
    sql.append("INSERT INTO ADDRESS (");
    sql.append("BUSINESS_KEY,");
    sql.append("CONTACT_ID,");
    sql.append("ADDRESS_ID,");
    sql.append("USE_TYPE,");
    sql.append("SORT_CODE,");
    sql.append("TMODEL_KEY) ");
    sql.append("VALUES (?,?,?,?,?,?)");
    insertSQL = sql.toString();

    // build selectSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("USE_TYPE,");
    sql.append("SORT_CODE,");
    sql.append("TMODEL_KEY ");
    sql.append("FROM ADDRESS ");
    sql.append("WHERE BUSINESS_KEY=? ");
    sql.append("AND CONTACT_ID=? ");
    sql.append("ORDER BY ADDRESS_ID");
    selectSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM ADDRESS ");
    sql.append("WHERE BUSINESS_KEY=?");
    deleteSQL = sql.toString();
  }

  /**

   * Drop the ADDRESS table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE ADDRESS: ");

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
   * Create the ADDRESS table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE ADDRESS: ");
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
   * Insert new row into the ADDRESS table.<p>
   *
   * @param  businessKey String to the BusinessEntity object that owns the Contact to be inserted
   * @param  contactID The unique ID generated when saving the parent Contact instance.
   * @param  addrList Collection of Address objects to be inserted
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void insert(String businessKey,int contactID,Vector addrList,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (addrList == null)
      throw new JUDDIException("attempt to insert a null Collection of Address instances");

    if (addrList.size() == 0)
      return; // everything is valid but no elements to insert

    if (businessKey == null)
      throw new JUDDIException("attempt to insert a Collection of Address instances using a null BusinessKey");

    PreparedStatement statement = null;

    try
    {
      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,businessKey.toString());
      statement.setInt(2,contactID);

      int addressID = 0;
      int listSize = addrList.size();
      for (int i=0; i<listSize; i++)
      {
        Address address = (Address)addrList.elementAt(i);
        statement.setInt(3,addressID);
        statement.setString(4,address.getUseType());
        statement.setString(5,address.getSortCode());
        statement.setString(6,address.getTModelKey());

        log.info("insert into ADDRESS table:\n\n\t" + insertSQL +
          "\n\t BUSINESS_KEY=" + businessKey.toString() +
          "\n\t CONTACT_ID=" + contactID +
          "\n\t ADDRESS_ID=" + addressID +
          "\n\t USE_TYPE=" + address.getUseType() +
          "\n\t SORT_CODE=" + address.getSortCode() +
          "\n\t TMODEL_KEY=" + address.getTModelKey() + "\n");

        int returnCode = statement.executeUpdate();

        log.info("insert was successful, return code=" + returnCode);
        addressID++;
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
   * Select all rows from the CONTACT table for a given BusinessKey.<p>
   *
   * @param  businessKey String
   * @param  contactID Unique ID representing the parent Contact instance
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static Vector select(String businessKey,int contactID,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (businessKey == null)
      throw new JUDDIException("attempt to select a Collection of Address instances using a null BusinessKey");

    Vector addrList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,businessKey.toString());
      statement.setInt(2,contactID);

      log.info("select from ADDRESS table:\n\n\t" + selectSQL +
        "\n\t BUSINESS_KEY=" + businessKey.toString() +
        "\n\t CONTACT_ID=" + contactID + "\n");

      // execute the statement
      resultSet = statement.executeQuery();

      while (resultSet.next())
      {
        Address address = new Address();
        address.setUseType(resultSet.getString("USE_TYPE"));
        address.setSortCode(resultSet.getString("SORT_CODE"));
        address.setTModelKey(resultSet.getString("TMODEL_KEY"));
        addrList.add(address);
        address = null;
      }

      log.info("select was successful, rows selected=" + addrList.size());
      return addrList;
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
   * Delete multiple rows from the ADDRESS table that are assigned to the
   * BusinessKey specified.<p>
   *
   * @param  businessKey String
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void delete(String businessKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (businessKey == null)
      throw new JUDDIException("attempt to delete a Collection of Address instances using a null BusinessKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,businessKey.toString());

      log.info("delete from ADDRESS table:\n\n\t" + deleteSQL +
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

        Vector contactList = new Vector();
        Contact contact = new Contact("Bill Bob");
        contact.setUseType("service");
        contactList.add(contact);
        int contactID = 0;

        Vector addrList = new Vector();
        Address address = null;

        address = new Address();
        address.setUseType("Mailing");
        address.setSortCode("a");
        addrList.add(address);

        address = new Address();
        address.setUseType("Shipping");
        address.setSortCode("b");
        addrList.add(address);

        address = new Address();
        address.setUseType("Marketing");
        address.setSortCode("c");
        addrList.add(address);

        address = new Address();
        address.setUseType("Sales");
        address.setSortCode("d");
        addrList.add(address);

        address = new Address();
        address.setUseType("Engineering");
        address.setSortCode("e");
        addrList.add(address);

        // begin a new transaction
        txn.begin(connection);

        // insert a new BusinessEntity
        BusinessEntityTable.insert(business,connection);

        // insert a new Contact
        ContactTable.insert(businessKey,contactList,connection);

        // insert a Collection of Address objects
        AddressTable.insert(businessKey,contactID,addrList,connection);

        // select the Collection of Address objects
        addrList = AddressTable.select(businessKey,contactID,connection);

        // delete the Collection of Address objects
        AddressTable.delete(businessKey,connection);

        // re-select the Collection of Address objects
        addrList = AddressTable.select(businessKey,contactID,connection);

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
