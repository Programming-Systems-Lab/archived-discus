package psl.discus.javasrc.p2p;

import net.jxta.discovery.*;
import net.jxta.document.*;
import net.jxta.endpoint.Message;
import net.jxta.endpoint.MessageElement;
import net.jxta.exception.PeerGroupException;
import net.jxta.id.IDFactory;
import net.jxta.peergroup.PeerGroup;
import net.jxta.peergroup.PeerGroupFactory;
import net.jxta.pipe.*;
import net.jxta.protocol.*;
import net.jxta.impl.util.Base64;
import org.apache.log4j.Logger;
import org.apache.xml.security.utils.XMLUtils;
import org.apache.xml.serialize.XMLSerializer;
import org.apache.xml.serialize.OutputFormat;
import org.w3c.dom.Element;
import org.w3c.dom.*;
import org.w3c.dom.Document;
import org.xml.sax.InputSource;
import psl.discus.javasrc.security.*;
import psl.discus.javasrc.shared.*;

import javax.sql.DataSource;
import javax.xml.parsers.*;
import java.io.*;
import java.util.*;

/**
 * Implementation of the JXTA client for finding p2p UDDI services
 * and sending to them UDDI requests
 * Some of the code here was borrowed from the JXTA Programmer's Guide,
 * http://www.jxta.org/jxtaprogguide_final.pdf
 *
 * @author matias
 *
 *  Client Side: This is the client side of the p2p UDDI application.
 *  The client searches for the module specification advertisement
 *  associated with the service, extracts the pipe information to
 *  connect to the service, and saves the input pipes advertisement.
 *  It can then create a new output pipe to connect to the
 *  service and send a message to the service.
 *
 */
public class Client implements PipeMsgListener, Tags {

    private static final Logger logger = Logger.getLogger(Client.class);

    private PeerGroup netPeerGroup = null;

    private DiscoveryService discoveryService;
    private PipeService pipeService;
    private PipeAdvertisement inputPipeAdv;

    private static Random random = new Random();
        // used to uniquely number each msg sent so we can identify it when the response come

    private Hashtable queries;  // holds a reference to each query sent, so as to properly dispatch the response

    private MimeMediaType xmlMimeMediaType = new MimeMediaType("text/xml");

    private Hashtable knownPipeAds;
    private static final String CMD_MESSAGE = "msg";
    private static final String CMD_QUIT = "quit";
    private static final String PIPE_ADV_FILE = "pipe.adv";

    private DocumentBuilder db;
    private SignatureManager signatureManager;
    private ClientDAO clientDAO;
    private Element inputPipeXMLAd;
    private XMLSerializer xmlSerializer;

    /**
     * Driver method for testing
     */
    /*
    public static void main(String args[]) {

        logger.debug("Starting Client peer ....");
        Client client = new Client(new FakeDataSource());

        // dummy XML element for sending
        Document doc = client.db.newDocument();
        Element query = doc.createElement("foobar");
        Text text  = doc.createTextNode("hello there");
        query.appendChild(text);

        try {
            BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
            String line = null;
            while ((line = reader.readLine()) != null) {

                if (line.equals(CMD_MESSAGE)) {
                    client.sendQuery(query);
                } else if (line.equals(CMD_QUIT)) {
                    System.exit(0);
                }

            }
        } catch (IOException e) {
            logger.error(e);
        }

    }
    */

    public Client(DataSource ds) {

        knownPipeAds = new Hashtable();

        DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
        dbf.setNamespaceAware(true);
        try {
            db = dbf.newDocumentBuilder();
        } catch (ParserConfigurationException e) {
            throw new RuntimeException("Could not get DocumentBuilder: " + e);
        }

        try {
            signatureManager = new SignatureManagerImpl(ds);
        } catch (SignatureManagerException e) {
            throw new RuntimeException("Could not initialize SignatureManager: " + e);
        }

        clientDAO = new ClientDAO(ds);

        xmlSerializer = new XMLSerializer();
        OutputFormat format = new OutputFormat();
        format.setOmitXMLDeclaration(true);
        xmlSerializer.setOutputFormat(format);

        startJxta();
        createInputPipe();

        // load known pipes
        try {
            Vector endpoints = clientDAO.getServiceSpaceEndpoints();
            for (Enumeration e = endpoints.elements(); e.hasMoreElements();) {
                ServiceSpaceEndpoint endpoint = (ServiceSpaceEndpoint) e.nextElement();
                knownPipeAds.put(endpoint.getPipeAdvertisement().getID(),endpoint);
            }
        }
        catch (DAOException e) {
            throw new RuntimeException("Could not initialize Client: " + e);
        }

        queries = new Hashtable();
    }

