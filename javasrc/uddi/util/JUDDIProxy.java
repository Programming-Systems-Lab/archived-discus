/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.util;

import org.uddi4j.client.UDDIProxy;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.Arrays;
import java.util.Properties;
import java.util.Vector;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import org.w3c.dom.Element;

import org.uddi4j.transport.Transport;
import org.uddi4j.transport.TransportException;
import org.uddi4j.transport.TransportFactory;
import org.uddi4j.UDDIElement;
import org.uddi4j.UDDIException;
import org.uddi4j.datatype.Name;
import org.uddi4j.datatype.business.BusinessEntity;
import org.uddi4j.datatype.service.BusinessService;
import org.uddi4j.datatype.tmodel.TModel;
import org.uddi4j.datatype.assertion.PublisherAssertion;
import org.uddi4j.request.AddPublisherAssertions;
import org.uddi4j.request.DeleteBinding;
import org.uddi4j.request.DeleteBusiness;
import org.uddi4j.request.DeletePublisherAssertions;
import org.uddi4j.request.DeleteService;
import org.uddi4j.request.DeleteTModel;
import org.uddi4j.request.DiscardAuthToken;
import org.uddi4j.request.FindBinding;
import org.uddi4j.request.FindBusiness;
import org.uddi4j.request.FindRelatedBusinesses;
import org.uddi4j.request.FindService;
import org.uddi4j.request.FindTModel;
import org.uddi4j.request.GetAssertionStatusReport;
import org.uddi4j.request.GetAuthToken;
import org.uddi4j.request.GetBindingDetail;
import org.uddi4j.request.GetBusinessDetail;
import org.uddi4j.request.GetBusinessDetailExt;
import org.uddi4j.request.GetPublisherAssertions;
import org.uddi4j.request.GetRegisteredInfo;
import org.uddi4j.request.GetServiceDetail;
import org.uddi4j.request.GetTModelDetail;
import org.uddi4j.request.SaveBinding;
import org.uddi4j.request.SaveBusiness;
import org.uddi4j.request.SaveService;
import org.uddi4j.request.SaveTModel;
import org.uddi4j.request.SetPublisherAssertions;
import org.uddi4j.request.ValidateValues;
import org.uddi4j.request.FindBinding;
import org.uddi4j.request.FindService;
import org.uddi4j.response.AuthToken;
import org.uddi4j.response.AssertionStatusReport;
import org.uddi4j.response.BindingDetail;
import org.uddi4j.response.BusinessDetail;
import org.uddi4j.response.BusinessDetailExt;
import org.uddi4j.response.CompletionStatus;
import org.uddi4j.response.BusinessList;
import org.uddi4j.response.DispositionReport;
import org.uddi4j.response.PublisherAssertions;
import org.uddi4j.response.RegisteredInfo;
import org.uddi4j.response.RelatedBusinessesList;
import org.uddi4j.response.ServiceDetail;
import org.uddi4j.response.ServiceList;
import org.uddi4j.response.TModelDetail;
import org.uddi4j.response.TModelList;
import org.uddi4j.util.AuthInfo;
import org.uddi4j.util.CategoryBag;
import org.uddi4j.util.DiscoveryURLs;
import org.uddi4j.util.FindQualifiers;
import org.uddi4j.util.IdentifierBag;
import org.uddi4j.util.TModelBag;
import org.uddi4j.util.UploadRegister;
import org.uddi4j.util.KeyedReference;

/**
 * @author Steve Viens
 * @version 0.6
 */
public class JUDDIProxy extends org.uddi4j.client.UDDIProxy
{
   // Variables
   protected URL inquiryURL = null;
   protected URL publishURL = null;
   TransportFactory transportFactory = null;
   Properties    config     = null;


   /**
    * Default constructor.
    */
   public JUDDIProxy() {
      transportFactory = TransportFactory.newInstance();
   }

   /**
    * Construct a UDDIProxy object.
    *
    * @param inquiryURL URL to be used for inquiry requests.
    * @param publishURL URL to be used for publish requests.
    * @param transport  null indicates standard HTTP transport. Can pass in
    *                   a different transport to use.
    */
   public JUDDIProxy(URL inquiryURL, URL publishURL) {
      this();
      this.inquiryURL = inquiryURL;
      this.publishURL = publishURL;
   }

   /**
    * Construct a UDDIProxy object
    * 
    * @param p      Properties object which contains configuration information for
    *               UDDI4J. This properties object overrides settings normally retrieved
    *               from system properties. 
    *               <P>
    *               The exception to this is org.uddi4j.logEnabled in which the system 
    *               properties take precedence. This enables users to turn debug on and 
    *               off at runtime.
    *               <P>
    *               This object may also specify properties affecting UDDIProxy.
    *               Current settings affected by this properties object include:
    *               <DL>
<<<<<<< JUDDIProxy.java
=======
    *               <DT>org.uddi4j.logEnabled
    *               <DD>Determines if debug information is generated. Property value
    *               is "true", request and response messages are logged. The SOAP
    *               transport class does the actual logging.
>>>>>>> 1.4
    *               <DT>org.uddi4j.TransportClassName
    *               <DD>Specifies the name of the SOAP transport support class.
    *               Options include, org.uddi4j.transport.ApacheSOAPTransport,
    *               org.uddi4j.transport.ApacheAxisTransport,
    *               org.uddi4j.transport.HPSOAPTransport.
    *               <DT>org.uddi4j.inquiryURL
    *               <DD>The URL to be used by uddiproxy for inquiry requests.
    *               setInquiryURL method overrides.
    *               <DT>org.uddi4j.publishURL
    *               <DD>The URL to be used by uddiproxy for publish requests.
    *               setPublishURL methods override.
    *               </DL>
    * @exception java.net.MalformedURLException In case the publish or
    *            inquiry URLs are malformed.
    */
   public JUDDIProxy(Properties p) throws MalformedURLException {
      setConfiguration(p);
   }

   /**
    * Set the configuration properties
    * 
    * @param p Properties object containing configuration information. See
    * {@link #JUDDIProxy(Properties)} for configuration details.
    * @exception java.net.MalformedURLException In case the publish or
    *            inquiry URLs are malformed.
    */
   public void setConfiguration(Properties p) throws MalformedURLException {
      // Save the property object within the proxy
      config = p;

      // use it in send to create factory, or create in the proxy
      transportFactory = TransportFactory.newInstance(p);
      if (p.getProperty("org.uddi4j.inquiryURL") != null) {
         setInquiryURL(p.getProperty( "org.uddi4j.inquiryURL"));
      }
      if (p.getProperty("org.uddi4j.publishURL") != null) {
         setPublishURL(p.getProperty("org.uddi4j.publishURL"));
      }
   }

