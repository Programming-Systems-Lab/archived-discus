package psl.discus.javasrc.security;

import org.apache.xml.security.Init;
import org.apache.xml.security.keys.KeyInfo;
import org.apache.xml.security.samples.utils.resolver.OfflineResolver;
import org.apache.xml.security.signature.XMLSignature;
import org.apache.xml.security.transforms.Transforms;
import org.apache.xml.security.utils.Constants;
import org.apache.xml.security.utils.XMLUtils;
import org.apache.xpath.XPathAPI;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.xml.sax.InputSource;

import javax.naming.*;
import javax.sql.DataSource;
import javax.xml.parsers.*;
import java.io.*;
import java.security.*;
import java.security.cert.X509Certificate;
import java.util.Properties;

import psl.discus.javasrc.shared.FakeDataSource;
import psl.discus.javasrc.shared.Util;


/**
 * Handles signing and verification of XML documents, using a KeyStore that holds the private and all the extenal
 * public keys.
 * @author matias
 */
public class SignatureManagerImpl implements SignatureManager {

    /* The following are retrieved from the SecurityManager.properties file
    private static final String KEYSTORE_PASS = "discus";
    private static final String PRIVATEKEY_ALIAS = "100";
    private static final String PRIVATEKEY_PASS = "foobar";
    private static final String CERT_ALIAS = "100";
    */

    private static KeyStore keyStore;       // the keystore is retrieved from the database
    private static PrivateKey privateKey;

    static {
        Util.debug("Initializing crypto...");
        Init.init();
        Util.debug("Crypto initialized.");
    }

    private DocumentBuilder db;
    private String certAlias;
    private Signature signSignature;

