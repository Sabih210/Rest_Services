//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rest_Services.TpeClient {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="TpeClient.IKernel")]
    public interface IKernel {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IKernel/ProcessTransaction", ReplyAction="http://tempuri.org/IKernel/ProcessTransactionResponse")]
        [return: System.ServiceModel.MessageParameterAttribute(Name="r_Resonse")]
        string ProcessTransaction(string r_Channel, string r_TranType, string r_Request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IKernel/ProcessTransaction", ReplyAction="http://tempuri.org/IKernel/ProcessTransactionResponse")]
        [return: System.ServiceModel.MessageParameterAttribute(Name="r_Resonse")]
        System.Threading.Tasks.Task<string> ProcessTransactionAsync(string r_Channel, string r_TranType, string r_Request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IKernelChannel : Rest_Services.TpeClient.IKernel, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class KernelClient : System.ServiceModel.ClientBase<Rest_Services.TpeClient.IKernel>, Rest_Services.TpeClient.IKernel {
        
        public KernelClient() {
        }
        
        public KernelClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public KernelClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public KernelClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public KernelClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string ProcessTransaction(string r_Channel, string r_TranType, string r_Request) {
            return base.Channel.ProcessTransaction(r_Channel, r_TranType, r_Request);
        }
        
        public System.Threading.Tasks.Task<string> ProcessTransactionAsync(string r_Channel, string r_TranType, string r_Request) {
            return base.Channel.ProcessTransactionAsync(r_Channel, r_TranType, r_Request);
        }
    }
}
