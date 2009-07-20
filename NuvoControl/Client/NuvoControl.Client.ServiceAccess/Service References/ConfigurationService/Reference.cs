﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuvoControl.Client.ServiceAccess.ConfigurationService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Point", Namespace="http://schemas.datacontract.org/2004/07/System.Windows")]
    [System.SerializableAttribute()]
    public partial struct Point : System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private double _xField;
        
        private double _yField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public double _x {
            get {
                return this._xField;
            }
            set {
                if ((this._xField.Equals(value) != true)) {
                    this._xField = value;
                    this.RaisePropertyChanged("_x");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public double _y {
            get {
                return this._yField;
            }
            set {
                if ((this._yField.Equals(value) != true)) {
                    this._yField = value;
                    this.RaisePropertyChanged("_y");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ConfigurationService.IConfigure")]
    public interface IConfigure {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigure/GetGraphicConfiguration", ReplyAction="http://tempuri.org/IConfigure/GetGraphicConfigurationResponse")]
        NuvoControl.Common.Configuration.Graphic GetGraphicConfiguration();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigure/GetZoneKonfiguration", ReplyAction="http://tempuri.org/IConfigure/GetZoneKonfigurationResponse")]
        NuvoControl.Common.Configuration.Zone GetZoneKonfiguration(NuvoControl.Common.Configuration.Address zoneId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigure/GetFunction", ReplyAction="http://tempuri.org/IConfigure/GetFunctionResponse")]
        NuvoControl.Common.Configuration.Function GetFunction(System.Guid id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigure/GetFunctions", ReplyAction="http://tempuri.org/IConfigure/GetFunctionsResponse")]
        NuvoControl.Common.Configuration.Function[] GetFunctions(NuvoControl.Common.Configuration.Address zoneId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConfigure/AddFunction", ReplyAction="http://tempuri.org/IConfigure/AddFunctionResponse")]
        bool AddFunction(NuvoControl.Common.Configuration.Function newFunction);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IConfigureChannel : NuvoControl.Client.ServiceAccess.ConfigurationService.IConfigure, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class ConfigureClient : System.ServiceModel.ClientBase<NuvoControl.Client.ServiceAccess.ConfigurationService.IConfigure>, NuvoControl.Client.ServiceAccess.ConfigurationService.IConfigure {
        
        public ConfigureClient() {
        }
        
        public ConfigureClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ConfigureClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConfigureClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConfigureClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public NuvoControl.Common.Configuration.Graphic GetGraphicConfiguration() {
            return base.Channel.GetGraphicConfiguration();
        }
        
        public NuvoControl.Common.Configuration.Zone GetZoneKonfiguration(NuvoControl.Common.Configuration.Address zoneId) {
            return base.Channel.GetZoneKonfiguration(zoneId);
        }
        
        public NuvoControl.Common.Configuration.Function GetFunction(System.Guid id) {
            return base.Channel.GetFunction(id);
        }
        
        public NuvoControl.Common.Configuration.Function[] GetFunctions(NuvoControl.Common.Configuration.Address zoneId) {
            return base.Channel.GetFunctions(zoneId);
        }
        
        public bool AddFunction(NuvoControl.Common.Configuration.Function newFunction) {
            return base.Channel.AddFunction(newFunction);
        }
    }
}