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
public abstract class TreatyType implements java.io.Serializable {


      //--------------------------/
     //- Class/Member Variables -/
    //--------------------------/

    private int _treatyID = 0;

    /**
     * keeps track of state for field: _treatyID
    **/
    private boolean _has_treatyID;

    private java.lang.String _clientServiceSpace;

    private java.lang.String _providerServiceSpace;

    private java.util.ArrayList _serviceInfoList;


      //----------------/
     //- Constructors -/
    //----------------/

    public TreatyType() {
        super();
        _serviceInfoList = new ArrayList();
    } //-- psl.discus.javasrc.schemas.treaty.TreatyType()


      //-----------/
     //- Methods -/
    //-----------/

    /**
     * 
     * @param vServiceInfo
    **/
    public void addServiceInfo(ServiceInfo vServiceInfo)
        throws java.lang.IndexOutOfBoundsException
    {
        _serviceInfoList.add(vServiceInfo);
    } //-- void addServiceInfo(ServiceInfo) 

    /**
     * 
     * @param index
     * @param vServiceInfo
    **/
    public void addServiceInfo(int index, ServiceInfo vServiceInfo)
        throws java.lang.IndexOutOfBoundsException
    {
        _serviceInfoList.add(index, vServiceInfo);
    } //-- void addServiceInfo(int, ServiceInfo) 

    /**
    **/
    public void clearServiceInfo()
    {
        _serviceInfoList.clear();
    } //-- void clearServiceInfo() 

    /**
    **/
    public java.util.Enumeration enumerateServiceInfo()
    {
        return new org.exolab.castor.util.IteratorEnumeration(_serviceInfoList.iterator());
    } //-- java.util.Enumeration enumerateServiceInfo() 

    /**
    **/
    public java.lang.String getClientServiceSpace()
    {
        return this._clientServiceSpace;
    } //-- java.lang.String getClientServiceSpace() 

    /**
    **/
    public java.lang.String getProviderServiceSpace()
    {
        return this._providerServiceSpace;
    } //-- java.lang.String getProviderServiceSpace() 

    /**
     * 
     * @param index
    **/
    public ServiceInfo getServiceInfo(int index)
        throws java.lang.IndexOutOfBoundsException
    {
        //-- check bounds for index
        if ((index < 0) || (index > _serviceInfoList.size())) {
            throw new IndexOutOfBoundsException();
        }
        
        return (ServiceInfo) _serviceInfoList.get(index);
    } //-- ServiceInfo getServiceInfo(int) 

    /**
    **/
    public ServiceInfo[] getServiceInfo()
    {
        int size = _serviceInfoList.size();
        ServiceInfo[] mArray = new ServiceInfo[size];
        for (int index = 0; index < size; index++) {
            mArray[index] = (ServiceInfo) _serviceInfoList.get(index);
        }
        return mArray;
    } //-- ServiceInfo[] getServiceInfo() 

    /**
    **/
    public int getServiceInfoCount()
    {
        return _serviceInfoList.size();
    } //-- int getServiceInfoCount() 

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
     * @param vServiceInfo
    **/
    public boolean removeServiceInfo(ServiceInfo vServiceInfo)
    {
        boolean removed = _serviceInfoList.remove(vServiceInfo);
        return removed;
    } //-- boolean removeServiceInfo(ServiceInfo) 

    /**
     * 
     * @param clientServiceSpace
    **/
    public void setClientServiceSpace(java.lang.String clientServiceSpace)
    {
        this._clientServiceSpace = clientServiceSpace;
    } //-- void setClientServiceSpace(java.lang.String) 

    /**
     * 
     * @param providerServiceSpace
    **/
    public void setProviderServiceSpace(java.lang.String providerServiceSpace)
    {
        this._providerServiceSpace = providerServiceSpace;
    } //-- void setProviderServiceSpace(java.lang.String) 

    /**
     * 
     * @param index
     * @param vServiceInfo
    **/
    public void setServiceInfo(int index, ServiceInfo vServiceInfo)
        throws java.lang.IndexOutOfBoundsException
    {
        //-- check bounds for index
        if ((index < 0) || (index > _serviceInfoList.size())) {
            throw new IndexOutOfBoundsException();
        }
        _serviceInfoList.set(index, vServiceInfo);
    } //-- void setServiceInfo(int, ServiceInfo) 

    /**
     * 
     * @param serviceInfoArray
    **/
    public void setServiceInfo(ServiceInfo[] serviceInfoArray)
    {
        //-- copy array
        _serviceInfoList.clear();
        for (int i = 0; i < serviceInfoArray.length; i++) {
            _serviceInfoList.add(serviceInfoArray[i]);
        }
    } //-- void setServiceInfo(ServiceInfo) 

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
