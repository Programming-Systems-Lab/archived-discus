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

import org.w3c.dom.Document;
import psl.discus.javasrc.schemas.treaty.Treaty;
import psl.discus.javasrc.schemas.treaty.ServiceInfo;
import psl.discus.javasrc.schemas.treaty.ServiceMethod;
import psl.discus.javasrc.schemas.treaty.TreatyDAO;
import psl.discus.javasrc.schemas.securityManagerResponse.SecurityManagerResponse;
import psl.discus.javasrc.shared.FakeDataSource;
import psl.discus.javasrc.shared.Util;

public class SecurityManagerImpl implements SecurityManager {

    public static final int STATUS_OK = 0;
    public static final int STATUS_ERROR = -1;

    DataSource ds;
    //DocumentBuilder documentBuilder;
    TreatyDAO treatyDAO;

    Random random;  // used to create random ids for treaties

    public SecurityManagerImpl(DataSource ds) {

        this.ds = ds;

        /*try {
            documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
        } catch (Exception e) {
            throw new RuntimeException("Could not instantiate documentbuilder :(. " + e.getMessage());
        }
        */
        random = new Random(System.currentTimeMillis());
        treatyDAO = new TreatyDAO(ds);
    }

    public String verifyTreaty(String signedTreatyXMLDoc)
            throws SecurityManagerException {

        try {

            // TODO: if necessary decrypt, and verify treaty document.
            // from the verification we get the service space id
            String treatyDoc = signedTreatyXMLDoc;
            int requesterId = 100;


            // first we need to make a Treaty instance object from the treaty document...
            StringReader reader = new StringReader(treatyDoc);
            Treaty treaty = Treaty.unmarshal(reader);
            reader.close();

            // now, for each service in the treaty, we get the authorized methods, and set
            // the authorized flag for each method accordingly
            ServiceInvokationPermissionDAO dao = new ServiceInvokationPermissionDAO(ds);

            for (Enumeration e = treaty.enumerateServiceInfo(); e.hasMoreElements();) {
                ServiceInfo serviceInfo = (ServiceInfo) e.nextElement();
                ServiceInvokationPermission permission = dao.getPermissions(requesterId, serviceInfo.getServiceName());

                for (Enumeration methods = serviceInfo.enumerateServiceMethod(); e.hasMoreElements();) {
                    ServiceMethod method = (ServiceMethod) e.nextElement();
                    MethodPermission mp = permission.getMethod(method.getMethodName());
                    if (mp == null) {
                        // method was not found in the permissions, set authorized to false
                        method.setAuthorized(false);
                    } else if (!mp.getParams().containsAll(Arrays.asList(method.getParameter()))) {
                        // permission did not contain all the params that are requested in the method
                        method.setAuthorized(false);
                    } else {
                        method.setAuthorized(true);
                    }

                    method.setNumInvokations(method.getNumInvokations());
                }
            }

            // now, assign the treaty an id number and store in the database
            treaty.setTreatyID(random.nextInt());
            treatyDAO.addTreaty(treaty);

            // finally, return the treaty inside a SecurityManagerResponse XML document
            StringWriter writer = new StringWriter();
            treaty.marshal(writer);
            writer.close();

            SecurityManagerResponse response = new SecurityManagerResponse();
            response.setStatus(STATUS_OK);
            response.setContent(writer.toString());

            writer = new StringWriter();
            response.marshal(writer);
            writer.close();

            return writer.toString();

        } catch (Exception e) {
            SecurityManagerResponse response = new SecurityManagerResponse();
            response.setStatus(STATUS_ERROR);
            response.setMessage(e.toString());

            StringWriter writer = new StringWriter();
            try {
                response.marshal(writer);
                writer.close();
            } catch (Exception e2) {
                throw new SecurityManagerException(e2);
            }

            return writer.toString();
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
        /*FileInputStream fin = new FileInputStream("mytreaty.xml");
        BufferedReader reader = new BufferedReader(new InputStreamReader(fin));

        String line = null;
        StringBuffer buf = new StringBuffer();
        while ((line=reader.readLine()) != null) {
            buf.append(line);
        }
        */
        SecurityManager manager = new SecurityManagerImpl(new FakeDataSource());
        String result = manager.verifyTreaty("foobar" /*buf.toString()*/);

        Util.debug(result);

    }
}