   /**
    * Get the configuration properties
    * @return The configuration properties
    */
   public Properties getConfiguration() {
      return config;
   }

   /**
    * Set the URL to be used for inquiry requests.
    * 
    * @param url Inquiry URL string
    * @exception java.net.MalformedURLException In case the Inquiry URL is
    * malformed.
    */
   public void setInquiryURL(String url) throws java.net.MalformedURLException {
      this.inquiryURL = new java.net.URL(url);
   }

   /**
    * Set the URL to be used for inquiry requests.
    *
    * @param url Inquiry URL
    */
   public void setInquiryURL(URL url) {
      this.inquiryURL = url;
   }

   /**
    * Set the URL to be used for publish requests. If
    * HTTPS is specified as the procotol, it must be added
    * as a supported protocol. For Sun's SSL support, this can be done
    * with the following code fragment:
    * <PRE>
    *    System.setProperty("java.protocol.handler.pkgs",
    *                       "com.sun.net.ssl.internal.www.protocol");
    *    java.security.Security.addProvider(new com.sun.net.ssl.internal.ssl.Provider());
    * </PRE>
    *
    * @param url
    * @exception java.net.MalformedURLException
    *                   Thrown if HTTPS is not registered as a valid URL protocol.
    */
   public void setPublishURL(String url) throws java.net.MalformedURLException {
      this.publishURL = new java.net.URL(url);
   }

   /**
    * Set the URL to be used for publish requests.
    *
    * @param url Publish URL
    */
   public void setPublishURL(URL url) {
      this.publishURL = url;
   }


   /**
    * The find_binding method returns a bindingDetail message that contains
    * a bindingTemplates structure with zero or more bindingTemplate structures
    * matching the criteria specified in the argument list.
    *
    * @param findQualifiers
    *                   This collection of findQualifier elements can be used to alter the default
    *                   behavior of search functionality.
    * @param serviceKey Used to specify a particular instance of a businessService element
    *                   in the registered data.  Only bindings in the specific businessService
    *                   data identified by the serviceKey passed will be searched.
    * @param tModelBag  This is a list of tModel uuid_key values that represent the technical
    *                   fingerprint to locate in a bindingTemplate structure contained within
    *                   the businessService instance specified by the serviceKey value.  If more
    *                   than one tModel key is specified in this structure, only bindingTemplate
    *                   information that exactly matches all of the tModel keys specified will
    *                   be returned (logical AND).  The order of the keys in the tModelBag is
    *                   not relevant.  All tModelKey values begin with a uuid URN qualifier
    *                   (e.g. "uuid:" followed by a known tModel UUID value.
    * @param maxRows    This optional integer value allows the requesting program to limit
    *                   the number of results returned.
    * @return This function returns a bindingDetail message on success.  In the event that no matches were
    * located for the specified criteria, the bindingDetail structure returned in the response the will be
    * empty (e.g. contain no bindingTemplate data.)
    *         In the even of a large number of matches, an Operator Site may truncate the result set.  If
    * this occurs, the response message will contain the truncated attribute with the value of this attribute
    * set to true.
    *         Searching using tModelBag will also return any bindingTemplate information that matches due to
    * hostingRedirector references.  The resolved bindingTemplate structure will be returned, even if that
    * bindingTemplate is owned by a different businessService structure.
    * @exception UDDIException
    * @exception TransportException
    */
   public BindingDetail find_binding(FindQualifiers findQualifiers,
                                     String serviceKey,
                                     TModelBag tModelBag,
                                     int maxRows)
          throws UDDIException, TransportException {
      FindBinding request = new FindBinding();
      request.setFindQualifiers(findQualifiers);
      request.setServiceKey(serviceKey);
      request.setTModelBag(tModelBag);
      if (maxRows>0) request.setMaxRows(maxRows);
      return new BindingDetail(send(request, true));
   }

   /**
    * The find_business message returns a businessList message that matches
    * the conditions specified in the arguments.
    * @param names     vector of Name objects .
    * @param discoveryURLs
    *                  This is a list of URL's to be matched against the data associated
    *                  with the discoveryURL's contents of registered businessEntity information.
    *                  To search for URL without regard to useType attribute values, pass
    *                  the useType component of the discoveryURL elements as empty attributes.
    *                  If useType values are included, then the match will be made only on
    *                  registered information that match both the useType and URL value.
    *                  The returned businessList contains businessInfo structures matching
    *                  any of the URL's passed (logical OR).
    * @param identifierBag
    *                  This is a list of business identifier references. The returned businessList
    *                  contains businessInfo structures matching any of the identifiers passed
    *                  (logical OR).
    * @param categoryBag
    *                   This is a list of category references.  The returned businessList
    *                   contains businessInfo structures matching all of the categories
    *                   passed (logical AND).
    * @param tModelBag  The registered businessEntity data contains bindingTemplates that in turn
    *                   contain specific tModel references.  The tModelBag argument lets you
    *                   search for businesses that have bindings that are compatible with a
    *                   specific tModel pattern.  The returned businessList contains businessInfo
    *                   structures that match all of the tModel keys passed (logical AND).
    *                   tModelKey values must be formatted as URN qualified UUID values
    *                   (e.g. prefixed with "uuid:")
    *
    * @param findQualifiers
    *                can be used to alter the default behavior of search functionality.
    * @param maxRows allows the requesting program to limit the number of results returned.
    * @return This function returns a businessList on success.  In the event that no
    *         matches were located for the specified criteria, a businessList
    *         structure with zero businessInfo structures is returned.
    * @exception UDDIException
    * @exception TransportException
    */
   public BusinessList find_business(Vector names,
                                     DiscoveryURLs discoveryURLs,
                                     IdentifierBag identifierBag,
                                     CategoryBag categoryBag,
                                     TModelBag tModelBag,
                                     FindQualifiers findQualifiers,
                                     int maxRows)
          throws UDDIException, TransportException {
        FindBusiness request = new FindBusiness();
        request.setNameVector(names);
        request.setDiscoveryURLs(discoveryURLs);
        request.setIdentifierBag(identifierBag);
        request.setCategoryBag(categoryBag);
        request.setTModelBag(tModelBag);
        request.setFindQualifiers(findQualifiers);
        if (maxRows>0) request.setMaxRows(maxRows);
        return new BusinessList(send(request, true));
   }


