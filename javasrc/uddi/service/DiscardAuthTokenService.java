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
import org.uddi4j.request.GetAuthToken;
import org.uddi4j.request.DiscardAuthToken;
import org.uddi4j.response.AuthToken;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.util.AuthInfo;
import org.apache.log4j.Logger;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class DiscardAuthTokenService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(DiscardAuthTokenService.class);

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((DiscardAuthToken)request);
  }

  /**
   *
   */
  public DispositionReport invoke(DiscardAuthToken request)
    throws JUDDIException
  {
    AuthInfo authInfo = request.getAuthInfo();


    // 1. Check that an authToken was actually included in the

    //    request. If not then throw an AuthTokenRequiredException.

    // 2. Check that the authToken passed in is a valid one.

    //    If not then throw an AuthTokenRequiredException.

    // 3. Check that the authToken passed in has not yet.

    //    expired. If so then throw an AuthTokenExpiredException.

    Auth.validateAuthToken(authInfo);



    // okay, let's give it a shot
    Auth.discardAuthToken(authInfo);

    // didn't encounter an exception so let's return
    // the pre-created successful DispositionReport
    return this.success;
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    // initialize all jUDDI Subsystems
    psl.discus.javasrc.uddi.util.SysManager.startup();

    try
    {
      // generate the request object
      GetAuthToken getRequest = new GetAuthToken("sviens","password");

      // invoke the service
      AuthToken getResponse = (AuthToken)(new GetAuthTokenService().invoke(getRequest));

      // create a request
      DiscardAuthToken discardRequest1 = new DiscardAuthToken(getResponse.getAuthInfoString());
      // invoke the service with a valid AuthToken value
      DispositionReport discardResponse = (DispositionReport)(new DiscardAuthTokenService().invoke(discardRequest1));

      // create a request
      DiscardAuthToken discardRequest2 = new DiscardAuthToken("**-BadAuthToken-**");
      // invoke the service with an invalid AuthToken value
      DispositionReport discardResponse2 = (DispositionReport)(new DiscardAuthTokenService().invoke(discardRequest2));
    }
    catch(JUDDIException juddiex)
    {
      // dump the exception info to the console
      System.out.println(juddiex.toString());
    }
    finally
    {
      // terminate all jUDDI Subsystems
      psl.discus.javasrc.uddi.util.SysManager.shutdown();
    }
  }
}

