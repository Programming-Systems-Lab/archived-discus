package psl.discus.javasrc.security;

/**
 * @author Matias Pelenur
 */
public class SignatureManagerException extends Exception {

    public SignatureManagerException() {
        super();
    }

    public SignatureManagerException(String s) {
        super(s);
    }

    public SignatureManagerException(Exception e) {
        super(e.getMessage());
    }

}