    public SignatureManagerImpl(DataSource ds)
            throws SignatureManagerException {

        Util.debug("Initializing SignatureManagerImpl");

        String keyStorePass = null, privateKeyAlias = null, privateKeyPass = null;
        // lookup values using JNDI
        /*
        try {
            Context initCtx = new InitialContext();
            Context envCtx = (Context) initCtx.lookup("java:comp/env");

            keyStorePass = (String) envCtx.lookup("KeyStorePass");
            privateKeyAlias = (String) envCtx.lookup("PrivateKeyAlias");
            privateKeyPass = (String) envCtx.lookup("PrivateKeyPass");
            certAlias = (String) envCtx.lookup("CertAlias");

        } catch (NamingException e) {
            throw new SignatureManagerException("Could not find JNDI values: " + e);
        }
        */

        try {
            InputStream in = this.getClass().getClassLoader().getResourceAsStream("SecurityManager.properties");
            if (in == null) {
                throw new SignatureManagerException("SignatureManagerImpl: could not find SecurityManager.properties");
            }
            else {

                Properties props = new Properties();
                props.load(in);

                keyStorePass = props.getProperty("keyStorePass");
                privateKeyAlias = props.getProperty("privateKeyAlias");
                privateKeyPass = props.getProperty("privateKeyPass");
                certAlias = props.getProperty("certAlias");

            }
        } catch (Exception e) {
            throw new SignatureManagerException("SignatureManagerImpl: could not load properties: " + e);
        }

        Util.print("Loading keystore from database...");
        if (keyStore == null) {
            // get keystore from the database
            try {
                KeyStoreDAO dao = new KeyStoreDAO(ds);
                keyStore = KeyStore.getInstance(KeyStoreDAO.KEYSTORE_TYPE);
                dao.loadKeyStore(0, keyStore, keyStorePass.toCharArray());
            } catch (Exception e) {
                throw new SignatureManagerException(e);
            }
        }
        Util.println("done.");

        Util.print("Getting private key...");
        if (privateKey == null) {
            // get privatekey from the keystore
            try {
                privateKey = (PrivateKey) keyStore.getKey(privateKeyAlias, privateKeyPass.toCharArray());
                if (privateKey == null)
                    throw new SignatureManagerException("Could not get PrivateKey");
            } catch (Exception e) {
                throw new SignatureManagerException(e);
            }
        }
        Util.println("done.");

        try {
            signSignature = Signature.getInstance("SHA1withDSA");
            signSignature.initSign(privateKey);
        } catch (Exception e) {
            throw new SignatureManagerException(e);
        }



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
     * @return an array of two Strings, where the first is a status code (0 is OK)
     * and the second is either the signed XML document or the error message.
     */
    public String[] signDocument(String xml)
            throws SignatureManagerException {

        try {
            StringReader reader = new StringReader(xml);
            Document doc = db.parse(new InputSource(reader));

            Document signedDoc = signDocument(doc);
            ByteArrayOutputStream out = new ByteArrayOutputStream();
            XMLUtils.outputDOM(signedDoc, out);

            return new String[]{String.valueOf(STATUS_OK), out.toString()};

        } catch (Exception e) {
            return new String[]{String.valueOf(STATUS_ERROR), e.getMessage()};
        }

    }

    /**
     * Signs the given document with the private key, and adds the signature to the document passed.
     */
    public Document signDocument(Document givenDoc)
            throws SignatureManagerException {

        try {
            Util.info("signing document...");
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

            Util.info("signing done.");
            return doc;
        } catch (Exception e) {
            Util.debug("Error in signing: " + e);
            throw new SignatureManagerException(e);
        }
    }

    /*
    public byte[] signBytes(byte[] bytesToSign)
        throws SignatureManagerException {

        try {
            signSignature.update(bytesToSign);
            return signSignature.sign();
        }
        catch (Exception e)
        {
            throw new SignatureManagerException("could not sign bytes: " + e.getMessage());
        }
    }*/

    /**
     * (For web-service calls)
     * Verifies a signed XML document and returns the document and the id of the signing service space
     * @return an array three Strings, where the first is a status code (0 is OK),
     * the second element is the given xml document but without the signature, or the error message
     * and the third (if no error) is the signing service space id.
     */
    public String[] verifyDocument(String xml)
            throws SignatureManagerException {

        try {
            StringReader reader = new StringReader(xml);
            Document doc = db.parse(new InputSource(reader));

            FileOutputStream fout = new FileOutputStream("testout.xml");
            XMLUtils.outputDOM(doc, fout);

            SignatureManagerResponse vr = verifyDocument(doc);

            ByteArrayOutputStream out = new ByteArrayOutputStream();
            XMLUtils.outputDOM(vr.document, out);

            return new String[]{String.valueOf(STATUS_OK), out.toString(), vr.alias};

        } catch (Exception e) {
            return new String[]{String.valueOf(STATUS_ERROR), e.getMessage()};
        }


    }

    /**
     * Verifies a signed XML document and returns the document and the id of the signing service space
     */
    public SignatureManagerResponse verifyDocument(Document signedDoc)
            throws SignatureManagerException {

        Util.info("verifying document...");

        SignatureManagerResponse vr = new SignatureManagerResponse();
        try {
            Element nscontext = XMLUtils.createDSctx(signedDoc, "ds", Constants.SignatureSpecNS);
            Element sigElement = (Element) XPathAPI.selectSingleNode(signedDoc, "//ds:Signature[1]", nscontext);

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
            sigElement = (Element) XPathAPI.selectSingleNode(doc, "//ds:Signature[1]", nscontext);
            doc.getFirstChild().removeChild(sigElement);
            //XMLUtils.outputDOM(doc,new FileOutputStream("out.xml"));

            vr.document = doc;
            Util.info("verification done.");

        } catch (Exception e) {
            throw new SignatureManagerException(e);
        }

        return vr;

    }

    /**     * Gets a known certificate. This can be used by the SecurityManager to     * vouch for other service spaces by signing their certificates (public keys)     * @return the Base64-encoded certificate     */
    public String getCertificate(String cerificateAlias)        throws SignatureManagerException {
        throws new SignatureManagerException("not implemented yet");
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
        SignatureManagerResponse vr = sigManager.verifyDocument(d);

        Util.debug(vr.alias);
    }

}
