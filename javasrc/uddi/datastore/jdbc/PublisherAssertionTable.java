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
import org.uddi4j.datatype.assertion.PublisherAssertion;
import org.uddi4j.util.*;
import org.apache.log4j.Logger;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.PreparedStatement;
import java.sql.Statement;
import java.util.Enumeration;
import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class PublisherAssertionTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(PublisherAssertionTable.class);

  static String dropSQL = null;

  static String createSQL = null;
  static String insertSQL = null;
  static String selectSQL = null;
  static String deleteSQL = null;
  static String selectByFromKeySQL = null;
  static String selectByToKeySQL = null;
  static String deleteByFromKeySQL = null;
  static String deleteByToKeySQL = null;

  static
  {
    // buffer used to build SQL statements
    StringBuffer sql = null;

    // build dropSQL

    dropSQL = "DROP TABLE PUBLISHER_ASSERTION";



    sql = new StringBuffer(150);
    sql.append("CREATE TABLE PUBLISHER_ASSERTION (");
    sql.append("FROM_KEY VARCHAR(41) NOT NULL,");
    sql.append("TO_KEY VARCHAR(41) NOT NULL,");
    sql.append("TMODEL_KEY_REF VARCHAR(41) NOT NULL,");
    sql.append("KEY_NAME VARCHAR(255) NOT NULL,");
    sql.append("KEY_VALUE VARCHAR(255) NOT NULL,");
    sql.append("PRIMARY KEY (FROM_KEY,TO_KEY))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(150);
    sql.append("INSERT INTO PUBLISHER_ASSERTION (");
    sql.append("FROM_KEY,");
    sql.append("TO_KEY,");
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
    sql.append("FROM PUBLISHER_ASSERTION ");
    sql.append("WHERE FROM_KEY=? ");
    sql.append("AND TO_KEY=?");
    selectSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(200);
    sql.append("DELETE FROM PUBLISHER_ASSERTION ");
    sql.append("WHERE FROM_KEY=? ");
    sql.append("AND TO_KEY=?");
    deleteSQL = sql.toString();

    // build selectByFromKeySQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("TO_KEY,");
    sql.append("TMODEL_KEY_REF,");
    sql.append("KEY_NAME,");
    sql.append("KEY_VALUE ");
    sql.append("FROM PUBLISHER_ASSERTION ");
    sql.append("WHERE FROM_KEY=?");
    selectByFromKeySQL = sql.toString();

    // build deleteByFromKeySQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM PUBLISHER_ASSERTION ");
    sql.append("WHERE FROM_KEY=?");
    deleteByFromKeySQL = sql.toString();

    // build selectByToKeySQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("FROM_KEY,");
    sql.append("TMODEL_KEY_REF,");
    sql.append("KEY_NAME,");
    sql.append("KEY_VALUE ");
    sql.append("FROM PUBLISHER_ASSERTION ");
    sql.append("WHERE TO_KEY=?");
    selectByToKeySQL = sql.toString();

    // build deleteByToKeySQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM PUBLISHER_ASSERTION ");
    sql.append("WHERE TO_KEY=?");
    deleteByToKeySQL = sql.toString();
  }

  /**

   * Drop the PUBLISHER_ASSERTION table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE PUBLISHER_ASSERTION: ");

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
   * Create the PUBLISHER_ASSERTION table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE PUBLISHER_ASSERTION: ");
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
   * Insert new row into the PUBLISHER_ASSERTION table.
   *
   * @param  business object holding values to be inserted
   * @param  JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void insert(PublisherAssertion assertion,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (assertion == null)
      throw new JUDDIException("attempt to insert a null PublisherAssertion");

    if (assertion.getFromKey() == null)
      throw new JUDDIException("attempt to insert a PublisherAssertion with a null FromKey");

    if (assertion.getToKey() == null)
      throw new JUDDIException("attempt to insert a PublisherAssertion with a null ToKey");

    if (assertion.getKeyedReference() == null)
      throw new JUDDIException("attempt to insert a PublisherAssertion with a null KeyedReference");

    if (assertion.getKeyedReference().getKeyName() == null)
      throw new JUDDIException("attempt to insert a PublisherAssertion with a KeyedReference containing a null KeyName");

    if (assertion.getKeyedReference().getKeyValue() == null)
      throw new JUDDIException("attempt to insert a PublisherAssertion with a KeyedReference containing a null KeyValue");

    PreparedStatement statement = null;

    try
    {
      // prep insert values
      String tModelKeyRef = null;
      String keyedRefName = null;
      String keyedRefValue = null;

      if (assertion.getKeyedReference() != null)
      {
        String tModelKey = assertion.getKeyedReference().getTModelKey();
        if (tModelKey != null)
          tModelKeyRef = tModelKey;

        keyedRefName = assertion.getKeyedReference().getKeyName();
        keyedRefValue = assertion.getKeyedReference().getKeyValue();
      }

      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,assertion.getFromKeyString());
      statement.setString(2,assertion.getToKeyString());
      statement.setString(3,tModelKeyRef);
      statement.setString(4,keyedRefName);
      statement.setString(5,keyedRefValue);

      log.info("insert into PUBLISHER_ASSERTION table:\n\n\t" + insertSQL +
        "\n\t FROM_KEY=" + assertion.getFromKeyString() +
        "\n\t TO_KEY=" + assertion.getToKeyString() +
        "\n\t TMODEL_KEY_REF=" + tModelKeyRef +
        "\n\t KEY_NAME=" + keyedRefName +
        "\n\t KEY_VALUE=" + keyedRefValue + "\n");

      // insert
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
   * Select one row from the BUSINESS_ENTITY table.
   *
   * @param  fromKey from BusinessKey
   * @param  toKey to BusinessKey
   * @param  JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static PublisherAssertion select(String fromKey,String toKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (fromKey == null)
      throw new JUDDIException("attempt to select a PublisherAssertion with a null FromKey");

    if (toKey == null)
      throw new JUDDIException("attempt to select a PublisherAssertion with a null ToKey");

    PublisherAssertion assertion = null;
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,fromKey.toString());
      statement.setString(2,toKey.toString());

      log.info("select from PUBLISHER_ASSERTION table:\n\n\t" + selectSQL +
        "\n\t FROM_KEY=" + fromKey.toString() +
        "\n\t TO_KEY=" + toKey.toString() + "\n");

      resultSet = statement.executeQuery();
      if (resultSet.next())
      {
        KeyedReference keyedRef = new KeyedReference();
        keyedRef.setKeyName(resultSet.getString("KEY_NAME"));
        keyedRef.setKeyValue(resultSet.getString("KEY_VALUE"));
        keyedRef.setTModelKey(resultSet.getString("TMODEL_KEY_REF"));

        assertion = new PublisherAssertion();
        assertion.setFromKeyString(fromKey);
        assertion.setToKeyString(toKey);
        assertion.setKeyedReference(keyedRef);
      }

      if (assertion != null)
        log.info("select successful, at least one row was found");
      else
        log.info("select executed successfully but no rows were found with FROM_KEY=" + fromKey.toString() + " and a TO_KEY=" + toKey.toString());

      return assertion;
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
   * Delete row from the PUBLISHER_ASSERTION table.
   *
   * @param  fromKey from BusinessKey
   * @param  toKey to BusinessKey
   * @param  JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void delete(String fromKey,String toKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (fromKey == null)
      throw new JUDDIException("attempt to delete a PublisherAssertion with a null FromKey");

    if (toKey == null)
      throw new JUDDIException("attempt to delete a PublisherAssertion with a null ToKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,fromKey.toString());
      statement.setString(2,toKey.toString());

      log.info("delete from PUBLISHER_ASSERTION table:\n\n\t" + deleteSQL +
        "\n\t FROM_KEY=" + fromKey.toString() +
        "\n\t TO_KEY=" + toKey.toString() + "\n");

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
   * Select all rows from the PUBLISHER_ASSERTION table for a given BusinessKey.<p>
   *
   * @param  fromKey BusinessKey
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static Vector selectByFromKey(String fromKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (fromKey == null)
      throw new JUDDIException("attempt to select a PublisherAssertion with a null FromKey");

    Vector assertList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectByFromKeySQL);
      statement.setString(1,fromKey.toString());

      log.info("select from PUBLISHER_ASSERTION table:\n\n\t" + selectByFromKeySQL +
        "\n\t FROM_KEY=" + fromKey.toString() + "\n");

      // execute the statement
      resultSet = statement.executeQuery();

      PublisherAssertion assertion = null;
      while (resultSet.next())
      {
        KeyedReference keyedRef = new KeyedReference();
        keyedRef.setKeyName(resultSet.getString("KEY_NAME"));
        keyedRef.setKeyValue(resultSet.getString("KEY_VALUE"));
        keyedRef.setTModelKey(resultSet.getString("TMODEL_KEY_REF"));

        assertion = new PublisherAssertion();
        assertion.setFromKeyString(fromKey);
        assertion.setToKeyString(resultSet.getString("TO_KEY"));
        assertion.setKeyedReference(keyedRef);
        assertList.add(assertion);

        assertion = null;
      }

      log.info("select was successful, rows selected=" + assertList.size());
      return assertList;
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
   * Delete multiple rows from the PUBLISHER_ASSERTION table that have a
   * FROM_KEY BusinessKey that is equal to the toKey parameter specified.<p>
   *
   * @param  fromKey BusinessKey
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void deleteByFromKey(String fromKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (fromKey == null)
      throw new JUDDIException("attempt to delete a PublisherAssertion with a null FromKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteByFromKeySQL);
      statement.setString(1,fromKey.toString());

      log.info("delete from PUBLISHER_ASSERTION table:\n\n\t" + deleteByFromKeySQL +
        "\n\t FROM_KEY=" + fromKey.toString() + "\n");

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
   * Select all rows from the PUBLISHER_ASSERTION table for a given BusinessKey.<p>
   *
   * @param  toKey BusinessKey
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static Vector selectByToKey(String toKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (toKey == null)
      throw new JUDDIException("attempt to select a PublisherAssertion with a null ToKey");

    Vector assertList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectByToKeySQL);
      statement.setString(1,toKey.toString());

      log.info("select from PUBLISHER_ASSERTION table:\n\n\t" + selectByToKeySQL +
        "\n\t TO_KEY=" + toKey.toString() + "\n");

      // execute the statement
      resultSet = statement.executeQuery();

      PublisherAssertion assertion = null;
      while (resultSet.next())
      {
        KeyedReference keyedRef = new KeyedReference();
        keyedRef.setKeyName(resultSet.getString("KEY_NAME"));
        keyedRef.setKeyValue(resultSet.getString("KEY_VALUE"));
        keyedRef.setTModelKey(resultSet.getString("TMODEL_KEY_REF"));

        assertion = new PublisherAssertion();
        assertion.setFromKey(new FromKey(resultSet.getString("FROM_KEY")));
        assertion.setToKeyString(toKey);
        assertion.setKeyedReference(keyedRef);
        assertList.add(assertion);

        assertion = null;
      }

      log.info("select was successful, rows selected=" + assertList.size());
      return assertList;
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
   * Delete multiple rows from the PUBLISHER_ASSERTION table that have a
   * TO_KEY BusinessKey that is equal to the toKey parameter specified.<p>
   *
   * @param  toKey BusinessKey
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void deleteByToKey(String toKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (toKey == null)
      throw new JUDDIException("attempt to delete a PublisherAssertion with a null ToKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteByToKeySQL);
      statement.setString(1,toKey.toString());

      log.info("delete from PUBLISHER_ASSERTION table:\n\n\t" + deleteByToKeySQL +
        "\n\t TO_KEY=" + toKey.toString() + "\n");

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
        String fromKey = UUID.nextID();
        String toKey = UUID.nextID();

        KeyedReference keyRef = new KeyedReference();
        keyRef.setTModelKey(UUID.nextID());
        keyRef.setKeyName("whoaaaa BABY!");
        keyRef.setKeyValue("en");

        PublisherAssertion assert1 = new PublisherAssertion(fromKey,toKey,keyRef);
        PublisherAssertion assert2 = new PublisherAssertion(UUID.nextID(),toKey,keyRef);
        PublisherAssertion assert3 = new PublisherAssertion(fromKey,UUID.nextID(),keyRef);
        PublisherAssertion assert4 = new PublisherAssertion(UUID.nextID(),UUID.nextID(),keyRef);
        PublisherAssertion assert5 = new PublisherAssertion(UUID.nextID(),UUID.nextID(),keyRef);

        Vector assertList = null;

        // begin a new transaction
        txn.begin(connection);

        // insert a new PublisherAssertion
        PublisherAssertionTable.insert(assert1,connection);

        // insert another new PublisherAssertion
        PublisherAssertionTable.insert(assert2,connection);

        // insert one more new PublisherAssertion
        PublisherAssertionTable.insert(assert3,connection);

        // insert one more new PublisherAssertion
        PublisherAssertionTable.insert(assert4,connection);

        // insert one more new PublisherAssertion
        PublisherAssertionTable.insert(assert5,connection);

        // select a PublisherAssertion using both from & to BusinessKeys
        PublisherAssertion assertion = PublisherAssertionTable.select(fromKey,toKey,connection);

        // delete that PublisherAssertion
        PublisherAssertionTable.delete(fromKey,toKey,connection);

        // re-select that PublisherAssertion
        assertion = PublisherAssertionTable.select(fromKey,toKey,connection);

        // select a Collection of PublisherAssertion objects using fromKey
        assertList = PublisherAssertionTable.selectByFromKey(fromKey,connection);

        // delete that Collection of PublisherAssertion objects
        PublisherAssertionTable.deleteByFromKey(fromKey,connection);

        // re-select that Collection of PublisherAssertion objects
        assertList = PublisherAssertionTable.selectByFromKey(fromKey,connection);

        // select a Collection of PublisherAssertion objects using toKey
        assertList = PublisherAssertionTable.selectByToKey(toKey,connection);

        // delete that Collection of PublisherAssertion objects
        PublisherAssertionTable.deleteByToKey(toKey,connection);

        // re-select that Collection of PublisherAssertion objects
        assertList = PublisherAssertionTable.selectByToKey(toKey,connection);

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
