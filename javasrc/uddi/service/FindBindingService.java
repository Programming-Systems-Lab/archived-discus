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
import org.uddi4j.request.FindBinding;
import org.uddi4j.response.BindingDetail;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.util.FindQualifiers;
import org.uddi4j.util.TModelBag;
import org.apache.log4j.Logger;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class FindBindingService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(FindBindingService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((FindBinding)request);
  }

  /**
   *
   */
  public BindingDetail invoke(FindBinding request)
    throws JUDDIException
  {
    String serviceKey         = request.getServiceKey();
    TModelBag tModelBag       = request.getTModelBag();
    FindQualifiers qualifiers = request.getFindQualifiers();
    int maxRows               = request.getMaxRowsInt();
 
    // perform the requested action
    return find_binding(serviceKey,tModelBag,qualifiers,maxRows);
  }

  /**
   *
   */
  public BindingDetail find_binding(String serviceKey,TModelBag tmodelbag,FindQualifiers findQualifiers,int maxRows)
    throws JUDDIException
  {
    BindingDetail detail = new BindingDetail();
    return detail;
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    // initialize all jUDDI Subsystems
    psl.discus.javasrc.uddi.util.SysManager.startup();

    // create a request
    FindBinding request = new FindBinding();
    // to-do ... need more here!

    try
    {
      // invoke the service
      BindingDetail response = (BindingDetail)(new FindBindingService().invoke(request));
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
