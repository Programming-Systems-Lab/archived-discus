package psl.discus.javasrc.schemas.treaty;

import java.io.FileWriter;
import java.io.FileReader;
import java.io.IOException;

/**
 * Author: Matias
 * Date: Mar 19, 2002
 * Time: 1:24:32 PM
 */
public class Test {

    public static void main(String args[]) {
        try {
            if (args.length > 0 && args[0].equals("read")) {
                FileReader reader = new FileReader("mytreaty.xml");
                Treaty treaty = Treaty.unmarshal(reader);

                debug("unmarshalled treaty, treatyid=" + treaty.getTreatyID() +
                        ", serviceinfos=" + treaty.getServiceInfoCount());

            } else {

                Treaty treaty = new Treaty();
                treaty.setTreatyID(1000);
                treaty.setClientServiceSpace("myservicespace");
                treaty.setProviderServiceSpace("providerss");

                // add one service info
                ServiceInfo serviceInfo = new ServiceInfo();
                serviceInfo.setServiceName("service");

                ServiceMethod method = new ServiceMethod();
                method.setMethodName("method");
                method.addParameter("foo");
                method.addParameter("bar");
                method.setNumInvokations(1);
                method.setAuthorized(true);

                serviceInfo.addServiceMethod(method);

                treaty.addServiceInfo(serviceInfo);

                FileWriter writer = new FileWriter("mytreaty.xml");
                treaty.marshal(writer);

                debug("marshalled treaty");
            }
        } catch (Exception e) {
            e.printStackTrace();

        }
    }

    private static void debug(String s) {
        System.out.println(s);
    }
}
