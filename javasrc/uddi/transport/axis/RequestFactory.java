/* * jUDDI - An open source Java implementation of UDDI v2.0 * http://juddi.org/ * * Copyright (c) 2002, Steve Viens and contributors * All rights reserved. */package psl.discus.javasrc.uddi.transport.axis;import psl.discus.javasrc.uddi.error.JUDDIException;import org.apache.log4j.Logger;import org.w3c.dom.Element;import java.util.Hashtable;/** * The RequestFactory's sole responsibility is to transform * incoming requests from a raw XML Element into a Java request * object. * * @author  Steve Viens * @version 0.6 */public class RequestFactory{  // private reference to the jUDDI logger  static Logger log = Logger.getLogger(RequestFactory.class);  // uddi request type registry (note: keys are in lower case)  static Hashtable classTable= new Hashtable();  static  {    // uddi inquiry api    classTable.put("find_binding",              "org.uddi4j.request.FindBinding");    classTable.put("find_business",             "org.uddi4j.request.FindBusiness");    classTable.put("find_relatedbusinesses",    "org.uddi4j.request.FindRelatedBusinesses");    classTable.put("find_service",              "org.uddi4j.request.FindService");    classTable.put("find_tmodel",               "org.uddi4j.request.FindTModel");    classTable.put("get_bindingdetail",         "org.uddi4j.request.GetBindingDetail");    classTable.put("get_businessdetailext",     "org.uddi4j.request.GetBusinessDetailExt");    classTable.put("get_businessdetail",        "org.uddi4j.request.GetBusinessDetail");    classTable.put("get_servicedetail",         "org.uddi4j.request.GetServiceDetail");    classTable.put("get_tmodeldetail",          "org.uddi4j.request.GetTModelDetail");    classTable.put("validate_values",           "org.uddi4j.request.ValidateValues");    // uddi publish api    classTable.put("add_publisherassertions",   "org.uddi4j.request.AddPublisherAssertions");    classTable.put("delete_binding",            "org.uddi4j.request.DeleteBinding");    classTable.put("delete_business",           "org.uddi4j.request.DeleteBusiness");    classTable.put("delete_publisherassertions","org.uddi4j.request.DeletePublisherAssertions");    classTable.put("delete_service",            "org.uddi4j.request.DeleteService");    classTable.put("delete_tmodel",             "org.uddi4j.request.DeleteTModel");    classTable.put("discard_authtoken",         "org.uddi4j.request.DiscardAuthToken");    classTable.put("get_assertionstatusreport", "org.uddi4j.request.GetAssertionStatusReport");    classTable.put("get_authtoken",             "org.uddi4j.request.GetAuthToken");    classTable.put("get_publisherassertions",   "org.uddi4j.request.GetPublisherAssertions");    classTable.put("get_registeredinfo",        "org.uddi4j.request.GetRegisteredInfo");    classTable.put("save_binding",              "org.uddi4j.request.SaveBinding");    classTable.put("save_business",             "org.uddi4j.request.SaveBusiness");    classTable.put("save_service",              "org.uddi4j.request.SaveService");    classTable.put("save_tmodel",               "org.uddi4j.request.SaveTModel");    classTable.put("set_publisherassertions",   "org.uddi4j.request.SetPublisherAssertions");    classTable.put("validate_values",           "org.uddi4j.request.ValidateValues");  }  /**   *   */  public static synchronized Object getRequest(Element requestDOM)    throws JUDDIException  {    String requestName = requestDOM.getLocalName();    if (requestName == null)    {      log.error("A null requestName was passed to the getRequest method of " +        "the PublishRequestFactory");      throw new JUDDIException();    }    else if (requestName.length() == 0)    {      log.error("A zero length requestName was passed to the getRequest " +        "method of the PublishRequestFactory");      throw new JUDDIException();    }    String requestClassName = (String)classTable.get(requestName.toLowerCase());    if ((requestClassName == null) || (requestClassName.length() == 0))    {      log.error("A class was not found for the requestName: " + requestName);      throw new JUDDIException();    }    return createClass(requestClassName,requestDOM);  }  /**   *   */  private static synchronized Object createClass(String requestClassName, Element requestDOM)    throws JUDDIException  {    try    {      Class requestClass = Class.forName(requestClassName);      Class parameters[] = new Class[1];      parameters[0]= Class.forName("org.w3c.dom.Element");      // find a constructor that takes a DOM Element as a parameter      java.lang.reflect.Constructor requestClassConstructor = requestClass.getConstructor(parameters);      Object constructorParam[] = new Object[1];      constructorParam[0]= requestDOM;      return (Object)requestClassConstructor.newInstance(constructorParam);    }    catch (ClassNotFoundException ex)    {      System.out.println("The following specified class was not found in classpath: " + requestClassName);      throw new JUDDIException();    }    catch (InstantiationException ex)    {      System.out.println("Exception while instantiating the specified class: " + requestClassName);      throw new JUDDIException();    }    catch (IllegalAccessException ex)    {      ex.printStackTrace();    }    catch (java.lang.NoSuchMethodException ex)    {      System.out.println("Exception finding necessary constructor for specified class: " + requestClassName);      throw new JUDDIException();    }    catch (java.lang.reflect.InvocationTargetException ex)    {      System.out.println("Exception while instantiating the specified class: " + requestClassName);      throw new JUDDIException();    }    throw new RuntimeException("Could not create instance of " + requestClassName);  }}