    private void startJxta() {

        logger.debug("Starting jxta...");

        try {
            // create, and Start the default jxta NetPeerGroup
            netPeerGroup = PeerGroupFactory.newNetPeerGroup();
        } catch (PeerGroupException e) {
            e.printStackTrace();
            throw new RuntimeException("fatal error : group creation failure");
        }

        // get the discovery, and pipe service
        discoveryService = netPeerGroup.getDiscoveryService();
        pipeService = netPeerGroup.getPipeService();

        // Load locally-cached advertisments
        // NOTE: this is commented out because unfortunately JXTA will cache all
        // received advertisements, even the ones that we want to ignore,
        // so we need to keep the ones we want to cache separately (in the database)
        /*try {
            Enumeration ads = discoveryService.getLocalAdvertisements(DiscoveryService.ADV,
                    "Name", "JXTASPEC:" + Server.SERVICE_NAME);
            while (ads.hasMoreElements()) {
                ModuleSpecAdvertisement msAdv = (ModuleSpecAdvertisement) ads.nextElement();
                PipeAdvertisement pipeAdv = msAdv.getPipeAdvertisement();
                knownPipeAds.put(pipeAdv.getID(), pipeAdv);
                logger.debug("Got from local cache: pipe id " + pipeAdv.getID());
            }
        } catch (IOException e) {
            logger.error("Problem getting local advertisements: " + e);
        }*/

        ServiceFinder finder = new ServiceFinder();
        finder.start();

        logger.debug("jxta started.");

    }

    /**
     * Creates the local listener pipe to listen for responses from the other peers.
     */
    private void createInputPipe() {

        // first we try loading an existing pipe advertisement from file
        File advFile = new File(PIPE_ADV_FILE);

        if (!advFile.exists()) {
            // we need to create a new pipe advertisement
            inputPipeAdv = (PipeAdvertisement)
                    AdvertisementFactory.newAdvertisement(PipeAdvertisement.getAdvertisementType());
            inputPipeAdv.setName("DiscusP2PClient");
            inputPipeAdv.setType(PipeService.UnicastType);
            inputPipeAdv.setPipeID(IDFactory.newPipeID(netPeerGroup.getPeerGroupID()));

            logger.debug("created a new pipe with id " + inputPipeAdv.getID());

            // save this advertisement to use it next time
            try {
                FileOutputStream fout = new FileOutputStream(advFile);
                BufferedWriter out = new BufferedWriter(new OutputStreamWriter(fout));
                StructuredTextDocument doc = (StructuredTextDocument) inputPipeAdv.getDocument(xmlMimeMediaType);
                doc.sendToWriter(out);
                out.flush();
                out.close();
            } catch (IOException e) {
                logger.error("Could not save pipe ad: " + e);
            }
            logger.debug("saved new pipe advertisment");
        } else {
            logger.debug("reading in pipe advertisement file");
            try {
                FileInputStream is = new FileInputStream(advFile);
                inputPipeAdv = (PipeAdvertisement)
                        AdvertisementFactory.newAdvertisement(xmlMimeMediaType, is);
                is.close();
            } catch (Exception e) {
                logger.fatal("failed to read or parse pipe advertisement");
                return;
            }
        }

        try {
            pipeService.createInputPipe(inputPipeAdv, this);
        } catch (IOException e) {
            logger.error("could not create input pipe!: " + e);
        }

        // create XML representation for sending
        org.w3c.dom.Document doc = db.newDocument();
        inputPipeXMLAd = doc.createElement(PIPE_TAG);
        inputPipeXMLAd.setAttribute("xmlns:jxta", "http://jxta.org");
        Element id = doc.createElement("Id");
        id.appendChild(doc.createTextNode(inputPipeAdv.getID().toString()));
        inputPipeXMLAd.appendChild(id);

        Element type = doc.createElement("Type");
        type.appendChild(doc.createTextNode(inputPipeAdv.getType()));
        inputPipeXMLAd.appendChild(type);

        Element name = doc.createElement("Name");
        name.appendChild(doc.createTextNode(inputPipeAdv.getName()));
        inputPipeXMLAd.appendChild(name);

    }

