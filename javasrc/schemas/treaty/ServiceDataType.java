/*
 * This class was automatically generated with 
 * <a href="http://castor.exolab.org">Castor 0.9.3</a>, using an
 * XML Schema.
 * $Id$
 */

package psl.discus.javasrc.schemas.treaty;

  //---------------------------------/
 //- Imported classes and packages -/
//---------------------------------/

import java.io.Reader;
import java.io.Serializable;
import java.io.Writer;
import java.util.ArrayList;
import java.util.Enumeration;
import org.exolab.castor.xml.*;
import org.exolab.castor.xml.MarshalException;
import org.exolab.castor.xml.ValidationException;
import org.xml.sax.DocumentHandler;

/**
 * 
 * @version $Revision$ $Date$
**/
public abstract class ServiceDataType implements java.io.Serializable {


      //--------------------------/
     //- Class/Member Variables -/
    //--------------------------/

    private java.lang.String _serviceName;

    private java.util.ArrayList _serviceMethodList;


      //----------------/
     //- Constructors -/
    //----------------/

    public ServiceDataType() {
        super();
        _serviceMethodList = new ArrayList();
    } //-- psl.discus.javasrc.schemas.treaty.ServiceDataType()


      //-----------/
     //- Methods -/
    //-----------/

    /**
     * 
     * @param vServiceMethod
    **/
    public void addServiceMethod(ServiceMethod vServiceMethod)
        throws java.lang.IndexOutOfBoundsException
    {
        _serviceMethodList.add(vServiceMethod);
    } //-- void addServiceMethod(ServiceMethod) 

    /**
     * 
     * @param index
     * @param vServiceMethod
    **/
    public void addServiceMethod(int index, ServiceMethod vServiceMethod)
        throws java.lang.IndexOutOfBoundsException
    {
        _serviceMethodList.add(index, vServiceMethod);
    } //-- void addServiceMethod(int, ServiceMethod) 

    /**
    **/
    public void clearServiceMethod()
    {
        _serviceMethodList.clear();
    } //-- void clearServiceMethod() 

    /**
    **/
    public java.util.Enumeration enumerateServiceMethod()
    {
        return new org.exolab.castor.util.IteratorEnumeration(_serviceMethodList.iterator());
    } //-- java.util.Enumeration enumerateServiceMethod() 

    /**
     * 
     * @param index
    **/
    public ServiceMethod getServiceMethod(int index)
        throws java.lang.IndexOutOfBoundsException
    {
        //-- check bounds for index
        if ((index < 0) || (index > _serviceMethodList.size())) {
            throw new IndexOutOfBoundsException();
        }
        
        return (ServiceMethod) _serviceMethodList.get(index);
    } //-- ServiceMethod getServiceMethod(int) 

    /**
    **/
    public ServiceMethod[] getServiceMethod()
    {
        int size = _serviceMethodList.size();
        ServiceMethod[] mArray = new ServiceMethod[size];
        for (int index = 0; index < size; index++) {
            mArray[index] = (ServiceMethod) _serviceMethodList.get(index);
        }
        return mArray;
    } //-- ServiceMethod[] getServiceMethod() 

    /**
    **/
    public int getServiceMethodCount()
    {
        return _serviceMethodList.size();
    } //-- int getServiceMethodCount() 

    /**
    **/
    public java.lang.String getServiceName()
    {
        return this._serviceName;
    } //-- java.lang.String getServiceName() 

    /**
    **/
    public boolean isValid()
    {
        try {
            validate();
        }
        catch (org.exolab.castor.xml.ValidationException vex) {
            return false;
        }
        return true;
    } //-- boolean isValid() 

    /**
     * 
     * @param out
    **/
    public abstract void marshal(java.io.Writer out)
        throws org.exolab.castor.xml.MarshalException, org.exolab.castor.xml.ValidationException;

    /**
     * 
     * @param handler
    **/
    public abstract void marshal(org.xml.sax.DocumentHandler handler)
        throws org.exolab.castor.xml.MarshalException, org.exolab.castor.xml.ValidationException;

    /**
     * 
     * @param vServiceMethod
    **/
    public boolean removeServiceMethod(ServiceMethod vServiceMethod)
    {
        boolean removed = _serviceMethodList.remove(vServiceMethod);
        return removed;
    } //-- boolean removeServiceMethod(ServiceMethod) 

    /**
     * 
     * @param index
     * @param vServiceMethod
    **/
    public void setServiceMethod(int index, ServiceMethod vServiceMethod)
        throws java.lang.IndexOutOfBoundsException
    {
        //-- check bounds for index
        if ((index < 0) || (index > _serviceMethodList.size())) {
            throw new IndexOutOfBoundsException();
        }
        _serviceMethodList.set(index, vServiceMethod);
    } //-- void setServiceMethod(int, ServiceMethod) 

    /**
     * 
     * @param serviceMethodArray
    **/
    public void setServiceMethod(ServiceMethod[] serviceMethodArray)
    {
        //-- copy array
        _serviceMethodList.clear();
        for (int i = 0; i < serviceMethodArray.length; i++) {
            _serviceMethodList.add(serviceMethodArray[i]);
        }
    } //-- void setServiceMethod(ServiceMethod) 

    /**
     * 
     * @param serviceName
    **/
    public void setServiceName(java.lang.String serviceName)
    {
        this._serviceName = serviceName;
    } //-- void setServiceName(java.lang.String) 

    /**
    **/
    public void validate()
        throws org.exolab.castor.xml.ValidationException
    {
        org.exolab.castor.xml.Validator validator = new org.exolab.castor.xml.Validator();
        validator.validate(this);
    } //-- void validate() 

}
