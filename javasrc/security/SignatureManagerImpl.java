package psl.discus.javasrc.security;

import java.io.*;
import java.security.KeyStore;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.security.KeyStoreException;
import java.security.cert.X509Certificate;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerException;
import javax.sql.DataSource;
import javax.naming.Context;
import javax.naming.InitialContext;
import javax.naming.NamingException;

import org.apache.xml.security.Init;
import org.apache.xml.security.exceptions.XMLSecurityException;
import org.apache.xml.security.keys.KeyInfo;
import org.apache.xml.security.samples.utils.resolver.OfflineResolver;
import org.apache.xml.security.signature.XMLSignature;
import org.apache.xml.security.transforms.Transforms;
import org.apache.xml.security.utils.Constants;
import org.apache.xml.security.utils.XMLUtils;
import org.apache.xpath.XPathAPI;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.DOMException;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;


/**
 * Handles signing and verification of XML documents, using a KeyStore that holds the private and all the extenal
 * public keys.
 * @author matias
 */
public class SignatureManagerImpl implements SignatureManager {

    private static final String KEYSTORE_TYPE = "JKS";

    /* The following are retrieved from the web.xml file
    private static final String KEYSTORE_PASS = "discus";
    private static final String PRIVATEKEY_ALIAS = "100";
    private static final String PRIVATEKEY_PASS = "foobar";
    private static final String CERT_ALIAS = "100";
    */

    private static KeyStore keyStore;       // the keystore is retrieved from the database
    private static PrivateKey privateKey;

    private DocumentBuilder db;
    private String certAlias;

    public SignatureManagerImpl(DataSource ds)
            throws SignatureManagerException {

        Util.debug("Initializing SignatureManagerImpl");

        // lookup values using JNDI
        String keyStorePass=null, privateKeyAlias=null, privateKeyPass=null;

        try {
            Context initCtx = new InitialContext();
            Context envCtx = (Context) initCtx.lookup("java:comp/env");

            keyStorePass = (String) envCtx.lookup("KeyStorePass");
            privateKeyAlias = (String) envCtx.lookup("PrivateKeyAlias");
            privateKeyPass = (String) envCtx.lookup("PrivateKeyPass");
            certAlias = (String) envCtx.lookup("CertAlias");

        } catch (NamingException e) {
            throw new SignatureManagerException("Could not find datasource: " + e);
        }

        Util.print("Loading keystore from database...");
        if (keyStore == null) {
            // get keystore from the database
            try {
                KeyStoreDAO dao = new KeyStoreDAO(ds);
                keyStore = KeyStore.getInstance(KEYSTORE_TYPE);
                dao.loadKeyStore(0,keyStore,keyStorePass.toCharArray());
            } catch (Exception e) {
                throw new SignatureManagerException(e);
            }
        }
        Util.println("done.");

        Util.print("Getting private key...");
        if (privateKey == null) {
            // get privatekey from the keystore
            try {
                privateKey = (PrivateKey) keyStore.getKey(privateKeyAlias,privateKeyPass.toCharArray());
                if (privateKey == null)
                    throw new SignatureManagerException("Could not get PrivateKey");
            } catch (Exception e) {
                throw new SignatureManagerException(e);
            }
        }
        Util.println("done.");

        // we will need a documentbuilder to create XML documents -- instantiate one here
        DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
        dbf.setNamespaceAware(true);
        try {
            db = dbf.newDocumentBuilder();
        } catch (ParserConfigurationException e) {
            throw new SignatureManagerException(e);
        }
    }


    /**
     * (For web-service calls)
     * Signs the given XML document with this service space's private key.
     * @returns an array of two Strings, where the first is a status code (0 is OK)
     * and the second is either the signed XML document or the error message.
     */
    public String[] signDocument(String xml)
        throws SignatureManagerException {

        try {
            StringReader reader = new StringReader(xml);
            Document doc = db.parse(new InputSource(reader));

            Document signedDoc = signDocument(doc);
            ByteArrayOutputStream out = new ByteArrayOutputStream();
            XMLUtils.outputDOM(signedDoc,out);

            return new String[] { String.valueOf(STATUS_OK), out.toString() };

        } catch (Exception e) {
            return new String[] { String.valueOf(STATUS_ERROR), e.getMessage() } ;
        }

    }

