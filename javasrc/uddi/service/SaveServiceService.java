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
import org.uddi4j.request.SaveService;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.datatype.service.BusinessService;
import org.uddi4j.response.ServiceDetail;
import org.uddi4j.util.AuthInfo;
import org.apache.log4j.Logger;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class SaveServiceService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(SaveServiceService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((SaveService)request);
  }

  /**
   *
   */
  public ServiceDetail invoke(SaveService request)
    throws JUDDIException
  {
    Vector serviceVector = request.getBusinessServiceVector();
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
    return save_service(authorizedName,serviceVector);
  }

  /**
   *
   */
  public ServiceDetail save_service(String authorizedName,Vector serviceVector)
    throws JUDDIException
  {
    // aquire a jUDDI datastore instance
    DataStore datastore = factory.aquireDataStore();
        
    try
    {
      datastore.beginTrans();

      for (int i=0; i<serviceVector.size(); i++)
      {
        // move the BusinessService into a form we can work with easily
        BusinessService service = (BusinessService)serviceVector.elementAt(i);
        String serviceKey = service.getServiceKey();
        String businessKey = service.getBusinessKey();

        // check that the business entity that this service
        // belongs to really exists. If not then throw an 
        // InvalidKeyPassedException.
        if (!datastore.isValidBusinessKey(businessKey))
          throw new InvalidKeyPassedException("BusinessKey: "+businessKey);

        // check to make sure that 'authorizedName' controls the
        // business entity that this service belongs to. If not 
        // then throw a UserMismatchException.
        if (!datastore.isBusinessOwner(businessKey,authorizedName))
          throw new UserMismatchException("BusinessKey: "+businessKey);

        // If the BusinessService doesn't have a ServiceKey 
        // already then we assume that it's a brand new 
        // BusinessService and we'll generate a new key.
        if ((serviceKey != null) && (serviceKey.length() > 0))
        {
          // check that this business service really exists. 
          // If not then throw an InvalidKeyPassedException.
          if (!datastore.isValidServiceKey(serviceKey))
            throw new InvalidKeyPassedException("ServiceKey: "+serviceKey);

          // service exists and we control it so let's delete it.
          datastore.deleteService(serviceKey);
        }
        else  // no key so generate a new one.
          service.setServiceKey(UUID.nextID());

        // everything checks out so let's save it.
        datastore.saveService(service);
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

    // create a new ServiceDetail and stuff the
    // original but 'updated' serviceVector in.
    ServiceDetail detail = new ServiceDetail();
    detail.setBusinessServiceVector(serviceVector);
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
    SaveService request = new SaveService();
    // to-do ... need more here!

    try
    {
      // invoke the service
      ServiceDetail response = (ServiceDetail)(new SaveServiceService().invoke(request));
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

