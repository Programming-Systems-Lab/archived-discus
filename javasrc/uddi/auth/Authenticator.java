/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.auth;

import psl.discus.javasrc.uddi.error.JUDDIException;

import org.uddi4j.response.AuthToken;
import org.uddi4j.util.AuthInfo;

/**
 * @author  Steve Viens
 * @version 0.6
 */
public interface Authenticator
{
 /**
  *
  */
  void init();

 /**
  *
  */
  void destroy();

 /**
  *
  */
  public AuthToken getAuthToken(String authorizedName,String credential)
    throws JUDDIException;

 /**
  *
  */
  public void discardAuthToken(AuthInfo authInfo)
    throws JUDDIException;

 /**
  *
  */
  public void validateAuthToken(AuthInfo authInfo)
    throws JUDDIException;


 /**
  *
  */
  public String getAuthorizedName(AuthInfo authInfo)
    throws JUDDIException;
}