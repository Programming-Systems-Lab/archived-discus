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
import org.uddi4j.request.DeleteService;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.util.AuthInfo;
import org.apache.log4j.Logger;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class DeleteServiceService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(DeleteServiceService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((DeleteService)request);
  }

  /**
   *
   */
  public DispositionReport invoke(DeleteService request)
    throws JUDDIException
  {
    Vector serviceKeyVector = request.getServiceKeyStrings();
    AuthInfo authInfo       = request.getAuthInfo();


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
    delete_service(authorizedName,serviceKeyVector);

    // didn't encounter an exception so let's return
    // the pre-created successful DispositionReport
    return this.success;
  }

  /**
   *
   */
  public void delete_service(String authorizedName,Vector serviceKeyVector)
    throws JUDDIException
  {
    if ((serviceKeyVector != null) && (serviceKeyVector.size() > 0))
    {
      // aquire a jUDDI datastore instance
      DataStore datastore = factory.aquireDataStore();

      try
      {
        datastore.beginTrans();

        for (int i=0; i<serviceKeyVector.size(); i++)
        {
          // grab the next key from the vector
          String serviceKey = (String)serviceKeyVector.elementAt(i);
          
          // check that this business service really exists. 
          // If not then throw an InvalidKeyPassedException.
          if ((serviceKey == null) || (serviceKey.length() == 0) || 
              (!datastore.isValidServiceKey(serviceKey)))
            throw new InvalidKeyPassedException("ServiceKey: "+serviceKey);

          // check to make sure that 'authorizedName' controls the
          // business entity that this service belongs to. If not 
          // then throw a UserMismatchException.
          if (!datastore.isServiceOwner(serviceKey,authorizedName))
            throw new UserMismatchException("ServiceKey: "+serviceKey);

          // service exists and we control it so let's delete it.
          datastore.deleteService(serviceKey);
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
  }

  
  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    // initialize all jUDDI Subsystems
    psl.discus.javasrc.uddi.util.SysManager.startup();

    // create a request
    DeleteService request = new DeleteService();
    // to-do ... need more here!

    try
    {
      // invoke the service
      DispositionReport response = (DispositionReport)(new DeleteServiceService().invoke(request));
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

