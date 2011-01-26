using System;
using Entile.Common;
using Entile.Service;

namespace Entile.TestHost.ManualNotification
{
    public class ManualNotificationModule : IEntileModule
    {
        private INotificationQueue _notificationQueue;
        private IRegistrator _registrator;

        public void Initialize(INotificationQueue notificationQueue, IRegistrator registrator)
        {
            _registrator = registrator;
            _notificationQueue = notificationQueue;
        }

        public string RemoteTileUriFormat
        {
            get { return null; } // No remote tiles supported for this module
        }

        public string ModuleName
        {
            get { return "ManualNotification"; }
        }

        public Type ServiceType
        {
            get { return typeof(ManualNotificationService); }
        }
    }
}