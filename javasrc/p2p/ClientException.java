package psl.discus.javasrc.p2p;

public class ClientException extends Exception {

    public ClientException() {
        super();
    }

    public ClientException(String s) {
        super(s);
    }

    public ClientException(Exception e) {
        super(e.getMessage());
    }

}
