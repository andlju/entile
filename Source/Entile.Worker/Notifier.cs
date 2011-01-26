using System;
using Entile.Common;

namespace Entile.Worker
{
    public class Notifier : INotifier
    {
        private const string NotificationStatusReceived = "Received";
        private const string NotificationStatusQueueFull = "QueueFull";
        private const string NotificationStatusSuppressed = "Suppressed";
        private const string NotificationStatusDropped = "Dropped";

        private const string SubscriptionStatusActive = "Active";
        private const string SubscriptionStatusExpired = "Expired";

        private const string DeviceConnectionStatusConnected = "Connected";
        private const string DeviceConnectionStatusInactive = "Inactive";
        private const string DeviceConnectionStatusTemporarilyDisconnected = "TempDisconnected";

        private readonly INotificationAgent _notificationAgent;
        private readonly IRegistrator _registrator;
        private readonly INotificationQueue _queue;

        public Notifier(IRegistrator registrator, INotificationQueue queue)
        {
            _registrator = registrator;
            _queue = queue;
            _notificationAgent = new NotificationAgent();
        }

        public void DoWork()
        {
            INotificationItem item;
            while ((item = _queue.TakeItem()) != null)
            {
                HandleNotification(item);
            }
        }

        private void HandleNotification(INotificationItem notification)
        {
            string clientUniqueId = notification.ClientUniqueId;
            string subscriptionUri = GetSubscriptionUri(clientUniqueId);
            
            var response = _notificationAgent.SendNotification(subscriptionUri, notification);
            
            HandleResponse(response);
        }

        private string GetSubscriptionUri(string uniqueId)
        {
            var registration = _registrator.GetRegistration(uniqueId);
            if (registration == null)
            {
                throw new ArgumentException("Unique ID not found among subscriptions.", "uniqueId");
            }

            // The URI that the Push Notification Service returns to the Push Client when creating a notification channel.
            return registration.NotificationChannel;
        }

        private void HandleResponse(NotificationResponse response)
        {
            var notificationItem = response.NotificationItem;

            // Handle responses according to:
            // http://msdn.microsoft.com/en-us/library/ff941100(v=VS.92).aspx

            // Notification was sent to the queue and will be received. All is good.
            if (response.NotificationStatus == NotificationStatusReceived)
            {
                /*if (response.DeviceConnectionStatus == DeviceConnectionStatusTemporarilyDisconnected)
                {
                    // We may want to do something clever here though, like not send anything else
                    // to this device for an hour or so.
                }*/
                return;
            }

            // If the notification was suppressed, don't do anything. 
            // This could happen because the tile isn't pinned to the start page
            // or that the client isn't properly configured. 
            // We could potentially implement the "Ask user to pin"-recipe
            if (response.NotificationStatus == NotificationStatusSuppressed)
            {
                return;
            }

            // If the subscription has expired, remove it
            if (response.SubscriptionStatus == SubscriptionStatusExpired)
            {
                _registrator.Unregister(notificationItem.ClientUniqueId);
                return;
            }

            // If the queue is full, retry in a few minutes
            if (response.NotificationStatus == NotificationStatusQueueFull &&
                (
                response.DeviceConnectionStatus == DeviceConnectionStatusConnected ||
                response.DeviceConnectionStatus == DeviceConnectionStatusTemporarilyDisconnected
                )
               )
            {
                notificationItem.FailedAttempts++;
                if (notificationItem.FailedAttempts <= notificationItem.MaxNumberOfAttempts)
                {
                    // Wait exponentially longer for each failed attempt
                    // Start with 2 minutes, then 4, then 8 etc.
                    var minutesToWait = (2 ^ notificationItem.FailedAttempts);

                    // Re-send this notification in a few minutes
                    _queue.EnqueueItem(notificationItem, DateTime.Now.AddMinutes(minutesToWait));
                }
                return;
            }

            // If server returned a 503, retry in a few minutes
            if (response.StatusCode == 503)
            {
                notificationItem.FailedAttempts++;
                if (notificationItem.FailedAttempts <= notificationItem.MaxNumberOfAttempts)
                {
                    // Wait exponentially longer for each failed attempt
                    // Start with 2 minutes, then 4, then 8 etc.
                    var minutesToWait = (2 ^ notificationItem.FailedAttempts);

                    // Re-send this notification in a few minutes
                    _queue.EnqueueItem(notificationItem, DateTime.Now.AddMinutes(minutesToWait));
                }
                return;
            }

            // Device is in an inactive state. We may retry once every hour
            if (response.StatusCode == 412 &&
                response.NotificationStatus == NotificationStatusDropped &&
                response.DeviceConnectionStatus == DeviceConnectionStatusInactive)
            {
                notificationItem.FailedAttempts++;
                if (notificationItem.FailedAttempts <= notificationItem.MaxNumberOfAttempts)
                {
                    // Re-send this notification in an hour
                    _queue.EnqueueItem(notificationItem, DateTime.Now.AddHours(1));
                }
                return;
            }

            // Throttling limit reached, let's try again in an hour
            if (response.NotificationStatus == NotificationStatusDropped && response.SubscriptionStatus == SubscriptionStatusActive)
            {
                notificationItem.FailedAttempts++;
                if (notificationItem.FailedAttempts <= notificationItem.MaxNumberOfAttempts)
                {
                    // Re-send this notification in an hour
                    _queue.EnqueueItem(notificationItem, DateTime.Now.AddHours(1));
                }
                return;
            }
        }
    }
}