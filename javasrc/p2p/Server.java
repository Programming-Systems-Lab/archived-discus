package psl.discus.javasrc.p2p;

import java.io.*;

import net.jxta.discovery.DiscoveryService;
import net.jxta.document.*;
import net.jxta.endpoint.Message;
import net.jxta.endpoint.MessageElement;
import net.jxta.exception.PeerGroupException;
import net.jxta.id.IDFactory;
import net.jxta.peergroup.PeerGroup;
import net.jxta.peergroup.PeerGroupFactory;
import net.jxta.pipe.*;
import net.jxta.platform.ModuleClassID;
import net.jxta.protocol.ModuleClassAdvertisement;
import net.jxta.protocol.ModuleSpecAdvertisement;
import net.jxta.protocol.PipeAdvertisement;
import net.jxta.impl.util.Base64;
import org.apache.log4j.Level;
import org.apache.log4j.Logger;
import org.apache.xml.security.utils.XMLUtils;
import org.xml.sax.InputSource;
import org.apache.xml.serialize.*;
import org.w3c.dom.NodeList;
import org.w3c.dom.Node;
import org.w3c.dom.Element;

import javax.xml.parsers.*;
import javax.sql.DataSource;

import psl.discus.javasrc.security.*;
import psl.discus.javasrc.shared.FakeDataSource;
import psl.discus.javasrc.uddi.*;

/**
 *
 * Initial implementation of the JXTA server for listening to p2p UDDI requests
 * Most of this was borrowed from the JXTA Programmer's Guide, http://www.jxta.org/jxtaprogguide_final.pdf
 *
 * @author matias
 *
 * Server side: This is the server side of the Discus p2p UDDI system. The
 * server side application advertises the uddi service, starts the
 * service, and receives messages on a service defined pipe
 * endpoint. The service associated module spec and class
 * advertisement are published in the NetPeerGroup. Clients can
 * discover the module advertisements and create output pipes to
 * connect to the service. The server application creates an input
 * pipe and waits to receive messages.
 */
public class Server implements PipeMsgListener {

    static PeerGroup group = null;
    private DiscoveryService discoveryService;
    private PipeService pipeService;

    private static final Logger logger = Logger.getLogger(Server.class);
    public static final String SERVICE_NAME = "discusUddi";
    public static final String MODULE_ADV_FILE = "module.adv";
    public static final String MODULE_SPEC_ADV_FILE = "modulespec.adv";
    public static final String PIPE_ADV_FILE = "pipe.adv";
    public static final String DATA_TAG = "data";
    public static final String QUERY_TAG = "query";
    //public static final String INPUT_PIPE_TAG = "InputPipe";
    public static final MimeMediaType xmlMimeMediaType = new MimeMediaType("text/xml");
    private DocumentBuilder db;
    private SignatureManager signatureManager;
    private MessageDispatcher messageDispatcher;
    private XMLSerializer xmlSerializer;

    public static final String PIPE_TAG = "jxta:PipeAdvertisement";
    public static final String MODULE_SPEC_ID_TAG = "ModuleSpecId";
    public static final String PIPE_ID_TAG = "PipeId";
    public static final String PARAM_TAG = "Parm";
    public static final String ENVELOPE_TAG = "Envelope";


    public static void main(String args[]) {
        Server myapp = new Server(new FakeDataSource());
        logger.debug("Starting Service Peer ....");
        myapp.startJxta();

    }



    public Server(DataSource ds) {
        DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
        dbf.setNamespaceAware(true);
        try {
            db = dbf.newDocumentBuilder();
        } catch (ParserConfigurationException e) {
            throw new RuntimeException("Could not get DocumentBuilder: " + e);
        }

        try {
            logger.debug("Instantiating SignatureManager...");
            signatureManager = new SignatureManagerImpl(ds);
        }
        catch (SignatureManagerException e) {
            throw new RuntimeException("Could not initialize SignatureManager: " + e);
        }

        try {
            logger.debug("Instantiating MessageDispatcher...");
            messageDispatcher = new MessageDispatcher(signatureManager);
        } catch (MessageDispatcherException e) {
            throw new RuntimeException("Could not initialize MessageDispatcher: " + e);
        }

        xmlSerializer = new XMLSerializer();
        OutputFormat format = new OutputFormat();
        format.setOmitXMLDeclaration(true);
        xmlSerializer.setOutputFormat(format);
    }

