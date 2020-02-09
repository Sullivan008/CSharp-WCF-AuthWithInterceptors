using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace ServerForAuthenticationAndAuthorization.Extensions.IDispatchMessageInspector
{
    public class InterceptorBehaviorExtension : BehaviorExtensionElement, IServiceBehavior
    {
        public override Type BehaviorType =>
            typeof(InterceptorBehaviorExtension);

        protected override object CreateBehavior() =>
            new InterceptorBehaviorExtension();

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var item in serviceHostBase.ChannelDispatchers)
            {
                if (item is ChannelDispatcher channelDispatcher)
                {
                    foreach (var endpointDispatcher in channelDispatcher.Endpoints)
                    {
                        endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new IdentityMessageInspector());
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        { }
    }
}