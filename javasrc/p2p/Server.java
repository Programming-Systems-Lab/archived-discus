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
import org.apache.log4j.Level;
import org.apache.log4j.Logger;

/**
 *
 * Initial implementation of the JXTA server for listening to p2p UDDI requests
 * Most of this was borrowed directly from the JXTA Programmer's Guide, http://www.jxta.org/jxtaprogguide_final.pdf
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
 * pipe that waits to receive messages.
 */
public class Server implements PipeMsgListener {

    static PeerGroup group = null;
    private DiscoveryService discoveryService;
    private PipeService pipeService;

    private static Logger logger = Logger.getLogger(Server.class);
    public static final String SERVICE_NAME = "discusUddi";
    public static final String MODULE_ADV_FILE = "module.adv";
    public static final String MODULE_SPEC_ADV_FILE = "modulespec.adv";
    public static final String PIPE_ADV_FILE = "pipe.adv";
    public static final String DATA_TAG = "Data";
    public static final String INPUT_PIPE_TAG = "InputPipe";
    public static final MimeMediaType xmlMimeMediaType = new MimeMediaType("text/xml");

    static {
        logger.setLevel(Level.DEBUG);
    }

    public static void main(String args[]) {
        Server myapp = new Server();
        logger.debug("Starting Service Peer ....");
        myapp.startJxta();

    }

    public Server() {
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

                // TODO: add a signature and/or public key to this advertisement
                StructuredDocument signatureDoc =
                        StructuredDocumentFactory.newStructuredDocument(xmlMimeMediaType,"Parm","mysig");

                moduleSpecAd.setParam(signatureDoc);


                // display the advertisement as a plain text document.
                logger.debug("Created service advertisement:");
                StructuredTextDocument doc = (StructuredTextDocument)
                        moduleSpecAd.getDocument(new MimeMediaType("text/xml"));
                StringWriter str_out = new StringWriter();
                doc.sendToWriter(str_out);
                logger.debug(str_out.toString());
                str_out.close();

                // Ok the Module advertisement was created, just publish
                // it in my local cache and into the NetPeerGroup.
                discoveryService.publish(moduleSpecAd, DiscoveryService.ADV);

                // and save it to file
                FileOutputStream fout = new FileOutputStream(moduleSpecAdFile);
                BufferedWriter out = new BufferedWriter(new OutputStreamWriter(fout));
                doc = (StructuredTextDocument)
                        moduleSpecAd.getDocument(xmlMimeMediaType);
                doc.sendToWriter(out);
                out.flush();
                out.close();
                logger.debug("saved new module spec advertisment");
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
        MessageElement inputPipeElement = msg.getElement(INPUT_PIPE_TAG);
        if (newMessage == null)
            logger.debug("null msg received");
        else
            logger.info("Received message: " + newMessage);

        if (inputPipeElement != null) {

            try {
                PipeAdvertisement clientPipeAd = (PipeAdvertisement)
                                    AdvertisementFactory.newAdvertisement(
                                            xmlMimeMediaType, inputPipeElement.getStream());
                OutputPipe clientPipe = pipeService.createOutputPipe(clientPipeAd, WAIT_TIMEOUT);

                Message outmsg = pipeService.createMessage();
                outmsg.setString(DATA_TAG,"Hello there old chap!");

                clientPipe.send(outmsg);
                logger.debug("sent message");

            } catch (IOException e) {
                logger.error("Could not create client pipe: " + e);
            }
        }
    }
}
