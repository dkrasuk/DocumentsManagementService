﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccessLayer.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.0.1.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://kwsesbapu03.alfa.bank.int:8243/services/Contragent.ContragentHttpsSoap11E" +
            "ndpoint/")]
        public string DataAccessLayer_ContragentSoapService_Contragent {
            get {
                return ((string)(this["DataAccessLayer_ContragentSoapService_Contragent"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://kwsesbapu03.alfa.bank.int:8243/services/LoanDealPT.LoanDealPTHttpsSoap11E" +
            "ndpoint/")]
        public string DataAccessLayer_LoanDealSoapWebService_LoanDeal {
            get {
                return ((string)(this["DataAccessLayer_LoanDealSoapWebService_LoanDeal"]));
            }
        }
    }
}
