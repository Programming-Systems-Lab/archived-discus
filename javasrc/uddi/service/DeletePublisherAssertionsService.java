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
import psl.discus.javasrc.uddi.util.*;

import org.uddi4j.UDDIElement;
import org.uddi4j.request.DeletePublisherAssertions;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.util.AuthInfo;
import org.apache.log4j.Logger;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class DeletePublisherAssertionsService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(DeletePublisherAssertionsService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((DeletePublisherAssertions)request);
  }

  /**
   *
   */
  public DispositionReport invoke(DeletePublisherAssertions request)
    throws JUDDIException
  {
    Vector assertionsVector = request.getPublisherAssertionVector();
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
    delete_publisherAssertions(authorizedName,assertionsVector);

    // didn't encounter an exception so let's return
    // the pre-created successful DispositionReport
    return this.success;
  }

  /**
   *
   */
  public void delete_publisherAssertions(String authorizedName,Vector publisherAssertions)
    throws JUDDIException
  {
    return;
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    // initialize all jUDDI Subsystems
    psl.discus.javasrc.uddi.util.SysManager.startup();

    // create a request
    DeletePublisherAssertions request = new DeletePublisherAssertions();
    // to-do ... need more here!

    try
    {
      // invoke the service
      DispositionReport response = (DispositionReport)(new DeletePublisherAssertionsService().invoke(request));
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

