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
import org.uddi4j.response.AuthToken;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.util.AuthInfo;
import org.apache.log4j.Logger;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class GetAuthTokenService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(GetAuthTokenService.class);

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((GetAuthToken)request);
  }

  /**
   *
   */
  public AuthToken invoke(GetAuthToken request)
    throws JUDDIException
  {
    String authorizedName = request.getUserID();
    String credential     = request.getCred();

    // perform the requested action
    return Auth.getAuthToken(authorizedName,credential);
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
      GetAuthToken request = new GetAuthToken("sviens","password");

      // invoke the service
      AuthToken response = (AuthToken)(new GetAuthTokenService().invoke(request));

      // write response to the console
      System.out.println("UDDIService: getAuthToken");
      System.out.println(" AuthInfo: "+response.getAuthInfoString());
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