using System;
using WindowsPhone.Recipes.Push.Messasges;

namespace Entile.Service
{
    public class PushRecipeNotificationAgent : INotificationAgent
    {
        public NotificationResponse SendNotification(string channelUri, TileNotification notification)
        {
            var tile = new TilePushNotificationMessage(MessageSendPriority.High);
            tile.BackgroundImageUri = new Uri(notification.BackgroundUri);
            tile.Count = notification.Counter;
            tile.Title = notification.Title;
            var messageResponse = tile.Send(new Uri(channelUri));
            
            var notificationResponse = GetNotificationResponse(messageResponse);

            return notificationResponse;
        }

        public NotificationResponse SendNotification(string channelUri, ToastNotification notification)
        {
            var toast = new ToastPushNotificationMessage(MessageSendPriority.High);
            toast.Title = notification.Text1;
            toast.SubTitle = notification.Text2;
            var messageResponse = toast.Send(new Uri(channelUri));

            var notificationResponse = GetNotificationResponse(messageResponse);

            return notificationResponse;
        }

        public NotificationResponse SendNotification(string channelUri, object notification)
        {
            throw new NotImplementedException();

            var raw = new RawPushNotificationMessage(MessageSendPriority.High);
            // TODO Set raw data

            var messageResponse = raw.Send(new Uri(channelUri));

            var notificationResponse = GetNotificationResponse(messageResponse);

            return notificationResponse;
        }

        private NotificationResponse GetNotificationResponse(MessageSendResult messageResponse)
        {
            var notificationResponse = new NotificationResponse();
            notificationResponse.DeviceConnectionStatus = messageResponse.DeviceConnectionStatus.ToString();
            notificationResponse.NotificationStatus = messageResponse.NotificationStatus.ToString();
            notificationResponse.SubscriptionStatus = messageResponse.SubscriptionStatus.ToString();
            return notificationResponse;
        }
    }
}