    public void startJxta() {

        logger.debug("Starting jxta...");
        try {
            // create, and Start the default jxta NetPeerGroup
            group = PeerGroupFactory.newNetPeerGroup();

        } catch (PeerGroupException e) {
            // could not instanciate the group, print the stack and exit
            logger.fatal("error: could not instantiate netPeerGroup");
            return;
        }

        // get the discovery, and pipe service
        discoveryService = group.getDiscoveryService();
        pipeService = group.getPipeService();

        logger.debug("jxta started.");

        startServer();
    }

    private void startServer() {

        logger.debug("Starting the server...");

        try {


            // First load or create the Module class advertisement associated with the service
            // We build the module class advertisement using the advertisement
            // Factory class by passing it the type of the advertisement we
            // want to construct. The Module class advertisement is to be used
            // to simply advertise the existence of the service. This is a
            // a very small advertisement that only advertise the existence
            // of service. In order to access the service, a peer will
            // have to discover the associated module spec advertisement.
            ModuleClassAdvertisement moduleClassAd;
            ModuleClassID mcID = null;
            File moduleAdFile = new File(MODULE_ADV_FILE);
            if (!moduleAdFile.exists()) {
                // create new module

                moduleClassAd = (ModuleClassAdvertisement)
                        AdvertisementFactory.newAdvertisement(
                                ModuleClassAdvertisement.getAdvertisementType());

                moduleClassAd.setName("JXTAMOD:" + SERVICE_NAME);
                moduleClassAd.setDescription("Discus UDDI service");

                mcID = IDFactory.newModuleClassID();
                moduleClassAd.setModuleClassID(mcID);

                // Ok the Module Class advertisement was created, now publish
                // it in the local cache and to the NetPeerGroup
                discoveryService.publish(moduleClassAd, DiscoveryService.ADV);

                // and save it to file
                FileOutputStream fout = new FileOutputStream(moduleAdFile);
                BufferedWriter out = new BufferedWriter(new OutputStreamWriter(fout));
                StructuredTextDocument doc = (StructuredTextDocument)
                        moduleClassAd.getDocument(xmlMimeMediaType);
                doc.sendToWriter(out);
                out.flush();
                out.close();
                logger.debug("saved new module class advertisment");
            } else {
                // load module class ad from file
                logger.debug("reading in module advertisement file");
                try {
                    FileInputStream is = new FileInputStream(moduleAdFile);
                    moduleClassAd = (ModuleClassAdvertisement)
                            AdvertisementFactory.newAdvertisement(
                                    xmlMimeMediaType, is);
                    is.close();
                } catch (Exception e) {
                    logger.fatal("failed to read/parse module class advertisement");
                    return;
                }
            }

            discoveryService.remotePublish(moduleClassAd, DiscoveryService.ADV);


            // Create the Module Spec advertisement associated with the service
            // We build the module Spec Advertisement using the advertisement
            // Factory class by passing in the type of the advertisement we
            // want to construct. The Module Spec advertisement will contain
            // all the information necessary for a client to contact the service
            // for example a pipe advertisement to be used to contact the service
            ModuleSpecAdvertisement moduleSpecAd;
            PipeAdvertisement pipeAd;
            File moduleSpecAdFile = new File(MODULE_SPEC_ADV_FILE);
            if (!moduleSpecAdFile.exists()) {
                // create a new module spec ad
                moduleSpecAd = (ModuleSpecAdvertisement)
                        AdvertisementFactory.newAdvertisement(
                                ModuleSpecAdvertisement.getAdvertisementType());

                // Setup some of the information field about the servive.
                moduleSpecAd.setName("JXTASPEC:" + SERVICE_NAME);
                moduleSpecAd.setVersion("Version 1.0");
                moduleSpecAd.setCreator("discus");
                moduleSpecAd.setModuleSpecID(IDFactory.newModuleSpecID(mcID));
                moduleSpecAd.setSpecURI("http://discus/");
                //moduleSpecAd.setParam();

                // Create a pipe advertisement for the Service. The client MUST use
                // the same pipe advertisement to talk to the server. When the client
                // discovers the module advertisement it will extract the pipe
                // advertisement to create its pipe.

                File advFile = new File(PIPE_ADV_FILE);
                if (!advFile.exists()) {
                    // we need to create a new pipe advertisement
                    pipeAd = (PipeAdvertisement)
                            AdvertisementFactory.newAdvertisement(PipeAdvertisement.getAdvertisementType());
                    pipeAd.setName(SERVICE_NAME);
                    pipeAd.setType(PipeService.UnicastType);
                    pipeAd.setPipeID(IDFactory.newPipeID(group.getPeerGroupID()));

                    logger.debug("created a new pipe with id " + pipeAd.getID());

                    // save this advertisement
                    FileOutputStream fout = new FileOutputStream(advFile);
                    BufferedWriter out = new BufferedWriter(new OutputStreamWriter(fout));
                    StructuredTextDocument doc = (StructuredTextDocument) pipeAd.getDocument(xmlMimeMediaType);
                    doc.sendToWriter(out);
                    out.flush();
                    out.close();
                    logger.debug("saved new pipe advertisment");
                } else {
                    logger.debug("reading in pipe advertisement file");
                    try {
                        FileInputStream is = new FileInputStream(advFile);
                        pipeAd = (PipeAdvertisement)
                                AdvertisementFactory.newAdvertisement(
                                        xmlMimeMediaType, is);
                        is.close();
                    } catch (Exception e) {
                        logger.fatal("failed to read/parse pipe advertisement");
                        return;
                    }
                }
                // add the pipe advertisement to the ModuleSpecAdvertisement
                moduleSpecAd.setPipeAdvertisement(pipeAd);

                // We need to add a signature and/or public key to this advertisement
                // Since we can't sign the whole advertisement (because then it wouldn't be a normal
                // JXTA advertisement), we add as a parameter the signed advertisement ID and pipe ID
                // Additionally, we wrap the IDs in a "data" tag that is signed.

                // Change: for now, Base64-encode signed data! The reason for this is that
                // JXTA changes the indenting/spacing, and the signature is broken when
                // it gets to the client...
                {
                    org.w3c.dom.Document dataDoc = db.newDocument();
                    Element data = dataDoc.createElement(DATA_TAG);

                    Element msid = dataDoc.createElement(MODULE_SPEC_ID_TAG);
                    msid.appendChild(dataDoc.createTextNode(moduleSpecAd.getID().toString()));
                    data.appendChild(msid);

                    Element pipeid = dataDoc.createElement(PIPE_ID_TAG);
                    pipeid.appendChild(dataDoc.createTextNode(pipeAd.getID().toString()));
                    data.appendChild(pipeid);

                    dataDoc.appendChild(data);

                    dataDoc = signatureManager.signDocument(dataDoc);

                    // debugging
                    //XMLUtils.outputDOMc14nWithComments(dataDoc,System.out);
                    xmlSerializer.setOutputByteStream(System.out);
                    xmlSerializer.serialize(dataDoc);

                    ByteArrayOutputStream out = new ByteArrayOutputStream();
                    //XMLUtils.outputDOMc14nWithComments(dataDoc,out);
                    xmlSerializer.setOutputByteStream(out);
                    xmlSerializer.serialize(dataDoc);
                    out.close();

                    // it's sad, but we have to encode it because otherwise signature gets mangled
                    // and cannot be verified.... sort of defeats the whole point of XML signatures...
                    String encoded = Base64.encodeBase64(out.toByteArray());

                    StructuredTextDocument signatureDoc = (StructuredTextDocument)
                            StructuredDocumentFactory.newStructuredDocument(xmlMimeMediaType, PARAM_TAG, encoded);
                    moduleSpecAd.setParam(signatureDoc);
                }
                // debugging: display the advertisement as a plain text document.
                {
                    logger.debug("Created service advertisement:");
                    StructuredTextDocument doc = (StructuredTextDocument)
                            moduleSpecAd.getDocument(xmlMimeMediaType);
                    StringWriter str_out = new StringWriter();
                    doc.sendToWriter(str_out);
                    logger.debug(str_out.toString());

                }
                // Ok the Module advertisement was created, just publish
                // it in my local cache and into the NetPeerGroup.
                discoveryService.publish(moduleSpecAd, DiscoveryService.ADV);

                // and save it to file
                {
                    FileOutputStream fout = new FileOutputStream(moduleSpecAdFile);
                    BufferedWriter out = new BufferedWriter(new OutputStreamWriter(fout));
                    StructuredTextDocument doc = (StructuredTextDocument)
                            moduleSpecAd.getDocument(xmlMimeMediaType);
                    doc.sendToWriter(out);
                    out.flush();
                    out.close();
                    logger.debug("saved new module spec advertisment");
                }
            } else {
                // load module class ad from file
                logger.debug("reading in module spec ad file");
                try {
                    FileInputStream is = new FileInputStream(moduleSpecAdFile);
                    moduleSpecAd = (ModuleSpecAdvertisement)
                            AdvertisementFactory.newAdvertisement(
                                    xmlMimeMediaType, is);
                    is.close();
                } catch (Exception e) {
                    logger.fatal("failed to read/parse module class advertisement");
                    return;
                }

                pipeAd = moduleSpecAd.getPipeAdvertisement();
            }

            discoveryService.remotePublish(moduleSpecAd, DiscoveryService.ADV);

            // we are now ready to start the service
            // create the input pipe endpoint clients will
            // use to connect to the service
            pipeService.createInputPipe(pipeAd, this);

        } catch (Exception ex) {
            ex.printStackTrace();
            logger.debug("Server: Error publishing the module");
        }
    }


