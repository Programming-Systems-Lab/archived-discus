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
import psl.discus.javasrc.uddi.util.Config;
import psl.discus.javasrc.uddi.uuidgen.UUID;

import org.uddi4j.UDDIElement;
import org.uddi4j.request.SaveTModel;
import org.uddi4j.datatype.tmodel.TModel;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.response.TModelDetail;
import org.uddi4j.util.UploadRegister;
import org.apache.log4j.Logger;
import org.uddi4j.util.AuthInfo;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class SaveTModelService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(SaveTModelService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  // private reference to the name of this UDDI Operator Site
  private static String operator = Config.getProperty("juddi.operatorName");

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((SaveTModel)request);
  }

  /**
   *
   */
  public TModelDetail invoke(SaveTModel request)
    throws JUDDIException
  {
    Vector tModelVector = request.getTModelVector();
    AuthInfo authInfo   = request.getAuthInfo();


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
    return save_tModel(authorizedName,tModelVector);
  }

  /**
   *
   */
  public TModelDetail save_tModel(String authorizedName,Vector tModelVector)
    throws JUDDIException
  {
    // aquire a jUDDI datastore instance
    DataStore datastore = factory.aquireDataStore();
    
    if ((tModelVector != null) && (tModelVector.size() > 0))
    {
      try
      {
        datastore.beginTrans();

        for (int i=0; i<tModelVector.size(); i++)
        {
          // move the TModel into a form we can work with easily
          TModel tModel = (TModel)tModelVector.elementAt(i);
          String tModelKey = tModel.getTModelKey();

          // If the TModel doesn't have a TModelKey already 
          // then we assume that it's a brand new TModel 
          // and we'll generate a new key.
          if ((tModelKey != null) && (tModelKey.length() > 0))
          {
            // check that this TModel really exists. If not 
            // then throw an InvalidKeyPassedException.
            if (!datastore.isValidTModelKey(tModelKey))
              throw new InvalidKeyPassedException("TModelKey: "+tModelKey);

            // check to make sure that 'authorizedName' controls this
            // TModel. If not then throw a UserMismatchException.
            if (!datastore.isTModelOwner(tModelKey,authorizedName))
              throw new UserMismatchException("TModelKey: "+tModelKey);

            // TModel exists and we control it so let's delete it.
            datastore.deleteTModel(tModelKey);
          }
          else // no key so generate a new one.
            tModel.setTModelKey(UUID.nextID());
          
          // everything checks out so let's store
          // 'authorizedName' and 'operator' values
          // in each TModel and save it.
          tModel.setAuthorizedName(authorizedName);
          tModel.setOperator(operator);
          datastore.saveTModel(tModel);
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
    // create a new TModelDetail and stuff the
    // original but 'updated' tModelVector in.
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
    SaveTModel request = new SaveTModel();
    // to-do ... need more here!

    try
    {
      // invoke the service
      TModelDetail response = (TModelDetail)(new SaveTModelService().invoke(request));
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

