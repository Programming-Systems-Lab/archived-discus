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
import org.uddi4j.request.GetAssertionStatusReport;
import org.uddi4j.response.AssertionStatusReport;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.util.AuthInfo;
import org.apache.log4j.Logger;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class GetAssertionStatusReportService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(GetAssertionStatusReportService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((GetAssertionStatusReport)request);
  }

  /**
   *
   */
  public AssertionStatusReport invoke(GetAssertionStatusReport request)
    throws JUDDIException
  {
    String completionStatus = request.getCompletionStatusString();
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

    // perform the requested action
    return getAssertionStatusReport(authorizedName,completionStatus);
  }

  /**
   *
   */
  public AssertionStatusReport getAssertionStatusReport(String authorizedName,String completionStatus)
    throws JUDDIException
  {
    AssertionStatusReport report = new AssertionStatusReport();
    return report;
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    // initialize all jUDDI Subsystems
    psl.discus.javasrc.uddi.util.SysManager.startup();

    // create a request
    GetAssertionStatusReport request = new GetAssertionStatusReport();
    // to-do ... need more here!

    try
    {
      // invoke the service
      AssertionStatusReport response = (AssertionStatusReport)(new GetAssertionStatusReportService().invoke(request));
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

