using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Entile.Common;

namespace Entile.Service
{
    public class EntileInstanceProvider : IInstanceProvider
    {
        private readonly Type _serviceType;
        private readonly IRegistrator _registrator;
        private readonly INotificationQueue _notificationQueue;

        public EntileInstanceProvider(Type serviceType, IRegistrator registrator, INotificationQueue notificationQueue)
        {
            _serviceType = serviceType;
            _registrator = registrator;
            _notificationQueue = notificationQueue;
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
                var instance = Activator.CreateInstance(_serviceType, _registrator, _notificationQueue);
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
                var instance = Activator.CreateInstance(_serviceType, _notificationQueue);
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
        private readonly INotificationQueue _notificationQueue;

        public EntileInstanceBehavior(IRegistrator registrator, INotificationQueue notificationQueue)
        {
            _registrator = registrator;
            _notificationQueue = notificationQueue;
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
                        new EntileInstanceProvider(serviceDescription.ServiceType, _registrator, _notificationQueue);
                }
            }
        }
    }

    public class EntileServiceHost : WebServiceHost
    {
        private readonly IRegistrator _registrator;
        private readonly INotificationQueue _notificationQueue;
        public EntileServiceHost(IRegistrator registrator, INotificationQueue notificationQueue, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            _registrator = registrator;
            _notificationQueue = notificationQueue;
        }

        protected override void OnOpening()
        {
            Description.Behaviors.Add(new EntileInstanceBehavior(_registrator, _notificationQueue));
            base.OnOpening();
        }
    }

    public class EntileRegistrationServiceFactory : WebServiceHostFactory
    {
        private readonly IRegistrator _registrator;
        private readonly INotificationQueue _notificationQueue;

        public EntileRegistrationServiceFactory(IRegistrator registrator, INotificationQueue notificationQueue)
        {
            _registrator = registrator;
            _notificationQueue = notificationQueue;
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new EntileServiceHost(_registrator, _notificationQueue, serviceType, baseAddresses);
        }
    }
}