/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.auth;

import psl.discus.javasrc.uddi.util.Config;
import psl.discus.javasrc.uddi.error.JUDDIException;



import org.uddi4j.util.*;
import org.uddi4j.response.*;

import org.apache.log4j.Logger;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public class Auth
{
  // private reference to the jUDDI logger

  private static Logger log = Logger.getLogger(Auth.class);



  // class variables

  private static final String defaultClassName = "org.juddi.auth.SimpleAuthenticator";

  private static Auth instance = null;



  // instance variables

  private Authenticator authenticator;


  /**

   *

   */

  private Auth(Authenticator authenticator)

  {

    this.authenticator = authenticator;

  }



  /**

   * called only from org.juddi.util.Startup.startup() to get a head start on

   * aquiring neccessary resources and starting background threads.

   */

  public static void startup()

  {

    if (instance != null)

      shutdown();

    instance = getInstance();

  }



  /**

   * called only from org.juddi.util.Shutdown.shutdown() to release any

   * aquired resources and stop any background threads.

   */

  public static synchronized void shutdown()

  {

    // make sure we only attempt to stop this once (another thread

    // may have already taken care of this before us)



    if (instance == null)

      return;



    // release any aquired resources and stop any background threads.

    instance.authenticator.destroy();

    instance = null;

  }



  /**
   *
   */
  public static AuthToken getAuthToken(String userid,String credentials)
    throws JUDDIException
  {

    return getInstance().authenticator.getAuthToken(userid,credentials);

  }


  /**

   *

   */

  public static void validateAuthToken(AuthInfo authInfo)

    throws JUDDIException

  {

    getInstance().authenticator.validateAuthToken(authInfo);

  }



  /**
   *
   */
  public static String getAssociatedUserID(AuthInfo authInfo)
    throws JUDDIException
  {

    return getInstance().authenticator.getAuthorizedName(authInfo);
  }


  /**

   *

   */

  public static void discardAuthToken(AuthInfo authInfo)

    throws JUDDIException

  {

    getInstance().authenticator.discardAuthToken(authInfo);

  }



  /**
   *
   */
  private static Auth getInstance()
  {
    if (instance == null)
      instance = createInstance();
    return instance;
  }

  /**
   *
   */
  private static synchronized Auth createInstance()
  {
    // check to see if another thread beat us to this method.

    // If so simply return the Authenticator instance they

    // created.



    if (instance != null)

      return instance;



    // try to obtain the name of the Authenticator subclass



    String configClassName =

      Config.getProperty("juddi.authenticator.className");



    String className = (configClassName == null)

      ? defaultClassName

      : configClassName;



    log.info("juddi.authenticator.className = " + className);



    // create a new Authenticator instance using the

    // implementation specified in jUDDI properties.



    Authenticator authenticator = AuthenticatorFactory.getAuthenticator(className);

    // finally, construct and return the new Auth singlton



    return new Auth(authenticator);
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    try
    {
      // initialize low level components
      psl.discus.javasrc.uddi.util.SysManager.startup();

      // test a valid account and an invalid account
      System.out.println(Auth.getAuthToken("sviens","password").getAuthInfoString());
      System.out.println(Auth.getAuthToken("sviens","xxxxxxxx").getAuthInfoString());
    }
    catch(JUDDIException juddiex)
    {
      juddiex.printStackTrace();
    }
    finally
    {
      // release resources aquired by low level components
      psl.discus.javasrc.uddi.util.SysManager.shutdown();
    }
  }
}
