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
import org.uddi4j.request.ValidateValues;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.apache.log4j.Logger;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class ValidateValuesService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(ValidateValuesService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

 /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return invoke((ValidateValues)request);
  }

 /**
   *
   */
  public DispositionReport invoke(ValidateValues request)
    throws JUDDIException
  {
    Vector businessVector = request.getBusinessEntityVector();
    Vector serviceVector  = request.getBusinessServiceVector();
    Vector tModelVector   = request.getTModelVector();

    // if we got any Business Entities then let's check'm
    if ((businessVector != null) && (businessVector.size() > 0))
      validate_values_businessEntity(businessVector);
      
    // if we got any Services then let's check'm
    if ((serviceVector != null) && (serviceVector.size() > 0))
      validate_values_businessService(serviceVector);

    // if we got any TModels then let's check'm
    if ((tModelVector != null) && (tModelVector.size() > 0))
      validate_values_tModel(tModelVector);

    // didn't encounter an exception so let's return
    // the pre-created successful DispositionReport
    return this.success;
  }

  /**
   *
   */
  public void validate_values_businessEntity(Vector businessEntity)
    throws JUDDIException
  {
  }

  /**
   *
   */
  public void validate_values_businessService(Vector businessService)
    throws JUDDIException
  {
  }

  /**
   *
   */
  public void validate_values_tModel(Vector tModel)
    throws JUDDIException
  {
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    // initialize all jUDDI Subsystems
    psl.discus.javasrc.uddi.util.SysManager.startup();

    // create a request
    ValidateValues request = new ValidateValues();
    // to-do ... need more here!

    try
    {
      // invoke the service
      DispositionReport response = (DispositionReport)(new ValidateValuesService().invoke(request));
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

