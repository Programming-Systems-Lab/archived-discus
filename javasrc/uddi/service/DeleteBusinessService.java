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

import org.uddi4j.UDDIElement;
import org.uddi4j.request.DeleteBusiness;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.util.AuthInfo;
import org.apache.log4j.Logger;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class DeleteBusinessService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(DeleteBusinessService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((DeleteBusiness)request);
  }

  /**
   *
   */
  public DispositionReport invoke(DeleteBusiness request)
    throws JUDDIException
  {
    Vector businessKeyVector = request.getBusinessKeyStrings();
    AuthInfo authInfo        = request.getAuthInfo();

    // 1. Check that an authToken was actually included in the

    //    request. If not then throw an AuthTokenRequiredException.

    // 2. Check that the authToken passed in is a valid one.

    //    If not then throw an AuthTokenRequiredException.

    // 3. Check that the authToken passed in has not yet.

    //    expired. If so then throw an AuthTokenExpiredException.

    Auth.validateAuthToken(authInfo);



    // lookup and validate authentication info (publish only)
    String authorizedName = Auth.getAssociatedUserID(authInfo);

    // okay, let's give it a shot
    delete_business(authorizedName,businessKeyVector);

    // didn't encounter an exception so let's return
    // the pre-created successful DispositionReport
    return this.success;
  }

  /**
   *
   */
  public void delete_business(String authorizedName,Vector businessKeyVector)
    throws JUDDIException
  {
    // aquire a jUDDI datastore instance
    DataStore datastore = factory.aquireDataStore();

    try
    {
      datastore.beginTrans();

      for (int i=0; i<businessKeyVector.size(); i++)
      {
        // grab the next key from the vector
        String businessKey = (String)businessKeyVector.elementAt(i);

        // check that this business entity really exists. 
        // If not then throw an InvalidKeyPassedException.
        if ((!datastore.isValidBusinessKey(businessKey)))
          throw new InvalidKeyPassedException("BusinessKey: "+businessKey);

        // check to make sure that 'authorizedName' controls this
        // business entity. If not then throw a UserMismatchException.
        if (!datastore.isBusinessOwner(businessKey,authorizedName))
          throw new UserMismatchException("BusinessKey: "+businessKey);

        // business exists and we control it so let's delete it.
        datastore.deleteBusiness(businessKey);
      }

      datastore.commit();
    }
    catch(Exception ex)
    {
      // we must rollback for *any/all* exceptions
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
    DeleteBusiness request = new DeleteBusiness();
    // to-do ... need more here!

    try
    {
      // invoke the service
      DispositionReport response = (DispositionReport)(new DeleteBusinessService().invoke(request));
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

