package psl.discus.javasrc.p2p;

/**
 * Just used to declared shared XML tags and other constants
 * @author matias
 */
public interface Tags {

    public static final String SERVICE_NAME = "discusUddi";

    public static final String MSGNUM_TAG = "Seqnum";
    public static final String DATA_TAG = "Data";
    public static final String QUERY_TAG = "Query";
    public static final String RESPONSE_TAG = "Response";

    public static final String PIPE_TAG = "jxta:PipeAdvertisement";
    public static final String MODULE_SPEC_ID_TAG = "ModuleSpecId";
    public static final String PIPE_ID_TAG = "PipeId";
    public static final String PARAM_TAG = "Parm";

    public static final String SOAP_ENVELOPE_TAG = "SOAP-ENV:Envelope";
    public static final String SOAP_BODY_TAG = "SOAP-ENV:Body";
}
