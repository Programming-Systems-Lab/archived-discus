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
import org.juddi.transport.axis.RequestHandler;
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


    public MessageDispatcher(SignatureManager signatureManager)
        throws MessageDispatcherException {
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

    public Message dispatchMessage(InputStream in)
        throws MessageDispatcherException {

        try {

            // first verify message
            Document doc = documentBuilder.parse(in);
            SignatureManagerResponse result = signatureManager.verifyDocument(doc);

            // ok, document verified (otherwise an exception would have been thrown)
            doc = result.document;

            // most unfortunately, the current Axis API doesn't have a way to create a
            // Message from a Document object... so we have to output it and then re-parse it... sigh

            ByteArrayOutputStream out = new ByteArrayOutputStream();
            xmlSerializer.setOutputByteStream(out);
            xmlSerializer.serialize(doc);

            SOAPEnvelope env = new SOAPEnvelope(new ByteArrayInputStream(out.toByteArray()));
            Message msg = new Message(env, true);

            RequestHandler handler = new RequestHandler();

            DiscusCredential credential = new DiscusCredential();
            credential.setServiceSpaceId(1);

            Message response = handler.invoke(msg, credential);

            response.writeContentToStream(System.out);
            return response;

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
        SignatureManager sigManager = new SignatureManagerImpl(new FakeDataSource());
        MessageDispatcher md = new MessageDispatcher(sigManager);

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

        ByteArrayOutputStream out = new ByteArrayOutputStream();
        org.apache.xml.security.utils.XMLUtils.outputDOMc14nWithComments(signedDoc,out);
        //md.xmlSerializer.setOutputByteStream(out);
        //md.xmlSerializer.serialize(signedDoc);

        logger.debug(out.toString());

        md.dispatchMessage(new ByteArrayInputStream(out.toByteArray()));
   }



}
