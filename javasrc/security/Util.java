package psl.discus.javasrc.security;

import org.apache.log4j.*;

import java.util.*;

/**
 * @author Matias Pelenur
 */
public final class Util {

    public static Logger logger = Logger.getLogger("psl.discus.javasrc.security");

    public static int parseInt(Object o) {
        try {
            return Integer.parseInt(String.valueOf(o));
        } catch (NumberFormatException e) {
            return -1;
        }
    }

    public static String replaceString(String s, String from, String to) {

        StringBuffer buf = new StringBuffer();
        int pos = s.indexOf(from);
        int lastpos = 0;
        int fromLength = from.length();
        while (pos != -1) {
            buf.append(s.substring(lastpos,pos));
            buf.append(to);
            lastpos = pos + fromLength;
            pos = s.indexOf(from,lastpos);
        }
        buf.append(s.substring(lastpos));

        return buf.toString();
    }

    public static void debug(Object o) {
        //System.out.println("debug: " + o);
        logger.debug(o);
    }

    public static void print(Object o) {
        //System.out.print("print: " + o);
        logger.debug(o);
    }

    public static void println(Object o) {
        //System.out.println("println: " + o);
        logger.debug(o);
    }

    public static void info(Object o) {
        logger.info(o);
    }

    public static void error(Object o) {
        logger.error(o);
    }

    public static String collectionToString(Collection params) {
        StringBuffer buf = new StringBuffer();
        for (Iterator i = params.iterator();i.hasNext();) {
            String s = String.valueOf(i.next());
            buf.append(s);
            if (i.hasNext())
                buf.append(", ");
        }

        return buf.toString();
    }

}
