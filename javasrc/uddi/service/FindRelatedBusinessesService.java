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
import org.uddi4j.datatype.*;
import org.uddi4j.request.FindRelatedBusinesses;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.response.RelatedBusinessesList;
import org.uddi4j.util.FindQualifiers;
import org.uddi4j.util.KeyedReference;
import org.apache.log4j.Logger;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class FindRelatedBusinessesService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(FindRelatedBusinessesService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((FindRelatedBusinesses)request);
  }

  /**
   *
   */
  public RelatedBusinessesList invoke(FindRelatedBusinesses request)
    throws JUDDIException
  {
    String businessKey         = request.getBusinessKey();
    FindQualifiers qualifiers  = request.getFindQualifiers();
    KeyedReference keyedRef    = request.getKeyedReference();
    int maxRows                = request.getMaxRowsInt();

    // perform the requested action
    return find_relatedBusinesses(businessKey,qualifiers,keyedRef,maxRows);
  }

  /**
   *
   */
  public RelatedBusinessesList find_relatedBusinesses(String businessKey,FindQualifiers qualifiers,KeyedReference keyedReference,int maxRows)
    throws JUDDIException
  {
    RelatedBusinessesList list = new RelatedBusinessesList();
    return list;
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    // initialize all jUDDI Subsystems
    psl.discus.javasrc.uddi.util.SysManager.startup();

    // create a request
    FindRelatedBusinesses request = new FindRelatedBusinesses();
    // to-do ... need more here!

    try
    {
      // invoke the service
      RelatedBusinessesList response = (RelatedBusinessesList)(new FindRelatedBusinessesService().invoke(request));
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

