/*
 * This class was automatically generated with 
 * <a href="http://castor.exolab.org">Castor 0.9.3</a>, using an
 * XML Schema.
 * $Id$
 */

package psl.discus.javasrc.schemas.securityManagerResponse;

  //---------------------------------/
 //- Imported classes and packages -/
//---------------------------------/

import java.io.Reader;
import java.io.Serializable;
import java.io.Writer;
import org.exolab.castor.xml.*;
import org.exolab.castor.xml.MarshalException;
import org.exolab.castor.xml.ValidationException;
import org.xml.sax.DocumentHandler;

/**
 * 
 * @version $Revision$ $Date$
**/
public abstract class SecurityManagerResponseType implements java.io.Serializable {


      //--------------------------/
     //- Class/Member Variables -/
    //--------------------------/

    private int _status = 0;

    /**
     * keeps track of state for field: _status
    **/
    private boolean _has_status;

    private java.lang.String _message;

    private java.lang.String _content;


      //----------------/
     //- Constructors -/
    //----------------/

    public SecurityManagerResponseType() {
        super();
    } //-- psl.discus.javasrc.schemas.securityManagerResponse.SecurityManagerResponseType()


      //-----------/
     //- Methods -/
    //-----------/

    /**
    **/
    public java.lang.String getContent()
    {
        return this._content;
    } //-- java.lang.String getContent() 

    /**
    **/
    public java.lang.String getMessage()
    {
        return this._message;
    } //-- java.lang.String getMessage() 

    /**
    **/
    public int getStatus()
    {
        return this._status;
    } //-- int getStatus() 

    /**
    **/
    public boolean hasStatus()
    {
        return this._has_status;
    } //-- boolean hasStatus() 

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
     * @param content
    **/
    public void setContent(java.lang.String content)
    {
        this._content = content;
    } //-- void setContent(java.lang.String) 

    /**
     * 
     * @param message
    **/
    public void setMessage(java.lang.String message)
    {
        this._message = message;
    } //-- void setMessage(java.lang.String) 

    /**
     * 
     * @param status
    **/
    public void setStatus(int status)
    {
        this._status = status;
        this._has_status = true;
    } //-- void setStatus(int) 

    /**
    **/
    public void validate()
        throws org.exolab.castor.xml.ValidationException
    {
        org.exolab.castor.xml.Validator validator = new org.exolab.castor.xml.Validator();
        validator.validate(this);
    } //-- void validate() 

}
