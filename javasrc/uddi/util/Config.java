/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.util;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.Properties;

import java.util.Enumeration;

/**
 * This class provides read access to key/value pairs loaded from the
 * properties file (juddi.properties).
 *
 * The home directory can be set using the JUDIHOME property. Set it on
 * the command line when starting the server.
 * <BR><B>
 * usage: java -DJUDDIHOME="/usr/local/juddi" appname
 * </B>
 *
 * <UL> Defaults
 * <LI> <CODE>..\conf\juddi.properties</CODE>
 * </UL>
 *
 * <PRE><CODE>
 * Use:
 *    import org.juddi.util.Config;
 *     ...
 *
 *    String stringPropertyValue = null;
 *    Integer integerPropertyValue = null;
 *    Boolean booleanPropertyValue = null;
 *      ...
 *    try {
 *      stringPropertyValue = Config.getProperty("juddi.test-string-property");
 *      integerPropertyValue = Config.getPropertyInteger("juddi.test-integer-property");
 *      booleanPropertyValue = Config.getPropertyBoolean("juddi.test-boolean-property");
 *    } catch (RuntimeException e) { }
  *     ...
 * </CODE></PRE>
 *
 * @author  Steve Viens
 * @version 0.6
 */
public class Config
{
  private static final int DEFAULT_MAX_ACCESS_POINT_LENGTH = 255;

  private static final int DEFAULT_MAX_ACCESS_POINT_TYPE_LENGTH = 16;

  private static final int DEFAULT_MAX_ADDRESS_LINE_LENGTH = 80;

  private static final int DEFAULT_MAX_AUTH_INFO_LENGTH = 4096;

  private static final int DEFAULT_MAX_AUTHORIZED_NAME_LENGTH = 255;

  private static final int DEFAULT_MAX_BINDING_KEY_LENGTH = 41;

  private static final int DEFAULT_MAX_BUSINESS_KEY_LENGTH = 41;

  private static final int DEFAULT_MAX_DESCRIPTION_LENGTH = 255;

  private static final int DEFAULT_MAX_DISCOVERY_URL_LENGTH = 255;

  private static final int DEFAULT_MAX_EMAIL_LENGTH = 255;

  private static final int DEFAULT_MAX_FROM_KEY_LENGTH = 41;

  private static final int DEFAULT_MAX_HOSTING_REDIRECTOR_LENGTH = 41;

  private static final int DEFAULT_MAX_INSTANCE_PARMS_LENGTH = 255;

  private static final int DEFAULT_MAX_KEY_NAME_LENGTH = 255;

  private static final int DEFAULT_MAX_NAME_ELEMENTS_ALLOWED = 5;

  private static final int DEFAULT_MAX_NAME_LENGTH = 255;

  private static final int DEFAULT_MAX_OVERVIEW_URL_LENGTH = 41;

  private static final int DEFAULT_MAX_PERSON_NAME_LENGTH = 255;

  private static final int DEFAULT_MAX_PHONE_LENGTH = 50;

  private static final int DEFAULT_MAX_SERVICE_KEY_LENGTH = 41;

  private static final int DEFAULT_MAX_SORT_CODE_LENGTH = 10;

  private static final int DEFAULT_MAX_TMODEL_KEY_LENGTH = 41;

  private static final int DEFAULT_MAX_TO_KEY_LENGTH = 41;

  private static final int DEFAULT_MAX_UPLOAD_REGISTER_LENGTH = 255;

  private static final int DEFAULT_MAX_URL_TYPE_LENGTH = 16;

  private static final int DEFAULT_MAX_USE_TYPE_LENGTH = 255;



  private static final String propertiesFileName = "juddi.properties";
  private static Properties properties; // configuration properties
  private static File homeDir; // top level directory
  private static File confDir; // conf file directory

  /**
   * Initialization block - executed when the class loads.
   *
   * Rationale is that many components will be reading configuration
   * information at load time.  Might as well pre-load the configuration
   * information.
   */
  static
  {
    loadProperties();
  }

  /**

   *

   */

  public static String getOperatorURI()

  {

    return getProperty("juddi.operatorName");

  }



