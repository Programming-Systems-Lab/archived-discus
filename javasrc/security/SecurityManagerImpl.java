/**
 * User: mp2079
 * Date: Mar 8, 2002
 * Time: 11:47:20 AM
 */
package psl.discus.javasrc.security;

import java.security.PrivateKey;
import java.security.PublicKey;
import java.util.Collection;
import java.util.Enumeration;
import java.util.Arrays;
import java.util.Random;
import java.io.*;

import javax.sql.DataSource;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.xml.sax.InputSource;
import psl.discus.javasrc.schemas.treaty.Treaty;
import psl.discus.javasrc.schemas.treaty.ServiceInfo;
import psl.discus.javasrc.schemas.treaty.ServiceMethod;
import psl.discus.javasrc.schemas.treaty.TreatyDAO;
import psl.discus.javasrc.schemas.securityManagerResponse.SecurityManagerResponse;
import psl.discus.javasrc.shared.FakeDataSource;
import psl.discus.javasrc.shared.Util;

public class SecurityManagerImpl implements SecurityManager {

    DataSource ds;
    SignatureManager signatureManager;
    TreatyDAO treatyDAO;
    DocumentBuilder db;

    Random random;  // used to create random ids for treaties

    public SecurityManagerImpl(DataSource ds, SignatureManager signatureManager)
        throws SecurityManagerException {

        this.ds = ds;
        this.signatureManager = signatureManager;

        random = new Random(System.currentTimeMillis());
        treatyDAO = new TreatyDAO(ds);

        DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
        dbf.setNamespaceAware(true);
        try {
            db = dbf.newDocumentBuilder();
        } catch (ParserConfigurationException e) {
            throw new SecurityManagerException(e);
        }

    }

    /**
     * Verifies the given treaty.
     * @return an array of 2 strings, where the first string is the status code (0 is OK)
     * and the second string is the content (either treaty XML or error message)
     */
    public String[] verifyTreaty(String treatyXML, boolean isSigned)
            throws SecurityManagerException {

        try {

            if (treatyXML == null)
                throw new SecurityManagerException("signedTreaty parameter is null");

            // TODO: if necessary decrypt

            // verify treaty document.
            // from the verification we get the service space id
            Treaty treaty = null;
            if (isSigned) {
                StringReader reader = new StringReader(treatyXML);
                Document signedTreatyDoc = db.parse(new InputSource(reader));
                SignatureManager.VerificationResponse vr = signatureManager.verifyDocument(signedTreatyDoc);

                treaty = Treaty.unmarshal(vr.document);
                Util.debug("(warning: this is a signed treaty, but using service space id from treaty and not cert)");
            }
            else {
                StringReader reader = new StringReader(treatyXML);
                treaty = Treaty.unmarshal(reader);
                reader.close();
            }

            // TODO TODO TODO! For signed treaties, get the service space id based on the alias of
            // the certificate, NOT just from the treaty!
            int requesterId = Util.parseInt(treaty.getClientServiceSpace());

            // now, for each service in the treaty, we get the authorized methods, and set
            // the authorized flag for each method accordingly
            ServiceInvokationPermissionDAO dao = new ServiceInvokationPermissionDAO(ds);

            for (Enumeration e = treaty.enumerateServiceInfo(); e.hasMoreElements();) {
                ServiceInfo serviceInfo = (ServiceInfo) e.nextElement();
                ServiceInvokationPermission permission = dao.getPermissions(requesterId, serviceInfo.getServiceName());

                for (Enumeration methods = serviceInfo.enumerateServiceMethod(); methods.hasMoreElements();) {
                    ServiceMethod method = (ServiceMethod) methods.nextElement();
                    MethodPermission mp = permission.getMethod(method.getMethodName(), method.getParameter());
                    if (mp == null) {
                        // method was not found in the permissions, set authorized to false
                        method.setAuthorized(false);
                    } else {
                        // method with this parameters was found. set authorized to true, and fill in
                        // the real methodImplementation
                        method.setAuthorized(true);
                        method.setMethodImplementation(mp.getMethodImplementation());
                        method.setNumInvokations(method.getNumInvokations());
                    }
                }
            }

            // now, assign the treaty an id number and store in the database
            treaty.setTreatyID(random.nextInt());
            treatyDAO.addTreaty(treaty);

            // finally, return the treaty inside a SecurityManagerResponse XML document
            StringWriter writer = new StringWriter();
            treaty.marshal(writer);
            writer.close();

            return new String[] { String.valueOf(STATUS_OK), writer.toString(), String.valueOf(requesterId) };

        } catch (Exception e) {
            return new String[] { String.valueOf(STATUS_ERROR), e.toString() };
        }

    }

    public String doRequestCheck(String signedRequestXMLDoc) {
        return null;
    }

    public String doResponseCheck(String signedResponseXMLDoc) {
        return null;
    }

    public String addServiceSpace(String serviceSpaceXMLDoc) {
        return null;
    }

    public String removeServiceSpace(int serviceSpaceId) {
        return null;
    }

    public String addPermission(String servicePermissionXMLDoc) {
        return null;
    }

    public String removePermission(String servicePermissionXMLDoc) {
        return null;
    }


    // for testing
    public static void main(String[] args)
        throws Exception {

        // read treaty file
        FileInputStream fin = new FileInputStream("mytreaty.xml.signed.xml");
        BufferedReader reader = new BufferedReader(new InputStreamReader(fin));

        String line = null;
        StringBuffer buf = new StringBuffer();
        while ((line=reader.readLine()) != null) {
            buf.append(line).append("\r\n");
        }

        SecurityManagerImpl manager = new SecurityManagerImpl(new FakeDataSource(),new SignatureManager());
        String[] result = manager.verifyTreaty(buf.toString(),true);

        Util.debug(result);

    }
}
