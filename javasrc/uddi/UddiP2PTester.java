package psl.discus.javasrc.uddi;

import psl.discus.javasrc.shared.FakeDataSource;
import psl.discus.javasrc.p2p.*;
import org.uddi4j.request.FindService;
import org.uddi4j.request.GetServiceDetail;
import org.uddi4j.util.*;
import org.uddi4j.datatype.Name;
import org.w3c.dom.*;
import org.apache.log4j.Logger;
import org.apache.xml.serialize.XMLSerializer;
import psl.discus.javasrc.uddi.service.UDDIService;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import java.io.*;
import java.util.Vector;

/**
 * This class is used to test sending a SOAP UDDI query via the P2P Client
 * @author matias
 */
public final class UddiP2PTester implements ClientResponseListener {

    private static final Logger logger = Logger.getLogger(UddiP2PTester.class);

    public static void main(String[] args)
        throws Exception {

        UddiP2PTester tester = new UddiP2PTester();

        // initialize the Client
        FakeDataSource ds = new FakeDataSource();
        Client client = new Client(ds);

        ClientQuery findServiceQuery = null;
        ClientQuery getServiceDetailQuery = null;
        DocumentBuilder builder = DocumentBuilderFactory.newInstance().newDocumentBuilder();

        // make uddi call
        {
            FindService findService = new FindService("*");
            findService.setMaxRows(5);
            Name name = new Name("service1");
            Vector names = new Vector();
            names.add(name);
            findService.setNameVector(names);

            Document document = builder.newDocument();
            Element findServiceEnvelope = document.createElement(Tags.SOAP_ENVELOPE_TAG);
            findServiceEnvelope.setAttribute("xmlns:SOAP-ENV","http://schemas.xmlsoap.org/soap/envelope/");
            Element body = document.createElement(Tags.SOAP_BODY_TAG);
            findServiceEnvelope.appendChild(body);

            findService.saveToXML(body);

            findServiceQuery = new ClientQuery(findServiceEnvelope,tester);

        }

        {
            Vector keys = new Vector();
            keys.add(UDDIService.GATEKEEPER_KEY);
            GetServiceDetail getServiceDetail = new GetServiceDetail(keys);

            Document document = builder.newDocument();
            Element getServiceDetailEnvelope = document.createElement(Tags.SOAP_ENVELOPE_TAG);
            getServiceDetailEnvelope.setAttribute("xmlns:SOAP-ENV","http://schemas.xmlsoap.org/soap/envelope/");
            Element body = document.createElement(Tags.SOAP_BODY_TAG);
            getServiceDetailEnvelope.appendChild(body);

            getServiceDetail.saveToXML(body);

            getServiceDetailQuery = new ClientQuery(getServiceDetailEnvelope,tester);

        }

        try {
            BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
            String line = null;

            while ((line = reader.readLine()) != null) {
                System.out.print("\n>");
                if (line.equals("findservice")) {
                    client.sendQuery(findServiceQuery);
                } else if (line.equals("servicedetails")) {
                    client.sendQuery(getServiceDetailQuery);
                } else if (line.equals("quit")) {
                    System.exit(0);
                }
            }
        } catch (IOException e) {
            logger.error(e);
        }

    }

    public void clientResponseEvent(ClientResponseEvent evt) {

        XMLSerializer serializer = new XMLSerializer();

        Element response = evt.getResponse();
        NodeList nodes = response.getChildNodes();
        for (int i=0;i<nodes.getLength();i++)
        {
            Node currentNode = nodes.item(i);
            if (currentNode.getNodeName().equals(Tags.SOAP_ENVELOPE_TAG)) {
                serializer.setOutputByteStream(System.out);

                logger.info("got uddi response:\n");
                try {
                    serializer.serialize((Element) currentNode);
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        }
    }

}
