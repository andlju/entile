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
            
            _registrator.ClientExtraInfoUpdated += (a, e) => SendTile(e.UniqueId);
        }

        private void SendTile(string uniqueId)
        {
            var extraInfo = _registrator.GetExtraInfo(uniqueId);
            string tileTitle;
            if (extraInfo.TryGetValue("TileTitle", out tileTitle))
            {
                var tile = new TileNotification(uniqueId)
                                {
                                    Title = tileTitle
                                };
                
                _notificationQueue.EnqueueItem(tile);
            }
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