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
public class ContactTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(ContactTable.class);

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

    dropSQL = "DROP TABLE CONTACT";



    // build createSQL
    sql = new StringBuffer(150);
    sql.append("CREATE TABLE CONTACT (");
    sql.append("BUSINESS_KEY VARCHAR(41) NOT NULL,");
    sql.append("CONTACT_ID INT NOT NULL,");
    sql.append("USE_TYPE VARCHAR(255),");
    sql.append("PERSON_NAME VARCHAR(255) NOT NULL,");
    sql.append("PRIMARY KEY (BUSINESS_KEY,CONTACT_ID),");
    sql.append("FOREIGN KEY (BUSINESS_KEY) REFERENCES BUSINESS_ENTITY (BUSINESS_KEY))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(150);
    sql.append("INSERT INTO CONTACT (");
    sql.append("BUSINESS_KEY,");
    sql.append("CONTACT_ID,");
    sql.append("USE_TYPE,");
    sql.append("PERSON_NAME) ");
    sql.append("VALUES (?,?,?,?)");
    insertSQL = sql.toString();

    // build selectSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("USE_TYPE,");
    sql.append("PERSON_NAME ");
    sql.append("FROM CONTACT ");
    sql.append("WHERE BUSINESS_KEY=? ");
    sql.append("ORDER BY CONTACT_ID");
    selectSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM CONTACT ");
    sql.append("WHERE BUSINESS_KEY=?");
    deleteSQL = sql.toString();
  }

  /**

   * Drop the CONTACT table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE CONTACT: ");

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
   * Create the CONTACT table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE CONTACT: ");
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
   * Insert new row into the CONTACT table.<p>
   *
   * @param  businessKey BusinessKey to the BusinessEntity object that owns the Contact to be inserted
   * @param  contacts Vector of Contact objects holding values to be inserted
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void insert(String businessKey,Vector contactList,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (contactList == null)
      throw new JUDDIException("attempt to insert a null Collection of Contact instances");

    if (contactList.size() == 0)
      return; // everything is valid but no elements to insert

    if (businessKey == null)
      throw new JUDDIException("attempt to insert a Collection of Contact instances using a null BusinessKey");

    PreparedStatement statement = null;

    try
    {
      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,businessKey.toString());

      int contactID = 0;
      int listSize = contactList.size();
      for (int i=0; i<listSize; i++)
      {
        Contact contact = (Contact)contactList.elementAt(i);

        statement.setInt(2,contactID);
        statement.setString(3,contact.getUseType());
        statement.setString(4,contact.getPersonName().getText());

        log.info("insert into CONTACT table:\n\n\t" + insertSQL +
          "\n\t BUSINESS_KEY=" + businessKey.toString() +
          "\n\t CONTACT_ID=" + contactID +
          "\n\t USE_TYPE=" + contact.getUseType() +
          "\n\t PERSON_NAME=" + contact.getPersonName() + "\n");

        int returnCode = statement.executeUpdate();

        log.info("insert was successful, return code=" + returnCode);
        contactID++;
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
   * @param  businessKey BusinessKey
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static Vector select(String businessKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (businessKey == null)
      throw new JUDDIException("attempt to select a Collection of Contact instances using a null BusinessKey");

    Vector contactList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,businessKey.toString());

      log.info("select from CONTACT table:\n\n\t" + selectSQL +
        "\n\t BUSINESS_KEY=" + businessKey.toString() + "\n");

      // execute the statement
      resultSet = statement.executeQuery();

      Contact contact = null;
      while (resultSet.next())
      {
        contact = new Contact();
        contact.setUseType(resultSet.getString("USE_TYPE"));
        contact.setPersonName(resultSet.getString("PERSON_NAME"));
        contactList.add(contact);
        contact = null;
      }

      log.info("select was successful, rows selected=" + contactList.size());
      return contactList;
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
   * Delete multiple rows from the CONTACT table that are assigned to the
   * BusinessKey specified.<p>
   *
   * @param  businessKey BusinessKey
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void delete(String businessKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (businessKey == null)
      throw new JUDDIException("attempt to delete a Collection of Contact instances using a null BusinessKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,businessKey.toString());

      log.info("delete from CONTACT table:\n\n\t" + deleteSQL +
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
        business.setOperator("www.jUDDI.org");

        Vector contactList = new Vector();
        Contact contact = null;

        contact = new Contact("Steve Viens");
        contact.setUseType("sales");
        contactList.add(contact);

        contact = new Contact("Marley Viens");
        contact.setUseType("support");
        contactList.add(contact);

        contact = new Contact("Chris Michaels");
        contact.setUseType("marketing");
        contactList.add(contact);

        // begin a new transaction
        txn.begin(connection);

        // insert a new BusinessEntity
        BusinessEntityTable.insert(business,connection);

        // insert a Collection of Contact objects
        ContactTable.insert(businessKey,contactList,connection);

        // select a Collection Contact objects by BusinessKey
        contactList = ContactTable.select(businessKey,connection);

        // delete a Collection Contact objects by BusinessKey
        ContactTable.delete(businessKey,connection);

        // re-select a Collection Contact objects by BusinessKey
        contactList = ContactTable.select(businessKey,connection);

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
