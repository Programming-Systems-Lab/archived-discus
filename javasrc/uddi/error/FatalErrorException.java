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
 *
 * @author  Steve Viens
 * @version 0.6
 */
public class FatalErrorException extends JUDDIException
{
  public FatalErrorException()
  {
    this(ResultCodes.E_FATAL_ERROR_MSG);
  }

  public FatalErrorException(String msg)
  {
    super(msg);

    ErrInfo errInfo = new ErrInfo();
    errInfo.setErrCode(ResultCodes.E_FATAL_ERROR_CODE);
    errInfo.setText(ResultCodes.E_FATAL_ERROR_MSG);

    Result result = new Result();
    result.setErrno(ResultCodes.E_FATAL_ERROR);
    result.setErrInfo(errInfo);

    this.setFaultActor("");
    this.setFaultCode("Client");
    this.setFaultString(msg);
    this.addResult(result);
  }
}