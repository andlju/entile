using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using Entile.Common;
using Entile.Service;

namespace Entile.TestHost.ManualNotification
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ManualNotificationService : IManualNotificationService
    {
        private IRegistrator _registrator;
        private INotificationQueue _notificationQueue;

        public ManualNotificationService(IRegistrator registrator, INotificationQueue notificationQueue)
        {
            _registrator = registrator;
            _notificationQueue = notificationQueue;
        }

        public void SendNotification(string clientUniqueId, string title, string body)
        {
            _notificationQueue.EnqueueItem(new ToastNotification(clientUniqueId) {Title = title, Body = body});
        }

        public IEnumerable<string> GetRegisteredClients()
        {
            return _registrator.ListAllSubscribers().Select(sub=>sub.UniqueId);
        }
    }
}