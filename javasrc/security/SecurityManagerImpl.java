/**
 * User: mp2079
 * Date: Mar 8, 2002
 * Time: 11:47:20 AM
 */
package psl.discus.javasrc.security;

import java.io.*;
import java.util.Enumeration;
import java.util.Random;

import javax.sql.DataSource;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.xml.sax.InputSource;
import org.apache.xml.security.utils.XMLUtils;
import org.apache.log4j.Logger;
import psl.discus.javasrc.schemas.execServiceMethodRequest.ExecServiceMethodRequest;
import psl.discus.javasrc.schemas.treaty.ServiceInfo;
import psl.discus.javasrc.schemas.treaty.ServiceMethod;
import psl.discus.javasrc.schemas.treaty.Treaty;
import psl.discus.javasrc.shared.*;

public class SecurityManagerImpl implements SecurityManager {

    private static final Logger logger = Logger.getLogger(SecurityManagerImpl.class);

    private DataSource ds;
    private SignatureManager signatureManager;
    private TreatyDAO treatyDAO;
    private DocumentBuilder db;

    private Random random;  // used to create random ids for treaties

    private static final Logger treatyLogger = Logger.getLogger("TreatyLogger");
    private static final Logger encryptedTreatyLogger = Logger.getLogger("EncryptedTreatyLogger");

