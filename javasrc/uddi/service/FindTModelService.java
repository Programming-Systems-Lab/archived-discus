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
import org.uddi4j.request.FindTModel;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.response.TModelList;
import org.uddi4j.util.CategoryBag;
import org.uddi4j.util.FindQualifiers;
import org.uddi4j.util.IdentifierBag;
import org.apache.log4j.Logger;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class FindTModelService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(FindTModelService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((FindTModel)request);
  }

  /**
   *
   */
  public TModelList invoke(FindTModel request)
    throws JUDDIException
  {
    String name                 = request.getNameString();
    CategoryBag categoryBag     = request.getCategoryBag();
    IdentifierBag identifierBag = request.getIdentifierBag();
    FindQualifiers qualifiers   = request.getFindQualifiers();
    int maxRows                 = request.getMaxRowsInt();

    // perform the requested action
    return find_tModel(name,categoryBag,identifierBag,qualifiers,maxRows);
  }

  /**
   *
   */
  public TModelList find_tModel(String name,CategoryBag categoryBag,IdentifierBag identifierBag,FindQualifiers findQualifiers,int maxRows)
    throws JUDDIException
  {
    TModelList list = new TModelList();
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
    FindTModel request = new FindTModel();
    // to-do ... need more here!

    try
    {
      // invoke the service
      TModelList response = (TModelList)(new FindTModelService().invoke(request));
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