  /**

   *

   */

  public static int getMaxNameLength()

  {

    Integer max = getPropertyInteger("juddi.maxNameLength");



    return (max != null) ? max.intValue() : DEFAULT_MAX_NAME_LENGTH;

  }



  /**

   *

   */

  public static int getMaxNameElementsAllowed()

  {

    Integer max = getPropertyInteger("juddi.maxNameElementsAllowed");



    return (max != null) ? max.intValue() : DEFAULT_MAX_NAME_ELEMENTS_ALLOWED;

  }



  /**
   * Get a reference to the home directory. Call this to find

   * the root directory for the install. This is typically the

   * same as the bowserver install directory.
   *
   * @return String Home directory.
   */
  public static String getHomeDir()
  {
    if(homeDir==null)
      initDirectories();

    return homeDir.getPath();
  }

  /**
   * Get a reference to the conf directory.
   * Call this to find the directory where the conf file lives.
   *
   * @return String conf directory.
   */
  public static String getConfigDir()
  {
    if(confDir==null)
      initDirectories();

    return confDir.getPath();
  }

  /**
   * Retrieves a configuration property as a String object.
   * Loads the juddi.properties file if not already initialized.
   *
   * @param Key Name of the property to be returned.
   * @return  Value of the property as a string or null if no property found.
   */
  public static String getProperty(String key)
  {
    String retval = null;

    // no property name to lookup - return null
    if (key==null)
      return null;

    if(properties==null)
      loadProperties();

    // no properties to look into - return null
    if(properties==null)
      return null;

    retval = properties.getProperty(key);

    // no property value was found - return null
    if(retval==null)
      return null;

    // return the value WITHOUT any spaces before or behind the value
    return retval.trim();
  }

  /**
   * Get a configuration property as an Integer object.
   *
   * @param Key Name of the numeric property to be returned.
   * @return  Value of the property as an Integer or null if no property found.
   */
  public static Integer getPropertyInteger(String key)
  {
    // no property name to lookup - return null
    if (key==null)
      return null;

    Integer iVal = null;
    String sVal = getProperty(key);

    if ((sVal != null) && (sVal.length() > 0))
    {
      try
      {
        iVal = new Integer(sVal);
      }
      catch (NumberFormatException nfe)
      {
        System.out.println(nfe.getMessage());
      }
    }

    return iVal;
  }

  /**
   * Get a configuration property as an Long object.
   *
   * @param Key Name of the numeric property to be returned.
   * @return  Value of the property as an Long or null if no property found.
   */
  public static Long getPropertyLong(String key)
  {
    // no property name to lookup - return null
    if(key==null)
      return null;

    Long iVal = null;
    String sVal = getProperty(key);

    if((sVal != null) && (sVal.length() > 0))
    {
      try
      {
        iVal = new Long(sVal);
      }
      catch (NumberFormatException nfe)
      {
        System.out.println(nfe.getMessage());
      }
    }

    return iVal;
  }

  /**
   * Get a configuration property as a Boolean object.
   *
   * @param Key Name of the numeric property to be returned.
   * @return  Value of the property as an Boolean or null if no property found.
   *
   * Note that the value of the returned Boolean will be false if the
   * property sought after exists but is not equal to "true" (ignoring case).
   */
  public static Boolean getPropertyBoolean(String key)
  {
    // no property name to lookup - return null
    if(key==null)
      return null;

    Boolean bVal = null;
    String sVal = getProperty(key);

    if((sVal != null) && (sVal.length() > 0))
      bVal = new Boolean(sVal);

    return bVal;
  }


  /***************************************************************************/
  /*************************** LOW LEVEL CALLS *******************************/
  /***************************************************************************/


  /**
   * called only from org.juddi.util.Startup.startup()
   */
  static void startup()
  {
    if (properties != null)
      shutdown();
    loadProperties();
  }

  /**
   * called only from org.juddi.util.Shutdown.shutdown() to release any
   * aquired resources and stop any background threads.
   */
  static void shutdown()
  {
    if(properties != null)
    {
      // release any aquired resources and stop any background threads.
      //ConfiguratorFactory.destroyConfigurator(conf); // not implemented yet
      properties = null;
    }
  }

