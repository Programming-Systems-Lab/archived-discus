/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.service;

import psl.discus.javasrc.uddi.error.*;

import org.uddi4j.UDDIElement;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.response.ErrInfo;
import org.uddi4j.response.Result;

import java.util.Vector;

import psl.discus.javasrc.security.ServiceSpace;
import psl.discus.javasrc.security.ServicePermissionDAO;

/**
 * MODIFIED by matias
 * Added ServiceSpace property so that xxxService classes can known which service space
 * is making the call, and also a reference to a ServicePermissionDAO
 *
 * @author  Steve Viens
 * @version 0.6
 */
public abstract class UDDIService {


    /**
     *  Valid UDDI Version 2.0 SOAPMessage 'generic' attribute
     */
    public static final String GENERIC = "2.0";

    /**
     *  Valid UDDI Version 2.0 SOAPMessage 'xmlns' attribute
     */
    public static final String XMLNS = "urn:uddi-org:api_v2";

    static final DispositionReport success = getSuccessfullDispRpt();

    public abstract UDDIElement invoke(UDDIElement request)
            throws JUDDIException;

    /* added by matias */
    public static final String GATEKEEPER_KEY = "0000000-0000-0000-0000-000000000001";
    public static final String GATEKEEPER_BINDING_KEY = "0000000-0000-0000-0000-000000000002";
    protected ServiceSpace caller;

    public ServiceSpace getCaller() {
        return caller;
    }

    public void setCaller(ServiceSpace caller) {
        this.caller = caller;
    }

    protected ServicePermissionDAO servicePermissionDAO;

    public ServicePermissionDAO getServicePermissionDAO() {
        return servicePermissionDAO;
    }

    public void setServicePermissionDAO(ServicePermissionDAO servicePermissionDAO) {
        this.servicePermissionDAO = servicePermissionDAO;
    }



    protected static void checkVersionInfo(UDDIElement request)
            throws UnrecognizedVersionException {
        if (request.GENERIC == null)
            throw new UnrecognizedVersionException("The UDDI request received did " +
                    "not include a 'generic' attribute. This attribute must be present " +
                    "in all UDDI requests in order to process the request successfully. " +
                    "Currently only version 2.0 (generic=\"2.0\") of the UDDI specification " +
                    "is supported.");

        if (request.XMLNS == null)
            throw new UnrecognizedVersionException("The UDDI request received did " +
                    "not include a 'xmlns' attribute. This attribute must be present " +
                    "in all UDDI requests in order to process the request successfully. " +
                    "Currently only version 2.0 (xmlns=\"urn:uddi-org:api_v2\") of the " +
                    "UDDI specification is supported.");

        if (!request.GENERIC.equalsIgnoreCase(GENERIC))
            throw new UnrecognizedVersionException("Currently only version 2.0 " +
                    "(generic=\"2.0\") of the UDDI specification is supported. The UDDI " +
                    "request received included a 'generic' attribute value of '" +
                    request.GENERIC + "'");

        if (!request.XMLNS.equalsIgnoreCase(XMLNS))
            throw new UnrecognizedVersionException("Currently only version 2.0 " +
                    "(xmlns=\"urn:uddi-org:api_v2\") of the UDDI specification is supported. " +
                    "The UDDI request received included an 'xmlns' attribute value of '" +
                    request.XMLNS + "'");
    }

    private synchronized static DispositionReport getSuccessfullDispRpt() {
        if (success != null)
            return success;

        Result result = new Result();
        result.setErrno("0");
        result.setErrInfo(null);
        result.setKeyType("");

        Vector results = new Vector(1);
        results.addElement(result);

        DispositionReport dispRpt = new DispositionReport();
        dispRpt.setResults(results);

        return dispRpt;
    }
}
