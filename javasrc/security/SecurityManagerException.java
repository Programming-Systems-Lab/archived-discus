package psl.discus.javasrc.security;

/**
 * Author: Matias
 * Date: Mar 19, 2002
 * Time: 6:01:56 PM
 */
public class SecurityManagerException extends Exception {

    public SecurityManagerException() {
        super();
    }

    public SecurityManagerException(String s) {
        super(s);
    }

    public SecurityManagerException(Exception e) {
        super(e.getMessage());
    }

}
