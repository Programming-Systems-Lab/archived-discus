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
import org.uddi4j.request.GetBindingDetail;
import org.uddi4j.response.BindingDetail;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.apache.log4j.Logger;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class GetBindingDetailService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(GetBindingDetailService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((GetBindingDetail)request);
  }

  /**
   *
   */
  public BindingDetail invoke(GetBindingDetail request)
    throws JUDDIException
  {
    Vector bindingKeys = request.getBindingKeyStrings();

    // perform the requested action
    return get_bindingDetail(bindingKeys);
  }

  /**
   *
   */
  public BindingDetail get_bindingDetail(Vector bindingKeyVector)
    throws JUDDIException
  {
    Vector bindingVector = new Vector();

    if ((bindingKeyVector != null) && (bindingKeyVector.size() > 0))
    {
      // aquire a jUDDI datastore instance
      DataStore datastore = factory.aquireDataStore();

      try
      {
        datastore.beginTrans();

        // Check that every key in the vector really exists. 
        // If 'any one' of the keys do not exist then throw 
        // an InvalidKeyPassedException.

        for (int i=0; i<bindingKeyVector.size(); i++)
        {
          // grab the next key from the vector
          String bindingKey = (String)bindingKeyVector.elementAt(i);
          
          // check that this binding template really exists. 
          // If not then throw an InvalidKeyPassedException.
          if ((bindingKey == null) || (bindingKey.length() == 0) || 
              (!datastore.isValidBindingKey(bindingKey)))
            throw new InvalidKeyPassedException("BindingKey: "+bindingKey);

          // binding exists so let's fetch'n store it
          bindingVector.add(datastore.fetchBinding(bindingKey));
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
    
    // create a new ServiceDetail and
    // stuff the serviceVector into it.
    BindingDetail detail = new BindingDetail();
    detail.setBindingTemplateVector(bindingVector);
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
    GetBindingDetail request = new GetBindingDetail();
    // to-do ... need more here!

    try
    {
      // invoke the service
      BindingDetail response = (BindingDetail)(new GetBindingDetailService().invoke(request));
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