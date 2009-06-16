﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuvoControl.Client.TestViewer.ConfigurationServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Point", Namespace="http://schemas.datacontract.org/2004/07/System.Drawing")]
    [System.SerializableAttribute()]
    public partial struct Point : System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int xField;
        
        private int yField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int x {
            get {
                return this.xField;
            }
            set {
                if ((this.xField.Equals(value) != true)) {
                    this.xField = value;
                    this.RaisePropertyChanged("x");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int y {
            get {
                return this.yField;
            }
            set {
                if ((this.yField.Equals(value) != true)) {
                    this.yField = value;
                    this.RaisePropertyChanged("y");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ConfigurationServiceReference.IConfigure")]
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
    public interface IConfigureChannel : NuvoControl.Client.TestViewer.ConfigurationServiceReference.IConfigure, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class ConfigureClient : System.ServiceModel.ClientBase<NuvoControl.Client.TestViewer.ConfigurationServiceReference.IConfigure>, NuvoControl.Client.TestViewer.ConfigurationServiceReference.IConfigure {
        
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
