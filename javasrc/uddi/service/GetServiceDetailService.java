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
import org.uddi4j.datatype.service.BusinessService;
import org.uddi4j.datatype.binding.*;
import org.uddi4j.request.GetServiceDetail;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.response.ServiceDetail;
import org.apache.log4j.Logger;

import java.util.Vector;
import java.util.Properties;
import java.io.InputStream;

import psl.discus.javasrc.shared.DiscusProperties;


/**
 * MODIFIED by matias
 * The get_serviceDetail method always returns a fixed ServiceDetail object that has
 * the access point for the gatekeeper.
 *
 * @author  Steve Viens
 * @version 0.6
 */
public class GetServiceDetailService extends UDDIService {
    // private reference to the jUDDI logger
    private static Logger log = Logger.getLogger(GetServiceDetailService.class);

    // private reference to the jUDDI logger
    private static DataStoreFactory factory = DataStoreFactory.getInstance();

    // ADDED by matias
    /**
     * We only use one service detail, the one for the gatekeeper (since all service calls
     * must be made through the gatekeeper)
     */
    private static ServiceDetail gatekeeperServiceDetail;

    static {
        String gatekeeperAccessPoint = DiscusProperties.getProperty("gatekeeper.accesspoint");
        if (gatekeeperAccessPoint == null) {
            log.warn("WARNING: gatekeeper.accesspoint not found in properties!");
            gatekeeperAccessPoint = "";
        }

        // set up service detail, from the bottom up: binding template, business service, service detail

        TModelInstanceDetails tModelInstanceDetails = new TModelInstanceDetails();
        BindingTemplate template = new BindingTemplate(GATEKEEPER_BINDING_KEY,tModelInstanceDetails);
        AccessPoint accessPoint = new AccessPoint(gatekeeperAccessPoint,"http");
        template.setAccessPoint(accessPoint);

        BindingTemplates bts = new BindingTemplates();
        Vector templates = new Vector();
        templates.add(template);
        bts.setBindingTemplateVector(templates);

        BusinessService businessService = new BusinessService(GATEKEEPER_KEY);
        businessService.setBindingTemplates(bts);
        Vector businessServices = new Vector();
        businessServices.add(businessService);

        gatekeeperServiceDetail = new ServiceDetail("DISCUS");  // DISCUS == operator
        gatekeeperServiceDetail.setBusinessServiceVector(businessServices);

    }


    /**
     *
     */
    public UDDIElement invoke(UDDIElement request)
            throws JUDDIException {
        // morph UDDIElement into a specific request object
        return this.invoke((GetServiceDetail) request);
    }

    /**
     *
     */
    public ServiceDetail invoke(GetServiceDetail request)
            throws JUDDIException {
        Vector serviceKeys = request.getServiceKeyStrings();

        // perform the requested action
        return get_serviceDetail(serviceKeys);
    }

    /**
     * ADDED by matias: a new get_serviceDetail method, that is hard-coded to always return
     * the access point for our gatekeeper
     */
    public ServiceDetail get_serviceDetail(Vector serviceKeyVector)
      throws JUDDIException
    {
        return gatekeeperServiceDetail;
    }

    /**
     * REMOVED: the old get_serviceDetail method
     */
    /*
    public ServiceDetail get_serviceDetail(Vector serviceKeyVector)
      throws JUDDIException
    {
      Vector serviceVector = new Vector();

      // aquire a jUDDI datastore instance
      DataStore datastore = factory.aquireDataStore();

      try
      {
        datastore.beginTrans();

        // Check that every key in the vector really exists.
        // If 'any one' of the keys do not exist then throw
        // an InvalidKeyPassedException.

        for (int i=0; i<serviceKeyVector.size(); i++)
        {
          // grab the next key from the vector
          String serviceKey = (String)serviceKeyVector.elementAt(i);

          // check that this business service really exists.
          // If not then throw an InvalidKeyPassedException.
          if ((serviceKey == null) || (serviceKey.length() == 0) ||
              (!datastore.isValidServiceKey(serviceKey)))
            throw new InvalidKeyPassedException("ServiceKey: "+serviceKey);

          // service exists so let's fetch'n store it
          serviceVector.add(datastore.fetchService(serviceKey));
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

      // create a new ServiceDetail and
      // stuff the serviceVector into it.
      ServiceDetail detail = new ServiceDetail();
      detail.setBusinessServiceVector(serviceVector);
      return detail;
    }
    */

    /***************************************************************************/
    /***************************** TEST DRIVER *********************************/
    /***************************************************************************/


    public static void main(String[] args) {
        // initialize all jUDDI Subsystems
        psl.discus.javasrc.uddi.util.SysManager.startup();

        // create a request
        GetServiceDetail request = new GetServiceDetail();
        // to-do ... need more here!

        try {
            // invoke the service
            ServiceDetail response = (ServiceDetail) (new GetServiceDetailService().invoke(request));
            // to-do ... need more here!
        } catch (JUDDIException juddiex) {
            System.out.println(juddiex.toString());
        } finally {
            // terminate all jUDDI Subsystems
            psl.discus.javasrc.uddi.util.SysManager.shutdown();
        }
    }
}

