package psl.discus.javasrc.uddi;

public class MessageDispatcherException extends Exception {

    Exception e;

    public MessageDispatcherException() {
        super();
    }

    public MessageDispatcherException(String s) {
        super(s);
    }

    public MessageDispatcherException(Exception e) {
        super();
        this.e = e;
    }

    public Exception getNextException() {
        return e;
    }

    public String toString() {
        return MessageDispatcherException.class.getName() + ": " + (e != null ? e.toString() : getMessage());
    }
}
