/*
 * jUDDI - An open source Java implementation of UDDI v2.0
 * http://juddi.org/
 *
 * Copyright (c) 2002, Steve Viens and contributors
 * All rights reserved.
 */

package psl.discus.javasrc.uddi.datastore;

import org.uddi4j.datatype.assertion.*;
import org.uddi4j.datatype.binding.*;
import org.uddi4j.datatype.business.*;
import org.uddi4j.datatype.service.*;
import org.uddi4j.datatype.tmodel.*;

import java.util.Vector; 

/**
 * @author  Steve Viens
 * @version 0.6
 */
public interface DataStore
{  
 /**
  * begin transaction
  */
  public void beginTrans()
    throws psl.discus.javasrc.uddi.error.JUDDIException;

 /**
  * commit transaction
  */
  public void commit()
    throws psl.discus.javasrc.uddi.error.JUDDIException;
  
 /**
  * rollback transaction
  */
  public void rollback()
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public void saveBusiness(BusinessEntity business)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public BusinessEntity fetchBusiness(String businessKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public void deleteBusiness(String businessKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public boolean isBusinessOwner(String businessKey,String authorizedName)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public boolean isValidBusinessKey(String businessKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public void saveService(BusinessService service)
    throws psl.discus.javasrc.uddi.error.JUDDIException;


  /**
   *
   */
  public BusinessService fetchService(String serviceKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public void deleteService(String serviceKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public Vector fetchServiceByBusinessKey(String businessKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public void deleteServiceByBusinessKey(String businessKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public boolean isValidServiceKey(String serviceKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public boolean isServiceOwner(String serviceKey,String authorizedName)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public void saveBinding(BindingTemplate binding)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public BindingTemplate fetchBinding(String bindingKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public void deleteBinding(String bindingKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public Vector fetchBindingByServiceKey(String serviceKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public void deleteBindingByServiceKey(String serviceKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public boolean isValidBindingKey(String bindingKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public boolean isBindingOwner(String bindingKey,String authorizedName)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public void saveTModel(TModel tModel)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public TModel fetchTModel(String tModelKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public void deleteTModel(String tModelKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public boolean isValidTModelKey(String tModelKey)
    throws psl.discus.javasrc.uddi.error.JUDDIException;

  /**
   *
   */
  public boolean isTModelOwner(String tModelKey,String authorizedName)
    throws psl.discus.javasrc.uddi.error.JUDDIException;
}