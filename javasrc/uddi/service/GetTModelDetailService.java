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
import org.uddi4j.request.GetTModelDetail;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.response.TModelDetail;
import org.apache.log4j.Logger;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class GetTModelDetailService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(GetTModelDetailService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((GetTModelDetail)request);
  }

  /**
   *
   */
  public TModelDetail invoke(GetTModelDetail request)
    throws JUDDIException
  {
    Vector tModelKeys = request.getTModelKeyStrings();

    // perform the requested action
    return get_tModelDetail(tModelKeys);
  }

  /**
   *
   */
  public TModelDetail get_tModelDetail(Vector tModelKeyVector)
    throws JUDDIException
  {
    Vector tModelVector = new Vector();

    if ((tModelKeyVector != null) && (tModelKeyVector.size() > 0))
    {
      // aquire a jUDDI datastore instance
      DataStore datastore = factory.aquireDataStore();

      try
      {
        datastore.beginTrans();

        // Check that every key in the vector really exists. 
        // If 'any one' of the keys do not exist then throw 
        // an InvalidKeyPassedException.

        for (int i=0; i<tModelKeyVector.size(); i++)
        {
          // grab the next key from the vector
          String tModelKey = (String)tModelKeyVector.elementAt(i);

          // is the TModel key passed in a valid one?
          if ((tModelKey == null) || (tModelKey.length() == 0) || (!datastore.isValidTModelKey(tModelKey)))
            throw new InvalidKeyPassedException("TModelKey: "+tModelKey);

          // TModel exists so let's fetch'n store it
          tModelVector.add(datastore.fetchTModel(tModelKey));
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
    }
    
    // create a new TModelDetail and
    // stuff the tModelVector into it.
    TModelDetail detail = new TModelDetail();
    detail.setTModelVector(tModelVector);
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
    GetTModelDetail request = new GetTModelDetail();
    // to-do ... need more here!

    try
    {
      // invoke the service
      TModelDetail response = (TModelDetail)(new GetTModelDetailService().invoke(request));
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

