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

import org.apache.log4j.Logger;
import org.uddi4j.UDDIElement;
import org.uddi4j.datatype.Name;
import org.uddi4j.datatype.business.BusinessEntity;
import org.uddi4j.request.GetAuthToken;
import org.uddi4j.request.SaveBusiness;
import org.uddi4j.response.BusinessDetail;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.util.AuthInfo;
import org.uddi4j.response.AuthToken;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class SaveBusinessService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(SaveBusinessService.class);

  // private reference to the jUDDI datastore factory
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
    return this.invoke((SaveBusiness)request);
  }

  /**
   *
   */
  public BusinessDetail invoke(SaveBusiness request)
    throws JUDDIException
  {
    Vector businessVector = request.getBusinessEntityVector();
    AuthInfo authInfo     = request.getAuthInfo();


    // 1. Check that an authToken was actually included in the

    //    request. If not then throw an AuthTokenRequiredException.

    // 2. Check that the authToken passed in is a valid one.

    //    If not then throw an AuthTokenRequiredException.

    // 3. Check that the authToken passed in has not yet.

    //    expired. If so then throw an AuthTokenExpiredException.

    Auth.validateAuthToken(authInfo);



    // lookup and validate authentication info (publish only)
    String authorizedName = Auth.getAssociatedUserID(authInfo);

    // first make sure we need to continue with this request
    if ((businessVector == null) || (businessVector.size() == 0))
      return new BusinessDetail();
   
    // perform the requested action
    return save_business(authorizedName,businessVector);
  }

  /**
   *
   */
  public BusinessDetail save_business(String authorizedName,Vector businessVector)
    throws JUDDIException
  {
    // aquire a jUDDI datastore instance
    DataStore datastore = factory.aquireDataStore();
    
    try
    {
      datastore.beginTrans();

      for (int i=0; i<businessVector.size(); i++)
      {
        // move the BusinessEntity into a form we can work with easily
        BusinessEntity business = (BusinessEntity)businessVector.elementAt(i);
        String businessKey = business.getBusinessKey();

        // If the BusinessEntity doesn't have a BusinessKey 
        // already then we assume that it's a brand new 
        // BusinessEntity and we'll generate a new key.
        if ((businessKey != null) && (businessKey.length() > 0))
        {
          // check that this business entity really exists. 
          // If not then throw an InvalidKeyPassedException.
          if (!datastore.isValidBusinessKey(businessKey))
            throw new InvalidKeyPassedException("BusinessKey: "+businessKey);

          // check to make sure that 'authorizedName' controls this
          // business entity. If not then throw a UserMismatchException.
          if (!datastore.isBusinessOwner(businessKey,authorizedName))
            throw new UserMismatchException("BusinessKey: "+businessKey);

          // business exists and we control it so let's delete it.
          datastore.deleteBusiness(businessKey);
        }
        else // no key so generate a new one.
          business.setBusinessKey(UUID.nextID());

        // everything checks out so let's store
        // 'authorizedName' and 'operator' values
        // in each BusinessEntity and save it.
        business.setAuthorizedName(authorizedName);
        business.setOperator(operator);
        datastore.saveBusiness(business);
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

    // create a new BusinessDetail and stuff the
    // original but 'updated' businessVector in.
    BusinessDetail detail = new BusinessDetail();
    detail.setBusinessEntityVector(businessVector);
    return detail;
  }
  
  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
    throws JUDDIException
  {
    // initialize all jUDDI Subsystems
    psl.discus.javasrc.uddi.util.SysManager.startup();

    // generate an AuthToken
    GetAuthToken authTokenRequest = new GetAuthToken("sviens","password");
    GetAuthTokenService authTokenService = new GetAuthTokenService();
    AuthToken authToken = (AuthToken)authTokenService.invoke(authTokenRequest);
    String authInfo = authToken.getAuthInfoString();

    // generate a Name Vector
    Vector nameVector = new Vector();
    nameVector.add(new Name("Met Life Insurance"));
    nameVector.add(new Name("Fidelity Investments"));

    // generate a BusinessEntity
    BusinessEntity businessEntity = new BusinessEntity();
    businessEntity.setBusinessKey(UUID.nextID());
    businessEntity.setAuthorizedName("sviens");
    businessEntity.setOperator("SteveViens.com");
    businessEntity.setNameVector(nameVector);

    // generate a BusinessEntity Vector
    Vector businessEntityVector = new Vector();
    businessEntityVector.add(businessEntity);

    // create a request
    SaveBusiness request = new SaveBusiness();
    request.setAuthInfo(authInfo);
    request.setBusinessEntityVector(businessEntityVector);


    try
    {
      // invoke the service
      SaveBusinessService saveBizSvc = new SaveBusinessService();
      BusinessDetail response = (BusinessDetail)saveBizSvc.invoke(request);
      // to-do ... need more here!
    }
    catch(JUDDIException juddiex)
    {
      System.out.println(juddiex.toString());
      juddiex.printStackTrace();
    }
    finally
    {
      // terminate all jUDDI Subsystems
      psl.discus.javasrc.uddi.util.SysManager.shutdown();
    }
  }
}

