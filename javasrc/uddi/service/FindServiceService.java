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
import org.uddi4j.datatype.Name;
import org.uddi4j.request.FindService;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.response.*;
import org.uddi4j.util.CategoryBag;
import org.uddi4j.util.FindQualifiers;
import org.uddi4j.util.TModelBag;
import org.apache.log4j.Logger;

import java.util.Vector;

import psl.discus.javasrc.security.ServicePermission;
import psl.discus.javasrc.shared.DAOException;

/**
 * MODIFIED by matias
 * The find_service method looks up the requested method on the security matrix for the
 * calling service space, and either returns a fixed key if the service is allowed
 * or nothing otherwise.
 *
 * @author  Steve Viens
 * @version 0.6
 */
public class FindServiceService extends UDDIService {
    // private reference to the jUDDI logger
    private static final Logger log = Logger.getLogger(FindServiceService.class);

    private static DataStoreFactory factory = DataStoreFactory.getInstance();

    // added by matias
    private static final ServiceList EMPTY_SERVICELIST = new ServiceList();
    private static ServiceList gatekeeperServiceList;

    static {
        // prepare the service list that has the key for the gatekeeper service
        // we use this for all responses, so we just create it once.
        ServiceInfo info = new ServiceInfo();
        info.setName("GateKeeper");
        info.setServiceKey(GATEKEEPER_KEY);

        ServiceInfos infos = new ServiceInfos();
        Vector infoVector = new Vector();
        infoVector.add(info);
        infos.setServiceInfoVector(infoVector);

        gatekeeperServiceList = new ServiceList();
        gatekeeperServiceList.setServiceInfos(infos);

    }

    /**
     *
     */
    public UDDIElement invoke(UDDIElement request)
            throws JUDDIException {
        // morph UDDIElement into a specific request object
        return this.invoke((FindService) request);
    }

    /**
     *
     */
    public ServiceList invoke(FindService request)
            throws JUDDIException {
        String businessKey = request.getBusinessKey();
        Vector nameVector = request.getNameVector();
        CategoryBag categoryBag = request.getCategoryBag();
        TModelBag tModelBag = request.getTModelBag();
        FindQualifiers qualifiers = request.getFindQualifiers();
        int maxRows = request.getMaxRowsInt();

        // perform the requested action
        return find_service(businessKey, nameVector, categoryBag, tModelBag, qualifiers, maxRows);
    }

    /**
     * MODIFIED by matias
     * [This method had not yet been implemented by the jUDDI authors]
     *
     * For FindService calls, we just check with the security matrix to see if there are any
     * permissions for this service space and service name. If there are, then we return
     * a fixed service key, that when queried will return the access point to our gatekeeper
     */
    public ServiceList find_service(String businessKey, Vector names, CategoryBag categoryBag, TModelBag tModelBag, FindQualifiers findQualifiers, int maxRows)
            throws JUDDIException {

        ServiceList list = new ServiceList();

        // get service name, check if this service space has permission to call methods on that service
        String name = ((Name)names.elementAt(0)).getText();

        try {
            log.info("looking up service " + name + " for service space " + caller.getName());
            ServicePermission permission = servicePermissionDAO.getPermissions(caller.getServiceSpaceId(),name);

            if (permission.getMethods().size() > 0) {
                log.debug("returning gatekeeper servicelist");
                return gatekeeperServiceList;
            }
            else {
                log.debug("returning empty list");
                return EMPTY_SERVICELIST;
            }

        } catch (DAOException e) {
            throw new JUDDIException(e.toString());
        }

    }


    /***************************************************************************/
    /***************************** TEST DRIVER *********************************/
    /***************************************************************************/


    public static void main(String[] args) {
        // initialize all jUDDI Subsystems
        psl.discus.javasrc.uddi.util.SysManager.startup();

        // create a request
        FindService request = new FindService();
        // to-do ... need more here!

        try {
            // invoke the service
            ServiceList response = (ServiceList) (new FindServiceService().invoke(request));
            // to-do ... need more here!
        } catch (JUDDIException juddiex) {
            System.out.println(juddiex.toString());
        } finally {
            // terminate all jUDDI Subsystems
            psl.discus.javasrc.uddi.util.SysManager.shutdown();
        }
    }
}