    // By implementing PipeMsgListener, we define this method to deal with messages as they occur
    public void pipeMsgEvent(PipeMsgEvent event) {

        final int WAIT_TIMEOUT = 10*1000;

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
            logger.debug("null msg received");
            return;
        }

        logger.debug("Received message: " + newMessage);

        // verify message, then extract query and input pipe
        org.w3c.dom.Document doc = null;
        try {
            doc = db.parse(new InputSource(new StringReader(newMessage)));
        }
        catch (Exception e) {
            logger.warn("Could not parse message: " + e);
            return;
        }

        SignatureManagerResponse response = null;
        try {
            response = signatureManager.verifyDocument(doc);
        }
        catch (SignatureManagerException e) {
            logger.warn("Could not verify signature: " + e);

            // for now we continue anyway
            logger.warn("continuing anyway!");
        }

        if (response != null) {
            doc = response.getDocument();
        }

        String uddiResponse = null;
        {
            // extract SOAP Envelope
            logger.debug("extracting SOAP envelope");
            NodeList list = doc.getElementsByTagName(ENVELOPE_TAG);
            if (list == null || list.getLength() == 0) {
                logger.error("did not find envelope element in document");
                return;
            }

            Node envelopeNode = list.item(0);
            org.w3c.dom.Document envelopeDoc = db.newDocument();
            Node importedNode = envelopeDoc.importNode(envelopeNode,true);
            envelopeDoc.appendChild(importedNode);


            try {
                uddiResponse = messageDispatcher.dispatchMessage(envelopeDoc, response.getSigner());
            } catch (MessageDispatcherException e) {
                logger.error("could not dispatch message to UDDI registry: " + e);
                return;
            }
        }

