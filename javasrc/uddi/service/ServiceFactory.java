/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.service;

import psl.discus.javasrc.uddi.error.JUDDIException;
import psl.discus.javasrc.uddi.util.Config;
import psl.discus.javasrc.uddi.util.SysManager;

import org.uddi4j.request.GetAuthToken;
import org.uddi4j.response.AuthToken;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.apache.log4j.Logger;

import java.util.Hashtable;

/**
 * Implementation of Factory pattern used to create an implementation of
 * the psl.discus.javasrc.uddi.auth.Authenticator interface.
 *
 * The name of the Authenticator implementation to create is passed to the
 * getAuthenticator method.  If a null value is passed then the default
 * Authenticator implementation "psl.discus.javasrc.uddi.auth.SimpleAuthenticator" is
 * created.
 *
 * @author  Steve Viens
 * @version 0.6
 */
public abstract class ServiceFactory 
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(ServiceFactory.class);

  // In memory database of UDDI element names to jUDDI class mappings
  // and UDDI4j request class names to jUDDI Class name mappings.
  private static Hashtable serviceTable= new Hashtable();

  static
  {
    // IMPORTANT: We change the case of the value used to lookup values in this
    // table to lower case before performing a 'get' on the hashtable so we can
    // be sure that we return the correct class name regardless of the case of
    // the value passed in. - Steve

    // use to obtain the service class via the UDDI element name (read IMPORTANT above regarding case)
    serviceTable.put("add_publisherassertions",                       "psl.discus.javasrc.uddi.service.AddPublisherAssertionsService");
    serviceTable.put("delete_binding",                                "psl.discus.javasrc.uddi.service.DeleteBindingService");
    serviceTable.put("delete_business",                               "psl.discus.javasrc.uddi.service.DeleteBusinessService");
    serviceTable.put("delete_publisherassertions",                    "psl.discus.javasrc.uddi.service.DeletePublisherAssertionsService");
    serviceTable.put("delete_service",                                "psl.discus.javasrc.uddi.service.DeleteServiceService");
    serviceTable.put("delete_tmodel",                                 "psl.discus.javasrc.uddi.service.DeleteTModelService");
    serviceTable.put("discard_authtoken",                             "psl.discus.javasrc.uddi.service.DiscardAuthTokenService");
    serviceTable.put("find_binding",                                  "psl.discus.javasrc.uddi.service.FindBindingService");
    serviceTable.put("find_business",                                 "psl.discus.javasrc.uddi.service.FindBusinessService");
    serviceTable.put("find_relatedbusinesses",                        "psl.discus.javasrc.uddi.service.FindRelatedBusinessesService");
    serviceTable.put("find_service",                                  "psl.discus.javasrc.uddi.service.FindServiceService");
    serviceTable.put("find_tmodel",                                   "psl.discus.javasrc.uddi.service.FindTModelService");
    serviceTable.put("get_assertionstatusreport",                     "psl.discus.javasrc.uddi.service.GetAssertionStatusReportService");
    serviceTable.put("get_authtoken",                                 "psl.discus.javasrc.uddi.service.GetAuthTokenService");
    serviceTable.put("get_bindingdetail",                             "psl.discus.javasrc.uddi.service.GetBindingDetailService");
    serviceTable.put("get_businessdetailext",                         "psl.discus.javasrc.uddi.service.GetBusinessDetailExtService");
    serviceTable.put("get_businessdetail",                            "psl.discus.javasrc.uddi.service.GetBusinessDetailService");
    serviceTable.put("get_publisherassertions",                       "psl.discus.javasrc.uddi.service.GetPublisherAssertionsService");
    serviceTable.put("get_registeredinfo",                            "psl.discus.javasrc.uddi.service.GetRegisteredInfoService");
    serviceTable.put("get_servicedetail",                             "psl.discus.javasrc.uddi.service.GetServiceDetailService");
    serviceTable.put("get_tmodeldetail",                              "psl.discus.javasrc.uddi.service.GetTModelDetailService");
    serviceTable.put("save_binding",                                  "psl.discus.javasrc.uddi.service.SaveBindingService");
    serviceTable.put("save_business",                                 "psl.discus.javasrc.uddi.service.SaveBusinessService");
    serviceTable.put("save_service",                                  "psl.discus.javasrc.uddi.service.SaveServiceService");
    serviceTable.put("save_tmodel",                                   "psl.discus.javasrc.uddi.service.SaveTModelService");
    serviceTable.put("set_publisherassertions",                       "psl.discus.javasrc.uddi.service.SetPublisherAssertionsService");
    serviceTable.put("validate_values",                               "psl.discus.javasrc.uddi.service.ValidateValuesService");

    // use to obtain the service class via the UDDI request class name (read IMPORTANT above regarding case)
    serviceTable.put("org.uddi4j.request.addpublisherassertions",     "psl.discus.javasrc.uddi.service.AddPublisherAssertionsService");
    serviceTable.put("org.uddi4j.request.deletebinding",              "psl.discus.javasrc.uddi.service.DeleteBindingService");
    serviceTable.put("org.uddi4j.request.deletebusiness",             "psl.discus.javasrc.uddi.service.DeleteBusinessService");
    serviceTable.put("org.uddi4j.request.deletepublisherassertions",  "psl.discus.javasrc.uddi.service.DeletePublisherAssertionsService");
    serviceTable.put("org.uddi4j.request.deleteservice",              "psl.discus.javasrc.uddi.service.DeleteServiceService");
    serviceTable.put("org.uddi4j.request.deletetmodel",               "psl.discus.javasrc.uddi.service.DeleteTModelService");
    serviceTable.put("org.uddi4j.request.discardauthtoken",           "psl.discus.javasrc.uddi.service.DiscardAuthTokenService");
    serviceTable.put("org.uddi4j.request.findbinding",                "psl.discus.javasrc.uddi.service.FindBindingService");
    serviceTable.put("org.uddi4j.request.findbusiness",               "psl.discus.javasrc.uddi.service.FindBusinessService");
    serviceTable.put("org.uddi4j.request.findrelatedbusinesses",      "psl.discus.javasrc.uddi.service.FindRelatedBusinessesService");
    serviceTable.put("org.uddi4j.request.findservice",                "psl.discus.javasrc.uddi.service.FindServiceService");
    serviceTable.put("org.uddi4j.request.findtmodel",                 "psl.discus.javasrc.uddi.service.FindTModelService");
    serviceTable.put("org.uddi4j.request.getassertionstatusreport",   "psl.discus.javasrc.uddi.service.GetAssertionStatusReportService");
    serviceTable.put("org.uddi4j.request.getauthtoken",               "psl.discus.javasrc.uddi.service.GetAuthTokenService");
    serviceTable.put("org.uddi4j.request.getbindingdetail",           "psl.discus.javasrc.uddi.service.GetBindingDetailService");
    serviceTable.put("org.uddi4j.request.getbusinessdetailext",       "psl.discus.javasrc.uddi.service.GetBusinessDetailExtService");
    serviceTable.put("org.uddi4j.request.getbusinessdetail",          "psl.discus.javasrc.uddi.service.GetBusinessDetailService");
    serviceTable.put("org.uddi4j.request.getpublisherassertions",     "psl.discus.javasrc.uddi.service.GetPublisherAssertionsService");
    serviceTable.put("org.uddi4j.request.getregisteredInfo",          "psl.discus.javasrc.uddi.service.GetRegisteredInfoService");
    serviceTable.put("org.uddi4j.request.getservicedetail",           "psl.discus.javasrc.uddi.service.GetServiceDetailService");
    serviceTable.put("org.uddi4j.request.gettmodeldetail",            "psl.discus.javasrc.uddi.service.GetTModelDetailService");
    serviceTable.put("org.uddi4j.request.savebinding",                "psl.discus.javasrc.uddi.service.SaveBindingService");
    serviceTable.put("org.uddi4j.request.savebusiness",               "psl.discus.javasrc.uddi.service.SaveBusinessService");
    serviceTable.put("org.uddi4j.request.saveservice",                "psl.discus.javasrc.uddi.service.SaveServiceService");
    serviceTable.put("org.uddi4j.request.savetmodel",                 "psl.discus.javasrc.uddi.service.SaveTModelService");
    serviceTable.put("org.uddi4j.request.setpublisherassertions",     "psl.discus.javasrc.uddi.service.SetPublisherAssertionsService");
    serviceTable.put("org.uddi4j.request.validatevalues",             "psl.discus.javasrc.uddi.service.ValidateValuesService");
  }

  /**
   *
   */
  public static synchronized UDDIService getService(String serviceName)
    throws JUDDIException
  {
    if (serviceName == null)
    {
      log.error("A null serviceName was passed to the getService method of " +
        "the ServiceFactory");
      throw new JUDDIException();
    }
    else if (serviceName.length() == 0)
    {
      log.error("A zero length serviceName was passed to the getService " +
        "method of the ServiceFactory");
      throw new JUDDIException();
    }

    // look up the name of the psl.discus.javasrc.uddi.UDDIService subclass in the
    // serviceTable hashtable. First we conver the serviceName passed in to
    // lower case so we have a better chance of finding a match - in case the
    // UDDI client used to make the call doesn't pass the request in the
    // correct case. - Steve
    String serviceClassName = (String)serviceTable.get(serviceName.toLowerCase());

    if ((serviceClassName == null) || (serviceClassName.length() == 0))
    {
      log.error("An implementation of the psl.discus.javasrc.uddi.service.UDDIService " +
        "class was not found for the serviceName: " + serviceName);
      throw new JUDDIException();
    }

    return createService(serviceClassName);
  }

  /**
   *
   */
  private static synchronized UDDIService createService(String serviceClassName)
    throws JUDDIException
  {
    try
    {
      Class serviceClass = Class.forName(serviceClassName);

      return (UDDIService)serviceClass.newInstance();
    }
    catch (ClassNotFoundException ex)
    {
      System.out.println("The subclass of psl.discus.javasrc.uddi.service.UDDIService " +
  "class specified was not found in classpath: " + serviceClassName +
  " not found.");
      throw new JUDDIException();
    }
    catch (InstantiationException ex)
    {
      System.out.println("Exception while instantiating the specified " +
        "subclass of psl.discus.javasrc.uddi.service.UDDIService: " + serviceClassName);
      throw new JUDDIException();
    }
    catch (IllegalAccessException ex)
    {
      ex.printStackTrace();
    }

    throw new RuntimeException("Could not create instance of the " +
      "psl.discus.javasrc.uddi.UDDIService subclass specified: " + serviceClassName);
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    try
    {
      // start up the jUDDI sub-systems (logging, authentication, etc.) - Steve
      SysManager.startup();

      // generate the request object
      GetAuthToken request = new GetAuthToken("sviens","password");

      // obtain the service object
      //UDDIService service = ServiceFactory.getService("get_authToken");
      UDDIService service = ServiceFactory.getService(request.getClass().getName());

      // call the service's 'invoke' method (invoke the service)
      AuthToken response = (AuthToken)service.invoke(request);

      // write response to the console
      System.out.println("UDDIService: get_authToken");
      System.out.println(" AuthInfo: "+response.getAuthInfoString());
    }
    catch(JUDDIException juddiex)
    {
      System.out.println(juddiex.toString());
    }
    finally
    {
      // shutdown the jUDDI sub-systems (logging, authentication, etc.). These
      // may have started up background threads or may have some other
      // external resource tied up (ie database connections). - Steve
      SysManager.shutdown();
    }
  }
}