/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.auth;

import psl.discus.javasrc.uddi.error.JUDDIException;
import psl.discus.javasrc.uddi.error.AuthTokenExpiredException;
import psl.discus.javasrc.uddi.error.AuthTokenRequiredException;
import psl.discus.javasrc.uddi.error.UnknownUserException;
import psl.discus.javasrc.uddi.util.Config;
import psl.discus.javasrc.uddi.uuidgen.UUID;

import org.apache.log4j.Logger;
import org.xml.sax.Attributes;
import org.xml.sax.ContentHandler;
import org.xml.sax.ErrorHandler;
import org.xml.sax.InputSource;
import org.xml.sax.Locator;
import org.xml.sax.SAXException;
import org.xml.sax.SAXParseException;
import org.xml.sax.XMLReader;
import org.uddi4j.util.AuthInfo;
import org.uddi4j.response.AuthToken;

import javax.xml.parsers.ParserConfigurationException;
import javax.xml.parsers.SAXParserFactory;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStream;
import java.io.IOException;
import java.util.Date;
import java.util.Enumeration;
import java.util.Hashtable;

/**
 * This is a simple implementation of jUDDI's Authenticator interface. The credential
 * store is simply an unencrypted xml document called 'juddi.users' that can be
 * found in jUDDI's config directory. Below is an example of what you might find
 * in this document. The name and admin attributes are not currently used.
 *
 *     Example juddi-users.xml document:
 *     =================================
 *     <?xml version="1.0" encoding="UTF-8"?>
 *     <juddi-users>
 *       <user userid="griddell"    password="password"   name="Graeme Riddell" admin="false" />
 *       <user userid="sviens"      password="password"   name="Steve Viens"    admin="true" />
 *       <user userid="aceponkus"   password="password"   name="Alex Ceponkus"  admin="false" />
 *       <user userid="bhablutzel"  password="password"   name="Bob Hablutzel"  admin="false" />
 *       <user userid="mcoene"      password="password"   name="Maarten Coene"  admin="false" />
 *     </juddi-users>
 *
 * @author  Steve Viens
 * @version 0.6
 */
