using System;

using Entile.Service;

namespace Entile.TestHost.ManualNotification
{
    public class ManualNotificationModule : IEntileModule
    {
        private INotifier _notifier;
        private IRegistrator _registrator;

        public void Initialize(INotifier notifier, IRegistrator registrator)
        {
            _registrator = registrator;
            _notifier = notifier;
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