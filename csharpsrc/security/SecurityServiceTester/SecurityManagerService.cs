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
// This source code was auto-generated by wsdl, Version=1.0.3705.209.
// 
using System.Diagnostics;
using System.Xml.Serialization;
using System;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Services;


/// <remarks/>
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name="SecurityManagerServiceBinding", Namespace="http://localhost/wsdl")]
public class SecurityManagerService : System.Web.Services.Protocols.SoapHttpClientProtocol {
    
    /// <remarks/>
    public SecurityManagerService() {
        this.Url = "http://church.psl.cs.columbia.edu:8080/security/jaxrpc/SecurityManagerService";
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://localhost/wsdl", ResponseNamespace="http://localhost/wsdl")]
    [return: System.Xml.Serialization.SoapElementAttribute("result")]
    public string[] doRequestCheck(string String_1, bool boolean_2) {
        object[] results = this.Invoke("doRequestCheck", new object[] {
                    String_1,
                    boolean_2});
        return ((string[])(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BegindoRequestCheck(string String_1, bool boolean_2, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("doRequestCheck", new object[] {
                    String_1,
                    boolean_2}, callback, asyncState);
    }
    
    /// <remarks/>
    public string[] EnddoRequestCheck(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((string[])(results[0]));
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://localhost/wsdl", ResponseNamespace="http://localhost/wsdl")]
    [return: System.Xml.Serialization.SoapElementAttribute("result")]
    public string[] signDocument(string String_1) {
        object[] results = this.Invoke("signDocument", new object[] {
                    String_1});
        return ((string[])(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginsignDocument(string String_1, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("signDocument", new object[] {
                    String_1}, callback, asyncState);
    }
    
    /// <remarks/>
    public string[] EndsignDocument(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((string[])(results[0]));
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://localhost/wsdl", ResponseNamespace="http://localhost/wsdl")]
    [return: System.Xml.Serialization.SoapElementAttribute("result")]
    public string[] verifyDocument(string String_1) {
        object[] results = this.Invoke("verifyDocument", new object[] {
                    String_1});
        return ((string[])(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginverifyDocument(string String_1, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("verifyDocument", new object[] {
                    String_1}, callback, asyncState);
    }
    
    /// <remarks/>
    public string[] EndverifyDocument(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((string[])(results[0]));
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://localhost/wsdl", ResponseNamespace="http://localhost/wsdl")]
    [return: System.Xml.Serialization.SoapElementAttribute("result")]
    public string[] verifyTreaty(string String_1, bool boolean_2) {
        object[] results = this.Invoke("verifyTreaty", new object[] {
                    String_1,
                    boolean_2});
        return ((string[])(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginverifyTreaty(string String_1, bool boolean_2, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("verifyTreaty", new object[] {
                    String_1,
                    boolean_2}, callback, asyncState);
    }
    
    /// <remarks/>
    public string[] EndverifyTreaty(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((string[])(results[0]));
    }
}
