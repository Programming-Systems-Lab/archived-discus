/* 
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.datastore.jdbc;

import psl.discus.javasrc.uddi.error.*;
import psl.discus.javasrc.uddi.util.*;
import psl.discus.javasrc.uddi.uuidgen.*;

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
public class AddressLineTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(AddressLineTable.class);

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

    dropSQL = "DROP TABLE ADDRESS_LINE";



    // build createSQL
    sql = new StringBuffer(150);
    sql.append("CREATE TABLE ADDRESS_LINE (");
    sql.append("BUSINESS_KEY VARCHAR(41) NOT NULL,");
    sql.append("CONTACT_ID INT NOT NULL,");
    sql.append("ADDRESS_ID INT NOT NULL,");
    sql.append("ADDRESS_LINE_ID INT NOT NULL,");
    sql.append("LINE VARCHAR(80) NOT NULL,");
    sql.append("KEY_NAME VARCHAR(255),");
    sql.append("KEY_VALUE VARCHAR(255),");
    sql.append("PRIMARY KEY (BUSINESS_KEY,CONTACT_ID,ADDRESS_ID,ADDRESS_LINE_ID),");
    sql.append("FOREIGN KEY (BUSINESS_KEY,CONTACT_ID,ADDRESS_ID) REFERENCES ADDRESS (BUSINESS_KEY,CONTACT_ID,ADDRESS_ID))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(150);
    sql.append("INSERT INTO ADDRESS_LINE (");
    sql.append("BUSINESS_KEY,");
    sql.append("CONTACT_ID,");
    sql.append("ADDRESS_ID,");
    sql.append("ADDRESS_LINE_ID,");
    sql.append("LINE,");
    sql.append("KEY_NAME,");
    sql.append("KEY_VALUE) ");
    sql.append("VALUES (?,?,?,?,?,?,?)");
    insertSQL = sql.toString();

    // build selectSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("LINE,");
    sql.append("KEY_NAME,");
    sql.append("KEY_VALUE ");
    sql.append("FROM ADDRESS_LINE ");
    sql.append("WHERE BUSINESS_KEY=? ");
    sql.append("AND CONTACT_ID=? ");
    sql.append("AND ADDRESS_ID=? ");
    sql.append("ORDER BY ADDRESS_LINE_ID");
    selectSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM ADDRESS_LINE ");
    sql.append("WHERE BUSINESS_KEY=?");
    deleteSQL = sql.toString();
  }

  /**

   * Drop the ADDRESS_LINE table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE ADDRESS_LINE: ");

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
   * Create the ADDRESS_LINE table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE ADDRESS_LINE: ");
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
   * Insert new row into the ADDRESS_LINE table.<p>
   *
   * @param  businessKey BusinessKey to the BusinessEntity object that owns the Contact to be inserted
   * @param  contactID The unique ID generated when saving the parent Contact instance.
   * @param  addressID The unique ID generated when saving the parent Address instance.
   * @param  contacts Enumeration of AddressLines objects holding values to be inserted.
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void insert(String businessKey,int contactID,int addressID,Vector lineList,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (lineList == null)
      throw new JUDDIException("attempt to insert a null Collection of AddressLine instances");

    if (lineList.size() == 0)
      return; // everything is valid but no elements to insert

    if (businessKey == null)
      throw new JUDDIException("attempt to insert a Collection of AddressLine instances using a null BusinessKey");

    PreparedStatement statement = null;

    try
    {
      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,businessKey.toString());
      statement.setInt(2,contactID);
      statement.setInt(3,addressID);

      int lineID = 0;
      int listSize = lineList.size();
      for (int i=0; i<listSize; i++)
      {
        AddressLine line = (AddressLine)lineList.elementAt(i);

        statement.setInt(4,lineID);
        statement.setString(5,line.getText());
        statement.setString(6,line.getKeyName());
        statement.setString(6,null);
        statement.setString(7,line.getKeyValue());
        statement.setString(7,null);

        log.info("insert into ADDRESS_LINE table:\n\n\t" + insertSQL +
          "\n\t BUSINESS_KEY=" + businessKey.toString() +
          "\n\t CONTACT_ID=" + contactID +
          "\n\t ADDRESS_ID=" + addressID +
          "\n\t ADDRESS_LINE_ID=" + lineID +
          "\n\t LINE=" + line.getText() +
          "\n\t KEY_NAME=" + line.getKeyName() + 
          "\n\t KEY_VALUE=" + line.getKeyValue() + "\n");

        int returnCode = statement.executeUpdate();

        log.info("insert was successful, return code=" + returnCode);
        lineID++;
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
   * @param  contactID Unique ID representing the parent Contact instance
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static Vector select(String businessKey,int contactID,int addressID,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (businessKey == null)
      throw new JUDDIException("attempt to select a Collection of AddressLine instances using a null BusinessKey");

    Vector lineList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,businessKey.toString());
      statement.setInt(2,contactID);
      statement.setInt(3,addressID);

      log.info("select from ADDRESS_LINE table:\n\n\t" + selectSQL +
        "\n\t BUSINESS_KEY=" + businessKey.toString() +
        "\n\t CONTACT_KEY=" + contactID +
        "\n\t ADDRESS_ID=" + addressID + "\n");

      // execute the statement
      resultSet = statement.executeQuery();

      AddressLine line = null;
      while (resultSet.next())
      {
        line = new AddressLine();
        line.setText(resultSet.getString("LINE"));
        //line.setKeyName(resultSet.getString("KEY_NAME"));                        // SMV*TEMPORARY
        //line.setKeyValue(resultSet.getString("KEY_VALUE"));                      // SMV*TEMPORARY
        lineList.add(line);
        line = null;
      }

      log.info("select was successful, rows selected=" + lineList.size());
      return lineList;
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
   * Delete multiple rows from the ADDRESS_LINE table that are assigned to the
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
      throw new JUDDIException("attempt to delete a Collection of AddressLine instances using a null BusinessKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,businessKey.toString());

      log.info("delete from ADDRESS_LINE table:\n\n\t" + deleteSQL +
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

        Contact contact = new Contact();
        contact.setPersonName("Bill Bob");
        contact.setUseType("service");

        Vector contactList = new Vector();
        contactList.add(contact);
        int contactID = 0;

        Address address = new Address();
        address.setUseType("Mailing");
        address.setSortCode("a");

        Vector addrList = new Vector();
        addrList.add(address);
        int addressID = 0;

        AddressLine addrLine1 = new AddressLine();
        addrLine1.setText("SteveViens.com, Inc.");

        AddressLine addrLine2 = new AddressLine();
        addrLine2.setText("PO BOX 6856");

        AddressLine addrLine3 = new AddressLine();
        addrLine3.setText("78 Marne Avenue");

        AddressLine addrLine4 = new AddressLine();
        addrLine4.setText("Portsmouth");

        AddressLine addrLine5 = new AddressLine();
        addrLine5.setText("New Hampshire");

        Vector lineList = new Vector();
        lineList.add(addrLine1);
        lineList.add(addrLine2);
        lineList.add(addrLine3);
        lineList.add(addrLine4);
        lineList.add(addrLine5);

        // begin a new transaction
        txn.begin(connection);

        // insert a new BusinessEntity
        BusinessEntityTable.insert(business,connection);

        // insert a new Contact
        ContactTable.insert(businessKey,contactList,connection);

        // insert a new Address
        AddressTable.insert(businessKey,contactID,addrList,connection);

        // insert a Collection of AddressLine objects
        AddressLineTable.insert(businessKey,contactID,addressID,lineList,connection);

        // select the Collection of AddressLine objects
        lineList = AddressLineTable.select(businessKey,contactID,addressID,connection);

        // delete the Collection of AddressLine objects
        AddressLineTable.delete(businessKey,connection);

        // re-select the Collection of AddressLine objects
        lineList = AddressLineTable.select(businessKey,contactID,addressID,connection);

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
