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
import org.uddi4j.datatype.business.BusinessEntity;
import org.uddi4j.response.BusinessEntityExt;
import org.uddi4j.request.GetBusinessDetailExt;
import org.uddi4j.response.BusinessDetailExt;
import org.uddi4j.response.BusinessDetail;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.apache.log4j.Logger;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class GetBusinessDetailExtService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(GetBusinessDetailExtService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((GetBusinessDetailExt)request);
  }

  /**
   *
   */
  public BusinessDetailExt invoke(GetBusinessDetailExt request)
    throws JUDDIException
  {
    Vector businessKeys = request.getBusinessKeyStrings();

    // perform the requested action
    return get_businessDetailExt(businessKeys);
  }

  /**
   *
   */
  public BusinessDetailExt get_businessDetailExt(Vector businessKeyVector)
    throws JUDDIException
  {
    Vector businessExtVector = new Vector();
    
    // aquire a jUDDI datastore instance
    DataStore datastore = factory.aquireDataStore();

    try
    {
      datastore.beginTrans();

      // Check that every key in the vector really exists.
      // If 'any one' of the keys do not exist then throw an 
      // InvalidKeyPassedException.

      for (int i=0; i<businessKeyVector.size(); i++)
      {
        // grab the next key from the vector
        String businessKey = (String)businessKeyVector.elementAt(i);

        // check that this business entity really exists. 
        // If not then throw an InvalidKeyPassedException.
        if (!datastore.isValidBusinessKey(businessKey))
          throw new InvalidKeyPassedException("BusinessKey: "+businessKey);

        // business exists so let's fetch'n store it
        BusinessEntity business = (BusinessEntity)datastore.fetchBusiness(businessKey);
        businessExtVector.add(new BusinessEntityExt(business));
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

    // create a new BusinessDetailExt and stuff 
    // the new businessExtVector into it.
    BusinessDetailExt detailExt = new BusinessDetailExt();
    detailExt.setBusinessEntityExtVector(businessExtVector);
    return detailExt;
  }

  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    // initialize all jUDDI Subsystems
    psl.discus.javasrc.uddi.util.SysManager.startup();

    // create a request
    GetBusinessDetailExt request = new GetBusinessDetailExt();
    // to-do ... need more here!

    try
    {
      // invoke the service
      BusinessDetailExt response = (BusinessDetailExt)(new GetBusinessDetailExtService().invoke(request));
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

