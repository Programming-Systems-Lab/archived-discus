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
import org.uddi4j.datatype.tmodel.TModel;
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
public class TModelCategoryTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(TModelCategoryTable.class);

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

    dropSQL = "DROP TABLE TMODEL_CATEGORY";



    // build createSQL
    sql = new StringBuffer(150);
    sql.append("CREATE TABLE TMODEL_CATEGORY (");
    sql.append("TMODEL_KEY VARCHAR(41) NOT NULL,");
    sql.append("CATEGORY_ID INT NOT NULL,");
    sql.append("TMODEL_KEY_REF VARCHAR(255),");
    sql.append("KEY_NAME VARCHAR(255),");
    sql.append("KEY_VALUE VARCHAR(255) NOT NULL,");
    sql.append("PRIMARY KEY (TMODEL_KEY,CATEGORY_ID),");
    sql.append("FOREIGN KEY (TMODEL_KEY) REFERENCES TMODEL (TMODEL_KEY))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(150);
    sql.append("INSERT INTO TMODEL_CATEGORY (");
    sql.append("TMODEL_KEY,");
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
    sql.append("FROM TMODEL_CATEGORY ");
    sql.append("WHERE TMODEL_KEY=? ");
    sql.append("ORDER BY CATEGORY_ID");
    selectSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM TMODEL_CATEGORY ");
    sql.append("WHERE TMODEL_KEY=?");
    deleteSQL = sql.toString();
  }

  /**

   * Drop the TMODEL_CATEGORY table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE TMODEL_CATEGORY: ");

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
   * Create the TMODEL_CATEGORY table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE TMODEL_CATEGORY: ");
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
   * Insert new row into the TMODEL_CATEGORY table.<p>
   *
   * @param  tModelKey String to the parent TModelEntity object.
   * @param  refs A Vector of KeyedReference instances to insert.
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void insert(String tModelKey,Vector keyRefs,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (keyRefs == null)
      throw new JUDDIException("attempt to insert a null Collection of KeyedReference instances");

    if (tModelKey == null)
      throw new JUDDIException("attempt to insert a Collection of KeyedReference instances using a null TModelKey");

    PreparedStatement statement = null;

    try
    {
      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,tModelKey.toString());

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

        log.info("insert into TMODEL_CATEGORY table:\n\n\t" + insertSQL +
          "\n\t BUSINESS_KEY=" + tModelKey.toString() +
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
   * Select all rows from the TMODEL_CATEGORY table for a given TModelKey.<p>
   *
   * @param  tModelKey String
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static Vector select(String tModelKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (tModelKey == null)
      throw new JUDDIException("attempt to select multiple KeyedReference instances using a null TModelKey");

    Vector keyRefList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,tModelKey.toString());

      log.info("select from TMODEL_CATEGORY table:\n\n\t" + selectSQL +
        "\n\t TMODEL_KEY=" + tModelKey.toString() + "\n");

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
   * Delete multiple rows from the TMODEL_CATEGORY table that are assigned to the
   * TModelKey specified.<p>
   *
   * @param  tModelKey String
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void delete(String tModelKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (tModelKey == null)
      throw new JUDDIException("attempt to delete multiple KeyedReference instances using a null TModelKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,tModelKey.toString());

      log.info("delete from TMODEL_CATEGORY table:\n\n\t" + deleteSQL +
        "\n\t TMODEL_KEY=" + tModelKey.toString() + "\n");

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
        OverviewDoc overviewDoc = new OverviewDoc();
        overviewDoc.setOverviewURL("http://www.steveviens.com/overviewdoc.html");

        String tModelKey = UUID.nextID();
        TModel tModel = new TModel();
        tModel.setTModelKey(tModelKey);
        tModel.setAuthorizedName("sviens");
        tModel.setOperator("WebServiceRegistry.com");
        tModel.setName("Tuscany Web Service Company");
        tModel.setOverviewDoc(overviewDoc);

        Vector keyRefs = new Vector();
        keyRefs.add(new KeyedReference(UUID.nextID(),"blah, blah, blah"));
        keyRefs.add(new KeyedReference(UUID.nextID(),"Yadda, Yadda, Yadda"));
        keyRefs.add(new KeyedReference(UUID.nextID(),"WhoobWhoobWhoobWhoob"));
        keyRefs.add(new KeyedReference(UUID.nextID(),"Haachachachacha"));

        // begin a new transaction
        txn.begin(connection);

        // insert a new TModel
        TModelTable.insert(tModel,connection);

        // insert a Collection of new Category KeyedReference objects
        TModelCategoryTable.insert(tModelKey,keyRefs,connection);

        // insert another new TModel
        tModel.setTModelKey(UUID.nextID());
        TModelTable.insert(tModel,connection);

        // insert another Collection of new Category KeyedReference objects
        TModelCategoryTable.insert(tModel.getTModelKey(),keyRefs,connection);

        // select a Collection of Category KeyedReference objects
        keyRefs = TModelCategoryTable.select(tModelKey,connection);

        // delete a Collection of Category KeyedReference objects
        TModelCategoryTable.delete(tModelKey,connection);

        // re-select a Collection of Category KeyedReference objects
        keyRefs = TModelCategoryTable.select(tModelKey,connection);

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
