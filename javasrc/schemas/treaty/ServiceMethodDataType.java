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
public abstract class ServiceMethodDataType implements java.io.Serializable {


      //--------------------------/
     //- Class/Member Variables -/
    //--------------------------/

    private java.lang.String _methodName;

    private java.util.ArrayList _parameterList;

    private int _numInvokations = 1;

    /**
     * keeps track of state for field: _numInvokations
    **/
    private boolean _has_numInvokations;

    private boolean _authorized = false;

    /**
     * keeps track of state for field: _authorized
    **/
    private boolean _has_authorized;

    private java.lang.String _methodImplementation;


      //----------------/
     //- Constructors -/
    //----------------/

    public ServiceMethodDataType() {
        super();
        _parameterList = new ArrayList();
    } //-- psl.discus.javasrc.schemas.treaty.ServiceMethodDataType()


      //-----------/
     //- Methods -/
    //-----------/

    /**
     * 
     * @param vParameter
    **/
    public void addParameter(java.lang.String vParameter)
        throws java.lang.IndexOutOfBoundsException
    {
        _parameterList.add(vParameter);
    } //-- void addParameter(java.lang.String) 

    /**
     * 
     * @param index
     * @param vParameter
    **/
    public void addParameter(int index, java.lang.String vParameter)
        throws java.lang.IndexOutOfBoundsException
    {
        _parameterList.add(index, vParameter);
    } //-- void addParameter(int, java.lang.String) 

    /**
    **/
    public void clearParameter()
    {
        _parameterList.clear();
    } //-- void clearParameter() 

    /**
    **/
    public void deleteAuthorized()
    {
        this._has_authorized= false;
    } //-- void deleteAuthorized() 

    /**
    **/
    public void deleteNumInvokations()
    {
        this._has_numInvokations= false;
    } //-- void deleteNumInvokations() 

    /**
    **/
    public java.util.Enumeration enumerateParameter()
    {
        return new org.exolab.castor.util.IteratorEnumeration(_parameterList.iterator());
    } //-- java.util.Enumeration enumerateParameter() 

    /**
    **/
    public boolean getAuthorized()
    {
        return this._authorized;
    } //-- boolean getAuthorized() 

    /**
    **/
    public java.lang.String getMethodImplementation()
    {
        return this._methodImplementation;
    } //-- java.lang.String getMethodImplementation() 

    /**
    **/
    public java.lang.String getMethodName()
    {
        return this._methodName;
    } //-- java.lang.String getMethodName() 

    /**
    **/
    public int getNumInvokations()
    {
        return this._numInvokations;
    } //-- int getNumInvokations() 

    /**
     * 
     * @param index
    **/
    public java.lang.String getParameter(int index)
        throws java.lang.IndexOutOfBoundsException
    {
        //-- check bounds for index
        if ((index < 0) || (index > _parameterList.size())) {
            throw new IndexOutOfBoundsException();
        }
        
        return (String)_parameterList.get(index);
    } //-- java.lang.String getParameter(int) 

    /**
    **/
    public java.lang.String[] getParameter()
    {
        int size = _parameterList.size();
        java.lang.String[] mArray = new java.lang.String[size];
        for (int index = 0; index < size; index++) {
            mArray[index] = (String)_parameterList.get(index);
        }
        return mArray;
    } //-- java.lang.String[] getParameter() 

    /**
    **/
    public int getParameterCount()
    {
        return _parameterList.size();
    } //-- int getParameterCount() 

    /**
    **/
    public boolean hasAuthorized()
    {
        return this._has_authorized;
    } //-- boolean hasAuthorized() 

    /**
    **/
    public boolean hasNumInvokations()
    {
        return this._has_numInvokations;
    } //-- boolean hasNumInvokations() 

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
     * @param vParameter
    **/
    public boolean removeParameter(java.lang.String vParameter)
    {
        boolean removed = _parameterList.remove(vParameter);
        return removed;
    } //-- boolean removeParameter(java.lang.String) 

    /**
     * 
     * @param authorized
    **/
    public void setAuthorized(boolean authorized)
    {
        this._authorized = authorized;
        this._has_authorized = true;
    } //-- void setAuthorized(boolean) 

    /**
     * 
     * @param methodImplementation
    **/
    public void setMethodImplementation(java.lang.String methodImplementation)
    {
        this._methodImplementation = methodImplementation;
    } //-- void setMethodImplementation(java.lang.String) 

    /**
     * 
     * @param methodName
    **/
    public void setMethodName(java.lang.String methodName)
    {
        this._methodName = methodName;
    } //-- void setMethodName(java.lang.String) 

    /**
     * 
     * @param numInvokations
    **/
    public void setNumInvokations(int numInvokations)
    {
        this._numInvokations = numInvokations;
        this._has_numInvokations = true;
    } //-- void setNumInvokations(int) 

    /**
     * 
     * @param index
     * @param vParameter
    **/
    public void setParameter(int index, java.lang.String vParameter)
        throws java.lang.IndexOutOfBoundsException
    {
        //-- check bounds for index
        if ((index < 0) || (index > _parameterList.size())) {
            throw new IndexOutOfBoundsException();
        }
        _parameterList.set(index, vParameter);
    } //-- void setParameter(int, java.lang.String) 

    /**
     * 
     * @param parameterArray
    **/
    public void setParameter(java.lang.String[] parameterArray)
    {
        //-- copy array
        _parameterList.clear();
        for (int i = 0; i < parameterArray.length; i++) {
            _parameterList.add(parameterArray[i]);
        }
    } //-- void setParameter(java.lang.String) 

    /**
    **/
    public void validate()
        throws org.exolab.castor.xml.ValidationException
    {
        org.exolab.castor.xml.Validator validator = new org.exolab.castor.xml.Validator();
        validator.validate(this);
    } //-- void validate() 

}
