/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.uuidgen;

import psl.discus.javasrc.uddi.util.Config;
import org.apache.log4j.Logger;
import java.util.LinkedList;

/**
 * @author Steve Viens
 * @version 0.6
 */
public class UUID
{
  // private reference to the jUDDI logger
  private static Logger log = Logger.getLogger(UUID.class);

  // class variables

  private static final String defaultClassName = "org.juddi.uuidgen.JavaUUIDGen";
  private static final int defaultCacheSize = 200;
  private static final long defaultNapTime = 3000;
  private static UUID generator = null;


  // instance variables

  private UUIDGen uuidgen = null; // UUIDGen Interface implementation
  private CacheManager manager; // thread to maintian the UUID Cache
  private LinkedList cache;
	private int cacheSize;
	private long napTime;

  private class CacheManager extends Thread
  {
    public void run()
    {
      try
      {
        while(true)
        {
          sleep(napTime);


          int currentSize = cache.size();

          log.debug("jUDDI's UUID cache size: "+currentSize);


          if (currentSize == 0)

            log.warn("jUDDI's UUID cache is empty: "+currentSize);



          if (currentSize < cacheSize)
            addToCache(uuidgen.uuidgen(cacheSize-currentSize));
        }
      }
      catch(InterruptedException e)
      {
        /* CacheManager Thread Interrupted */
      }
    }
  }


  /**

	 *

	 */

	private UUID(UUIDGen uuidgen,int cacheSize,long napTime)

	{

    this.uuidgen = uuidgen;

    this.cacheSize = cacheSize;

    this.napTime = napTime;

  	this.cache = new LinkedList();



    // pre-load the UUID cache

    addToCache(uuidgen.uuidgen(cacheSize));



    // create and start the CacheManager thread

    this.manager = new CacheManager();

    this.manager.start();

  }



  /**

   * called only from org.juddi.util.Startup.startup() to get a head start on

   * aquiring neccessary resources and starting background threads.

   */

  public static void startup()

  {

    if (generator != null)

      shutdown();

    generator = getGenerator();

  }



  /**

   * Perform UUID generation termination tasks such as the release of

   * any aquired resources and stopping any background threads.

   */

  public synchronized static void shutdown()

  {

    // make sure we only attempt to stop this once (another thread

    // may have already taken care of this before us)



    if (generator == null)

      return;



    // stop the CacheManager thread



    generator.manager.interrupt();

    generator = null;

  }



  /**
   *
   */
  public static String nextID()
  {
    // first call to this method will take the
    // performance hit neccessary when loading 
    // the UUID cache for the first time.
    
    if (generator == null)
      generator = createGenerator();      

    try
    {
      while (generator.cache.size() <= 0)
        generator.manager.sleep(500);
    }
    catch(InterruptedException e) { }

    return (String)generator.cache.removeLast();
  }


  /**

   *

   */

  private synchronized void addToCache(String[] newIDs)

  {

    if (cache.size() >= cacheSize)

      return;



    for (int i=0; i<newIDs.length; i++)

      cache.add(newIDs[i]);

  }





  /**

   *

   */

  private static UUID getGenerator()

  {

    if (generator == null)

      generator = createGenerator();

    return generator;

  }



  /**
   *
   */
  private static synchronized UUID createGenerator()
  {
    // check to see if another thread beat us to this method. 
    // If so simply return the UUID instance they created.
    
    if (generator != null)
      return generator;
    
    // try to obtain the name of the UUIDGen subclass
    
    String configClassName = 
      Config.getProperty("juddi.uuidgen.className");

    // try to obtain the 'cache size' property value
    
    Integer configCacheSize =
      Config.getPropertyInteger("juddi.uuidgen.cacheSize");

    // try to obtain the 'nap time' property value
    
    Long configNapTime =
      Config.getPropertyLong("juddi.uuidgen.napTime");

    String className = (configClassName == null)
      ? defaultClassName
      : configClassName;
        
    int cacheSize = (configCacheSize == null)
      ? defaultCacheSize
      : configCacheSize.intValue();
        
    long napTime = (configNapTime == null)
      ? defaultNapTime
      : configNapTime.longValue();
       
    log.info("juddi.uuidgen.className = " + className);
    log.info("juddi.uuidgen.cacheSize = " + cacheSize);
    log.info("juddi.uuidgen.napTime: = " + napTime);

    // create a new UUIDGen instance using the 
    // implementation specified in jUDDI properties.
    
    UUIDGen uuidgen = UUIDGenFactory.getUUIDGen(className);

    // finally, construct and return the new UUID singlton
    
    return new UUID(uuidgen,cacheSize,napTime);
  }


  /***************************************************************************/
  /***************************** TEST DRIVER *********************************/
  /***************************************************************************/


  public static void main(String[] args)
  {
    try
    {
      // initialize uuid cache
      psl.discus.javasrc.uddi.util.SysManager.startup();

      // create 10 new UUIDs
      for (int i=0; i<1000; i++)
      {

        UUID.nextID();
        Thread.sleep(50);
      }
    }
    catch(Exception ex)
    {
      ex.printStackTrace();
    }
    finally
    {
      // release resources
      psl.discus.javasrc.uddi.util.SysManager.shutdown();
    }
  }
}
