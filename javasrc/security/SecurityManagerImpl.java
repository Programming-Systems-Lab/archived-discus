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

import javax.sql.DataSource;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Document;
import psl.discus.javasrc.schemas.treaty.Treaty;
import psl.discus.javasrc.schemas.treaty.ServiceInfo;
import psl.discus.javasrc.schemas.treaty.ServiceMethod;
import psl.discus.javasrc.schemas.treaty.TreatyDAO;

public abstract class SecurityManagerImpl implements SecurityManager {

    DataSource ds;
    DocumentBuilder documentBuilder;
    TreatyDAO treatyDAO;

    Random random;  // used to create random ids for treaties

    public SecurityManagerImpl(DataSource ds) {

        this.ds = ds;

        try {
            documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
        } catch (Exception e) {
            throw new RuntimeException("Could not instantiate documentbuilder :(. " + e.getMessage());
        }

        random = new Random(System.currentTimeMillis());
        treatyDAO = new TreatyDAO(ds);
    }

    public Document verifyTreaty(ServiceSpace requester, Document treatyDoc)
            throws SecurityManagerException {

        Document newTreaty = null;

        try {
            // first we need to make a Treaty instance object from the treaty document...
            Treaty treaty = Treaty.unmarshal(treatyDoc);

            // now, for each service in the treaty, we get the authorized methods, and set
            // the authorized flag for each method accordingly
            ServiceInvokationPermissionDAO dao = new ServiceInvokationPermissionDAO(ds);

            for (Enumeration e = treaty.enumerateServiceInfo(); e.hasMoreElements();) {
                ServiceInfo serviceInfo = (ServiceInfo) e.nextElement();
                ServiceInvokationPermission permission = dao.getPermissions(requester, serviceInfo.getServiceName());

                for (Enumeration methods = serviceInfo.enumerateServiceMethod(); e.hasMoreElements();) {
                    ServiceMethod method = (ServiceMethod) e.nextElement();
                    MethodPermission mp = permission.getMethod(method.getMethodName());
                    if (mp == null) {
                        // method was not found in the permissions, set authorized to false
                        method.setAuthorized(false);
                    }
                    else if (!mp.getParams().containsAll(Arrays.asList(method.getParameter()))) {
                        // permission did not contain all the params that are requested in the method
                        method.setAuthorized(false);
                    }
                    else {
                        method.setAuthorized(true);
                    }

                    // TODO: set the numInvokations to some meaningful number
                    method.setNumInvokations(1);
                }
            }

            // now, assign the treaty an id number and store in the database
            treaty.setTreatyID(random.nextInt());
            treatyDAO.addTreaty(treaty);

            // finally, return the treaty as an XML document
            newTreaty = documentBuilder.newDocument();
            treaty.marshal(newTreaty);

        } catch (Exception e) {
            throw new SecurityManagerException(e);
        }

        return newTreaty;
    }

}
