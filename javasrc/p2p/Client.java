package psl.discus.javasrc.p2p;

import java.io.*;
import java.util.Enumeration;
import java.util.Hashtable;

import net.jxta.discovery.DiscoveryEvent;
import net.jxta.discovery.DiscoveryListener;
import net.jxta.discovery.DiscoveryService;
import net.jxta.document.*;
import net.jxta.document.Document;
import net.jxta.endpoint.Message;
import net.jxta.endpoint.MessageElement;
import net.jxta.exception.PeerGroupException;
import net.jxta.peergroup.PeerGroup;
import net.jxta.peergroup.PeerGroupFactory;
import net.jxta.pipe.*;
import net.jxta.protocol.*;
import net.jxta.id.IDFactory;
import org.apache.log4j.*;
import org.apache.xml.security.utils.XMLUtils;
import org.w3c.dom.*;
import org.w3c.dom.Element;
import org.xml.sax.InputSource;

import javax.xml.parsers.*;
import javax.sql.DataSource;

import psl.discus.javasrc.security.*;

/**
 * Initial implementation of the JXTA client for finding p2p UDDI services
 * and sendingto them UDDI requests
 * A lot of this code was borrowed directly fromfrom the JXTA Programmer's Guide,
 * http://www.jxta.org/jxtaprogguide_final.pdf
 *
 * @author matias
 *
 *  Client Side: This is the client side of the p2p UDDI application.
 *  The client searches for the module specification advertisement
 *  associated with the service, extracts the pipe information to
 *  connect to the service, and saves the input pipes advertisement.
 *  It can then create a new output pipe to connect to the
 *  service and sends a message to the service.
 *
 */
public class Client implements PipeMsgListener {

    private static Logger logger = Logger.getLogger(Client.class);

    private PeerGroup netPeerGroup = null;

    private DiscoveryService discoveryService;
    private PipeService pipeService;
    private PipeAdvertisement inputPipeAdv;

    private MimeMediaType xmlMimeMediaType = new MimeMediaType("text/xml");

    private Hashtable knownPipeAds;
    private static final String CMD_MESSAGE = "msg";
    private static final String CMD_QUIT = "quit";
    private static final String PIPE_ADV_FILE = "pipe.adv";

    private DocumentBuilder db;
    private SignatureManager signatureManager;
    private Element inputPipeXMLAd;

