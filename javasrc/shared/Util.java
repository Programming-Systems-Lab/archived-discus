package psl.discus.javasrc.shared;

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
}
