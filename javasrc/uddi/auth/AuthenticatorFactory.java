/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.auth;

import psl.discus.javasrc.uddi.util.Config;
import org.apache.log4j.Logger;

/**
 * Implementation of Factory pattern used to create an implementation of
 * the org.juddi.auth.Authenticator interface.
 *
 * The name of the Authenticator implementation to create is passed to the
 * getAuthenticator method.  If a null value is passed then the default
 * Authenticator implementation "org.juddi.auth.SimpleAuthenticator" is
 * created.
 *
 * @author  Steve Viens
 * @version 0.6
 */
public abstract class AuthenticatorFactory
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(AuthenticatorFactory.class);

  // default Authenticator implementation

  private static final String defaultAuthenticator =

      "org.juddi.auth.SimpleAuthenticator";

  /**
   *
   */
  public static synchronized Authenticator getAuthenticator(String authClassName)
  {
    if ((authClassName == null) || (authClassName.length() == 0))
    {
      log.warn("An implementation of the org.juddi.auth.Authenticator " +
                "interface was not specified. We're going to default " +
                "to: " + defaultAuthenticator);

      // use the default Authenticator implementation
      authClassName = defaultAuthenticator;
    }

    Authenticator authInst = null;

    try
    {
      Class authClass = Class.forName(authClassName);
      authInst = (Authenticator)authClass.newInstance();
    }
    catch (Exception ex)
    {
      log.error(ex);
    }

    return authInst;
  }

  /**
   * Release any aquired external resources and stop any background threads.
   */
  public static void destroyAuthenticator(Authenticator auth)
  {
    if (auth != null)
      auth.destroy();
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  // test driver
  public static void main(String[] args)
  {
    // extract the anme of the org.juddi.auth.Authenticator implementation
    // to create from the juddi.properties file.
    String authClassName = Config.getProperty("juddi.authenticator.className");
    if (authClassName == null)
      throw new RuntimeException("The property 'juddi.authenticator' has not been set!");

    // okay, let's create it!
    Authenticator authenticator = AuthenticatorFactory.getAuthenticator(authClassName);
    if (authenticator != null)
      System.out.println("Got an Authenticator: "+authClassName);
    else
      System.out.println("Nope! Couldn't create an Authenticator :(");

    // remember to call the Authenticator interfaces 'destroy' method to
    // release any system resources that may have been allocated by the
    // Authenticator.
    AuthenticatorFactory.destroyAuthenticator(authenticator);
  }
}