    public void sendQuery(ClientQuery query) {

        final int BIND_TIMEOUT = 15000;     // how long we try to connect to the endpoint for before timing out
        logger.info("sending query...");

        for (Enumeration endpoints = knownPipeAds.elements(); endpoints.hasMoreElements();) {
            try {
                ServiceSpaceEndpoint endpoint = (ServiceSpaceEndpoint) endpoints.nextElement();
                PipeAdvertisement pipeAd = (PipeAdvertisement) endpoint.getPipeAdvertisement();

                // create the output pipe endpoint to connect
                // to the server, try 3 times to bind the pipe endpoint to
                // the listening endpoint pipe of the service
                OutputPipe endpointPipe = null;
                for (int i = 0; i < 3; i++) {
                    logger.debug("Trying to bind to pipe for " + endpoint.getServiceSpace() + "...");
                    try {
                        endpointPipe = pipeService.createOutputPipe(pipeAd, BIND_TIMEOUT);
                        break;
                    } catch (java.io.IOException e) {
                        // will try again;
                    }
                }
                if (endpointPipe == null) {
                    logger.warn("Could not resolve pipe endpoint - removing");

                    // this pipe advertisement seems outdated - remove it from the "known pipes" and the database
                    knownPipeAds.remove(pipeAd.getID());
                    clientDAO.removePipeAdvertisement(pipeAd);

                    continue;
                }

                // create the pipe message
                Message msg = pipeService.createMessage();

                org.w3c.dom.Document doc = db.newDocument();

                Element dataElement = doc.createElement(DATA_TAG);

                // first we uniquely identify this message, and store it in the queries hashtable
                // with a reference to its ClientQuery
                Long messageNumber = new Long(random.nextLong());
                QueryMessage queryMessage = new QueryMessage(query,endpoint.getServiceSpace().getServiceSpaceId());
                queries.put(messageNumber, queryMessage);

                Element msgNumElement = doc.createElement(MSGNUM_TAG);
                Text msgNumText = doc.createTextNode(String.valueOf(messageNumber));
                msgNumElement.appendChild(msgNumText);
                dataElement.appendChild(msgNumElement);

                // then we add the given query to our query element
                Element queryElement = doc.createElement(QUERY_TAG);

                // we need to import the queryelement because we are putting it into a different DOM document
                Node queryNode = doc.importNode(query.getQueryElement(),true);
                queryElement.appendChild(queryNode);

                dataElement.appendChild(queryElement);

                Node ad = doc.importNode(inputPipeXMLAd, true);
                dataElement.appendChild(ad);

                doc.appendChild(dataElement);

                doc = signatureManager.signDocument(doc);

                ByteArrayOutputStream out = new ByteArrayOutputStream();
                XMLUtils.outputDOM(doc, out);

                logger.debug("sending msg: \n" + out.toString());

                MessageElement element = msg.newMessageElement(DATA_TAG, xmlMimeMediaType, out.toByteArray());
                msg.addElement(element);

                // send the message
                endpointPipe.send(msg);
                logger.info("message sent to service space");

            } catch (Exception e) {
                logger.error("Problem sending message: " + e);
                continue;
            }
        }
    }

