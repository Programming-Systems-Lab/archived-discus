/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.error;


/**
 * Used in response DispositionReport.
 *
 * @author  Steve Viens
 * @version 0.6
 */
public class ResultCodes
{
  // Signifies that the authentication token information has timed out.
  public static final String E_ASSERTION_NOT_FOUND = "30000";
  public static final String E_ASSERTION_NOT_FOUND_CODE = "E_assertionNotFound";
  public static final String E_ASSERTION_NOT_FOUND_MSG = "A particular publisher " +
    "assertion cannot be identified in a save or delete operation.";

  // Signifies that the authentication token information has timed out.
  public static final String E_AUTH_TOKEN_EXPIRED = "10110";
  public static final String E_AUTH_TOKEN_EXPIRED_CODE = "E_authTokenExpired";
  public static final String E_AUTH_TOKEN_EXPIRED_MSG = "Authentication token " +
    "information has timed out.";

  // Signifies that an invalid authentication token was passed to an API call
  // that requires authentication.
  public static final String E_AUTH_TOKEN_REQUIRED = "10120";
  public static final String E_AUTH_TOKEN_REQUIRED_CODE = "E_authTokenRequired";
  public static final String E_AUTH_TOKEN_REQUIRED_MSG = "An invalid authentication " +
    "token was passed to an API call that requires authentication.";

  // Signifies that a save request exceeded the quantity limits for a given
  // structure type. See "Structure Limits" in Appendix D for details.
  public static final String E_ACCOUNT_LIMIT_EXCEEDED = "10160";
  public static final String E_ACCOUNT_LIMIT_EXCEEDED_CODE = "E_accountLimitExceeded";
  public static final String E_ACCOUNT_LIMIT_EXCEEDED_MSG = "Authentication token " +
    "information has timed out.";

  // Signifies that the request cannot be processed at the current time.
  public static final String E_BUSY = "10400";
  public static final String E_BUSY_CODE = "E_busy";
  public static final String E_BUSY_MSG = "The request cannot be processed at " +
    "the current time.";

  // Restrictions have been placed by the on the types of information that can
  // categorized within a specific taxonomy. The data provided does not conform
  // to the restrictions placed on the category used. Used with cateborization
  // only.
  public static final String E_CATEGORIZATION_NOT_ALLOWED = "20100";
  public static final String E_CATEGORIZATION_NOT_ALLOWED_CODE = "E_categorizationNotAllowed";
  public static final String E_CATEGORIZATION_NOT_ALLOWED_MSG = "The data " +
    "provided does not conform to the restrictions placed on the category used.";

  // Signifies that a serious technical error has occurred while processing the
  // request.
  public static final String E_FATAL_ERROR = "10500";
  public static final String E_FATAL_ERROR_CODE = "E_fatalError";
  public static final String E_FATAL_ERROR_MSG = "A serious technical error " +
    "has occurred while processing the request.";

  // Signifies that the uuid_key value passed did not match with any known key
  // values. The details on the invalid key will be included in the
  // dispositionReport structure.
  public static final String E_INVALID_KEY_PASSED = "10210";
  public static final String E_INVALID_KEY_PASSED_CODE = "E_invalidKeyPassed";
  public static final String E_INVALID_KEY_PASSED_MSG = "The uuid_key value " +
    "passed did not match with any known key values.";

  // Signifies that the authentication token information has timed out.
  public static final String E_INVALID_PROJECTION = "20230";
  public static final String E_INVALID_PROJECTION_CODE = "E_invalidProjection";
  public static final String E_INVALID_PROJECTION_MSG = "An attempt was made " +
    "to save a business entity containing a service projection that does not " +
    "match the business service being projected.";

  // Signifies that the given keyValue did not correspond to a category within
  // the taxonomy identified by the tModelKey. Used with categorization only.
  public static final String E_INVALID_CATEGORY = "20000";
  public static final String E_INVALID_CATEGORY_CODE = "E_invalidCategory";
  public static final String E_INVALID_CATEGORY_MSG = "The given keyValue did " +
    "not correspond to a category within the taxonomy identified by the tModelKey.";