    public static void main(String args[]) {
        Client myapp = new Client(new FakeDataSource());
        logger.debug("Starting Client peer ....");
        myapp.startJxta();
        myapp.createInputPipe();

        try {
            BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
            String line = null;
            while ((line = reader.readLine()) != null) {

                if (line.equals(CMD_MESSAGE)) {
                    myapp.sendMessageToAll("hi there");
                }
                else if (line.equals(CMD_QUIT)) {
                    System.exit(0);
                }

            }
        } catch (IOException e) {
            logger.error(e);
        }

    }

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
        }
        catch (SignatureManagerException e) {
            throw new RuntimeException("Could not initialize SignatureManager: " + e);
        }

    }

    public void startJxta() {
        try {
            // create, and Start the default jxta NetPeerGroup
            netPeerGroup = PeerGroupFactory.newNetPeerGroup();
        } catch (PeerGroupException e) {
            // could not instanciate the group, print the stack and exit
            logger.debug("fatal error : group creation failure");
            e.printStackTrace();
            System.exit(1);
        }

        // get the discovery, and pipe service
        discoveryService = netPeerGroup.getDiscoveryService();
        pipeService = netPeerGroup.getPipeService();

        // Load locally-cached advertisments
        try {
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
        }

        ServiceFinder finder = new ServiceFinder();
        finder.start();

    }

    private void createInputPipe() {

        // create the local listener pipe to listen for responses from server
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
                logger.fatal("failed to read/parse pipe advertisement");
                return;
            }
        }

        try {
            pipeService.createInputPipe(inputPipeAdv,this);
        }
        catch (IOException e) {
            logger.error("could not create input pipe!: " + e);
        }

        // create XML representation for sending
        org.w3c.dom.Document doc = db.newDocument();
        inputPipeXMLAd = doc.createElement(Server.PIPE_TAG);
        inputPipeXMLAd.setAttribute("xmlns:jxta","http://jxta.org");
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

    private void sendMessageToAll(String message) {

        final int BIND_TIMEOUT = 15000;

        for (Enumeration pipeAds = knownPipeAds.elements(); pipeAds.hasMoreElements();) {
            try {
                PipeAdvertisement pipeAdv = (PipeAdvertisement) pipeAds.nextElement();

                // create the output pipe endpoint to connect
                // to the server, try 3 times to bind the pipe endpoint to
                // the listening endpoint pipe of the service
                OutputPipe myPipe = null;
                for (int i = 0; i < 3; i++) {
                    logger.debug("Trying to bind to pipe...");
                    try {
                        myPipe = pipeService.createOutputPipe(pipeAdv, BIND_TIMEOUT);
                        logger.debug("bound to pipe");
                        break;
                    } catch (java.io.IOException e) {
                        // will try again;
                    }
                }
                if (myPipe == null) {
                    logger.debug("Could not resolve pipe endpoint");
                    continue;
                }

                // create the pipe message
                Message msg = pipeService.createMessage();

                //msg.setString(Server.DATA_TAG, message);
                //StringBuffer xml = new StringBuffer(); //.append("<?xml version=\"1.0\" ?>");

                /*net.jxta.document.Document pipeDoc = inputPipeAdv.getDocument(xmlMimeMediaType);
                ByteArrayOutputStream out = new ByteArrayOutputStream();
                pipeDoc.sendToStream(out);
                xml.append(out.toString());

                StringReader reader = new StringReader(xml.toString());
                org.w3c.dom.Document pipeXMLDoc = db.parse(new InputSource(reader));
                XMLUtils.outputDOM(pipeXMLDoc, System.out);

                org.w3c.dom.Document doc = db.newDocument();
                org.w3c.dom.Element main = doc.createElement("main");
                Node newNode = doc.importNode(pipeXMLDoc.getFirstChild(), true);
                main.appendChild(newNode);
                doc.appendChild(main);
                */
                org.w3c.dom.Document doc = db.newDocument();
                Element main = doc.createElement(Server.DATA_TAG);

                Element query = doc.createElement(Server.QUERY_TAG);
                query.appendChild(doc.createTextNode("this is the query"));
                main.appendChild(query);

                Node ad = doc.importNode(inputPipeXMLAd, true);
                main.appendChild(ad);

                doc.appendChild(main);

                //XMLUtils.outputDOM(doc, System.out);
                //System.out.flush();

                //logger.debug("signing...");
                doc = signatureManager.signDocument(doc);

                ByteArrayOutputStream out = new ByteArrayOutputStream();
                XMLUtils.outputDOM(doc, out);
                //MessageElement element = msg.newMessageElement(Server.INPUT_PIPE_TAG,xmlMimeMediaType,pipeDoc.getStream());
                MessageElement element = msg.newMessageElement(Server.DATA_TAG,xmlMimeMediaType,out.toByteArray());
                msg.addElement(element);

                // send the message to the service pipe
                myPipe.send(msg);
                logger.debug("message sent to the Server");

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
        String newMessage = msg.getString(Server.DATA_TAG);
        if (newMessage == null)
            logger.debug("null msg received");
        else
            logger.info("Received message: " + newMessage);
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
                                                         "Name", "JXTASPEC:" + Server.SERVICE_NAME, 1, this);
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
            logger.debug("got a discovery event");

            String peerAdvString = res.getPeerAdv();
            try {
                // instantiate the peer advertisement
                InputStream is = new ByteArrayInputStream((peerAdvString).getBytes());
                PeerAdvertisement peerAdv = (PeerAdvertisement)
                        AdvertisementFactory.newAdvertisement(xmlMimeMediaType, is);

                if (!peerAdv.getName().startsWith("discus")) {
                    return;     // ignore non-discus peers
                }

                logger.debug(" [ Got a Discovery Response [" + res.getResponseCount() +
                        " elements] from peer : " + peerAdv.getName() + " ]");
            } catch (java.io.IOException e) {
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
                    logger.debug(" got module spec id = " + moduleSpecAd.getID());

                    // for debug: print the advertisement as a plain text document
                    StructuredTextDocument doc = (StructuredTextDocument)
                            moduleSpecAd.getDocument(new MimeMediaType("text/xml"));

                    StringWriter out = new StringWriter();
                    doc.sendToWriter(out);
                    logger.debug(out.toString());
                    out.close();

                    // Get the signature document
                    StructuredDocument signatureDoc = moduleSpecAd.getParam();
                    if (signatureDoc != null) {
                        logger.debug("signature:");
                        signatureDoc.sendToStream(System.out);
                    }

                    // Get the pipe advertisement -- what we use to talk to the service
                    PipeAdvertisement pipeadv = moduleSpecAd.getPipeAdvertisement();
                    if (pipeadv == null) {
                        logger.debug("Error -- Null pipe advertisement!");
                        continue;
                    }

                    if (knownPipeAds.get(pipeadv.getID()) != null) {
                        logger.debug("pipe ad already known, ignoring.");
                        continue;
                    } else {
                        // add this pipe to our known pipes
                        knownPipeAds.put(pipeadv.getID(),pipeadv);
                    }

                } catch (Exception ex) {
                    ex.printStackTrace();
                    logger.debug("Client: Error sending message to the service");
                }
            }
        }
    }
}




