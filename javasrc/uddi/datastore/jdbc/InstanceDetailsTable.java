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
import org.uddi4j.datatype.binding.*;
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
public class InstanceDetailsTable
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(InstanceDetailsTable.class);

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

    dropSQL = "DROP TABLE INSTANCE_DETAILS";



    // build createSQL
    sql = new StringBuffer(150);
    sql.append("CREATE TABLE INSTANCE_DETAILS (");
    sql.append("BINDING_KEY VARCHAR(41) NOT NULL,");
    sql.append("TMODEL_INSTANCE_INFO_ID INT NOT NULL,");
    sql.append("INSTANCE_DETAILS_ID INT NOT NULL,");
    sql.append("INSTANCE_PARMS VARCHAR(255),");
    sql.append("PRIMARY KEY (BINDING_KEY,TMODEL_INSTANCE_INFO_ID,INSTANCE_DETAILS_ID),");
    sql.append("FOREIGN KEY (BINDING_KEY,TMODEL_INSTANCE_INFO_ID) REFERENCES TMODEL_INSTANCE_INFO (BINDING_KEY,TMODEL_INSTANCE_INFO_ID))");
    createSQL = sql.toString();

    // build insertSQL
    sql = new StringBuffer(150);
    sql.append("INSERT INTO INSTANCE_DETAILS (");
    sql.append("BINDING_KEY,");
    sql.append("TMODEL_INSTANCE_INFO_ID,");
    sql.append("INSTANCE_DETAILS_ID,");
    sql.append("INSTANCE_PARMS) ");
    sql.append("VALUES (?,?,?,?)");
    insertSQL = sql.toString();

    // build selectSQL
    sql = new StringBuffer(200);
    sql.append("SELECT ");
    sql.append("INSTANCE_PARMS ");
    sql.append("FROM INSTANCE_DETAILS ");
    sql.append("WHERE BINDING_KEY=? ");
    sql.append("AND TMODEL_INSTANCE_INFO_ID=? ");
    sql.append("ORDER BY INSTANCE_DETAILS_ID");
    selectSQL = sql.toString();

    // build deleteSQL
    sql = new StringBuffer(100);
    sql.append("DELETE FROM INSTANCE_DETAILS ");
    sql.append("WHERE BINDING_KEY=?");
    deleteSQL = sql.toString();
  }

  /**

   * Drop the INSTANCE_DETAILS table.

   *

   * @throws java.sql.SQLException

   */

  public static void drop(Connection connection)

    throws java.sql.SQLException

  {

    System.out.print("DROP TABLE INSTANCE_DETAILS: ");

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
   * Create the INSTANCE_DETAILS table.
   *
   * @throws java.sql.SQLException
   */
  public static void create(Connection connection)
    throws java.sql.SQLException
  {
    System.out.print("CREATE TABLE INSTANCE_DETAILS: ");
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
   * Insert new row into the INSTANCE_DETAILS table.<p>
   *
   * @param  bindingKey String to the BusinessEntity object that owns the Contact to be inserted
   * @param  detailsList Vector of InstanceDetails objects holding values to be inserted
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void insert(String bindingKey,int tModelInstanceInfoID,InstanceDetails details,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (details == null)
      throw new JUDDIException("attempt to insert a null InstanceDetail object");

    if (bindingKey == null)
      throw new JUDDIException("attempt to insert a Collection of InstanceDetail instances using a null BindingKey");

    PreparedStatement statement = null;

    try
    {
      statement = connection.prepareStatement(insertSQL);
      statement.setString(1,bindingKey.toString());
      statement.setInt(2,tModelInstanceInfoID);

      int detailsID = 0;

      String instParms = null;
      if (details.getInstanceParms() != null)
        instParms = details.getInstanceParms().getText();

      log.info("insert into INSTANCE_DETAILS table:\n\n\t" + insertSQL +
        "\n\t BINDING_KEY=" + bindingKey.toString() +
        "\n\t TMODEL_INSTANCE_INFO_ID=" + tModelInstanceInfoID +
        "\n\t INSTANCE_DETAILS_ID=" + detailsID +
        "\n\t INSTANCE_PARMS=" + instParms + "\n");

      statement.setInt(3,detailsID);
      statement.setString(4,instParms);

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
   * Select all rows from the INSTANCE_DETAILS table for a given BindingKey.<p>
   *
   * @param  bindingKey String
   * @param  tModelInstanceInfoID ID (sequence number) of the parent tModelInstanceInfo object
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static InstanceDetails select(String bindingKey,int tModelInstanceInfoID,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (bindingKey == null)
      throw new JUDDIException("attempt to select a Collection of InstanceDetail instances using a null BindingKey");

    InstanceDetails details = null;
    PreparedStatement statement = null;
    ResultSet resultSet = null;

    try
    {
      // create a statement to query with
      statement = connection.prepareStatement(selectSQL);
      statement.setString(1,bindingKey.toString());
      statement.setInt(2,tModelInstanceInfoID);

      log.info("select from INSTANCE_DETAILS table:\n\n\t" + selectSQL +
        "\n\t BINDING_KEY=" + bindingKey.toString() +
        "\n\t TMODEL_INSTANCE_INFO_ID=" + tModelInstanceInfoID + "\n");

      // execute the statement
      resultSet = statement.executeQuery();
      if (resultSet.next())
      {
        InstanceParms instanceParms = new InstanceParms(resultSet.getString("INSTANCE_PARMS"));
        details = new InstanceDetails();
        details.setInstanceParms(instanceParms);
      }

      if (details != null)
        log.info("select successful, at least one row was found");
      else
        log.info("select executed successfully but no rows were found with BINDING_KEY=" + bindingKey.toString() + " and TMODEL_INSTANCE_INFO_ID=" + tModelInstanceInfoID);

       return details;
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
   * Delete multiple rows from the INSTANCE_DETAILS table that are assigned
   * to the BindingKey specified.<p>
   *
   * @param  bindingKey String
   * @param  connection JDBC connection
   * @throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
   */
  public static void delete(String bindingKey,Connection connection)
    throws psl.discus.javasrc.uddi.error.JUDDIException, java.sql.SQLException
  {
    if (bindingKey == null)
      throw new JUDDIException("attempt to delete a Collection of InstanceDetail instances using a null BindingKey");

    PreparedStatement statement = null;

    try
    {
      // prepare the delete
      statement = connection.prepareStatement(deleteSQL);
      statement.setString(1,bindingKey.toString());

      log.info("delete from INSTANCE_DETAILS table:\n\n\t" + deleteSQL +
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
        String businessKey = UUID.nextID();
        BusinessEntity business = new BusinessEntity();
        business.setBusinessKey(businessKey);
        business.setAuthorizedName("sviens");
        business.setOperator("WebServiceRegistry.com");

        String serviceKey = UUID.nextID();
        BusinessService service = new BusinessService();
        service.setBusinessKey(businessKey);
        service.setServiceKey(serviceKey);

        String bindingKey = UUID.nextID();
        BindingTemplate binding = new BindingTemplate();
        binding.setServiceKey(serviceKey);
        binding.setBindingKey(bindingKey);
        binding.setAccessPoint(new AccessPoint("http://www.juddi.org/bindingtemplate.html","http"));

        Vector infoList = new Vector();
        infoList.add(new TModelInstanceInfo(UUID.nextID()));
        int infoID = 0;

        InstanceDetails details = new InstanceDetails();

        // begin a new transaction
        txn.begin(connection);

        // insert a new BusinessEntity
        BusinessEntityTable.insert(business,connection);

        // insert a new BusinessService
        BusinessServiceTable.insert(service,connection);

        // insert a new BindingTemplate
        BindingTemplateTable.insert(binding,connection);

        // insert a Collection of TModelInstanceInfo objects
        TModelInstanceInfoTable.insert(bindingKey,infoList,connection);

        // insert an InstanceDetails object
        InstanceDetailsTable.insert(bindingKey,infoID,details,connection);

        // select the InstanceDetails object
        details = InstanceDetailsTable.select(bindingKey,infoID,connection);

        // delete the InstanceDetails object
        InstanceDetailsTable.delete(bindingKey,connection);

        // re-select the InstanceDetails object
        details = InstanceDetailsTable.select(bindingKey,infoID,connection);

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
