using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace ClientForAuthenticationAndAuthorization.Extensions.IClientMessageInspectors
{
    public class InterceptorBehaviorExtension : BehaviorExtensionElement, IEndpointBehavior
    {
        public override Type BehaviorType =>
            typeof(InterceptorBehaviorExtension);

        protected override object CreateBehavior() =>
            new InterceptorBehaviorExtension();

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new CookieMessageInspector());
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        { }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        { }

        public void Validate(ServiceEndpoint endpoint)
        { }
    }
}