package psl.discus.javasrc.uddi;

import org.uddi4j.request.FindService;
import org.uddi4j.util.FindQualifiers;
import org.uddi4j.util.FindQualifier;
import org.apache.axis.*;
import org.apache.axis.client.AxisClient;
import org.apache.axis.message.SOAPEnvelope;
import org.apache.axis.utils.XMLUtils;
import org.apache.xml.serialize.XMLSerializer;
import org.apache.xml.serialize.OutputFormat;
import org.apache.log4j.Logger;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import psl.discus.javasrc.uddi.transport.axis.RequestHandler;
import org.xml.sax.SAXException;

import javax.xml.parsers.*;
import javax.sql.DataSource;

import java.io.*;

import psl.discus.javasrc.security.*;
import psl.discus.javasrc.shared.FakeDataSource;

/**
 * This class verifies an XML-signed UDDI SOAP message and dispatches it to the UDDI registry
 *
 * @author matias
 */
public class MessageDispatcher {

    private static final Logger logger = Logger.getLogger(MessageDispatcher.class);

    private SignatureManager signatureManager;
    private DocumentBuilder documentBuilder;
    private XMLSerializer xmlSerializer;
    private DataSource ds;


    public MessageDispatcher(DataSource ds, SignatureManager signatureManager)
        throws MessageDispatcherException {

        this.ds = ds;
        this.signatureManager = signatureManager;

        DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
        dbf.setNamespaceAware(true);
        try {
            documentBuilder = dbf.newDocumentBuilder();
        } catch (Exception e) {
            throw new MessageDispatcherException("Could not get DocumentBuilder: " + e);
        }

        xmlSerializer = new XMLSerializer();
    }

    /**
     * Verifies an XML document containing a signed SOAP message, and dispatches it to the UDDI registry
     * @param doc the message to verify and dispatch
     * @return an XML element containing the response from the UDDI registry
     * @throws MessageDispatcherException
     */
    public Element verifyAndDispatchMessage(Document doc)
        throws MessageDispatcherException {

        try {

            // first verify message
            SignatureManagerResponse result = signatureManager.verifyDocument(doc);

            // ok, document verified (otherwise an exception would have been thrown)
            doc = result.getDocument();

            // most unfortunately, the current Axis API doesn't have a way to create a
            // Message from a Document object... so we have to output it and then re-parse it... sigh

            ByteArrayOutputStream out = new ByteArrayOutputStream();
            xmlSerializer.setOutputByteStream(out);
            xmlSerializer.serialize(doc);

            SOAPEnvelope env = new SOAPEnvelope(new ByteArrayInputStream(out.toByteArray()));
            Message msg = new Message(env, true);

            RequestHandler handler = new RequestHandler(ds);

            logger.info("dispatching message to UDDI module...");
            Message response = handler.invoke(msg, result.getSigner());

            return response.getSOAPEnvelope().getAsDOM();

        } catch (Exception e) {
            e.printStackTrace();
            throw new MessageDispatcherException(e);
        }
    }

    /**
     * Dispatches a verified SOAP message to the UDDI registry
     * @param envelopeElement the SOAP message to dispatch, as an XML document
     * @param signer the service space that signed the request
     * @return the results from the UDDI registry
     * @throws MessageDispatcherException
     */
    public Element dispatchMessage(Element envelopeElement, ServiceSpace signer)
        throws MessageDispatcherException {

        try {

            ByteArrayOutputStream docBytes = new ByteArrayOutputStream();
            xmlSerializer.setOutputByteStream(docBytes);
            xmlSerializer.serialize(envelopeElement);

            logger.debug("soap document: \n" + docBytes.toString());

            SOAPEnvelope env = new SOAPEnvelope(new ByteArrayInputStream(docBytes.toByteArray()));
            Message msg = new Message(env, true);

            RequestHandler handler = new RequestHandler(ds);

            Message response = handler.invoke(msg, signer);

            return response.getSOAPEnvelope().getAsDOM();

        } catch (Exception e) {
            e.printStackTrace();
            throw new MessageDispatcherException(e);
        }

    }
    /**
     * Test driver
     */
    public static void main(String args[])
            throws Exception {

        //FileInputStream in = new FileInputStream(args[0]);
        DataSource ds = new FakeDataSource();
        SignatureManager sigManager = new SignatureManagerImpl(ds);
        MessageDispatcher md = new MessageDispatcher(ds, sigManager);

        FindService findService = new FindService("*");
        findService.setMaxRows(5);
        FindQualifiers fqs = new FindQualifiers();
        fqs.add(new FindQualifier("foobar"));
        findService.setFindQualifiers(fqs);

        Document document = md.documentBuilder.newDocument();
        Element envelope = document.createElement("Envelope");
        envelope.setAttribute("xmlns","http://schemas.xmlsoap.org/soap/envelope/");
        Element body = document.createElement("Body");
        envelope.appendChild(body);
        document.appendChild(envelope);

        findService.saveToXML(body);

        Document signedDoc = sigManager.signDocument(document);

        md.verifyAndDispatchMessage(signedDoc);
   }



}
