/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.transport.axis;

import psl.discus.javasrc.uddi.error.JUDDIException;
import psl.discus.javasrc.uddi.service.ServiceFactory;
import psl.discus.javasrc.uddi.service.UDDIService;

import org.apache.axis.AxisFault;
import org.apache.axis.Message;
import org.apache.axis.MessageContext;
import org.apache.axis.message.SOAPEnvelope;
import org.apache.axis.message.SOAPBodyElement;
import org.apache.axis.message.SOAPFaultElement;
import org.apache.axis.utils.JavaUtils;
import org.apache.log4j.Logger;
import org.uddi4j.UDDIElement;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.w3c.dom.Document;
import org.w3c.dom.Element;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.sql.DataSource;

import psl.discus.javasrc.security.*;

/**
 * MODIFED by matias
 * Added a new invoke method that takes a ServiceSpace object, which gets put in the
 * UDDIService that is called so that security checks can be performed.
 *
 * @author  Alex Ceponkus
 * @author  Steve Viens
 * @version 0.6
 */
public class RequestHandler extends org.apache.axis.handlers.BasicHandler {
    // private reference to the jUDDI logger
    private static Logger log = Logger.getLogger(RequestHandler.class);

    // create an XML document builder factory
    private static DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();

    // ADDED by matias: an instance of ServicePermissionDAO that is set in service invokations
    private ServicePermissionDAO servicePermissionDAO;

    // ADDED by matias
    public RequestHandler(DataSource ds) {

        servicePermissionDAO = new ServicePermissionDAO(ds);

    }

    /**
     * invoke method that overrides BasicHandler.invoke
     */
    public void invoke(MessageContext msgContext)
            throws org.apache.axis.AxisFault {

        msgContext.setResponseMessage(invoke(msgContext.getRequestMessage(), null));
    }

    /**
     * Our own invoke method, that takes a message and returns a message
     * @param msg
     * @param caller the service space that is sending this query
     * @throws org.apache.axis.AxisFault
     */
    public Message invoke(Message msg, ServiceSpace caller)
            throws org.apache.axis.AxisFault {

        try {
            // 1. pull the request element out of the SOAP envelope
            //Message msg = msgContext.getRequestMessage();
            SOAPEnvelope reqSoapEnv = msg.getSOAPPart().getAsSOAPEnvelope();
            Element reqSoapBody = reqSoapEnv.getFirstBody().getAsDOM();

            // 2. build up a response element for UDDI4j to 'saveToXML()' into
            DocumentBuilder builder = factory.newDocumentBuilder();
            Document document = builder.newDocument();
            Element holder = document.createElement("holder");
            document.appendChild(holder);  // holder element is thrown away
            Element response = document.getDocumentElement();

            // 3. build a UDDI4j request object from the contents of the SOAP body
            UDDIElement request = (UDDIElement) RequestFactory.getRequest(reqSoapBody);

            // 4. obtain correct jUDDI service object (using UDDI4j request class name)
            UDDIService service = ServiceFactory.getService(request.getClass().getName());
            service.setCaller(caller);
            service.setServicePermissionDAO(servicePermissionDAO);

            // 5. invoke jUDDI service and save response to the DOM element
            service.invoke(request).saveToXML(response);

            // 6. build SOAP response
            SOAPEnvelope resSoapEnv = new SOAPEnvelope();
            SOAPBodyElement resSoapBody = new SOAPBodyElement((Element) response.getChildNodes().item(0));
            resSoapEnv.addBodyElement(resSoapBody);

            // 7. let Axis know what the response is
            return new Message(resSoapEnv);

        } catch (JUDDIException juddiex) {
            AxisFault fault = AxisFault.makeFault(juddiex);
            fault.setFaultActor(juddiex.getFaultActor());
            fault.setFaultCode(juddiex.getFaultCode());
            fault.setFaultString(juddiex.getFaultString());

            try {
                // Let's check to see if there's a DispositionReport
                // available with this exception. If so then we need
                // to grab it and stuff it into the SOAP SOAPFault
                // detail.
                DispositionReport dispRpt = juddiex.getDispositionReport();
                if (dispRpt != null) {
                    // 2. build up a response element for UDDI4j to 'saveToXML()' into
                    DocumentBuilder builder = factory.newDocumentBuilder();
                    Document document = builder.newDocument();
                    Element holder = document.createElement("holder");
                    document.appendChild(holder);  // holder element is thrown away
                    Element faultDetail = document.getDocumentElement();

                    // 3. stuff disposition report into SOAPFault detail element
                    dispRpt.saveToXML(faultDetail);
                    Element[] elarray = new Element[1];
                    elarray[0] = (Element) faultDetail.getChildNodes().item(0);
                    fault.setFaultDetail(elarray);
                }
            } catch (ParserConfigurationException pcex) {
                log.error("Difficulty creating a new javax.xml DocumentBuilder", pcex);
            }

            throw fault;
        } catch (Exception ex) {
            throw AxisFault.makeFault(ex);
        }
    }

    /**
     * extending abstract BasicHandler.undo only to work with axis alpha 3 jar.
     * On jan 16, 2002 (and probably well before): undo method doesn't exist
     * on BasicHandler.java in Axis CVS
     */
    public void undo(MessageContext msgContext) {
    }
}