    public void pipeMsgEvent(PipeMsgEvent event) {
        Message msg;
        try {
            msg = event.getMessage();

            if (msg == null)
                return;
        } catch (Exception e) {
            e.printStackTrace();
            return;
        }

       // Get message
        String newMessage = msg.getString(DATA_TAG);

        if (newMessage == null) {
            logger.debug("data tag in message was empty or missing");
            return;
        }

        logger.debug("received message: " + newMessage);

        // verify message, then extract message number (for dispatching) and results (SOAP envelope)
        org.w3c.dom.Document msgDoc = null;
        try {
            msgDoc = db.parse(new InputSource(new StringReader(newMessage)));
        }
        catch (Exception e) {
            logger.warn("Could not parse message: " + e);
            return;
        }

        SignatureManagerResponse verificationResult = null;
        try {
            verificationResult = signatureManager.verifyDocument(msgDoc);
            logger.info("verified message, from service space " + verificationResult.getSigner().getName());
        }
        catch (SignatureManagerException e) {
            logger.warn("could not verify signature: " + e);

            // for now we continue anyway
            logger.warn("continuing anyway!");
        }

        if (verificationResult != null) {
            msgDoc = verificationResult.getDocument();
        }

        // extract message number - we need this to dispatch the message properly
        Long messageNumber = null;
        {
            NodeList list = msgDoc.getElementsByTagName(MSGNUM_TAG);
            if (list == null || list.getLength() == 0) {
                logger.error("did not find message number in document");
                return;
            }

            Node envelopeNode = list.item(0);
            Text msgNumText = (Text) envelopeNode.getFirstChild();
            messageNumber = new Long(Util.parseLong(msgNumText.getNodeValue()));

        }

        // extract response
        Element responseElement = null;
        {
            NodeList list = msgDoc.getElementsByTagName(RESPONSE_TAG);
            if (list == null || list.getLength() == 0) {
                logger.error("did not find response element in document");
                return;
            }

            responseElement = (Element) list.item(0);
        }

        // lookup QueryMessage object for this message number
        QueryMessage queryMessage = (QueryMessage) queries.get(messageNumber);
        if (queryMessage == null) {
            logger.warn("query not found for message number " + messageNumber);
            return;
        }

        // verify that the signer and the id on the query message match
        if (queryMessage.getServiceSpaceId() != verificationResult.getSigner().getServiceSpaceId()) {
            logger.warn("service space ids do not match!");
            return;
        }

        // now pass on the response to the object that sent the query
        ClientQuery query = queryMessage.getQuery();
        ClientResponseEvent evt = new ClientResponseEvent(this,query,responseElement);
        query.getResponseListener().clientResponseEvent(evt);
    }

    /**
     * Sends discovery messages to find available discus UDDI services
     * This keeps running.
     */
    class ServiceFinder extends Thread implements DiscoveryListener {

        public static final int SLEEP_TIME = 60 * 1000;

        public void run() {

            while (true) {
                logger.debug("sending discovery request");

                discoveryService.getRemoteAdvertisements(null, DiscoveryService.ADV,
                        "Name", "JXTASPEC:" + SERVICE_NAME, 1, this);
                try {
                    sleep(SLEEP_TIME);
                } catch (InterruptedException e) {
                }
            }

        }


