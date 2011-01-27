using System;
using System.IO;
using System.ServiceModel.Activation;
using System.Web.Hosting;
using Entile.Common;

namespace Entile.ReferenceHost.EntileModules
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SampleService : ISampleService
    {
        private readonly IRegistrator _registrator;
        private readonly INotificationQueue _notificationQueue;

        public SampleService(IRegistrator registrator, INotificationQueue notificationQueue)
        {
            _registrator = registrator;
            _notificationQueue = notificationQueue;
        }

        public Stream GetTileImage(string clientId)
        {
            var path = HostingEnvironment.MapPath("~/EntileImages/EntileLogo_Tile.png");
            return File.OpenRead(path);
        }

        public void SendSampleToastToAllClients()
        {
            foreach (var subscriber in _registrator.ListAllSubscribers())
            {
                var toast = new ToastNotification(subscriber.UniqueId)
                                {
                                    Title = "Hello World!",
                                    Body = "This is a sample Toast..."
                                };

                _notificationQueue.EnqueueItem(toast);
            }
        }

        public void SendSampleTileToAllClients()
        {
            foreach (var subscriber in _registrator.ListAllSubscribers())
            {
                // TODO Change the sample port
                var tile = new TileNotification(subscriber.UniqueId)
                                {
                                    Title = "Sample title",
                                    BackgroundUri = string.Format("http://localhost:1234/Sample/GetTileImage?clientId={0}", subscriber.UniqueId),
                                    Counter = 17
                                };

                _notificationQueue.EnqueueItem(tile);
            }
        }
    }
}