package psl.discus.javasrc.security;


/**
 * Author: Matias
 * Date: Mar 19, 2002
 * Time: 5:40:00 PM
 */
public class DAOException extends Exception {

    Exception e;

    public DAOException() {
        super();
    }

    public DAOException(String s) {
        super(s);
    }

    public DAOException(Exception e) {
        super();
        this.e = e;
    }

    public Exception getNextException() {
        return e;
    }

    public String toString() {
        return (e != null ? e.toString() : getMessage());
    }
}
