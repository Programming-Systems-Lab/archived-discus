package psl.discus.javasrc.security;

import org.w3c.dom.Document;

/**
 * @author Matias Pelenur
 */
public interface SignatureManager {
    int STATUS_OK = 0;
    int STATUS_ERROR = -1;

    /**
     * (For web-service calls)
     * Signs the given XML document with this service space's private key.
     * @returns an array of two Strings, where the first is a status code (0 is OK)
     * and the second is either the signed XML document or the error message.
     */
    public String[] signDocument(String xml)
        throws SignatureManagerException;

    /**
     * Signs the given document with the private key, and adds the signature to the document passed.
     */
    public Document signDocument(Document givenDoc)
            throws SignatureManagerException;

    /**
     * (For web-service calls)
     * Verifies a signed XML document and returns the document and the id of the signing service space
     * @returns an array three Strings, where the first is a status code (0 is OK),
     * the second element is the given xml document but without the signature, or the error message
     * and the third (if no error) is the signing service space id.
     */
    public String[] verifyDocument(String xml)
        throws SignatureManagerException;

    /**
     * Verifies a signed XML document and returns the document and the id of the signing service space
     */
    public SignatureManagerResponse verifyDocument(Document signedDoc)
            throws SignatureManagerException;


}
