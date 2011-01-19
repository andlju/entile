using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Web.Routing;
using Entile.Service.Store;

namespace Entile.Service
{
    public static class EntileHost
    {
        private static List<IEntileModule> _entileModules = new List<IEntileModule>();

        public static void Initialize()
        {

        }

        public static void RegisterModule(IEntileModule entileModule, IRegistrationStore registrationStore)
        {
            var moduleName = entileModule.ModuleName;

            // TODO Support other stores
            IRegistrationStore store = registrationStore;
            if (store == null)
                store = new XmlFileRegistrationStore(moduleName);

            var notifier = new Notifier(store);
            var registrator = new Registrator(store, entileModule.RemoteTileUriFormat);

            entileModule.Initialize(notifier, registrator);

            RouteTable.Routes.Add(new ServiceRoute(moduleName + "/Registration", new EntileRegistrationServiceFactory(registrator, notifier), typeof(RegistrationService)));
            RouteTable.Routes.Add(new ServiceRoute(moduleName, new EntileRegistrationServiceFactory(registrator, notifier), entileModule.ServiceType));
        }
    }
}