    public SecurityManagerImpl(DataSource ds, SignatureManager signatureManager)
            throws SecurityManagerException {

        logger.debug("Initializing SecurityManagerImpl");

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
    public String[] verifyTreaty(String treatyXML, boolean signed) {

        try {

            if (treatyXML == null)
                throw new SecurityManagerException("signedTreaty parameter is null");

            // TODO: if necessary decrypt

            Treaty treaty = null;
            int requesterId = 0;
            StringReader reader = new StringReader(treatyXML);
            Document treatyDoc = db.parse(new InputSource(reader));
            if (signed) {
                encryptedTreatyLogger.info(treatyXML.replace('\n',' '));

                // verify treaty document.
                // from the verification we get the service space id

                SignatureManagerResponse vr = signatureManager.verifyDocument(treatyDoc);
                treatyDoc = vr.document;
                requesterId = Util.parseInt(vr.alias);
                logger.info("verifying signed treaty from service space " + requesterId);

            }

            treaty = Treaty.unmarshal(treatyDoc);

            // if treaty is unsigned, (requestid==0), just use the service space id they gave us
            if (requesterId == 0) {
                requesterId = Util.parseInt(treaty.getClientServiceSpace());
                logger.info("verifying unsigned treaty as if it came from service space " + requesterId);
            }
            else {   // otherwise, use the one from the signature
                treaty.setClientServiceSpace(String.valueOf(requesterId));
            }

            // now, for each service in the treaty, we get the authorized methods, and set
            // the authorized flag for each method accordingly
            ServicePermissionDAO dao = new ServicePermissionDAO(ds);

            for (Enumeration e = treaty.enumerateServiceInfo(); e.hasMoreElements();) {
                ServiceInfo serviceInfo = (ServiceInfo) e.nextElement();
                ServicePermission permission = dao.getPermissions(requesterId, serviceInfo.getServiceName());

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
                        method.setNumInvokations(mp.getNumberInvokations());
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

            logger.debug("Done verifying treaty.");
            String treatyData = writer.toString();
            treatyLogger.info(treatyData.replace('\n',' '));
            return new String[]{STATUS_OK, treatyData, String.valueOf(requesterId)};

        } catch (Exception e) {
            return new String[]{STATUS_ERROR, e.toString()};
        }

    }

    public String[] doRequestCheck(String requestXML) {
        return doRequestCheck(requestXML, true);
    }

    public String[] doRequestCheck(String requestXML, boolean signed) {

        try {
            ExecServiceMethodRequest request = null;

            if (requestXML == null)
                throw new SecurityManagerException("requestXML parameter is null");

            // TODO: if necessary decrypt

            StringReader reader = new StringReader(requestXML);
            Document requestDoc = db.parse(new InputSource(reader));
            String requester = null;
            if (signed) {
                // verify document.
                // from the verification we get the service space id
                SignatureManagerResponse vr = signatureManager.verifyDocument(requestDoc);
                requestDoc = vr.document;
                requester = vr.alias;
            }

            request = ExecServiceMethodRequest.unmarshal(requestDoc);

            // now load treaty for this request, and look to see if we can find
            // a matching request in the treaty

            // right now, we are doing linear searches... but the Treaty class could use a
            // hashtable or something like it to make searching more efficient.

            if (requester != null) {
                logger.info("performing signed request check for service space " + requester +
                            "with treaty id " + request.getTreatyID());
            }
            else {
                logger.info("performing unsigned request check with treaty id " + request.getTreatyID());
            }

            TreatyData treatyData = treatyDAO.getTreaty(request.getTreatyID());
            if (treatyData.getStatus() != TreatyData.STATUS_CURRENT)
                throw new SecurityManagerException("Treaty has been revoked or is not current.");

            Treaty treaty = treatyData.getTreaty();
            if (requester != null && !treaty.getClientServiceSpace().equals(requester))
                throw new SecurityManagerException("Treaty requester does not match execute method requester");

            boolean authorized = false;
            String error = null;
            for (Enumeration e = treaty.enumerateServiceInfo(); e.hasMoreElements();) {
                ServiceInfo serviceInfo = (ServiceInfo) e.nextElement();
                if (serviceInfo.getServiceName().equals(request.getServiceName())) {
                    for (Enumeration methods = serviceInfo.enumerateServiceMethod(); methods.hasMoreElements();) {
                        ServiceMethod method = (ServiceMethod) methods.nextElement();
                        if (method.getMethodImplementation().equals(request.getMethodName())) {
                            if (method.getParameterCount() == request.getFilledParameterCount()) {
                                int numInvokations = method.getNumInvokations();
                                if (numInvokations > 0) {
                                    // OK! method is authorized
                                    authorized = true;
                                    method.setNumInvokations(numInvokations - 1);
                                } else
                                    error = "Exceeded allowed number of invokations";
                            } else
                                error = "Parameter count does not match treaty";
                            break;  // we already found the correct method, so search no more
                        }
                    }
                    /// removed: break;  // we already found the correct service, so search no more
                    /// because: we may get several ServiceInfo tags for the same service
                }
            }

            // if the method was authorized, we need to save changes to this treaty
            if (authorized) {
                treatyDAO.updateTreaty(treaty);

            } else {
                if (error == null)
                    error = "Service or method implementation not found.";
            }

            // write back request document into string to return it
            // (if it was signed, we return an unsigned version for the gatekeeper)
            ByteArrayOutputStream out = new ByteArrayOutputStream();
            XMLUtils.outputDOM(requestDoc,out);
            String returnXML = out.toString();

            logger.info("request check done. Result: " + (error == null ? "passed" : "not passed: " + error));

            if (error == null)
                return new String[] {STATUS_OK, returnXML};
            else
                return new String[] {STATUS_ERROR, error};

        } catch (Exception e) {
            logger.info("request check not passed: " + e.getMessage());
            return new String[]{STATUS_ERROR, e.getMessage()};
        }

    }

    /**
     * Revokes a treaty. Returns STATUS_OK if revoked OK, or STATUS_ERROR and an error message
     * if the operation failed.
     */
    public String[] revokeTreaty(int treatyid) {

        try {
            treatyDAO.updateTreatyStatus(treatyid, TreatyData.STATUS_REVOKED);
            return new String[] { STATUS_OK, "OK" };
        } catch (DAOException e) {
            return new String[] { STATUS_ERROR, e.getMessage() };
        }
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

        treatyLogger.info("testing");

        // read treaty file
        FileInputStream fin = new FileInputStream("mytreaty.xml.signed.xml");
        BufferedReader reader = new BufferedReader(new InputStreamReader(fin));

        String line = null;
        StringBuffer buf = new StringBuffer();
        while ((line = reader.readLine()) != null) {
            buf.append(line).append("\r\n");
        }

        DataSource ds = new FakeDataSource();
        SecurityManagerImpl manager = new SecurityManagerImpl(ds, new SignatureManagerImpl(ds));
        String[] result = manager.verifyTreaty(buf.toString(), true);

        logger.debug(result);

    }
}