public class SimpleAuthenticator implements ContentHandler, ErrorHandler, Authenticator
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(SimpleAuthenticator.class);

  private static final String usersFileName = "conf"+File.separator+"juddi.users";
  private static final long defaultSweepDelay = 3600000; // 60 minutes
  private static final long defaultMaxInactivity = 1200000;  // 20 minutes

  private Hashtable userTable;      // hashtable of UserInfo objects
  private Hashtable tokenTable;     // hashtable of TokenInfo objects
  private TokenSweeper sweeper;     // thread to maintian the TokenInfo Hashtable
  private long delayBetweenSweeps;  // millisec delay between removing expired tokens
  private long maximumInactivity;   // millisec before expiring inactive tokens

  private class TokenSweeper extends Thread
  {
    public void run()
    {
      try
      {
        while (true)
        {
          sleep(delayBetweenSweeps);

          Enumeration enum = tokenTable.keys();
          while (enum.hasMoreElements())
          {
            String tokenKey = (String)enum.nextElement();
            TokenInfo tokenInfo = (TokenInfo)tokenTable.get(tokenKey);

            long lastUsed = tokenInfo.lastUsed.getTime();
            long currTime = new Date().getTime();

            // if the authToken has exceeded the allowed inactive
            // time-span then remove the expired TokenInfo object
            // from the tokenTable Hashtable.
            if ((lastUsed + maximumInactivity) <= currTime)
              tokenTable.remove(tokenKey);
          }
        }
      }
      catch(InterruptedException e)
      {
        /* TokenSweeper Thread Interrupted */
      }
    }
  }

  class UserInfo
  {
    public String name;
    public String userid;
    public String password;
    public boolean admin;

    public String toString()
    {
      StringBuffer buff = new StringBuffer(50);
      buff.append(name);
      buff.append(" | ");
      buff.append(userid);
      buff.append(" | ");
      buff.append(password);
      buff.append(" | admin=");
      buff.append(admin);
      return buff.toString();
    }
  }

  class TokenInfo
  {
    public String authToken;
    public String userid;
    public Date lastUsed;

    public String toString()
    {
      StringBuffer buff = new StringBuffer(50);
      buff.append(authToken);
      buff.append(" | ");
      buff.append(lastUsed.getTime());
      return buff.toString();
    }
  }

  /**
   *
   */
  public SimpleAuthenticator()
  {
    super();
  }

  /**
   * Perform authenticator initialization tasks
   */
  public synchronized void init()
  {
    // make sure we only create amd start one of these (another thread
    // may have already taken care of this before us)
    if (sweeper != null)
      return;

    // create and populate a Hashtable of UserInfo objects (one per user)
    try {
      userTable = new Hashtable();
      build(new FileInputStream(getUsersFileName()));
    }
    catch (IOException ioex) {
      ioex.printStackTrace();
    }
    catch (SAXException saxex) {
      saxex.printStackTrace();
    }
    catch (ParserConfigurationException pcex) {
      pcex.printStackTrace();
    }

    // create and start the Expired Token Sweeper (aka expired token remover)
    Long sweepDelayLong = Config.getPropertyLong("juddi.authenticator.sweepInterval");
    if (sweepDelayLong != null)
      delayBetweenSweeps = sweepDelayLong.longValue();
    else
      delayBetweenSweeps = SimpleAuthenticator.defaultSweepDelay; // using default;

    Long timeoutDelayLong = Config.getPropertyLong("juddi.authenticator.authInfoTimeout");
    if (timeoutDelayLong != null)
      maximumInactivity = timeoutDelayLong.longValue();
    else
      maximumInactivity = SimpleAuthenticator.defaultMaxInactivity; // using default;

    // create a Hashtable for TokenInfo objects
    tokenTable = new Hashtable();

    // create and start the TokenSweeper thread
    sweeper = new TokenSweeper();
    sweeper.start();
  }

  /**
   * Perform authenticator termination tasks
   */
  public synchronized void destroy()
  {
    // make sure we only attempt to stop this once (another thread
    // may have already taken care of this before us)
    if (sweeper == null)
      return;

    sweeper.interrupt();
    sweeper = null;
  }

  /**
   *
   */
  public AuthToken getAuthToken(String authorizedName,String credential)
    throws JUDDIException
  {
    // start the token sweeper if it hasn't been started yet.
    if (sweeper == null)
      this.init();

    if (!userTable.containsKey(authorizedName))
      throw new UnknownUserException("Unknown user");

    UserInfo userInfo = (UserInfo)userTable.get(authorizedName);

    if (!credential.equals(userInfo.password))
      throw new UnknownUserException("Invalid cridentials");

    TokenInfo tokenInfo = new TokenInfo();
    tokenInfo.authToken = UUID.nextID();
    tokenInfo.userid = userInfo.userid;
    tokenInfo.lastUsed = new Date();
    tokenTable.put(tokenInfo.authToken,tokenInfo);

    AuthToken authToken = new AuthToken();
    authToken.setAuthInfo(tokenInfo.authToken);

    return authToken;
  }

  /**
   *
   */
  public void validateAuthToken(AuthInfo authInfo)
    throws JUDDIException
  {
    // start the token sweeper if it hasn't been started yet.
    if (sweeper == null)
      this.init();

    if ((authInfo == null) || (authInfo.getText() == null))
      throw new AuthTokenRequiredException("The authInfo argument is null");

    String authToken = authInfo.getText();
    if (authToken.trim().length() == 0)
      throw new AuthTokenRequiredException("The authToken specified has a length of zero");

    TokenInfo tokenInfo = (TokenInfo)tokenTable.get(authToken);
    if (tokenInfo == null)
      throw new AuthTokenRequiredException("The authToken specified does not exist: "+authInfo.getText());

    long lastUsed = tokenInfo.lastUsed.getTime();
    long currTime = new Date().getTime();

    if ((lastUsed + maximumInactivity) <= currTime)
      throw new AuthTokenExpiredException("The authToken specified has expired: "+authInfo.getText());

    // basically 'touch' this token's lastUsed date
    tokenInfo.lastUsed = new Date();
  }

  /**
   *
   */
  public String getAuthorizedName(AuthInfo authInfo)
    throws JUDDIException
  {
    // start the token sweeper if it hasn't been started yet.
    if (sweeper == null)
      this.init();

    if ((authInfo == null) || (authInfo.getText() == null))
      throw new AuthTokenRequiredException("The authInfo argument is null");

    String authToken = authInfo.getText();
    if (authToken.trim().length() == 0)
      throw new AuthTokenRequiredException("The authToken specified has a length of zero");

    TokenInfo tokenInfo = (TokenInfo)tokenTable.get(authToken);
    if (tokenInfo == null)
      throw new AuthTokenRequiredException("The authToken specified does not exist");

    long lastUsed = tokenInfo.lastUsed.getTime();
    long currTime = new Date().getTime();

    if ((lastUsed + maximumInactivity) <= currTime)
      throw new AuthTokenExpiredException("The authToken specified has expired");

    // basically 'touch' this token's lastUsed date
    tokenInfo.lastUsed = new Date();

    return tokenInfo.userid;
  }

  /**

   *

   */

  public void discardAuthToken(AuthInfo authInfo)

    throws JUDDIException

  {

    // start the token sweeper if it hasn't been started yet.

    if (sweeper == null)

      this.init();



    if ((authInfo == null) || (authInfo.getText() == null))

      throw new AuthTokenRequiredException("The authInfo argument is null");



    String authToken = authInfo.getText();

    if (authToken.trim().length() == 0)

      throw new AuthTokenRequiredException("The authToken value specified has a length of zero");



    if (!tokenTable.containsKey(authToken))

      throw new AuthTokenRequiredException("The authToken value specified does not exist");



    tokenTable.remove(authToken);

  }



  /**
   *
   */
  public String toString()
  {
    StringBuffer buff = new StringBuffer(100);

    Enumeration enum = userTable.keys();
    while (enum.hasMoreElements())
    {
      UserInfo userInfo = (UserInfo)userTable.get((String)enum.nextElement());
      buff.append(userInfo.toString()+"\n");
    }

    return buff.toString();
  }


  /***************************************************************************/
  /*************************** LOW LEVEL CALLS *******************************/
  /***************************************************************************/


  /**
   *
   */
  public Hashtable build(InputStream istream)
    throws ParserConfigurationException,SAXException,IOException
  {
    SAXParserFactory spf = SAXParserFactory.newInstance();
    spf.setNamespaceAware(true);

    XMLReader xr = spf.newSAXParser().getXMLReader();
    xr.setContentHandler(this);
    xr.setErrorHandler(this);
    xr.parse(new InputSource(istream));

    return (Hashtable)this.getObject();
  }

  /**
   * handle setDocumentLocator event
   */
  public void setDocumentLocator(org.xml.sax.Locator locator)
  {
  }

  /**
   * handle startDocument event
   */
  public void startDocument()
    throws SAXException
  {
  }

  /**
   * handle endDocument event
   */
  public void endDocument()
    throws SAXException
  {
  }

  /**
   * handle startElement event
   */
  public void startElement(String uri,String name,String qName,Attributes attributes)
    throws SAXException
  {
    if (name.equalsIgnoreCase("user"))
    {
      UserInfo userInfo = new UserInfo();

      for(int i=0; i<attributes.getLength(); i++)
      {
        if (attributes.getQName(i).equalsIgnoreCase("userid"))
          userInfo.userid = attributes.getValue(i);
        else if (attributes.getQName(i).equalsIgnoreCase("password"))
          userInfo.password = attributes.getValue(i);
        else if (attributes.getQName(i).equalsIgnoreCase("name"))
          userInfo.name = attributes.getValue(i);
        else if (attributes.getQName(i).equalsIgnoreCase("admin"))
          userInfo.admin = new Boolean(attributes.getValue(i)).booleanValue();
      }

      userTable.put(userInfo.userid,userInfo);
    }
  }

  /**
   * handle endElement event
   */
  public void endElement(String name,String string2,String string3)
    throws SAXException
  {
  }

  /**
   * handle characters event
   */
  public void characters(char[] chars,int int1, int int2)
    throws SAXException
  {
  }

  /**
   * handle ignorableWhitespace event
   */
  public void ignorableWhitespace(char[] chars,int int1, int int2)
    throws SAXException
  {
  }

  /**
   * handle processingInstruction event
   */
  public void processingInstruction(String string1,String string2)
    throws SAXException
  {
  }

  /**
   * handle startPrefixMapping event
   */
  public void startPrefixMapping(String string1,String string2)
    throws SAXException
  {
  }

  /**
   * handle endPrefixMapping event
   */
  public void endPrefixMapping(String string)
    throws SAXException
  {
  }

  /**
   * handle skippedEntity event
   */
  public void skippedEntity(String string)
    throws SAXException
  {
  }

  /**
   * handle warning event
   */
  public void warning(SAXParseException spex)
    throws SAXException
  {
  }

  /**
   * handle error event
   */
  public void error(SAXParseException spex)
    throws SAXException
  {
  }

  /**
   * handle fatalError event
   */
  public void fatalError(SAXParseException spex)
    throws SAXException
  {
  }

  /**
   * Retrieve the object built by the handling of SAX events.
   */
  private Object getObject()
  {
    return this.userTable;
  }

  /**
   *
   */
  private String getUsersFileName()
  {
    String homeDir = Config.getHomeDir();
    if (homeDir == null)
      homeDir = "";

    StringBuffer nameBuff = new StringBuffer(256);
    nameBuff.append(homeDir);
    nameBuff.append(File.separator);
    nameBuff.append(SimpleAuthenticator.usersFileName);

    return nameBuff.toString();
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String args[])
    throws Exception
  {
    // read the juddi.users XML doc (then build the obj and initialize)
    Authenticator authenticator = new SimpleAuthenticator();
    authenticator.init();

    // now let's dump the user list to the console for verification
    System.out.println(authenticator.toString());

    AuthToken aToken1 = null;
    try
    {
      // try to get an AuthInfo using a valid userid and valid password.
      aToken1 = authenticator.getAuthToken("sviens","password");
      if (aToken1 != null)
        System.out.println("token for sviens/password = "+aToken1.getAuthInfoString());

      authenticator.validateAuthToken(aToken1.getAuthInfo());
      Thread.sleep(5000);
      authenticator.validateAuthToken(aToken1.getAuthInfo());
      Thread.sleep(35000);
      authenticator.validateAuthToken(aToken1.getAuthInfo());
      Thread.sleep(5000);
      authenticator.discardAuthToken(aToken1.getAuthInfo());
      Thread.sleep(5000);
      authenticator.validateAuthToken(aToken1.getAuthInfo());
    }
    catch(Exception ex)
    {
      ex.printStackTrace();
    }
    finally
    {
      // shutdown the token sweeper
      authenticator.destroy();
    }

//    AuthInfo aInfo2 = null;
//    try
//    {
//      // try to get an AuthInfo using an invalid userid and valid password.
//      aInfo2 = authenticator.get_authToken("johndoe","password");
//      if (aInfo2 != null)
//        System.out.println("token for johndoe/password = "+aInfo2.getText());
//    }
//    catch(Exception ex)
//    {
//      ex.printStackTrace();
//    }

//    AuthInfo aInfo3 = null;
//    try
//    {
//      // try to get an AuthInfo using an invalid userid and valid password.
//      aInfo3 = authenticator.get_authToken("sviens","badpass");
//      if (aInfo3 != null)
//        System.out.println("token for sviens/badpass = "+aInfo3.getText());
//    }
//    catch(Exception ex)
//    {
//      ex.printStackTrace();
//    }

//    try
//    {
//      // try to get an AuthInfo using an invalid userid and valid password.
//      authenticator.discard_authToken(aInfo1);
//      System.out.println("token for sviens/password was discarded successfully");
//    }
//    catch(Exception ex)
//    {
//      ex.printStackTrace();
//    }

//    try
//    {
//      // try to get an AuthInfo using an invalid userid and valid password.
//      authenticator.discard_authToken(aInfo1);
//      System.out.println("token for sviens/password was discarded successfully");
//    }
//    catch(Exception ex)
//    {
//      ex.printStackTrace();
//    }

//    try
//    {
//      // try to get an AuthInfo using an invalid userid and valid password.
//      authenticator.discard_authToken(aInfo2);
//      System.out.println("token for johndoe/password was discarded successfully");
//    }
//    catch(Exception ex)
//    {
//      ex.printStackTrace();
//    }

//    try
//    {
//      AuthInfo aInfo4 = new AuthInfo();
//      // try to get an AuthInfo using an invalid userid and valid password.
//      authenticator.discard_authToken(aInfo4);
//      System.out.println("token for johndoe/password was discarded successfully");
//    }
//    catch(Exception ex)
//    {
//      ex.printStackTrace();
//    }

//    try
//    {
//      AuthInfo aInfo5 = new AuthInfo();
//      aInfo5.setText("");
//      // try to get an AuthInfo using an invalid userid and valid password.
//      authenticator.discard_authToken(aInfo5);
//      System.out.println("token for johndoe/password was discarded successfully");
//    }
//    catch(Exception ex)
//    {
//      ex.printStackTrace();
//    }

//    try
//    {
//      AuthInfo aInfo5 = new AuthInfo();
//      aInfo5.setText("      ");
//      // try to get an AuthInfo using an invalid userid and valid password.
//      authenticator.discard_authToken(aInfo5);
//      System.out.println("token for johndoe/password was discarded successfully");
//    }
//    catch(Exception ex)
//    {
//      ex.printStackTrace();
//    }

//    try
//    {
//      authenticator.validate_authToken(aInfo1);
//    }
//    catch(Exception ex)
//    {
//      ex.printStackTrace();
//    }
  }
}