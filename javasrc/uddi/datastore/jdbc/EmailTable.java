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
public class EmailTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(EmailTable.class);

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

    dropSQL = "DROP TABLE EMAIL";



    // build createSQL
    sql = new StringBuffer(150);
    sql.append("CREATE TABLE EMAIL (");
    sql.append("BUSINESS_KEY VARCHAR(41) NOT NULL,");
    sql.append("CONTACT_ID INT NOT NULL,");
    sql.append("EMAIL_ID INT NOT NULL,");
    sql.append("USE_TYPE VARCHAR(255),");
    sql.append("EMAIL_ADDRESS VARCHAR(255) NOT NULL,");
    sql.append("PRIMARY KEY (BUSINESS_KEY,CONTACT_ID,EMAIL_ID),");
    sql.append("FOREIGN KEY (BUSINESS_KEY,CONTACT_ID) REFERENCES CONTACT (BUSINESS_KEY,CONTACT_ID))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(150);
    sql.append("INSERT INTO EMAIL (");
    sql.append("BUSINESS_KEY,");
    sql.append("CONTACT_ID,");
    sql.append("EMAIL_ID,");
    sql.append("USE_TYPE,");
    sql.append("EMAIL_ADDRESS) ");
    sql.append("VALUES (?,?,?,?,?)");
    insertSQL = sql.toString();

    // build selectSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("USE_TYPE,");
    sql.append("EMAIL_ADDRESS ");
    sql.append("FROM EMAIL ");
    sql.append("WHERE BUSINESS_KEY=? ");
    sql.append("AND CONTACT_ID=? ");
    sql.append("ORDER BY EMAIL_ID");
    selectSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM EMAIL ");
    sql.append("WHERE BUSINESS_KEY=?");
    deleteSQL = sql.toString();
  }

  /**

   * Drop the EMAIL table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE EMAIL: ");

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
   * Create the EMAIL table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE EMAIL: ");
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
   * Insert new row into the EMAIL table.<p>
   *
   * @param  businessKey String to the BusinessEntity object that owns the Contact to be inserted
   * @param  contactID The unique ID generated when saving the parent Contact instance.
   * @param  contacts Vector of Email objects holding values to be inserted
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void insert(String businessKey,int contactID,Vector emailList,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (emailList == null)
      throw new JUDDIException("attempt to insert a null Collection of Email instances");

    if (emailList.size() == 0)
      return; // everything is valid but no elements to insert

    if (businessKey == null)
      throw new JUDDIException("attempt to insert a Collection of Email instances using a null BusinessKey");

    PreparedStatement statement = null;

    try
    {
      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,businessKey.toString());
      statement.setInt(2,contactID);

      int emailID = 0;
      int listSize = emailList.size();
      for (int i=0; i<listSize; i++)
      {
        Email email = (Email)emailList.elementAt(i);

        statement.setInt(3,emailID);
        statement.setString(4,email.getUseType());
        statement.setString(5,email.getText());

        log.info("insert into EMAIL table:\n\n\t" + insertSQL +
          "\n\t BUSINESS_KEY=" + businessKey.toString() +
          "\n\t CONTACT_ID=" + contactID +
          "\n\t EMAIL_ID=" + emailID +
          "\n\t USE_TYPE=" + email.getUseType() +
          "\n\t EMAIL_ADDRESS=" + email.getText() + "\n");

        int returnCode = statement.executeUpdate();

        log.info("insert was successful, return code=" + returnCode);
        emailID++;
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
      throw new JUDDIException("attempt to select a Collection of EMail instances using a null BusinessKey");

    Vector emailList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,businessKey.toString());
      statement.setInt(2,contactID);

      log.info("select from EMAIL table:\n\n\t" + selectSQL +
        "\n\t BUSINESS_KEY=" + businessKey.toString() +
        "\n\t CONTACT_ID=" + contactID + "\n");

      // execute the statement
      resultSet = statement.executeQuery();

      Email email = null;
      while (resultSet.next())
      {
        email = new Email();
        email.setUseType(resultSet.getString("USE_TYPE"));
        email.setText(resultSet.getString("EMAIL_ADDRESS"));
        emailList.add(email);
        email = null;
      }

      log.info("select was successful, rows selected=" + emailList.size());
      return emailList;
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
   * Delete multiple rows from the EMAIL table that are assigned to the
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
      throw new JUDDIException("attempt to delete a Collection of EMail instances using a null BusinessKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,businessKey.toString());

      log.info("delete from EMAIL table:\n\n\t" + deleteSQL +
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
        business.setOperator("WebServiceRegistry.com");

        Vector contactList = new Vector();
        Contact contact = new Contact("Billy Bob");
        contact.setUseType("service");
        contactList.add(contact);
        int contactID = 0;

        Vector emailList = new Vector();
        Email email = null;

        email = new Email("support@steveviens.com");
        email.setUseType("Support");
        emailList.add(email);

        email = new Email("marketing@steveviens.com");
        email.setUseType("Advertising");
        emailList.add(email);

        email = new Email("info@steveviens.com");
        email.setUseType("Information");
        emailList.add(email);

        email = new Email("admin@steveviens.com");
        email.setUseType("Administration");
        emailList.add(email);

        email = new Email("webmaster@steveviens.com");
        email.setUseType("Web Master");
        emailList.add(email);

        // begin a new transaction
        txn.begin(connection);

        // insert a new BusinessEntity
        BusinessEntityTable.insert(business,connection);

        // insert a new Contact
        ContactTable.insert(businessKey,contactList,connection);

        // insert a Collection of Email objects
        EmailTable.insert(businessKey,contactID,emailList,connection);

        // select a Collection of Email objects by BusinessKey
        emailList = EmailTable.select(businessKey,contactID,connection);

        // delete a Collection of Email objects by BusinessKey
        EmailTable.delete(businessKey,connection);

        // re-select a Collection of Email objects by BusinessKey
        emailList = EmailTable.select(businessKey,contactID,connection);

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
