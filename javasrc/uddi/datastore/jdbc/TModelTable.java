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
import org.uddi4j.datatype.tmodel.*;
import org.uddi4j.util.*;
import org.apache.log4j.Logger;



import java.sql.Connection;

import java.sql.PreparedStatement;

import java.sql.ResultSet;

import java.sql.Statement;

import java.sql.Timestamp;

import java.util.Enumeration;
import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class TModelTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(TModelTable.class);

  static String dropSQL = null;

  static String createSQL = null;
  static String insertSQL = null;
  static String deleteSQL = null;
  static String selectSQL = null;
  static String selectByAuthorizedNameSQL = null;
  static String verifyOwnershipSQL = null;

  static
  {
    // buffer used to build SQL statements
    StringBuffer sql = null;

    // build dropSQL

    dropSQL = "DROP TABLE TMODEL";



    // build createSQL
    sql = new StringBuffer(150);
    sql.append("CREATE TABLE TMODEL (");
    sql.append("TMODEL_KEY VARCHAR(41) NOT NULL,");
    sql.append("AUTHORIZED_NAME VARCHAR(255) NOT NULL,");
    sql.append("OPERATOR VARCHAR(255) NOT NULL,");
    sql.append("NAME VARCHAR(255) NOT NULL,");
    sql.append("OVERVIEW_DOC VARCHAR(255),");
    sql.append("LAST_UPDATE TIMESTAMP NOT NULL,");

    sql.append("PRIMARY KEY (TMODEL_KEY))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(150);
    sql.append("INSERT INTO TMODEL (");
    sql.append("TMODEL_KEY,");
    sql.append("AUTHORIZED_NAME,");
    sql.append("OPERATOR,");
    sql.append("NAME,");
    sql.append("OVERVIEW_DOC,");

    sql.append("LAST_UPDATE) ");
    sql.append("VALUES (?,?,?,?,?,?)");
    insertSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM TMODEL ");
    sql.append("WHERE TMODEL_KEY=?");
    deleteSQL = sql.toString();

    // build selectSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("AUTHORIZED_NAME,");
    sql.append("OPERATOR,");
    sql.append("NAME,");
    sql.append("OVERVIEW_DOC ");
    sql.append("FROM TMODEL ");
    sql.append("WHERE TMODEL_KEY=?");
    selectSQL = sql.toString();

    // build selectByAuthorizedNameSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("TMODEL_KEY,");
    sql.append("OPERATOR,");
    sql.append("NAME,");
    sql.append("OVERVIEW_DOC ");
    sql.append("FROM TMODEL ");
    sql.append("WHERE AUTHORIZED_NAME=?");
    selectByAuthorizedNameSQL = sql.toString();

    // build verifyOwnershipSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("* ");
    sql.append("FROM TMODEL ");
    sql.append("WHERE TMODEL_KEY=? ");
    sql.append("AND AUTHORIZED_NAME=?");
    verifyOwnershipSQL = sql.toString();
  }

  /**

   * Drop the TMODEL table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE TMODEL: ");

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
   * Create the TMODEL table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE TMODEL: ");
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
   * Insert new row into the TMODEL table.
   *
   * @param  business object holding values to be inserted
   * @param  JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void insert(TModel tModel,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (tModel == null)
      throw new JUDDIException("attempt to insert a null TModel");

    if (tModel.getTModelKey() == null)
      throw new JUDDIException("attempt to insert a TModel with a null TModelKey");

    if (tModel.getAuthorizedName() == null)
      throw new JUDDIException("attempt to insert a TModel with a null AuthorizedName");

    if (tModel.getOperator() == null)
      throw new JUDDIException("attempt to insert a TModel with a null Operator");

    if (tModel.getName() == null)
      throw new JUDDIException("attempt to insert a TModel with a null Name");

    PreparedStatement statement = null;
    Timestamp timeStamp = new Timestamp(System.currentTimeMillis());


    String overviewURL = null;
    if ((tModel.getOverviewDoc() != null) && (tModel.getOverviewDoc().getOverviewURL() != null))
      overviewURL = tModel.getOverviewDoc().getOverviewURL().getText();

    try
    {
      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,tModel.getTModelKey().toString());
      statement.setString(2,tModel.getAuthorizedName());
      statement.setString(3,tModel.getOperator());
      statement.setString(4,tModel.getName().getText());
      statement.setString(5,overviewURL);
      statement.setTimestamp(6,timeStamp);


      log.info("insert into TMODEL table:\n\n\t" + insertSQL +
        "\n\t TMODEL_KEY=" + tModel.getTModelKey().toString() +
        "\n\t AUTHORIZED_NAME=" + tModel.getAuthorizedName() +
        "\n\t OPERATOR=" + tModel.getOperator() +
        "\n\t NAME=" + tModel.getName().getText() +
        "\n\t OVERVIEW_DOC=" + overviewURL +

        "\n\t LAST_UPDATE=" + timeStamp.getTime() + "\n");


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
   * Delete row from the TMODEL table.
   *
   * @param  tModelKey
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void delete(String tModelKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (tModelKey == null)
      throw new JUDDIException("attempt to delete a TModel with a null TModelKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,tModelKey.toString());

      log.info("delete from TMODEL table:\n\n\t" + deleteSQL +
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

  /**
   * Select one row from the TMODEL table.
   *
   * @param  tModelKey
   * @param  connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static TModel select(String tModelKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (tModelKey == null)
      throw new JUDDIException("attempt to select a TModel using a null TModelKey");

    TModel tModel = null;
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,tModelKey.toString());

      log.info("select from TMODEL table:\n\n\t" + selectSQL +
        "\n\t TMODEL_KEY=" + tModelKey.toString() + "\n");

      resultSet = statement.executeQuery();
      if (resultSet.next())
      {
        tModel = new TModel();
        tModel.setTModelKey(tModelKey);
        tModel.setAuthorizedName(resultSet.getString("AUTHORIZED_NAME"));
        tModel.setOperator(resultSet.getString("OPERATOR"));
        tModel.setName(resultSet.getString("NAME"));

        OverviewDoc overviewDoc = new OverviewDoc();
        overviewDoc.setOverviewURL(resultSet.getString("OVERVIEW_DOC"));
        tModel.setOverviewDoc(overviewDoc);
      }

      if (tModel != null)
        log.info("select successful, at least one row was found");
      else
        log.info("select executed successfully but no rows were found with TMODEL_KEY=" + tModelKey.toString());

      return tModel;
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
   * Select all rows from the business_entities table for a given
   * 'AuthorizedName' value.
   *
   * @param  authorizedName The Authorized Name of a TModel owner.
   * @param  JDBC A JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static Vector selectByAuthorizedName(String authorizedName,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (authorizedName == null)
      throw new JUDDIException("attempt to select a Collection of TModel instances using a null AuthorizedName");

    Vector tModelList = new Vector();
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectByAuthorizedNameSQL);
      statement.setString(1,authorizedName.toString());

      log.info("select from TMODEL table:\n\n\t" + selectByAuthorizedNameSQL +
        "\n\t AUTHORIZED_NAME=" + authorizedName.toString() + "\n");

      // execute the statement
      resultSet = statement.executeQuery();

      TModel tModel = null;
      while (resultSet.next())
      {
        tModel = new TModel();
        tModel.setTModelKey(resultSet.getString("TMODEL_KEY"));
        tModel.setAuthorizedName(authorizedName);
        tModel.setOperator(resultSet.getString("OPERATOR"));
        tModel.setName(resultSet.getString("NAME"));

        OverviewDoc overviewDoc = new OverviewDoc();
        overviewDoc.setOverviewURL(resultSet.getString("OVERVIEW_DOC"));
        tModel.setOverviewDoc(overviewDoc);

        tModelList.add(tModel);
        tModel = null;
      }

      log.info("select was successful, rows selected=" + tModelList.size());
      return tModelList;
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
   * TModel identified by the tModelKey parameter
   *
   * @param  tModelKey
   * @param  authorizedName
   * @param  connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static boolean verifyOwnership(String tModelKey,String authorizedName,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if ((tModelKey == null) || (authorizedName == null))
      return false;

    boolean authorized = false;
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      statement = connection.prepareStatement(verifyOwnershipSQL);
      statement.setString(1,tModelKey);
      statement.setString(2,authorizedName);

      log.info("checking ownership of TMODEL:\n\n\t" + verifyOwnershipSQL +
        "\n\t TMODEL_KEY=" + tModelKey +
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
        OverviewDoc overviewDoc = new OverviewDoc();
        OverviewURL overviewURL = new OverviewURL();
        //overviewURL.setText("http://www.steveviens.com/overviewdoc.html");
        overviewDoc.setOverviewURL("http://www.steveviens.com/overviewdoc.html");
        overviewDoc.setOverviewURL(overviewURL);

        String tModelKey = UUID.nextID();
        TModel tModel = new TModel();
        tModel.setTModelKey(tModelKey);
        tModel.setAuthorizedName("sviens");
        tModel.setOperator("WebServiceRegistry.com");
        tModel.setName("Tuscany Web Service Company");
        tModel.setOverviewDoc(overviewDoc);

        // begin a new transaction
        txn.begin(connection);
        
        // insert a new TModel
        TModelTable.insert(tModel,connection);

        // select one of the TModel objects
        tModel = TModelTable.select(tModelKey,connection);

        // select a Collection of TModel objects by AuthorizedName
        Vector tModelList = TModelTable.selectByAuthorizedName("mviens",connection);

        TModelTable.verifyOwnership(tModelKey,"mviens",connection);
        TModelTable.verifyOwnership(tModelKey,"sviens",connection);

        // delete that TModel object
        TModelTable.delete(tModelKey,connection);

        // re-select that TModel object
        tModel = TModelTable.select(tModelKey,connection);

        // re-select a Collection of TModel objects by AuthorizedName
        tModelList = TModelTable.selectByAuthorizedName("mviens",connection);

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
