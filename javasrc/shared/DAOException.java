package psl.discus.javasrc.shared;


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

    public String getMessage() {
        return e.getMessage();
    }

    public Exception getNextException() {
        return e;
    }
}
