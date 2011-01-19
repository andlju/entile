using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace Entile.Service
{
    public class EntileInstanceProvider : IInstanceProvider
    {
        private readonly Type _serviceType;
        private readonly IRegistrator _registrator;
        private readonly INotifier _notifier;

        public EntileInstanceProvider(Type serviceType, IRegistrator registrator, INotifier notifier)
        {
            _serviceType = serviceType;
            _registrator = registrator;
            _notifier = notifier;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            // TODO Really ugly way of allowing different constructor types. 
            // Should use a simple IoC container or similar instead.
            
            // Try with both a Registrator and a Notifier
            try
            {
                var instance = Activator.CreateInstance(_serviceType, _registrator, _notifier);
                return instance;
            }
            catch (MissingMethodException)
            {
            }

            // Try with only a Registrator
            try
            {
                var instance = Activator.CreateInstance(_serviceType, _registrator);
                return instance;
            }
            catch (MissingMethodException)
            {
            }

            // Try with only a Notifier
            try
            {
                var instance = Activator.CreateInstance(_serviceType, _notifier);
                return instance;
            }
            catch (MissingMethodException)
            {
            }

            // Try with nothing
            try
            {
                var instance = Activator.CreateInstance(_serviceType);
                return instance;
            }
            catch (MissingMethodException)
            {
            }

            return null;
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }
    }

    public class EntileInstanceBehavior : IServiceBehavior
    {
        private readonly IRegistrator _registrator;
        private readonly INotifier _notifier;

        public EntileInstanceBehavior(IRegistrator registrator, INotifier notifier)
        {
            _registrator = registrator;
            _notifier = notifier;
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var cdb in serviceHostBase.ChannelDispatchers)
            {
                var cd = cdb as ChannelDispatcher;
                if (cd == null)
                    continue;

                foreach (var endpoint in cd.Endpoints)
                {
                    endpoint.DispatchRuntime.InstanceProvider =
                        new EntileInstanceProvider(serviceDescription.ServiceType, _registrator, _notifier);
                }
            }
        }
    }

    public class EntileServiceHost : WebServiceHost
    {
        private readonly IRegistrator _registrator;
        private readonly INotifier _notifier;
        public EntileServiceHost(IRegistrator registrator, INotifier notifier, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            _registrator = registrator;
            _notifier = notifier;
        }

        protected override void OnOpening()
        {
            Description.Behaviors.Add(new EntileInstanceBehavior(_registrator, _notifier));
            base.OnOpening();
        }
    }

    public class EntileRegistrationServiceFactory : WebServiceHostFactory
    {
        private readonly IRegistrator _registrator;
        private readonly INotifier _notifier;

        public EntileRegistrationServiceFactory(IRegistrator registrator, INotifier notifier)
        {
            _registrator = registrator;
            _notifier = notifier;
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new EntileServiceHost(_registrator, _notifier, serviceType, baseAddresses);
        }
    }
}