  // Signifies that the authentication token information has timed out.
  // NOTE: The UDDI specification indicates that the errno value should be 30100
  // but this value conflicts with the E_MESSAGE_TOO_LARGE errno. The value for
  // E_MESSAGE_TOO_LARGE errno has been changed to 30101. This is the same approach
  // Idoox (now Systinet) took.
  public static final String E_INVALID_COMPLETION_STATUS = "30100";
  public static final String E_INVALID_COMPLETION_STATUS_CODE = "E_invalidCompletionStatus";
  public static final String E_INVALID_COMPLETION_STATUS_MSG = "One of the " +
    "assertion status values passed is unrecognized.";

  // Signifies that an error occurred during processing of a save function
  // involving accessing data from a remote URL. The details of the HTTP Get
  // report will be included in the dispositionReport structure.
  public static final String E_INVALID_URL_PASSED = "10220";
  public static final String E_INVALID_URL_PASSED_CODE = "E_invalidURLPassed";
  public static final String E_INVALID_URL_PASSED_MSG = "An error occurred " +
    "during processing of a save function involving accessing data from a remote URL.";

  // Signifies that the authentication token information has timed out.
  public static final String E_INVALID_VALUE = "20200";
  public static final String E_INVALID_VALUE_CODE = "E_invalidValue";
  public static final String E_INVALID_VALUE_MSG = "A value that was passed " +
    "in a keyValue attribute did not pass validation. This applies to checked " +
    "categorizations, identifiers and other validated code lists.";

  // Signifies that a uuid_key value passed has been removed from the registry.
  // While the key was once valid as an accessor, and is still possibly valid,
  // the publisher has removed the information referenced by the uuid_key passed.
  public static final String E_KEY_RETIRED = "10310";
  public static final String E_KEY_RETIRED_CODE = "E_keyRetired";
  public static final String E_KEY_RETIRED_MSG = "A uuid_key value passed has " +
    "been removed from the registry.";

  // Signifies that an error was detected while processing elements that were
  // annotated with xml:lang qualifiers. Presently, only the description element
  // supports xml:lang qualifiacations.
  public static final String E_LANGUAGE_ERROR = "10060";
  public static final String E_LANGUAGE_ERROR_CODE = "E_languageError";
  public static final String E_LANGUAGE_ERROR_MSG = "An error was detected " +
    "while processing elements that were annotated with xml:lang qualifiers.";

  // Signifies that the authentication token information has timed out.
  // NOTE: The UDDI specification indicates that the errno value should be 30100
  // but this value conflicts with the E_INVALID_COMPLETION_STATUS so this one
  // has been changed. This is the same approach Idoox (now Systinet) took.
  public static final String E_MESSAGE_TOO_LARGE = "30101";  // 30100 in the spec
  public static final String E_MESSAGE_TOO_LARGE_CODE = "E_messageTooLarge";
  public static final String E_MESSAGE_TOO_LARGE_MSG = "The message is too large.";

  // Signifies that the partial name value passed exceeds the maximum name
  // length designated by the policy of an implementation or Operator Site.
  public static final String E_NAME_TOO_LONG = "10020";
  public static final String E_NAME_TOO_LONG_CODE = "E_nameTooLong";
  public static final String E_NAME_TOO_LONG_MSG = "The partial name value " +
    "passed exceeds the maximum name length designated by the policy of an " +
    "implementation or Operator Site.";

  // Signifies that an attempt was made to use the publishing API to change data
  // that is mastered at another Operator Site. This error is only relevant to the
  // public Operator Sites and does not apply to other UDDI compatible registries.
  public static final String E_OPERATOR_MISMATCH = "10130";
  public static final String E_OPERATOR_MISMATCH_CODE = "E_operatorMismatch";
  public static final String E_OPERATOR_MISMATCH_MSG = "An attempt was made " +
    "to use the publishing API to change data that is mastered at another Operator " +
    "Site.";

