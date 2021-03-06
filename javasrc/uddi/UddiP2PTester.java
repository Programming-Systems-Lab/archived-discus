package psl.discus.javasrc.uddi;

import psl.discus.javasrc.shared.FakeDataSource;
import psl.discus.javasrc.shared.Util;
import psl.discus.javasrc.p2p.*;
import org.uddi4j.request.FindService;
import org.uddi4j.request.GetServiceDetail;
import org.uddi4j.util.*;
import org.uddi4j.datatype.Name;
import org.w3c.dom.*;
import org.apache.log4j.Logger;
import org.apache.xml.serialize.XMLSerializer;
import org.apache.axis.utils.XMLUtils;
import psl.discus.javasrc.uddi.service.UDDIService;
import psl.discus.javasrc.security.ServiceSpace;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import java.io.*;
import java.util.Vector;


/**
 * This class is used to test sending a SOAP UDDI query via the P2P Client
 * @author matias
 */
public final class UddiP2PTester implements ClientEventListener {

    private static Logger logger = Logger.getLogger(UddiP2PTester.class);
    private Client client;
    private DocumentBuilder builder;

    public UddiP2PTester() {

        try {
            builder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
        } catch (Exception e) {
            logger.fatal("Could not get documentbuilder: " + e);
        }

    }

    /**
     * Initializes the client and subscribe to service space notifications
     * @param listener the listener where the events should be sent you, usually either
     * this or the class using UddiP2PTester
     * @return the array of ServiceSpaceEndpoints returned by client.subscribeToNotifications()
     */
    public ServiceSpaceEndpoint[] init(ClientEventListener listener) {
        // initialize the Client
        FakeDataSource ds = new FakeDataSource();
        try {
            client = Client.getInstance(ds);
        } catch (ClientException e) {
            throw new RuntimeException("Could not get client instance!");
        }

        return client.subscribeToNotifications(listener);
    }


    /**
     * Sends a FindService query, to all service spaces
     * @param serviceName
     */
    public void sendFindServiceQuery(String serviceName, ClientEventListener responseListener) {
        logger.info("sending FindService query");
        FindService findService = new FindService("*");
        findService.setMaxRows(5);
        Name name = new Name(serviceName);
        Vector names = new Vector();
        names.add(name);
        findService.setNameVector(names);

        Document document = builder.newDocument();
        Element findServiceEnvelope = document.createElement(Tags.SOAP_ENVELOPE_TAG);
        findServiceEnvelope.setAttribute("xmlns:soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
        Element body = document.createElement(Tags.SOAP_BODY_TAG);
        findServiceEnvelope.appendChild(body);

        findService.saveToXML(body);

        ClientQuery findServiceQuery = new ClientQuery(findServiceEnvelope, responseListener);
        findServiceQuery.setName("findService for '" + serviceName + "'");

        client.sendQueryToAll(findServiceQuery);

    }

    /**
     * Sends a GetServiceDetailsQuery, but only to the specified service space
     * @param serviceSpaceEndpoint
     */
    public void sendGetServiceDetailsQuery(ServiceSpaceEndpoint serviceSpaceEndpoint,
                                           ClientEventListener responseListener) {
        logger.info("sending GetServiceDetails query");
        Vector keys = new Vector();
        keys.add(UDDIService.GATEKEEPER_KEY);
        GetServiceDetail getServiceDetail = new GetServiceDetail(keys);

        Document document = builder.newDocument();
        Element getServiceDetailEnvelope = document.createElement(Tags.SOAP_ENVELOPE_TAG);
        getServiceDetailEnvelope.setAttribute("xmlns:soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
        Element body = document.createElement(Tags.SOAP_BODY_TAG);
        getServiceDetailEnvelope.appendChild(body);

        getServiceDetail.saveToXML(body);

        ClientQuery getServiceDetailQuery = new ClientQuery(getServiceDetailEnvelope, responseListener);
        getServiceDetailQuery.setName("getServiceDetail");

        try {
            client.sendQuery(getServiceDetailQuery, serviceSpaceEndpoint);
        } catch (Exception e) {
            logger.error("problem sending query: " + e);
        }

    }


    public void clientResponseEvent(ClientResponseEvent evt) {

        logger.info("received a response from client for " +
                    evt.getSourceQuery() + " query");

        StringWriter writer = new StringWriter();

        Element response = evt.getResponse();
        NodeList nodes = response.getChildNodes();
        for (int i = 0; i < nodes.getLength(); i++) {
            Node currentNode = nodes.item(i);

            if (currentNode.getNodeName().equals(Tags.SOAP_ENVELOPE_TAG)) {
                XMLUtils.ElementToWriter((Element) currentNode, writer);
                logger.info(writer.toString());
            }
        }
    }

    public void clientNotificationEvent(ClientNotificationEvent evt) {
        logger.info("received a client notification: " +
                "found new service space: " + evt.getServiceSpaceEndpoint());

    }


    /**
     * Main driver
     */
    public static void main(String[] args)
                throws Exception {

        UddiP2PTester tester = new UddiP2PTester();

        tester.init(tester);
        try {
            BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
            String line = null;

            while ((line = reader.readLine()) != null) {
                System.out.print("\n>");
                if (line.equals("findservice")) {
                    //client.sendQueryToAll(findServiceQuery);
                } else if (line.equals("servicedetails")) {
                    //client.sendQueryToAll(getServiceDetailQuery);
                } else if (line.equals("quit")) {
                    System.exit(0);
                }
            }
        } catch (IOException e) {
            logger.error(e);
        }

    }


}
