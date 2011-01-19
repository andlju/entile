using System;

namespace Entile.Service
{
    public class Notifier : INotifier
    {
        private const string NotificationStatusReceived = "Received";
        private const string NotificationStatusQueueFull = "QueueFull";
        private const string NotificationStatusSuppressed = "Suppressed";
        private const string NotificationStatusDropped = "Dropped";

        private const string SubscriptionStatusActive = "Active";
        private const string SubscriptionStatusExpired = "Expired";

        private readonly INotificationAgent _notificationAgent;
        private readonly IRegistrationStore _registrationStore;

        public Notifier(IRegistrationStore registrationStore)
        {
            _registrationStore = registrationStore;
            _notificationAgent = new PushRecipeNotificationAgent();
        }

        public NotificationResponse SendNotification(string uniqueId, TileNotification notification)
        {
            string subscriptionUri = GetSubscriptionUri(uniqueId);
            var response = _notificationAgent.SendNotification(subscriptionUri, notification);
            HandleResponse(uniqueId, response);
            return response;
        }

        public NotificationResponse SendNotification(string uniqueId, ToastNotification notification)
        {
            string subscriptionUri = GetSubscriptionUri(uniqueId);
            var response = _notificationAgent.SendNotification(subscriptionUri, notification);
            HandleResponse(uniqueId, response);
            return response;
        }

        public NotificationResponse SendNotification(string uniqueId, object notification)
        {
            string subscriptionUri = GetSubscriptionUri(uniqueId);
            var response = _notificationAgent.SendNotification(subscriptionUri, notification);
            HandleResponse(uniqueId, response);
            return response;
        }

        private string GetSubscriptionUri(string uniqueId)
        {
            var registration = _registrationStore.GetRegistration(uniqueId);
            if (registration == null)
            {
                throw new ArgumentException("Unique ID not found among subscriptions.", "uniqueId");
            }

            // The URI that the Push Notification Service returns to the Push Client when creating a notification channel.
            return registration.NotificationChannel;
        }

        private void HandleResponse(string uniqueId, NotificationResponse response)
        {
            // If the subscription has expired, remove it
            if (response.SubscriptionStatus == SubscriptionStatusExpired)
            {
                _registrationStore.RemoveRegistration(uniqueId);
            }
            // TODO Enqueue message if queue full, service unavailable or similar error
        }
    }
}