﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.0.3705.209
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 1.0.3705.209.
// 
namespace DISCUSGui.localhost {
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Web.Services;
    
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="PSLGatekeeper2Soap", Namespace="http://tempuri.org/")]
    public class PSLGatekeeper2 : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        /// <remarks/>
        public PSLGatekeeper2() {
            this.Url = "http://localhost/PSLGatekeeper2/PSLGatekeeper2.asmx";
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ExecuteServiceMethod", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ExecuteServiceMethod(string strXMLExecRequest) {
            object[] results = this.Invoke("ExecuteServiceMethod", new object[] {
                        strXMLExecRequest});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginExecuteServiceMethod(string strXMLExecRequest, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ExecuteServiceMethod", new object[] {
                        strXMLExecRequest}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndExecuteServiceMethod(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ExecuteAlphaProtocol", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] ExecuteAlphaProtocol(string strAlphaProtocol) {
            object[] results = this.Invoke("ExecuteAlphaProtocol", new object[] {
                        strAlphaProtocol});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginExecuteAlphaProtocol(string strAlphaProtocol, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ExecuteAlphaProtocol", new object[] {
                        strAlphaProtocol}, callback, asyncState);
        }
        
        /// <remarks/>
        public string[] EndExecuteAlphaProtocol(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/EnlistServicesByName", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string EnlistServicesByName(string strXmlTreatyReq) {
            object[] results = this.Invoke("EnlistServicesByName", new object[] {
                        strXmlTreatyReq});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginEnlistServicesByName(string strXmlTreatyReq, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("EnlistServicesByName", new object[] {
                        strXmlTreatyReq}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndEnlistServicesByName(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
    }
}