   /**
    * The find_relatedBusinesses API call is used to locate information about businessEntity
    * registrations that are related to a specific business entity whose key is passed in the
    * inquiry. The Related Businesses feature is used to manage registration of business units and
    * subsequently relate them based on organizational hierarchies or business partner relationships.
    *
    * This  returns zero or more relatedBusinessInfo structures .For the businessEntity specified in the
    * the response reports complete business relationships with other businessEntity
    * registrations. Business relationships are complete between two businessEntity registrations when the
    * publishers controlling each of the businessEntity structures involved in the relationship set
    * assertions affirming that relationship.
    *
    * @param businessKey      This is used to specify a particular  BusinessEntity instance.
    *
    * @param keyedReference   This is a single, optional keyedReference element that is used to
    *                         specify that  only businesses that are related to the focal point
    *                         in a specific way should be included in the results.
    * @param findQualifiers
    *                         Can be used to alter the default behavior of search functionality.
    * @return This function returns a RelatedBusinessesList on success.
    * @exception UDDIException
    * @exception TransportException
    */
   public RelatedBusinessesList find_relatedBusinesses(String businessKey,
                                                       KeyedReference keyedReference,
                                                       FindQualifiers findQualifiers)
          throws UDDIException, TransportException {
       FindRelatedBusinesses request = new FindRelatedBusinesses();
       request.setBusinessKey(businessKey);
       request.setKeyedReference(keyedReference);
       request.setFindQualifiers(findQualifiers);
       return new RelatedBusinessesList(send(request, true));

   }

  /**
    * This function returns a serviceList on success.  In the event that no
    * matches were located for the specified criteria, the serviceList
    * structure returned will contain an empty businessServices structure.
    *
    * @param businessKey
    *                 This uuid_key is used to specify a particular  BusinessEntity instance.
    * @param names    This optional Vector of Name objects represents one or more partial names qualified
    *                 with xml:lang attributes.  Any businessService data contained in the specified
    *                 businessEntity with a matching partial name value gets returned. A wildcard character %
    *                 may be used to signify any number of any characters.  Up to 5 name values may be
    *                 specified.  If multiple name values are passed, the match occurs on a logical OR basis
    *                 within any names supplied (e.g. any match on name/language pairs will cause a
    *                 registered service to be included in the final result set).
    * @param categoryBag
    *               : This is a list of category references.  The returned serviceList contains
    *                 businessInfo structures matching all of the categories passed (logical AND by
    *                 default).
    * @param tModelBag
    *                 This is a list of tModel uuid_key values that represent the technical fingerprint of
    *                 a bindingTemplate structure to find. Version 2.0 defines a  way  to  associate
    *                 businessService structures with more than one businessEntity. All bindingTemplate
    *                 structures within any businessService associated with the businessEntity specified by
    *                 the businessKey argument will be searched.  If more than one tModel key is specified
    *                 in this structure, only businessService structures that contain bindingTemplate
    *                 structures with fingerprint information that matches all of the tModel keys specified
    *                 will be returned (logical AND only).
    * @param findQualifiers
    *                 used to alter the default behavior of search functionality.
    * @param maxRows  allows the requesting program to limit the number of results returned.
    * @return         This function returns a serviceList on success.  In the event that no
    *                 matches were located for the specified criteria, the serviceList
    *                 structure returned will contain an empty businessServices structure.
    * @exception UDDIException
    * @exception TransportException
    */
   public ServiceList find_service (String businessKey,
                                    Vector names,
                                    CategoryBag categoryBag,
                                    TModelBag  tModelBag,
                                    FindQualifiers findQualifiers,
                                    int maxRows)
          throws UDDIException, TransportException {
      FindService request = new FindService();
      request.setBusinessKey(businessKey);
      request.setNameVector(names);
      request.setCategoryBag(categoryBag);
      request.setTModelBag(tModelBag);
      request.setFindQualifiers(findQualifiers);
      if (maxRows>0) request.setMaxRows(maxRows);
      return new ServiceList(send(request, true));
   }

  /**
    * This find_tModel message is for locating a list of tModel entries
    * that match a set of specific criteria. The response will be a list
    * of abbreviated information about tModels that match the criteria (tModelList).
    *
    * @param name    This string value  represents a partial name.  Since tModel data only has a single name,
    *                only a single name may be passed.  A wildcard character % may be used to signify any
    *                number of any characters. The returned tModelList contains tModelInfo elements for
    *                tModels whose name matches the value passed (via lexical-order - i.e., leftmost in
    *                left-to-right languages - partial match or wild card treatment).
    *
    * @param categoryBag
    *                This is a list of category references.  The returned tModelList contains tModelInfo
    *                elements matching all of the categories passed (logical AND by default).  FindQualifier
    *                can be used to alter this logical AND behavior.
    * @param identifierBag
    *                This is a list of business identifier references. The returned tModelList
    *                contains tModelInfo structures matching any of the identifiers
    *                passed (logical OR).
    * @param findQualifiers
    *                used to alter the default behavior of search functionality.
    * @param maxRows allows the requesting program to limit the number of results returned.
    * @return This function returns a tModelList on success.  In the event that no
    *         matches were located for the specified criteria, an empty tModelList
    *         object will be returned (e.g. will contain zero tModelInfo objects).
    *         This signifies zero matches.
    * @exception UDDIException
    * @exception TransportException
    */
   public TModelList find_tModel (String name,
                                   CategoryBag categoryBag,
                                   IdentifierBag identifierBag,
                                   FindQualifiers findQualifiers,
                                   int maxRows)
          throws UDDIException, TransportException {
      FindTModel request = new FindTModel();
      request.setName(name);
      request.setCategoryBag(categoryBag);
      request.setIdentifierBag(identifierBag);
      request.setFindQualifiers(findQualifiers);
      if (maxRows>0) request.setMaxRows(maxRows);
      return new TModelList(send(request, true));
   }


