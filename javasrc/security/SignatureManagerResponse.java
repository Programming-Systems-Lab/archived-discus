package psl.discus.javasrc.security;

import org.w3c.dom.Document;

public class SignatureManagerResponse {

    private Document document;       // the actual XML document, without the signature
    private ServiceSpace signer;     // which service space signed the document

    public SignatureManagerResponse(Document document, ServiceSpace signer) {
        this.document = document;
        this.signer = signer;
    }

    public Document getDocument() {
        return document;
    }

    public ServiceSpace getSigner() {
        return signer;
    }


}
