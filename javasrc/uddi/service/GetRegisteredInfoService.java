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
import org.uddi4j.request.GetRegisteredInfo;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.response.RegisteredInfo;
import org.uddi4j.util.AuthInfo;
import org.uddi4j.response.BusinessInfos;
import org.uddi4j.response.TModelInfos;
import org.apache.log4j.Logger;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class GetRegisteredInfoService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(GetRegisteredInfoService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((GetRegisteredInfo)request);
  }

  /**
   *
   */
  public RegisteredInfo invoke(GetRegisteredInfo request)
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



    // lookup and validate authentication info (publish only)
    String authorizedName = Auth.getAssociatedUserID(authInfo);

    // perform the requested action
    return get_registeredInfo(authorizedName);
  }

  /**
   *
   */
  public RegisteredInfo get_registeredInfo(String authorizedName)
    throws JUDDIException
  {
    BusinessInfos businessInfos = new BusinessInfos();
    TModelInfos tModelInfos = new TModelInfos();
    
    // TO-DO: Reference pg. 42 of the UDDI v2.0 Programmers Specification
    
    RegisteredInfo registeredInfo = new RegisteredInfo();
    registeredInfo.setBusinessInfos(businessInfos);
    registeredInfo.setTModelInfos(tModelInfos);
    return registeredInfo;
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    // initialize all jUDDI Subsystems
    psl.discus.javasrc.uddi.util.SysManager.startup();

    // create a request
    GetRegisteredInfo request = new GetRegisteredInfo();
    // to-do ... need more here!

    try
    {
      // invoke the service
      RegisteredInfo response = (RegisteredInfo)(new GetRegisteredInfoService().invoke(request));
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