   /**
    * The get_bindingDetail message is for requesting the run-time
    * bindingTemplate information location information for the purpose of
    * invoking a registered business API.
    *
    * @param bindingKey uuid_key string that represent specific instance
    *                   of known bindingTemplate data.
    * @return This function returns a bindingDetail message on successful match
    * @exception UDDIException
    * @exception TransportException
    */
   public BindingDetail get_bindingDetail(String bindingKey)
          throws UDDIException, TransportException {
      GetBindingDetail request = new GetBindingDetail();
      Vector keys = new Vector();
      keys.addElement(bindingKey);
      request.setBindingKeyStrings(keys);
      return new BindingDetail(send(request, true));
   }

   /**
    * The get_bindingDetail message is for requesting the run-time
    * bindingTemplate information location information for the purpose of
    * invoking a registered business API.
    *
    * @param bindingKey Vector of uuid_key strings that represent specific instances
    *                   of known bindingTemplate data.
    * @return This function returns a bindingDetail message on successful match of one
    *         or more bindingKey values.  If multiple bindingKey values were passed, the
    *         results will be returned in the same order as the keys passed.
    * @exception UDDIException
    * @exception TransportException
    */
   public BindingDetail get_bindingDetail(Vector bindingKeyStrings)
          throws UDDIException, TransportException {
      GetBindingDetail request = new GetBindingDetail();
      request.setBindingKeyStrings(bindingKeyStrings);
      return new BindingDetail(send(request, true));
   }

   /**
    * The get_businessDetail message returns complete businessEntity information
    * for one or more specified businessEntitys
    *
    * @param businessKey
    *               A uuid_key string that represents a specific instance of known
    *               businessEntity data.
    * @return This function returns a businessDetail object on successful match
    *         of one or more businessKey values.  If multiple businessKey values
    *         were passed, the results will be returned in the same order as the
    *         keys passed.
    * @exception UDDIException
    * @exception TransportException
    */
   public BusinessDetail get_businessDetail(String businessKey)
          throws UDDIException, TransportException {
      GetBusinessDetail request = new GetBusinessDetail();
      Vector keys = new Vector();
      keys.addElement(businessKey);
      request.setBusinessKeyStrings(keys);
      return new BusinessDetail(send(request, true));
   }
   /**
    * The get_businessDetail message returns complete businessEntity information
    * for one or more specified businessEntitys
    *
    * @param businessKeyStrings
    *               Vector of uuid_key strings that represent specific instances of known
    *               businessEntity data.
    * @return This function returns a businessDetail message on successful match
    *         of one or more businessKey values.  If multiple businessKey values
    *         were passed, the results will be returned in the same order as the
    *         keys passed.
    * @exception UDDIException
    * @exception TransportException
    */
   public BusinessDetail get_businessDetail(Vector businessKeyStrings)
          throws UDDIException, TransportException {
      GetBusinessDetail request = new GetBusinessDetail();
      request.setBusinessKeyStrings(businessKeyStrings);
      return new BusinessDetail(send(request, true));
   }

   /**
    * The get_businessDetailExt message returns extended businessEntity
    * information for one or more specified businessEntitys.  This
    * message returns exactly the same information as the get_businessDetail
    * message, but may contain additional attributes if the source is
    * an external registry (not an Operator Site) that is compatible
    * with this API specification.
    *
    * @param businessKey
    *               A uuid_key string that represents a specific instance of known
    *               businessEntity data.
    * @return This function returns a businessDetailExt message on successful match
    *         of one or more businessKey values.  If multiple businessKey values
    *         were passed, the results will be returned in the same order as the
    *         keys passed.
    * @exception UDDIException
    * @exception TransportException
    */
   public BusinessDetailExt get_businessDetailExt(String businessKey)
          throws UDDIException, TransportException {
      GetBusinessDetailExt request = new GetBusinessDetailExt();
      Vector keys = new Vector();
      keys.addElement(businessKey);
      request.setBusinessKeyStrings(keys);
      return new BusinessDetailExt(send(request, true));
   }
   /**
    * The get_businessDetailExt message returns extended businessEntity
    * information for one or more specified businessEntitys.  This
    * message returns exactly the same information as the get_businessDetail
    * message, but may contain additional attributes if the source is
    * an external registry (not an Operator Site) that is compatible
    * with this API specification.
    *
    * @param businessKeyStrings
    *               Vector of uuid_key strings that represent specific instances of known
    *               businessEntity data.
    * @return This function returns a businessDetailExt message on successful match
    *         of one or more businessKey values.  If multiple businessKey values
    *         were passed, the results will be returned in the same order as the
    *         keys passed.
    * @exception UDDIException
    * @exception TransportException
    */
   public BusinessDetailExt get_businessDetailExt(Vector businessKeyStrings)
          throws UDDIException, TransportException {
      GetBusinessDetailExt request = new GetBusinessDetailExt();
      request.setBusinessKeyStrings(businessKeyStrings);
      return new BusinessDetailExt(send(request, true));
   }

   /**
    * The get_serviceDetail message is used to request full information
    * about a known businessService structure.
    *
    * @param serviceKey A uuid_key string that represents a specific instance of
    *                   known businessService data.
    * @return This function returns a serviceDetail message on successful match
    *         of one or more serviceKey values.  If multiple serviceKey values
    *         were passed, the results will be returned in the same order as the
    *         keys passed.
    * @exception UDDIException
    * @exception TransportException
    */
   public ServiceDetail get_serviceDetail(String serviceKey)
          throws UDDIException, TransportException {
      GetServiceDetail request = new GetServiceDetail();
      Vector keys = new Vector();
      keys.addElement(serviceKey);
      request.setServiceKeyStrings(keys);
      return new ServiceDetail(send(request, true));
   }

   /**
    * The get_serviceDetail message is used to request full information
    * about a known businessService structure.
    *
    * @param serviceKeyStrings
    *               A vector of uuid_key strings that represent specific instances of
    *               known businessService data.
    * @return This function returns a serviceDetail message on successful match
    *         of one or more serviceKey values.  If multiple serviceKey values
    *         were passed, the results will be returned in the same order as the
    *         keys passed.
    * @exception UDDIException
    * @exception TransportException
    */
   public ServiceDetail get_serviceDetail(Vector serviceKeyStrings)
          throws UDDIException, TransportException {
      GetServiceDetail request = new GetServiceDetail();
      request.setServiceKeyStrings(serviceKeyStrings);
      return new ServiceDetail(send(request, true));
   }

