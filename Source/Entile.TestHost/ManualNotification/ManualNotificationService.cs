using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using Entile.Service;

namespace Entile.TestHost.ManualNotification
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ManualNotificationService : IManualNotificationService
    {
        private IRegistrator _registrator;
        private INotifier _notifier;

        public ManualNotificationService(IRegistrator registrator, INotifier notifier)
        {
            _registrator = registrator;
            _notifier = notifier;
        }

        public void SendNotification(string clientUniqueId, string title, string body)
        {
            _notifier.SendNotification(clientUniqueId, new ToastNotification() {Text1 = title, Text2 = body});
        }

        public IEnumerable<string> GetRegisteredClients()
        {
            return _registrator.ListAllSubscribers().Select(sub=>sub.UniqueId);
        }
    }
}