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
import psl.discus.javasrc.uddi.uuidgen.UUID;

import org.uddi4j.UDDIElement;
import org.uddi4j.request.SaveBinding;
import org.uddi4j.response.BindingDetail;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.datatype.binding.BindingTemplate;
import org.apache.log4j.Logger;
import org.uddi4j.util.AuthInfo;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class SaveBindingService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(SaveBindingService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((SaveBinding)request);
  }

  /**
   *
   */
  public BindingDetail invoke(SaveBinding request)
    throws JUDDIException
  {
    Vector bindingVector = request.getBindingTemplateVector();
    AuthInfo authInfo    = request.getAuthInfo();


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
    return save_binding(authorizedName,bindingVector);
  }

  /**
   *
   */
  public BindingDetail save_binding(String authorizedName,Vector bindingVector)
    throws JUDDIException
  {
    // aquire a jUDDI datastore instance
    DataStore datastore = factory.aquireDataStore();
    
    if ((bindingVector != null) && (bindingVector.size() > 0))
    {
      try
      {        
        datastore.beginTrans();

        for (int i=0; i<bindingVector.size(); i++)
        {
          // move the BindingTemplate into a form we can work with easily
          BindingTemplate binding = (BindingTemplate)bindingVector.elementAt(i);
          String serviceKey = binding.getServiceKey();
          String bindingKey = binding.getBindingKey();

          // check that the business service that this binding
          // belongs to really exists. If not then throw an 
          // InvalidKeyPassedException.
          if ((serviceKey == null) || (serviceKey.length() == 0) || 
              (!datastore.isValidServiceKey(serviceKey)))
            throw new InvalidKeyPassedException("ServiceKey: "+serviceKey);

          // check to make sure that 'authorizedName' controls the
          // business service that this binding template belongs to. 
          // If not then throw a UserMismatchException.
          if (!datastore.isServiceOwner(serviceKey,authorizedName))
            throw new UserMismatchException("ServiceKey: "+serviceKey);

          // If the BindingTemplate doesn't have a BindingKey 
          // already then we assume that it's a brand new 
          // BindingTemplate and we'll generate a new key.
          if ((bindingKey != null) && (bindingKey.length() > 0))
          {
            // check that this binding tempalte really exists. 
            // If not then throw an InvalidKeyPassedException.
            if (!datastore.isValidBindingKey(bindingKey))
              throw new InvalidKeyPassedException("BindingKey: "+bindingKey);

            // service exists and we control it so let's delete it.
            datastore.deleteBinding(bindingKey);
          }
          else // no key so generate a new one.
            binding.setBindingKey(UUID.nextID());
          
          // everything checks out so let's save it.
          datastore.saveBinding(binding);
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

    // create a new BindingDetail and stuff the
    // original but 'updated' bindingVector in.
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
    SaveBinding request = new SaveBinding();
    // to-do ... need more here!

    try
    {
      // invoke the service
      BindingDetail response = (BindingDetail)(new SaveBindingService().invoke(request));
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

