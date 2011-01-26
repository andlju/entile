using System;
using Entile.Common;
using Entile.Service;

namespace Entile.TestHost.WeekNumber
{

    public class WeekNumberModule : IEntileModule
    {
        private INotificationQueue _notificationQueue;
        private IRegistrator _registrator;

        private string _baseUri;

        public WeekNumberModule(string httpHost)
        {
            _baseUri = httpHost + ModuleName;
        }

        public void Initialize(INotificationQueue notificationQueue, IRegistrator registrator)
        {
            _notificationQueue = notificationQueue;
            _registrator = registrator;

            // As soon as the client is registered, let's send it the Live Tile!
            _registrator.ClientRegistered += (sender, args) =>
            {
                SendWeekNumberTile(args.UniqueId);
            };
            
        }

        public void SendWeekNumberTile(string uniqueId)
        {
            TileNotification tile = new TileNotification(uniqueId)
                                        {
                                            BackgroundUri = string.Format(RemoteTileUriFormat, uniqueId),
                                            Title = "Week number",
                                            Counter = 0
                                        };

            _notificationQueue.EnqueueItem(tile);
        }

        public string RemoteTileUriFormat
        {
            get { return _baseUri + "/GetWeekNumberImage?uniqueId={0}"; }
        }

        public string ModuleName
        {
            get { return "WeekNumber"; }
        }

        public Type ServiceType
        {
            get { return typeof(WeekNumberService); }
        }
    }
}