  /**
   * Load the configuration properties from the properties file.
   *
   * Caller must test to ensure that properties is Non-null.
   *
   *  @exception RuntimeException Translates an IOException from reading
   *   the properties file into a run time exception.
   */
  private static synchronized void loadProperties()
  {
    // If multiple threads are waiting to envoke this method only allow
    // the first one to do so.  The rest should just return since the first
    // thread through took care of loading the properties.
    if (properties!=null)
      return;

    String propertiesFile = null;

    try
    {
      String hd = getConfigDir();
      propertiesFile = hd + File.separator + propertiesFileName;
      FileInputStream fis = new FileInputStream(propertiesFile);

      try
      {
        properties = new Properties();
        // Load the properties object from the properties file
        properties.load(fis);
      }
      finally
      {
        fis.close(); // Always close the file, even on exception
      }
    }
    catch (IOException e)
    {
      System.out.println("Warning: problem loading juddi properties from " +
        propertiesFile + "\n" + e.toString());

      System.out.println("... continuing ...");
    }
  }

  /**
   * Initialize the jUDDI Root directory.
   *
   * This will probably always be the same as the applicatoins root directory.
   *
   * @exception RuntimeException jUDDI directory doesn't exist or access denied.
   */
  private static synchronized void initDirectories()
  {
    // Don't bother with this if simultaneous thread got to it first
    if(homeDir == null)
    {
      // Pull jUDDI home directory value out of the System class
      String dir = System.getProperty("juddi.homeDir");
      if(dir == null)
        throw new RuntimeException("juddi.homeDir jvm parameter has not been " +
          "properly set. jUDDI cannot function properly without this value.");      
      
      homeDir = new File(dir);
      
      // Try to access the home directory
      if(!homeDir.isDirectory())
      {
        throw new RuntimeException("jUDDI home directory " + 
          homeDir.getAbsolutePath() + " doesn't exist or cannot be accessed!");
      }
    }

    // Now ensure that the conf dir has been set.
    if(confDir==null)
      confDir = new File(homeDir.getPath() + File.separator + "conf");

    if(!confDir.isDirectory())
    {
      throw new RuntimeException("jUDDI conf directory " + 
        confDir.getAbsolutePath() + " doesn't exist or cannot be accessed!");
    }
  }

  /**

   * Returns a String containing a pipe-delimited ('|') list of name=value pairs

   * @return String pipe-delimited list of name=value pairs.

   */

  public String toString()

  {

    // make sure properties have been loaded

    if(properties==null)

      loadProperties();



    // let's create a place to put the property information

    StringBuffer buff = new StringBuffer(100);



    // gran an enumeration of the property names (or keys)

    Enumeration propKeys = properties.keys();

    while (propKeys.hasMoreElements())

    {

      // extract the Property Name (aka Key) and Value

      String propName = (String)propKeys.nextElement();

      String propValue = (String)properties.getProperty(propName);



      // append the name=value pair to the return buffer

      buff.append(propName.trim());

      buff.append("=");

      buff.append(propValue.trim());

      buff.append("\n");

    }



    return buff.toString();

  }





  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    // NOTE: Before running this test driver you will need to make sure the
    // following properties exist.
    //
    //    juddi.test-string-property = jUDDI3 Rocks!
    //    juddi.test-integer-property = 10
    //    juddi.test-boolean-property = true

    try
    {
      // initialize low level components
      SysManager.startup();

      // get and print each type of property including the Home Directory value
      System.out.println(Config.getProperty("juddi.test-string-property"));
      System.out.println(Config.getPropertyBoolean("juddi.test-boolean-property"));
      System.out.println(Config.getPropertyInteger("juddi.test-integer-property"));
      System.out.println(Config.getProperty("juddi.operatorName"));
      System.out.println(Config.getHomeDir());
    }
    catch(Exception ex)
    {
      ex.printStackTrace();
    }
    finally
    {
      // release resources aquired by low level components
      SysManager.shutdown();
    }
  }
}