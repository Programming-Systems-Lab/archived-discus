/*
 * This class was automatically generated with 
 * <a href="http://castor.exolab.org">Castor 0.9.3</a>, using an
 * XML Schema.
 * $Id$
 */

package psl.discus.javasrc.schemas.execServiceMethodRequest;

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
public abstract class ExecServiceMethodRequestType implements java.io.Serializable {


      //--------------------------/
     //- Class/Member Variables -/
    //--------------------------/

    private int _treatyID = 0;

    /**
     * keeps track of state for field: _treatyID
    **/
    private boolean _has_treatyID;

    private java.lang.String _serviceName;

    private java.lang.String _methodName;

    private java.util.ArrayList _parameterList;


      //----------------/
     //- Constructors -/
    //----------------/

    public ExecServiceMethodRequestType() {
        super();
        _parameterList = new ArrayList();
    } //-- psl.discus.javasrc.schemas.execServiceMethodRequest.ExecServiceMethodRequestType()


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
    public java.util.Enumeration enumerateParameter()
    {
        return new org.exolab.castor.util.IteratorEnumeration(_parameterList.iterator());
    } //-- java.util.Enumeration enumerateParameter() 

    /**
    **/
    public java.lang.String getMethodName()
    {
        return this._methodName;
    } //-- java.lang.String getMethodName() 

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
     * added by matias: counts only parameters that are filled in
     */
    public int getFilledParameterCount()
    {
        int count = 0;
        for (Enumeration e = enumerateParameter(); e.hasMoreElements();) {
            String param = (String) e.nextElement();
            if (param != null && param.length() > 0)
                count++;
        }

        return count;

    }

    /**
    **/
    public java.lang.String getServiceName()
    {
        return this._serviceName;
    } //-- java.lang.String getServiceName() 

    /**
    **/
    public int getTreatyID()
    {
        return this._treatyID;
    } //-- int getTreatyID() 

    /**
    **/
    public boolean hasTreatyID()
    {
        return this._has_treatyID;
    } //-- boolean hasTreatyID() 

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
     * @param methodName
    **/
    public void setMethodName(java.lang.String methodName)
    {
        this._methodName = methodName;
    } //-- void setMethodName(java.lang.String) 

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
     * 
     * @param serviceName
    **/
    public void setServiceName(java.lang.String serviceName)
    {
        this._serviceName = serviceName;
    } //-- void setServiceName(java.lang.String) 

    /**
     * 
     * @param treatyID
    **/
    public void setTreatyID(int treatyID)
    {
        this._treatyID = treatyID;
        this._has_treatyID = true;
    } //-- void setTreatyID(int) 

    /**
    **/
    public void validate()
        throws org.exolab.castor.xml.ValidationException
    {
        org.exolab.castor.xml.Validator validator = new org.exolab.castor.xml.Validator();
        validator.validate(this);
    } //-- void validate() 

}
