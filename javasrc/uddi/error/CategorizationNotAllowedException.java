/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.error;

import org.uddi4j.response.ErrInfo;
import org.uddi4j.response.Result;

/**
 * Thrown to indicate that a UDDI Exception was encountered.
 * (RETIRED) Replaced by E_valueNotAllowed in UDDI v2.0. See the
 * UDDI Programmers API, Appendix A for more information.
 *
 * @author  Steve Viens
 * @version 0.6
 */
public class CategorizationNotAllowedException extends JUDDIException
{
  public CategorizationNotAllowedException()
  {
    this(ResultCodes.E_CATEGORIZATION_NOT_ALLOWED_MSG);
  }

  public CategorizationNotAllowedException(String msg)
  {
    super(msg);

    ErrInfo errInfo = new ErrInfo();
    errInfo.setErrCode(ResultCodes.E_CATEGORIZATION_NOT_ALLOWED_CODE);
    errInfo.setText(ResultCodes.E_CATEGORIZATION_NOT_ALLOWED_MSG);

    Result result = new Result();
    result.setErrno(ResultCodes.E_CATEGORIZATION_NOT_ALLOWED);
    result.setErrInfo(errInfo);

    this.setFaultActor("");
    this.setFaultCode("Client");
    this.setFaultString(msg);
    this.addResult(result);
  }
}