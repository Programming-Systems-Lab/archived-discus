namespace DynamicPxy {
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Web.Services;
    
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="WeatherRetrieverSoap", Namespace="http://tempuri.org/")]
    public class WeatherRetriever : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        /// <remarks/>
        public WeatherRetriever() {
            this.Url = "http://www.vbws.com/services/weatherretriever.asmx";
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://tempuri.org/GetTemperature", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/")]
        public System.Single GetTemperature(string zipCode) {
            object[] results = this.Invoke("GetTemperature", new object[] {
                        zipCode});
            return ((System.Single)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetTemperature(string zipCode, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetTemperature", new object[] {
                        zipCode}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Single EndGetTemperature(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Single)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://tempuri.org/GetWeather", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/")]
        public CurrentWeather GetWeather(string zipCode) {
            object[] results = this.Invoke("GetWeather", new object[] {
                        zipCode});
            return ((CurrentWeather)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetWeather(string zipCode, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetWeather", new object[] {
                        zipCode}, callback, asyncState);
        }
        
        /// <remarks/>
        public CurrentWeather EndGetWeather(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((CurrentWeather)(results[0]));
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.SoapTypeAttribute("CurrentWeather", "http://tempuri.org/")]
    public class CurrentWeather {
        
        /// <remarks/>
        public string LastUpdated;
        
        /// <remarks/>
        public string IconUrl;
        
        /// <remarks/>
        public string Conditions;
        
        /// <remarks/>
        public System.Single CurrentTemp;
        
        /// <remarks/>
        public System.Single Humidity;
        
        /// <remarks/>
        public System.Single Barometer;
        
        /// <remarks/>
        public string BarometerDirection;
    }
}
