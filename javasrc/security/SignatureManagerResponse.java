package psl.discus.javasrc.security;

import org.w3c.dom.Document;

public class SignatureManagerResponse {
    public Document document;      // the actual XML document, without the signature
    public String alias;           // the alias of the certificate that signed it
}
