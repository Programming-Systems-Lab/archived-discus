package psl.discus.javasrc.shared;

import java.util.StringTokenizer;

/**
 * @author Matias Pelenur
 */
public final class Util {

    public static void debug(Object o) {
        System.out.println(o);
    }

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

    public static final void main(String args[]) {

        debug(replaceString(args[0],args[1],args[2]));
    }
}