  // Signifies that the authentication token information has timed out.
  public static final String E_PUBLISHER_CANCELLED = "30220";
  public static final String E_PUBLISHER_CANCELLED_CODE = "E_publisherCancelled";
  public static final String E_PUBLISHER_CANCELLED_MSG = "The target publisher " +
    "cancelled the custody transfer.";

  // Signifies that the authentication token information has timed out.
  public static final String E_REQUEST_DENIED = "30210";
  public static final String E_REQUEST_DENIED_CODE = "E_requestDenied";
  public static final String E_REQUEST_DENIED_MSG = "A custody transfer " +
    "request has been refused.";

  // Signifies that the authentication token information has timed out.
  public static final String E_SECRET_UNKNOWN = "30230";
  public static final String E_SECRET_UNKNOWN_CODE = "E_secretUnknown";
  public static final String E_SECRET_UNKNOWN_MSG = "The target publisher " +
    "was unable to match the shared secret and the five (5) attempt limit was " +
    "exhausted. The target publisher automatically cancelled the transfer operation.";

  // Signifies no failure occurred. This return code is used with the
  // dispositionReport for reporting results from requests with no natural
  // response document.
  public static final String E_SUCCESS = "0";
  public static final String E_SUCCESS_CODE = "E_success";
  public static final String E_SUCCESS_MSG = null;

  // Signifies that incompatible arguments were passed.
  public static final String E_TOO_MANY_OPTIONS = "10030";
  public static final String E_TOO_MANY_OPTIONS_CODE = "E_tooManyOptions";
  public static final String E_TOO_MANY_OPTIONS_MSG = "Incompatible arguments " +
    "were passed.";

  // Signifies that the authentication token information has timed out.
  public static final String E_TRANSFER_ABORTED = "30200";
  public static final String E_TRANSFER_ABORTED_CODE = "E_transferAborted";
  public static final String E_TRANSFER_ABORTED_MSG = "Signifies that a " +
    "custody transfer request will not succeed.";

  // Signifies that the value of the generic attribute passed is unsupported by
  // the Operator Instance being queried.
  public static final String E_UNRECOGNIZED_VERSION = "10040";
  public static final String E_UNRECOGNIZED_VERSION_CODE = "E_unrecognizedVersion";
  public static final String E_UNRECOGNIZED_VERSION_MSG = "The value of the " +
    "generic attribute passed is unsupported by the Operator Instance being queried.";

  // Signifies that the user ID and password pair passed in a get_authToken
  // message is not known to the Operator Site or is not valid.
  public static final String E_UNKNOWN_USER = "10150";
  public static final String E_UNKNOWN_USER_CODE = "E_unknownUser";
  public static final String E_UNKNOWN_USER_MSG = "The user ID and password " +
    "pair passed in a get_authToken message is not known to the Operator Site " +
    "or is not valid.";

  // Signifies that the implementor does not support a feature or API.
  public static final String E_UNSUPPORTED = "10050";
  public static final String E_UNSUPPORTED_CODE = "E_unsupported";
  public static final String E_UNSUPPORTED_MSG = "The implementor does not " +
    "support a feature or API.";

  // Signifies that an attempt was made to use the publishing API to change data
  // that is controlled by another party. In certain cases, E_operatorMismatch
  // takes precedence in reporting an error.
  public static final String E_USER_MISMATCH = "10140";
  public static final String E_USER_MISMATCH_CODE = "E_userMismatch";
  public static final String E_USER_MISMATCH_MSG = "An attempt was made to " +
    "use the publishing API to change data that is controlled by another party.";

  // Signifies that the authentication token information has timed out.
  public static final String E_VALUE_NOT_ALLOWED = "20210";
  public static final String E_VALUE_NOT_ALLOWED_CODE = "E_valueNotAllowed";
  public static final String E_VALUE_NOT_ALLOWED_MSG = "A value did not " +
    "pass validation because of contextual issues. The value may be valid in " +
    "some contexts, but not in the contextused.";
}