   /**
    * The get_tModelDetail message is used to request full information
    * about a known tModel structure.
    *
    * @param tModelKey A URN qualified uuid_key string that represent a specific
    *                  instance of known tModel data.  All tModelKey values begin with a
    *                  uuid URN qualifier (e.g. "uuid:" followed by a known tModel UUID value.)
    * @return This function returns a tModelDetail message on successful match
    *         of one or more tModelKey values.  If multiple tModelKey values
    *         were passed, the results will be returned in the same order as
    *         the keys passed.
    * @exception UDDIException
    * @exception TransportException
    */
   public TModelDetail get_tModelDetail(String tModelKey)
          throws UDDIException, TransportException {
      GetTModelDetail request = new GetTModelDetail();
      Vector keys = new Vector();
      keys.addElement(tModelKey);
      request.setTModelKeyStrings(keys);
      return new TModelDetail(send(request, true));
   }

   /**
    * The get_tModelDetail message is used to request full information
    * about a known tModel structure.
    *
    * @param tModelKeyStrings
    *               A Vector of URN qualified uuid_key strings that represent specific
    *               instances of known tModel data.  All tModelKey values begin with a
    *               uuid URN qualifier (e.g. "uuid:" followed by a known tModel UUID value.)
    * @return This function returns a tModelDetail message on successful match
    *         of one or more tModelKey values.  If multiple tModelKey values
    *         were passed, the results will be returned in the same order as
    *         the keys passed.
    * @exception UDDIException
    * @exception TransportException
    */
   public TModelDetail get_tModelDetail(Vector tModelKeyStrings)
          throws UDDIException, TransportException {
      GetTModelDetail request = new GetTModelDetail();
      request.setTModelKeyStrings(tModelKeyStrings);
      return new TModelDetail(send(request, true));
   }

   /**
    * The add_publisherAssertions message is used to add relationship assertions to the
    * existing set of assertions.
    *
    * @param authInfo     Contains an authentication token. Authentication tokens are obtained
    *                     using the get_authToken method.
    * @param publisherAssertion    Contains a relationship assertion.
    * @return    This function returns a  DispositionReport with a single success indicator.
    * @exception UDDIException
    * @exception TransportException
    */
   public  DispositionReport add_publisherAssertions (String authInfo,
                                                     PublisherAssertion publisherAssertion)
          throws UDDIException, TransportException {
      Vector pubVector = new Vector();
      pubVector.addElement(publisherAssertion);
      return add_publisherAssertions (authInfo, pubVector);
   }

   /**
    * The add_publisherAssertions message is used to add relationship assertions to the
    * existing set of assertions.
    *
    * @param authInfo     Contains an authentication token. Authentication tokens are obtained
    *                     using the get_authToken method.
    * @param publisherAssertion    Vector of publisherAssertion object. Each publisherAssertion
    *                              contains a relationship assertion.
    * @return   This function returns a  DispositionReport with a single success indicator.
    * @exception UDDIException
    * @exception TransportException
    */
   public  DispositionReport add_publisherAssertions (String authInfo,
                                                     Vector publisherAssertion)
          throws UDDIException, TransportException {
      AddPublisherAssertions request = new  AddPublisherAssertions();
      request.setAuthInfo(authInfo);
      request.setPublisherAssertionVector(publisherAssertion);
      return new DispositionReport(send(request, false));

   }

   /**
     * The get_assertionStatusReport message is used to request a status
     * report containing publisher assertions and status information.  This contains
     * all complete and incomplete  assertions and serves an administrative use including
     * the determination if there are any outstanding, incomplete assertions about relationships
     * involving businesses the publisher account is associated with.
     *
     * @param authInfo    Contains an authentication token. Authentication tokens are obtained
     *                    using the get_authToken method.
     * @param completionStatus    This argument (String) lets the publisher restrict the result set
     *                            to only those relationships that have the status value specified.
     * @return    Upon successful completion, an assertionStatusReport message is returned
     *            containing assertion status information.
     * @exception UDDIException
     * @exception TransportException
     */
      public AssertionStatusReport get_assertionStatusReport(String authInfo,
                                                             String completionStatus)
             throws UDDIException, TransportException {
         return get_assertionStatusReport(authInfo, new CompletionStatus(completionStatus));
      }

   /**
    * The get_assertionStatusReport message is used to request a status
    * report containing publisher assertions and status information. This contains
    * all complete and incomplete  assertions and serves an administrative use including
    * the determination if there are any outstanding, incomplete assertions about relationships
    * involving businesses the publisher account is associated with.
    *
    * @param authInfo    Contains an authentication token. Authentication tokens are obtained
    *                    using the get_authToken method.
    * @param completionStatus   This argument lets the publisher restrict the result set to
    *                           only those relationships that have the status value specified.
    * @return    Upon successful completion, an assertionStatusReport message is returned
    *            containing assertion status information.
    * @exception UDDIException
    * @exception TransportException
    */
   public AssertionStatusReport get_assertionStatusReport(String authInfo,
                                                          CompletionStatus completionStatus)
          throws UDDIException, TransportException {
      GetAssertionStatusReport request = new GetAssertionStatusReport();
      request.setAuthInfo(authInfo);
      request.setCompletionStatus(completionStatus);
      return new AssertionStatusReport(send(request, false));
   }


   /**
    * The get_publisherAssertions message is used to get a list of active
    * publisher assertions that are controlled by an individual publisher account.
    *
    * @param authInfo    Contains an authentication token. Authentication tokens are obtained
    *                    using the get_authToken method.
    * @return This function returns a PublisherAssertions message that contains a
    *         publisherAssertion element for each publisher assertion registered by the
    *         publisher account associated with the authentication information.
    * @exception UDDIException
    * @exception TransportException
    */
   public PublisherAssertions get_publisherAssertions(String authInfo)
          throws UDDIException, TransportException {
      GetPublisherAssertions request = new GetPublisherAssertions();
      request.setAuthInfo(authInfo);
      return new PublisherAssertions(send(request, false));
   }

