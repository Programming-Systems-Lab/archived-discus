package psl.discus.javasrc.services;

import psl.discus.javasrc.security.SecurityManager;
import psl.discus.javasrc.security.SecurityManagerException;

import java.rmi.Remote;
import java.rmi.RemoteException;

/**
 * @author Matias Pelenur
 */
public interface SecurityManagerService extends Remote  {

    /**
     * When a service space is going to invoke services on another service space, it first
     * creates a treaty for that service space containing a description of the services and
     * methods it wants to access.
     * This method processes a new treaty and returns a treaty containing a new treatyId
     * and the service,method,args entries that are actually authorized for the requesting
     * Service Space.
     *
     * @returns an XML Document conforming to the treaty schema
     */
    public String verifyTreaty(String signedTreatyXMLDoc)
            throws RemoteException;



}
