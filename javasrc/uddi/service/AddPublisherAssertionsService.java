/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.service;

import psl.discus.javasrc.uddi.auth.*;
import psl.discus.javasrc.uddi.datastore.*;
import psl.discus.javasrc.uddi.error.*;

import org.uddi4j.client.UDDIProxy;
import org.uddi4j.UDDIElement;
import org.uddi4j.request.AddPublisherAssertions;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.datatype.assertion.PublisherAssertion;
import org.uddi4j.util.AuthInfo;
import org.apache.log4j.Logger;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class AddPublisherAssertionsService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(AddPublisherAssertionsService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((AddPublisherAssertions)request);
  }

  /**
   *
   */
  public DispositionReport invoke(AddPublisherAssertions request)
    throws JUDDIException
  {
    Vector assertionsVector = request.getPublisherAssertionVector();
    AuthInfo authInfo       = request.getAuthInfo();

    // lookup and validate authentication info (publish only)
    String authorizedName = Auth.getAssociatedUserID(authInfo);

    // okay, let's give it a shot
    add_publisherAssertions(authorizedName,assertionsVector);

    // didn't encounter an exception so let's return
    // the pre-created successful DispositionReport
    return this.success;
  }

  /**
   *
   */
  public void add_publisherAssertions(String authorizedName,Vector assertionVector)
    throws JUDDIException
  {
    // aquire a jUDDI datastore instance
    DataStore datastore = factory.aquireDataStore();

    try
    {
      datastore.beginTrans();

      for (int i=0; i<assertionVector.size(); i++)
      {
        // move the BindingTemplate into a form we can work with easily
        PublisherAssertion assertion = (PublisherAssertion)assertionVector.elementAt(i);
        String fromKey = assertion.getFromKeyString();
        String toKey = assertion.getToKeyString();

        // check that the BusinessEntitys or tModels that are 
        // identified in this assertion really exists. If not 
        // then throw an InvalidKeyPassedException.
        if ((fromKey == null) || (fromKey.length() == 0) || 
            (!datastore.isValidBusinessKey(fromKey)) && (!datastore.isValidTModelKey(fromKey)))
          throw new InvalidKeyPassedException("fromKey: "+fromKey);

        if ((toKey == null) || (toKey.length() == 0) || 
            (!datastore.isValidBusinessKey(toKey)) && (!datastore.isValidTModelKey(toKey)))
          throw new InvalidKeyPassedException("toKey: "+toKey);

        // check to make sure that 'authorizedName' controls the
        // BusinessEntities or TModels that are identified in this
        // assertion. If not then throw a UserMismatchException.
        if (((!datastore.isBusinessOwner(fromKey,authorizedName)) && (!datastore.isTModelOwner(fromKey,authorizedName))) ||
            ((!datastore.isBusinessOwner(toKey,authorizedName)) && (!datastore.isTModelOwner(toKey,authorizedName))))
          throw new UserMismatchException("fromKey: "+fromKey+" || toKey: "+toKey);

        // everything checks out so let's save it.
        //saveAssertion(assertion,connection);
      }

      datastore.commit();
    }
    catch(Exception ex)
    {
      // we must rollback for *any* exception
      try { datastore.rollback(); }
      catch(Exception e) { }
      
      // write to the log
      log.error(ex);

      // prep JUDDIException to throw
      if (ex instanceof JUDDIException)
        throw (JUDDIException)ex;
      else
        throw new JUDDIException(ex);  
    }
    finally
    {
      factory.releaseDataStore(datastore);
    }
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    // initialize all jUDDI Subsystems
    psl.discus.javasrc.uddi.util.SysManager.startup();

    // create a request
    AddPublisherAssertions request = new AddPublisherAssertions();
    // to-do ... need more here!

    try
    {
      // invoke the service
      DispositionReport response = (DispositionReport)(new AddPublisherAssertionsService().invoke(request));
      // to-do ... need more here!
    }
    catch(JUDDIException juddiex)
    {
      System.out.println(juddiex.toString());
    }
    finally
    {
      // terminate all jUDDI Subsystems
      psl.discus.javasrc.uddi.util.SysManager.shutdown();
    }
  }
}