        {
            // extract inputpipe element
            logger.debug("extracting pipe element");
            NodeList list = doc.getElementsByTagName(PIPE_TAG);
            if (list == null || list.getLength() == 0) {
                logger.error("did not find pipe element in document");
                return;
            }

            // Unfortunately JXTA makes it very hard to create PipeAdvertisements from existing XML...
            // the only way is to give it the actual XML for the pipe, which means we need to extract it
            // and feed it in a very inefficient way

            Node pipeAdNode = list.item(0);
            try {
                ByteArrayOutputStream out = new ByteArrayOutputStream();
                out.write(new String("<?xml version=\"1.0\"?><!DOCTYPE jxta:PipeAdvertisement>").getBytes());
                XMLUtils.outputDOM(pipeAdNode, out);

                logger.debug("creating pipeadvertisement from " + out.toString());

                PipeAdvertisement clientPipeAd = (PipeAdvertisement)
                        AdvertisementFactory.newAdvertisement(
                                xmlMimeMediaType, new ByteArrayInputStream(out.toByteArray()));

                OutputPipe clientPipe = pipeService.createOutputPipe(clientPipeAd, WAIT_TIMEOUT);

                Message outmsg = pipeService.createMessage();
                outmsg.setString(DATA_TAG,uddiResponse);

                clientPipe.send(outmsg);
                logger.debug("sent message");

            } catch (Exception e) {
                logger.error("Could not create client pipe");
                e.printStackTrace();
            }
        }
    }

}