    /**
     * Signs the given document with the private key, and adds the signature to the document passed.
     */
    public Document signDocument(Document givenDoc)
            throws SignatureManagerException {

        Util.print("Signing document...");
        try {
            // make a copy of the document, so we don't modify the one that was passed
            Document doc = (Document) givenDoc.cloneNode(true);

            XMLSignature sig = new XMLSignature(doc, "http://localhost/treaty",
                                                XMLSignature.ALGO_ID_SIGNATURE_DSA);

            doc.getFirstChild().appendChild(sig.getElement());
            sig.getSignedInfo()
                    .addResourceResolver(new OfflineResolver());

            {
                Transforms transforms = new Transforms(doc);

                transforms.addTransform(Transforms.TRANSFORM_ENVELOPED_SIGNATURE);
                transforms.addTransform(Transforms.TRANSFORM_C14N_WITH_COMMENTS);
                sig.addDocument("", transforms, Constants.ALGO_ID_DIGEST_SHA1);
            }

            {
                X509Certificate cert =
                        (X509Certificate) keyStore.getCertificate(certAlias);

                if (cert == null)
                    throw new SignatureManagerException("Could not get signing certificate!");

                sig.addKeyInfo(cert);

                sig.sign(privateKey);

            }

            Util.debug("done");
            return doc;
        } catch (Exception e) {
            throw new SignatureManagerException(e);
        }
    }

    /**
     * (For web-service calls)
     * Verifies a signed XML document and returns the document and the id of the signing service space
     * @returns an array three Strings, where the first is a status code (0 is OK),
     * the second element is the given xml document but without the signature, or the error message
     * and the third (if no error) is the signing service space id.
     */
    public String[] verifyDocument(String xml)
        throws SignatureManagerException {

        try {
            StringReader reader = new StringReader(xml);
            Document doc = db.parse(new InputSource(reader));

            FileOutputStream fout = new FileOutputStream("testout.xml");
            XMLUtils.outputDOM(doc,fout);

            SignatureManagerResponse vr = verifyDocument(doc);

            ByteArrayOutputStream out = new ByteArrayOutputStream();
            XMLUtils.outputDOM(vr.document,out);

            return new String[] { String.valueOf(STATUS_OK), out.toString(), vr.alias };

        } catch (Exception e) {
            return new String[] { String.valueOf(STATUS_ERROR), e.getMessage() };
        }


    }

    /**
     * Verifies a signed XML document and returns the document and the id of the signing service space
     */
    public SignatureManagerResponse verifyDocument(Document signedDoc)
            throws SignatureManagerException {

        Util.print("Verifying document...");

        SignatureManagerResponse vr = new SignatureManagerResponse();
        try {
            Element nscontext = XMLUtils.createDSctx(signedDoc, "ds", Constants.SignatureSpecNS);
            Element sigElement = (Element) XPathAPI.selectSingleNode(signedDoc,"//ds:Signature[1]", nscontext);

            if (sigElement == null)
                throw new SignatureManagerException("No signature found in document.");

            XMLSignature signature = new XMLSignature(sigElement, "http://localhost/treaty.xml");

            signature.addResourceResolver(new OfflineResolver());

            KeyInfo ki = signature.getKeyInfo();

            if (ki == null)
                throw new SignatureManagerException("No signature found in document.");

            if (!ki.containsX509Data())
                throw new SignatureManagerException("No certificate found in signature.");

            X509Certificate cert = signature.getKeyInfo().getX509Certificate();
            if (cert == null)
                throw new SignatureManagerException("Could not get certificate from signature.");

            // try to find this certificate in our keystore
            String alias = keyStore.getCertificateAlias(cert);
            if (alias == null)
                throw new SignatureManagerException("Certificate was not found on keystore.");

            // check for validity of signature
            if (!signature.checkSignatureValue(cert))
                throw new SignatureManagerException("Document did not pass verification!");

            vr.alias = alias;

            // NOTE: this makes a copy of the whole document. Look at instead copying over everything but the signature
            Document doc = (Document) signedDoc.cloneNode(true);
            nscontext = XMLUtils.createDSctx(doc, "ds", Constants.SignatureSpecNS);
            sigElement = (Element) XPathAPI.selectSingleNode(doc,"//ds:Signature[1]", nscontext);
            doc.getFirstChild().removeChild(sigElement);
            //XMLUtils.outputDOM(doc,new FileOutputStream("out.xml"));

            vr.document = doc;
            Util.debug("done");

        } catch (Exception e) {
            throw new SignatureManagerException(e);
        }

        return vr;

    }

    static {
        Init.init();
    }

    public static void main(String args[]) throws Exception {

        //testSig(args[0]);
        testVerify(args[0]);


    }

    private static void testSig(String filename) throws Exception {

        DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
        dbf.setNamespaceAware(true);
        DocumentBuilder db = dbf.newDocumentBuilder();
        Document d = db.parse(new File(filename));

        SignatureManager sigManager = new SignatureManagerImpl(new FakeDataSource());
        Document signed = sigManager.signDocument(d);

        File f = new File(filename + ".signed.xml");

        XMLUtils.outputDOM(signed, new FileOutputStream(f));

    }

    private static void testVerify(String filename) throws Exception {

        DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
        dbf.setNamespaceAware(true);
        DocumentBuilder db = dbf.newDocumentBuilder();
        Document d = db.parse(new File(filename + ".signed.xml"));

        SignatureManager sigManager = new SignatureManagerImpl(new FakeDataSource());
        SignatureManagerResponse  vr = sigManager.verifyDocument(d);

        Util.debug(vr.alias);
    }

}
