using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using System.Threading;
using System.Web.Routing;
using Entile.Service;
using Entile.Service.Store;
using Entile.Worker;

namespace Entile.Server
{
    public static class EntileHost
    {
        private static List<IEntileModule> _entileModules = new List<IEntileModule>();
        private static Timer _notificationWorkerTimer;

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

            var registrator = new Registrator(store, entileModule.RemoteTileUriFormat);
            var notificationQueue = new InMemoryNotificationQueue();

            var notifier = new Notifier(registrator, notificationQueue);

            entileModule.Initialize(notificationQueue, registrator);

            RouteTable.Routes.Add(new ServiceRoute(moduleName + "/Registration", new EntileRegistrationServiceFactory(registrator, notificationQueue), typeof(RegistrationService)));
            RouteTable.Routes.Add(new ServiceRoute(moduleName, new EntileRegistrationServiceFactory(registrator, notificationQueue), entileModule.ServiceType));

            _notificationWorkerTimer = new Timer(a =>
                                                     {
                                                         try
                                                         {
                                                             notifier.DoWork();
                                                         } 
                                                         catch(Exception ex)
                                                         {
                                                              /* Intentionally left blank. We don't want the IIS process to fail */
                                                             // TODO Implement logging
                                                         }

                                                         _notificationWorkerTimer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(-1));
                                                     }, 
                                                     null,
                                                     TimeSpan.FromSeconds(1),
                                                     TimeSpan.FromMilliseconds(-1));
            
        }
    }
}