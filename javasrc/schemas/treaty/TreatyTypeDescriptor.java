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

import org.exolab.castor.mapping.AccessMode;
import org.exolab.castor.mapping.ClassDescriptor;
import org.exolab.castor.mapping.FieldDescriptor;
import org.exolab.castor.xml.*;
import org.exolab.castor.xml.FieldValidator;
import org.exolab.castor.xml.TypeValidator;
import org.exolab.castor.xml.XMLFieldDescriptor;
import org.exolab.castor.xml.handlers.*;
import org.exolab.castor.xml.util.XMLFieldDescriptorImpl;
import org.exolab.castor.xml.validators.*;

/**
 * 
 * @version $Revision$ $Date$
**/
public class TreatyTypeDescriptor extends org.exolab.castor.xml.util.XMLClassDescriptorImpl {


      //--------------------------/
     //- Class/Member Variables -/
    //--------------------------/

    private java.lang.String nsPrefix;

    private java.lang.String nsURI;

    private java.lang.String xmlName;

    private org.exolab.castor.xml.XMLFieldDescriptor identity;


      //----------------/
     //- Constructors -/
    //----------------/

    public TreatyTypeDescriptor() {
        super();
        nsURI = "http://localhost/Discus/Schema/Treaty.xsd";
        xmlName = "TreatyType";
        XMLFieldDescriptorImpl  desc           = null;
        XMLFieldHandler         handler        = null;
        FieldValidator          fieldValidator = null;
        
        //-- set grouping compositor
        setCompositorAsSequence();
        //-- initialize attribute descriptors
        
        //-- initialize element descriptors
        
        //-- _treatyID
        desc = new XMLFieldDescriptorImpl(java.lang.Integer.TYPE, "_treatyID", "TreatyID", NodeType.Element);
        handler = (new XMLFieldHandler() {
            public Object getValue( Object object ) 
                throws IllegalStateException
            {
                TreatyType target = (TreatyType) object;
                if(!target.hasTreatyID())
                    return null;
                return new Integer(target.getTreatyID());
            }
            public void setValue( Object object, Object value) 
                throws IllegalStateException, IllegalArgumentException
            {
                try {
                    TreatyType target = (TreatyType) object;
                    // ignore null values for non optional primitives
                    if (value == null) return;
                    
                    target.setTreatyID( ((Integer)value).intValue());
                }
                catch (Exception ex) {
                    throw new IllegalStateException(ex.toString());
                }
            }
            public Object newInstance( Object parent ) {
                return null;
            }
        } );
        desc.setHandler(handler);
        desc.setNameSpaceURI("http://localhost/Discus/Schema/Treaty.xsd");
        desc.setRequired(true);
        desc.setMultivalued(false);
        addFieldDescriptor(desc);
        
        //-- validation code for: _treatyID
        fieldValidator = new FieldValidator();
        fieldValidator.setMinOccurs(1);
        { //-- local scope
            IntegerValidator iv = new IntegerValidator();
            fieldValidator.setValidator(iv);
        }
        desc.setValidator(fieldValidator);
        
        //-- _clientServiceSpace
        desc = new XMLFieldDescriptorImpl(java.lang.String.class, "_clientServiceSpace", "ClientServiceSpace", NodeType.Element);
        desc.setImmutable(true);
        handler = (new XMLFieldHandler() {
            public Object getValue( Object object ) 
                throws IllegalStateException
            {
                TreatyType target = (TreatyType) object;
                return target.getClientServiceSpace();
            }
            public void setValue( Object object, Object value) 
                throws IllegalStateException, IllegalArgumentException
            {
                try {
                    TreatyType target = (TreatyType) object;
                    target.setClientServiceSpace( (java.lang.String) value);
                }
                catch (Exception ex) {
                    throw new IllegalStateException(ex.toString());
                }
            }
            public Object newInstance( Object parent ) {
                return null;
            }
        } );
        desc.setHandler(handler);
        desc.setNameSpaceURI("http://localhost/Discus/Schema/Treaty.xsd");
        desc.setRequired(true);
        desc.setMultivalued(false);
        addFieldDescriptor(desc);
        
        //-- validation code for: _clientServiceSpace
        fieldValidator = new FieldValidator();
        fieldValidator.setMinOccurs(1);
        { //-- local scope
            StringValidator sv = new StringValidator();
            sv.setWhiteSpace("preserve");
            fieldValidator.setValidator(sv);
        }
        desc.setValidator(fieldValidator);
        
        //-- _providerServiceSpace
        desc = new XMLFieldDescriptorImpl(java.lang.String.class, "_providerServiceSpace", "ProviderServiceSpace", NodeType.Element);
        desc.setImmutable(true);
        handler = (new XMLFieldHandler() {
            public Object getValue( Object object ) 
                throws IllegalStateException
            {
                TreatyType target = (TreatyType) object;
                return target.getProviderServiceSpace();
            }
            public void setValue( Object object, Object value) 
                throws IllegalStateException, IllegalArgumentException
            {
                try {
                    TreatyType target = (TreatyType) object;
                    target.setProviderServiceSpace( (java.lang.String) value);
                }
                catch (Exception ex) {
                    throw new IllegalStateException(ex.toString());
                }
            }
            public Object newInstance( Object parent ) {
                return null;
            }
        } );
        desc.setHandler(handler);
        desc.setNameSpaceURI("http://localhost/Discus/Schema/Treaty.xsd");
        desc.setRequired(true);
        desc.setMultivalued(false);
        addFieldDescriptor(desc);
        
        //-- validation code for: _providerServiceSpace
        fieldValidator = new FieldValidator();
        fieldValidator.setMinOccurs(1);
        { //-- local scope
            StringValidator sv = new StringValidator();
            sv.setWhiteSpace("preserve");
            fieldValidator.setValidator(sv);
        }
        desc.setValidator(fieldValidator);
        
        //-- _serviceInfoList
        desc = new XMLFieldDescriptorImpl(ServiceInfo.class, "_serviceInfoList", "ServiceInfo", NodeType.Element);
        handler = (new XMLFieldHandler() {
            public Object getValue( Object object ) 
                throws IllegalStateException
            {
                TreatyType target = (TreatyType) object;
                return target.getServiceInfo();
            }
            public void setValue( Object object, Object value) 
                throws IllegalStateException, IllegalArgumentException
            {
                try {
                    TreatyType target = (TreatyType) object;
                    target.addServiceInfo( (ServiceInfo) value);
                }
                catch (Exception ex) {
                    throw new IllegalStateException(ex.toString());
                }
            }
            public Object newInstance( Object parent ) {
                return new ServiceInfo();
            }
        } );
        desc.setHandler(handler);
        desc.setNameSpaceURI("http://localhost/Discus/Schema/Treaty.xsd");
        desc.setRequired(true);
        desc.setMultivalued(true);
        addFieldDescriptor(desc);
        
        //-- validation code for: _serviceInfoList
        fieldValidator = new FieldValidator();
        fieldValidator.setMinOccurs(1);
        desc.setValidator(fieldValidator);
        
    } //-- psl.discus.javasrc.schemas.treaty.TreatyTypeDescriptor()


