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
import psl.discus.javasrc.schemas.execServiceMethodRequest.ExecServiceMethodRequest;
import psl.discus.javasrc.schemas.treaty.ServiceInfo;
import psl.discus.javasrc.schemas.treaty.ServiceMethod;
import psl.discus.javasrc.schemas.treaty.Treaty;
import psl.discus.javasrc.schemas.treaty.TreatyDAO;
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
                // verify treaty document.
                // from the verification we get the service space id

                SignatureManager.VerificationResponse vr = signatureManager.verifyDocument(treatyDoc);
                treatyDoc = vr.document;
                requesterId = Util.parseInt(vr.alias);
                treaty.setClientServiceSpace(vr.alias);
            }

            treaty = Treaty.unmarshal(treatyDoc);
            if (requesterId == 0) requesterId = Util.parseInt(treaty.getClientServiceSpace());


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

            return new String[]{STATUS_OK, writer.toString(), String.valueOf(requesterId)};

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
                SignatureManager.VerificationResponse vr = signatureManager.verifyDocument(requestDoc);
                requestDoc = vr.document;
                requester = vr.alias;
            }

            request = ExecServiceMethodRequest.unmarshal(requestDoc);

            // now load treaty for this request, and look to see if we can find
            // a matching request in the treaty

            // right now, we are doing linear searches... but the Treaty class could use a
            // hashtable or something like it to make searching more efficient.

            Treaty treaty = treatyDAO.getTreaty(request.getTreatyID());
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
                            if (method.getParameterCount() == request.getParameterCount()) {
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
                    break;  // we already found the correct service, so search no more
                }
            }

            // if the method was authorized, we need to save changes to this treaty
            if (authorized) {
                treatyDAO.modifyTreaty(treaty.getTreatyID(), treaty);
                return new String[] { STATUS_OK , "OK" };
            }
            else {
                if (error == null)
                    error = "Service or method implementation not found.";
                return new String[] { STATUS_ERROR, error };
            }

        } catch (Exception e) {
            return new String[]{STATUS_ERROR, e.getMessage()};
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

        // read treaty file
        FileInputStream fin = new FileInputStream("mytreaty.xml.signed.xml");
        BufferedReader reader = new BufferedReader(new InputStreamReader(fin));

        String line = null;
        StringBuffer buf = new StringBuffer();
        while ((line = reader.readLine()) != null) {
            buf.append(line).append("\r\n");
        }

        SecurityManagerImpl manager = new SecurityManagerImpl(new FakeDataSource(), new SignatureManager());
        String[] result = manager.verifyTreaty(buf.toString(), true);

        Util.debug(result);

    }
}
