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
import org.uddi4j.request.DeleteBinding;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.util.AuthInfo;
import org.apache.log4j.Logger;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class DeleteBindingService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(DeleteBindingService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((DeleteBinding)request);
  }

  /**
   *
   */
  public DispositionReport invoke(DeleteBinding request)
    throws JUDDIException
  {
    Vector bindingKeyVector = request.getBindingKeyStrings();
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
    delete_binding(authorizedName,bindingKeyVector);

    // didn't encounter an exception so let's return
    // the pre-created successful DispositionReport
    return this.success;
  }

  /**
   *
   */
  public void delete_binding(String authorizedName,Vector bindingKeyVector)
    throws JUDDIException
  {
    if ((bindingKeyVector != null) && (bindingKeyVector.size() > 0))
    {
      // aquire a jUDDI datastore instance
      DataStore datastore = factory.aquireDataStore();

      try
      {
        datastore.beginTrans();

        for (int i=0; i<bindingKeyVector.size(); i++)
        {
          // grab the next key from the vector
          String bindingKey = (String)bindingKeyVector.elementAt(i);

          // check that this binding template really exists. 
          // If not then throw an InvalidKeyPassedException.
          if ((bindingKey == null) || (bindingKey.length() == 0) || 
              (!datastore.isValidBindingKey(bindingKey)))
            throw new InvalidKeyPassedException("BindingKey: "+bindingKey);

          // check to make sure that 'authorizedName' controls the
          // business entity that this binding belongs to. If not 
          // then throw a UserMismatchException.
          if (!datastore.isBindingOwner(bindingKey,authorizedName))
            throw new UserMismatchException("BindingKey: "+bindingKey);

          // binding exists and we control it so let's delete it.
          datastore.deleteBinding(bindingKey);
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
    DeleteBinding request = new DeleteBinding();
    // to-do ... need more here!

    try
    {
      // invoke the service
      DispositionReport response = (DispositionReport)(new DeleteBindingService().invoke(request));
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