      //-----------/
     //- Methods -/
    //-----------/

    /**
    **/
    public org.exolab.castor.mapping.AccessMode getAccessMode()
    {
        return null;
    } //-- org.exolab.castor.mapping.AccessMode getAccessMode() 

    /**
    **/
    public org.exolab.castor.mapping.ClassDescriptor getExtends()
    {
        return null;
    } //-- org.exolab.castor.mapping.ClassDescriptor getExtends() 

    /**
    **/
    public org.exolab.castor.mapping.FieldDescriptor getIdentity()
    {
        return identity;
    } //-- org.exolab.castor.mapping.FieldDescriptor getIdentity() 

    /**
    **/
    public java.lang.Class getJavaClass()
    {
        return psl.discus.javasrc.schemas.treaty.TreatyType.class;
    } //-- java.lang.Class getJavaClass() 

    /**
    **/
    public java.lang.String getNameSpacePrefix()
    {
        return nsPrefix;
    } //-- java.lang.String getNameSpacePrefix() 

    /**
    **/
    public java.lang.String getNameSpaceURI()
    {
        return nsURI;
    } //-- java.lang.String getNameSpaceURI() 

    /**
    **/
    public org.exolab.castor.xml.TypeValidator getValidator()
    {
        return this;
    } //-- org.exolab.castor.xml.TypeValidator getValidator() 

    /**
    **/
    public java.lang.String getXMLName()
    {
        return xmlName;
    } //-- java.lang.String getXMLName() 

}