   /**
    * The delete_binding message causes one or more bindingTemplate to be deleted.
    *
    * @param authInfo   Contains an authentication token. Authentication tokens are obtained
    *                   using the get_authToken method.
    * @param bindingKey A uuid_key value that represents a specific instance of
    *                   known bindingTemplate data.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator.
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport delete_binding(String authInfo,
                                           String bindingKey)
          throws UDDIException, TransportException {
      DeleteBinding request = new DeleteBinding();
      request.setAuthInfo(authInfo);
      Vector keys = new Vector();
      keys.addElement(bindingKey);
      request.setBindingKeyStrings(keys);
      return new DispositionReport(send(request, false));
   }

   /**
    * The delete_binding message causes one or more bindingTemplate to be deleted.
    *
    * @param authInfo Contains an authentication token. Authentication tokens are obtained
    *                 using the get_authToken method.
    * @param bindingKeyStrings
    *                 A vector of uuid_key strings that represents specific instances of
    *                 known bindingTemplate data.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport delete_binding(String authInfo,
                                           Vector bindingKeyStrings)
          throws UDDIException, TransportException {
      DeleteBinding request = new DeleteBinding();
      request.setAuthInfo(authInfo);
      request.setBindingKeyStrings(bindingKeyStrings);
      return new DispositionReport(send(request, false));
   }

   /**
    * The delete_business message is used to remove one or more
    * businessEntity structures.
    *
    * @param authInfo Contains an authentication token. Authentication tokens are obtained
    *                 using the get_authToken method.
    * @param businessKey
    *                 Uuid_key string that represents specific instance of known
    *                 businessEntity data.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport delete_business(String authInfo,
                                            String businessKey)
          throws UDDIException, TransportException {
      DeleteBusiness request = new DeleteBusiness();
      request.setAuthInfo(authInfo);
      Vector keys = new Vector();
      keys.addElement(businessKey);
      request.setBusinessKeyStrings(keys);
      return new DispositionReport(send(request, false));
   }

   /**
    * The delete_business message is used to remove one or more
    * businessEntity structures.
    *
    * @param authInfo Contains an authentication token. Authentication tokens are obtained
    *                 using the get_authToken method.
    * @param businessKeyStrings
    *                 Vector of uuid_key strings that represent specific instances of known
    *                 businessEntity data.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport delete_business(String authInfo,
                                            Vector businessKeyStrings)
          throws UDDIException, TransportException {
      DeleteBusiness request = new DeleteBusiness();
      request.setAuthInfo(authInfo);
      request.setBusinessKeyStrings(businessKeyStrings);
      return new DispositionReport(send(request, false));
   }

   /**
    * The delete_service message is used to remove one or more
    * businessService structures.
    *
    * @param authInfo   Contains an authentication token. Authentication tokens are obtained
    *                   using the get_authToken method.
    * @param serviceKey uuid_key string that represents specific instance of known
    *                   businessService data.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport delete_service(String authInfo,
                                           String serviceKey)
          throws UDDIException, TransportException {
      DeleteService request = new DeleteService();
      request.setAuthInfo(authInfo);
      Vector keys = new Vector();
      keys.addElement(serviceKey);
      request.setServiceKeyStrings(keys);
      return new DispositionReport(send(request, false));
   }

   /**
    * The delete_service message is used to remove one or more
    * businessService structures.
    *
    * @param authInfo Contains an authentication token. Authentication tokens are obtained
    *                 using the get_authToken method.
    * @param serviceKeyStrings
    *                 Vector of uuid_key strings that represent specific instances of known
    *                 businessService data.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport delete_service(String authInfo,
                                           Vector serviceKeyStrings)
          throws UDDIException, TransportException {
      DeleteService request = new DeleteService();
      request.setAuthInfo(authInfo);
      request.setServiceKeyStrings(serviceKeyStrings);
      return new DispositionReport(send(request, false));
   }

   /**
    * The delete_tModel message is used to remove or retire one or more
    * tModel structures.
    *
    * @param authInfo  Contains an authentication token. Authentication tokens are obtained
    *                  using the get_authToken method.
    * @param tModelKey uuid_key string that represents specific instance of known
    *                  tModel data.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport delete_tModel(String authInfo,
                                          String tModelKey)
          throws UDDIException, TransportException {
      DeleteTModel request = new DeleteTModel();
      request.setAuthInfo(authInfo);
      Vector keys = new Vector();
      keys.addElement(tModelKey);
      request.setTModelKeyStrings(keys);
      return new DispositionReport(send(request, false));
   }

   /**
    * The delete_tModel message is used to remove or retire one or more
    * tModel structures.
    *
    * @param authInfo Contains an authentication token. Authentication tokens are obtained
    *                 using the get_authToken method.
    * @param tModelKeyStrings
    *                 Vector of uuid_key strings that represent specific instances of known
    *                 tModel data.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport delete_tModel(String authInfo,
                                          Vector tModelKeyStrings)
          throws UDDIException, TransportException {
      DeleteTModel request = new DeleteTModel();
      request.setAuthInfo(authInfo);
      request.setTModelKeyStrings(tModelKeyStrings);
      return new DispositionReport(send(request, false));
   }

   /**
    * The delete_publisherAssertions message is used to delete specific publisher assertions
    * from the assertion collection controlled by a particular publisher account. Deleting assertions from
    * the assertion collection will affect the visibility of business relationships.  Deleting an assertion
    * will cause any relationships based on that assertion to be invalidated.
    *
    * @param authInfo    Contains an authentication token. Authentication tokens are obtained
    *                    using the get_authToken method.
    * @param publisherAssertion   Contains a relationship assertion.
    *
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport delete_publisherAssertions(String authInfo,
                                                        PublisherAssertion publisherAssertion)
          throws UDDIException, TransportException {
      Vector pubVector = new Vector();
      pubVector.addElement(publisherAssertion);
      return delete_publisherAssertions(authInfo, pubVector);
   }

   /**
    * The delete_publisherAssertions message is used to delete specific publisher assertions
    * from the assertion collection controlled by a particular publisher account. Deleting assertions from
    * the assertion collection will affect the visibility of business relationships.  Deleting an assertion
    * will cause any relationships based on that assertion to be invalidated.
    *
    * @param authInfo    Contains an authentication token. Authentication tokens are obtained
    *                    using the get_authToken method.
    * @param publisherAssertion   Is a vector of publisherAssertion values. Each publisherAssertion
    *                             contains a relationship assertion.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport delete_publisherAssertions(String authInfo,
                                                       Vector publisherAssertion)
          throws UDDIException, TransportException {
      DeletePublisherAssertions request = new DeletePublisherAssertions();
      request.setAuthInfo(authInfo);
      request.setPublisherAssertionVector(publisherAssertion);
      return new DispositionReport(send(request, false));
   }

   /**
    * The discard_authToken message is used to inform an Operator Site that the
    * authentication token can be discarded.  Subsequent calls that use the
    * same authToken may be rejected.  This message is optional for Operator
    * Sites that do not manage session state or that do not support the
    * get_authToken message.
    *
    * @param authInfo Contains an String authentication token. Authentication tokens are obtained
    *                 using the get_authToken method.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator.  Discarding an expired authToken will be
    *         processed and reported as a success condition.
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport discard_authToken(String authInfo)
          throws UDDIException, TransportException {
      DiscardAuthToken request = new DiscardAuthToken();
      request.setAuthInfo(authInfo);
      return new DispositionReport(send(request, false));
   }
   /**
    * The discard_authToken message is used to inform an Operator Site that the
    * authentication token can be discarded.  Subsequent calls that use the
    * same authToken may be rejected.  This message is optional for Operator
    * Sites that do not manage session state or that do not support the
    * get_authToken message.
    *
    * @param authInfo Contains an authentication token. Authentication tokens are obtained
    *                 using the get_authToken method.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator.  Discarding an expired authToken will be
    *         processed and reported as a success condition.
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport discard_authToken(AuthInfo authInfo)
          throws UDDIException, TransportException {
      DiscardAuthToken request = new DiscardAuthToken();
      request.setAuthInfo(authInfo);
      return new DispositionReport(send(request, false));
   }

   /**
    * The get_authToken message is used to obtain an authentication token.
    * Authentication tokens are opaque values that are required for all
    * other publisher API calls.  This message is not required for Operator
    * Sites that have an external mechanism defined for users to get an
    * authentication token.  This API is provided for implementations that
    * do not have some other method of obtaining an authentication token or
    * certificate, or that choose to use userID and Password based authentication.
    *
    * @param userid user that an individual authorized user was assigned by an Operator Site.
    *               Operator Sites will each provide a way for individuals to obtain a UserID
    *               and password that will be valid only at the given Operator Site.
    * @param cred   password or credential that is associated with the user.
    * @return This function returns an authToken object that contains a valid
    *         authInfo object that can be used in subsequent calls to publisher
    *         API calls that require an authInfo value.
    * @exception UDDIException
    * @exception TransportException
    */
   public AuthToken get_authToken(String userid,
                                  String cred)
          throws UDDIException, TransportException {
      GetAuthToken request = new GetAuthToken();
      request.setUserID(userid);
      request.setCred(cred);
      return new AuthToken(send(request, false));
   }


