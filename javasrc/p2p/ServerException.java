package psl.discus.javasrc.p2p;

public class ServerException extends Exception {

    public ServerException() {
        super();
    }

    public ServerException(String s) {
        super(s);
    }

    public ServerException(Exception e) {
        super(e.getMessage());
    }

}
