/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.service;

import psl.discus.javasrc.uddi.auth.*;
import psl.discus.javasrc.uddi.datastore.*;
import psl.discus.javasrc.uddi.error.*;
import psl.discus.javasrc.uddi.util.Config;


import org.uddi4j.UDDIElement;
import org.uddi4j.datatype.Name;
import org.uddi4j.request.FindBusiness;
import org.uddi4j.response.BusinessList;
import psl.discus.javasrc.uddi.util.DispositionReport;
import org.uddi4j.util.CategoryBag;
import org.uddi4j.util.DiscoveryURLs;
import org.uddi4j.util.FindQualifiers;
import org.uddi4j.util.FindQualifier;
import org.uddi4j.util.IdentifierBag;
import org.uddi4j.util.TModelBag;
import org.apache.log4j.Logger;

import java.util.Vector;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class FindBusinessService extends UDDIService
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(FindBusinessService.class);

  // private reference to the jUDDI logger
  private static DataStoreFactory factory = DataStoreFactory.getInstance();

  /**
   *
   */
  public UDDIElement invoke(UDDIElement request)
    throws JUDDIException
  {
    // morph UDDIElement into a specific request object
    return this.invoke((FindBusiness)request);
  }

  /**
   *
   */
  public BusinessList invoke(FindBusiness request)
    throws JUDDIException
  {
    Vector nameVector           = request.getNameVector();
    DiscoveryURLs discoveryURLs = request.getDiscoveryURLs();
    IdentifierBag identifierBag = request.getIdentifierBag();
    CategoryBag categoryBag     = request.getCategoryBag();
    TModelBag tModelBag         = request.getTModelBag();
    FindQualifiers qualifiers   = request.getFindQualifiers();
    int maxRows                 = request.getMaxRowsInt();

    // first make sure we need to continue with this request
    if (((nameVector == null)    || (nameVector.size() == 0))    && 
        ((discoveryURLs == null) || (discoveryURLs.size() == 0)) && 
        ((identifierBag == null) || (identifierBag.size() == 0)) && 
        ((categoryBag == null)   || (categoryBag.size() == 0)))
      return new BusinessList();

    // validate the 'name' parameters as much as possible up-front before 
    // calling into the data layer for relational validation.
    if (nameVector != null)
    {
      // only allowed to specify a maximum of 5 names (implementation dependent)

      int maxNameElementsAllowed = Config.getMaxNameElementsAllowed();
      if ((nameVector != null) && (nameVector.size() > maxNameElementsAllowed))
        throw new TooManyOptionsException(
          "A maximum of " +  maxNameElementsAllowed + " Names " +
          "may be specified in a find_business request.");
      
      // names can not exceed the maximum character length specified by the
      // UDDI specification (v2.0 specifies a max character length of 255).
      int maxNameLength = Config.getMaxNameLength();

      for (int i=0; i<nameVector.size(); i++)
      {
        String name = ((Name)nameVector.elementAt(i)).getText();
         if (name.length() > maxNameLength)
          throw new NameTooLongException(
            "Business Entity name '"+name+"' is longer than the " +
            "maximum allowed (max="+maxNameLength+").");
      }
    }

    // validate the 'qualifiers' parameter as much as possible up-front before 
    // calling into the data layer for relational validation.
    if (qualifiers != null)
    {
      // names can not exceed the maximum character length specified by the
      // UDDI specification (v2.0 specifies a max character length of 255).
      for (int i=0; i<qualifiers.size(); i++)
      {
        FindQualifier qualifier = (FindQualifier)qualifiers.get(i);
        String qText = qualifier.getText();

        if ((!qText.equals(FindQualifier.exactNameMatch)) ||
            (!qText.equals(FindQualifier.caseSensitiveMatch)) ||
            (!qText.equals(FindQualifier.orAllKeys)) ||
            (!qText.equals(FindQualifier.orLikeKeys)) ||
            (!qText.equals(FindQualifier.andAllKeys)) ||
            (!qText.equals(FindQualifier.sortByNameAsc)) ||
            (!qText.equals(FindQualifier.sortByNameDesc)) ||
            (!qText.equals(FindQualifier.sortByDateAsc)) ||
            (!qText.equals(FindQualifier.sortByDateDesc)) ||
            (!qText.equals(FindQualifier.serviceSubset)) ||
            (!qText.equals(FindQualifier.combineCategoryBags)))
          throw new UnsupportedException(
            "The FindQualifier '"+qText+"' is not supported.");
      }
    }

    // perform the requested action
    return find_business(nameVector,discoveryURLs,identifierBag,categoryBag,tModelBag,qualifiers,maxRows);
  }

  /**
   *
   */
  public BusinessList find_business(Vector nameVector,DiscoveryURLs discoveryURLs,IdentifierBag identifierBag,CategoryBag categoryBag,TModelBag tModelBag,FindQualifiers findQualifiers,int maxRows)
    throws JUDDIException
  {
    // aquire a jUDDI datastore instance
    DataStore datastore = factory.aquireDataStore();

    try
    {
      datastore.beginTrans();

      // TO-DO: do something here!
        
      datastore.commit();
    }
    catch(Exception ex)
    {
      // we must rollback for *any* exception
      try { datastore.rollback(); }
      catch(Exception e) { }
      
      // write to the log
      log.error(ex);

      // prep JUDDIException to throw
      if (ex instanceof JUDDIException)
        throw (JUDDIException)ex;
      else
        throw new JUDDIException(ex);  
    }
    finally
    {
      factory.releaseDataStore(datastore);
    }

    // create a new BusinessList and stuff 
    // the new businessVector into it.
    BusinessList list = new BusinessList();
    return list;
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    // initialize all jUDDI Subsystems
    psl.discus.javasrc.uddi.util.SysManager.startup();


    // create a request
    Vector nameVector = new Vector(5);

    nameVector.addElement(new Name("InflextionPoint"));

    nameVector.addElement(new Name("SteveViens.com"));

    nameVector.addElement(new Name("Liberty Mutual"));

    nameVector.addElement(new Name("Bowstreet"));

    nameVector.addElement(new Name("CMGi"));

    FindBusiness request = new FindBusiness();

    request.setNameVector(nameVector);

    request.setMaxRows(10);


    try
    {
      // invoke the service
      BusinessList response = (BusinessList)(new FindBusinessService().invoke(request));
      // to-do ... need more here!
    }
    catch(JUDDIException juddiex)
    {
      System.out.println(juddiex.toString());
    }
    finally
    {
      // terminate all jUDDI Subsystems
      psl.discus.javasrc.uddi.util.SysManager.shutdown();
    }
  }
}