   /**
    * The get_registeredInfo message is used to get an abbreviated list
    * of all businessEntity keys and tModel keys that are controlled by
    * the individual associated the credentials passed.
    *
    * @param authInfo Contains an authentication token.  Authentication tokens are obtained
    *                 using the get_authToken API call.
    * @return Upon successful completion, a registeredInfo object will be returned,
    *         listing abbreviated business information in one or more businessInfo
    *         objects, and tModel information in one or more tModelInfo objects.
    *         This API is useful for determining the full extent of registered
    *         information controlled by a single user in a single call.
    * @exception UDDIException
    * @exception TransportException
    */
   public RegisteredInfo get_registeredInfo(String authInfo)
          throws UDDIException, TransportException {
      GetRegisteredInfo request = new GetRegisteredInfo();
      request.setAuthInfo(authInfo);
      return new RegisteredInfo(send(request, false));
   }

   /**
    * The save_binding message is used to save or update a complete
    * bindingTemplate structure.  This message can be used to add or
    * update one or more bindingTemplate structures to one or more existing
    * businessService structures.
    *
    * @param authInfo Contains an authentication token. Authentication tokens are obtained
    *                 using the get_authToken method.
    * @param bindingTemplates
    *                 Vector of bindingTemplate objects.  The order in which these are
    *                 processed is not defined.  To save a new bindingTemplate, pass a
    *                 bindingTemplate object with an empty bindingKey attribute value.
    * @return This API returns a bindingDetail object containing the final results
    *         of the call that reflects the newly registered information for the
    *         effected bindingTemplate objects.
    * @exception UDDIException
    * @exception TransportException
    */
   public BindingDetail save_binding(String authInfo,
                                     Vector bindingTemplates)
          throws UDDIException, TransportException {
      SaveBinding request = new SaveBinding();
      request.setAuthInfo(authInfo);
      request.setBindingTemplateVector(bindingTemplates);
      return new BindingDetail(send(request, false));
   }

   /**
    * The save_business message is used to save or update information about a
    * complete businessEntity structure.  This API has the broadest scope of
    * all of the save_x API calls in the publisher API, and can be used to make
    * sweeping changes to the published information for one or more
    * businessEntity structures controlled by an individual.
    *
    * @param authInfo Contains an authentication token. Authentication tokens are obtained
    *                 using the get_authToken method.
    * @param businessEntities
    *                 Vector of businessEntity objects.  These objects can be obtained in advance
    *                 by using the get_businessDetail API call or by any other means.
    * @return This API returns a businessDetail message containing the final results
    *         of the call that reflects the new registered information for the
    *         businessEntity information provided.
    * @exception UDDIException
    * @exception TransportException
    */
   public BusinessDetail save_business(String authInfo,
                                       Vector businessEntities)
          throws UDDIException, TransportException {
      SaveBusiness request = new SaveBusiness();
      request.setAuthInfo(authInfo);
      request.setBusinessEntityVector(businessEntities);
      return new BusinessDetail(send(request, false));
   }

   /**
    * The save_service message adds or updates one or more businessService
    * structures.
    *
    * @param authInfo Contains an authentication token. Authentication tokens are obtained
    *                 using the get_authToken method.
    * @param businessServices
    *                 Vector of businessService objects.  These objects can be obtained in
    *                 advance by using the get_serviceDetail API call or by any other means.
    * @return This API returns a serviceDetail object containing the final results
    *         of the call that reflects the newly registered information for the
    *         effected businessService structures.
    * @exception UDDIException
    * @exception TransportException
    */
   public ServiceDetail save_service(String authInfo,
                                     Vector businessServices)
          throws UDDIException, TransportException {
      SaveService request = new SaveService();
      request.setAuthInfo(authInfo);
      request.setBusinessServiceVector(businessServices);
      return new ServiceDetail(send(request, false));
   }