        /**
         * by implementing DiscoveryListener we must define this method
         * to deal with discovery responses
         */
        public void discoveryEvent(DiscoveryEvent ev) {
            DiscoveryResponseMsg res = ev.getResponse();
            //logger.debug("got a discovery event");

            PeerAdvertisement peerAdv = null;
            try {
                // instantiate the peer advertisement
                InputStream is = new ByteArrayInputStream((res.getPeerAdv()).getBytes());
                peerAdv = (PeerAdvertisement)
                        AdvertisementFactory.newAdvertisement(xmlMimeMediaType, is);

                if (!peerAdv.getName().startsWith("discus")) {
                    return;     // ignore non-discus peers
                }

                logger.debug("Got a Discovery Response [" + res.getResponseCount() +
                        " elements] from peer : " + peerAdv.getName() + " ]");
            } catch (IOException e) {
                // bogus peer, skip this message all together.
                logger.error("error parsing remote peer's advertisement");
                return;
            }

            Enumeration enum = res.getResponses();
            while (enum.hasMoreElements()) {
                try {
                    String str = (String) enum.nextElement();
                    //logger.debug("raw msad: " + str);
                    // instantiate an advertisement object from each element
                    ModuleSpecAdvertisement moduleSpecAd = (ModuleSpecAdvertisement)
                            AdvertisementFactory.newAdvertisement
                            (xmlMimeMediaType, new ByteArrayInputStream(str.getBytes()));
                    //logger.debug("got module spec id = " + moduleSpecAd.getID());

                    // Get the pipe advertisement -- what we use to talk to the service
                    PipeAdvertisement pipeAd = moduleSpecAd.getPipeAdvertisement();
                    if (pipeAd == null) {
                        logger.debug("Error -- Null pipe advertisement!");
                        continue;
                    }

                    if (knownPipeAds.get(pipeAd.getID()) != null) {
                        logger.debug("pipe ad already known.");
                        continue;
                    } else {
                        // we'll add this pipe to our known pipes -- but first, verify it!

                        // Get the signature document
                        StructuredDocument paramDoc = moduleSpecAd.getParam();
                        if (paramDoc == null) {
                            logger.debug("advertisement was not signed, ignoring.");
                            continue;
                        }

                        try {

                            // base64-decode the signed data first
                            String paramValue = (String)paramDoc.getValue();
                            byte[] decoded = Base64.decodeBase64(paramValue);

                            org.w3c.dom.Document doc = db.parse(new ByteArrayInputStream(decoded));

                            logger.info("verifying module advertisement");
                            // for debugging
                            //XMLUtils.outputDOM(doc,System.out);
                            SignatureManagerResponse verificationResult = signatureManager.verifyDocument(doc);

                            // document was verified (otherwise an exception would have been thrown)
                            // now check that the ID's match
                            doc = verificationResult.getDocument();

                            {
                                NodeList list = doc.getElementsByTagName(MODULE_SPEC_ID_TAG);
                                if (list == null || list.getLength() == 0) {
                                    logger.debug("msId not found in signed data");
                                    continue;
                                } else {
                                    Node node = list.item(0);
                                    String givenId = ((Text) node.getFirstChild()).getData();
                                    if (!givenId.equals(moduleSpecAd.getID().toString())) {
                                        logger.debug("msid's didn't match!");
                                        continue;
                                    }
                                }
                            }

                            {
                                NodeList list = doc.getElementsByTagName(PIPE_ID_TAG);
                                if (list == null || list.getLength() == 0) {
                                    logger.debug("pipeId not found in signed data");
                                    continue;
                                } else {
                                    Node node = list.item(0);
                                    String givenId = ((Text) node.getFirstChild()).getData();
                                    if (!givenId.equals(pipeAd.getID().toString())) {
                                        logger.debug("pipeId's didn't match!");
                                        continue;
                                    }
                                }
                            }

                            // TODO: we should check if we actually trust this peer enough

                            // ok, if we got here we're all OK
                            ServiceSpace serviceSpace = verificationResult.getSigner();
                            logger.info("Adding pipe for service from peer " +
                                    peerAdv.getName() + " (id " + serviceSpace.getServiceSpaceId() + ")");

                            ServiceSpaceEndpoint endpoint = clientDAO.addPipeAdvertisement(serviceSpace,pipeAd);
                            knownPipeAds.put(pipeAd.getID(), endpoint);

                        } catch (Exception e) {
                            logger.debug("could not verify advertisement: " + e);
                            continue;
                        }

                    }

                } catch (Exception e) {
                    logger.debug("problem getting module ad: " + e);
                }
            }
        }
    }

    /**
     * This class encapsulates a query and the service space it is being sent to.
     * It's stored in a hashtable when a query message gets sent, and then is used to
     * check that the response for that message was signed by the same service space that
     * it was sent to.
     */
    class QueryMessage {

        private ClientQuery query;
        private int serviceSpaceId;

        public QueryMessage(ClientQuery query, int serviceSpaceId) {
            this.query = query;
            this.serviceSpaceId = serviceSpaceId;
        }

        public ClientQuery getQuery() {
            return query;
        }

        public int getServiceSpaceId() {
            return serviceSpaceId;
        }

    }
}