   /**
    * The save_tModel message adds or updates one or more tModel structures.
    *
    * @param authInfo Contains an authentication token. Authentication tokens are obtained
    *                 using the get_authToken method.
    * @param tModels  Vector of complete tModel structures.  If adding a new tModel,
    *                 the tModelKey value should be passed as an empty element.
    * @return This API returns a tModelDetail message containing the final results
    *         of the call that reflects the new registered information for the
    *         effected tModel structures
    * @exception UDDIException
    * @exception TransportException
    */
   public TModelDetail save_tModel(String authInfo,
                                   Vector tModels)
          throws UDDIException, TransportException {
      SaveTModel request = new SaveTModel();
      request.setAuthInfo(authInfo);
      request.setTModelVector(tModels);
      return new TModelDetail(send(request, false));
   }

   /**
    * The set_publisherAssertions message is used to save the complete set of publisher
    * assertions for an individual publisher account. When this message is processed, the publisher
    * assertions that are active prior to this API call for a given publisher account are examined by the
    * UDDI registry. Any new assertions not present prior to the call are added to the assertions attributed
    * to the publisher. As a result, new relationships may be activated (e.g. determined to have a completed
    * status), and existing relationships may be deactivated.
    *
    * @param authInfo    Contains an authentication token. Authentication tokens are obtained
    *                    using the get_authToken method.
    * @param pub     Contains a relationship assertion.
    * @return   Upon successful completion, a publisherAssertions message is returned containing
    *           all of the relationship assertions currently attributed to the publisher account
    *           that is associated with the authInfo data passed.
    * @exception UDDIException
    * @exception TransportException
    */
   public PublisherAssertions set_publisherAssertions (String  authInfo,
                                                       PublisherAssertion  pub)
          throws UDDIException, TransportException {
      Vector pubVector = new Vector();
      pubVector.addElement(pub);
      return set_publisherAssertions(authInfo,pubVector);
    }

   /**
    * The set_publisherAssertions message is used to save the complete set of publisher
    * assertions for an individual publisher account. When this message is processed, the publisher
    * assertions that are active prior to this API call for a given publisher account are examined by the
    * UDDI registry. Any new assertions not present prior to the call are added to the assertions attributed
    * to the publisher. As a result, new relationships may be activated (e.g. determined to have a completed
    * status), and existing relationships may be deactivated.
    *
    * @param authInfo    Contains an authentication token. Authentication tokens are obtained
    *                    using the get_authToken method.
    * @param publisherAssertion    Is a vector of publisherAssertion object. Each publisherAssertion
    *                              contains a relationship assertion.
    * @return   Upon successful completion, a publisherAssertions message is returned containing
    *           all of the relationship assertions currently attributed to the publisher account
    *           that is associated with the authInfo data passed.
    * @exception UDDIException
    * @exception TransportException
    */
   public  PublisherAssertions set_publisherAssertions (String  authInfo,
                                                        Vector publisherAssertion)
           throws UDDIException, TransportException {
       SetPublisherAssertions request = new SetPublisherAssertions();
       request.setAuthInfo(authInfo);
       request.setPublisherAssertionVector(publisherAssertion);
       return new PublisherAssertions(send(request, false));
   }

   /**
    * A UDDI operator sends the validate_values message to the appropriate external service, whenever a
    * publisher saves data that uses a categorization value or identifier whose use is regulated by the
    * external party who controls that service. The normal use is to verify that specific categories or
    * identifiers (checking the keyValue attribute values supplied) exist within the given taxonomy or
    * identifier system
    *
    * @param businessEntity
    *                  The vector of businessEntity structure being validated.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator.
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport validate_values_businessEntity(Vector businessEntity)
          throws UDDIException, TransportException {
       ValidateValues request = new ValidateValues();
       request.setBusinessEntityVector(businessEntity);
       return new DispositionReport(send(request, true));
   }

   /**
    * A UDDI operator sends the validate_values message to the appropriate external service, whenever a
    * publisher saves data that uses a categorization value or identifier whose use is regulated by the
    * external party who controls that service. The normal use is to verify that specific categories or
    * identifiers (checking the keyValue attribute values supplied) exist within the given taxonomy or
    * identifier system
    *
    * @param businessService
    *                  The vector of business service structure being validated.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator.
    * @exception UDDIException
    * @exception TransportException
    */
   public DispositionReport validate_values_businessService(Vector businessService)
             throws UDDIException, TransportException {
          ValidateValues request = new ValidateValues();
          request.setBusinessServiceVector(businessService);
          return new DispositionReport(send(request, true));
   }

   /**
    * A UDDI operator sends the validate_values message to the appropriate external service, whenever a
    * publisher saves data that uses a categorization value or identifier whose use is regulated by the
    * external party who controls that service. The normal use is to verify that specific categories or
    * identifiers (checking the keyValue attribute values supplied) exist within the given taxonomy or
    * identifier system
    *
    * @param tModel    The vector of tModel structure being validated.
    * @return Upon successful completion, a dispositionReport is returned with a
    *         single success indicator.
    * @exception UDDIException
    * @exception TransportException
    */

   public DispositionReport validate_values_tModel(Vector tModel)
             throws UDDIException, TransportException {
          ValidateValues request = new ValidateValues();
          request.setTModelVector(tModel);
          return new DispositionReport(send(request, true));
   }


   /**
    * Sends a UDDIElement to either the inquiry or publish URL.
    *
    * @param el
    * @param inquiry
    * @return An element representing a XML DOM tree containing the UDDI response.
    * @exception TransportException
    */
   public Element send(UDDIElement el, boolean inquiry) throws TransportException {

      Element result = null;

      if (inquiry) {
         result = transportFactory.getTransport().send(el, inquiryURL);
      } else {
         result = transportFactory.getTransport().send(el, publishURL);
      }
      return result;
   }

   /**
    * Sends an XML DOM tree indentified by the given element to either the
    * inquiry or publish URL. Can be used to send an manually constructed
    * message to the UDDI registry.
    *
    * @param el
    * @param inquiry
    * @return An element representing a XML DOM tree containing the UDDI response.
    * @exception TransportException
    */
   public Element send(Element el, boolean inquiry) throws TransportException {

      Element result = null;

      if (inquiry) {
         result = transportFactory.getTransport().send(el, inquiryURL);
      } else {
         result = transportFactory.getTransport().send(el, publishURL);
      }
      return result;
   }
}