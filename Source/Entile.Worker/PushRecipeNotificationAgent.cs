using System;
using Entile.Common;
using WindowsPhone.Recipes.Push.Messasges;

namespace Entile.Worker
{
    public class PushRecipeNotificationAgent : INotificationAgent
    {
        public NotificationResponse SendNotification(string channelUri, INotificationItem notification)
        {
            var tileNotification = notification as TileNotification;
            if (tileNotification != null)
                return SendTileNotification(channelUri, tileNotification);

            var toastNotification = notification as ToastNotification;
            if (toastNotification != null)
                return SendToastNotification(channelUri, toastNotification);
            
            return null;
        }

        private NotificationResponse SendTileNotification(string channelUri, TileNotification notificationItem)
        {
            var tile = new TilePushNotificationMessage(MessageSendPriority.High);
            tile.BackgroundImageUri = new Uri(notificationItem.BackgroundUri);
            tile.Count = notificationItem.Counter;
            tile.Title = notificationItem.Title;
            var messageResponse = tile.Send(new Uri(channelUri));
            
            var notificationResponse = GetNotificationResponse(messageResponse, notificationItem);

            return notificationResponse;
        }

        private NotificationResponse SendToastNotification(string channelUri, ToastNotification notificationItem)
        {
            var toast = new ToastPushNotificationMessage(MessageSendPriority.High);
            toast.Title = notificationItem.Title;
            toast.SubTitle = notificationItem.Body;
            var messageResponse = toast.Send(new Uri(channelUri));

            var notificationResponse = GetNotificationResponse(messageResponse, notificationItem);

            return notificationResponse;
        }

        private NotificationResponse SendRawNotification(string channelUri, object notification)
        {
            throw new NotImplementedException();

            var raw = new RawPushNotificationMessage(MessageSendPriority.High);
            // TODO Set raw data

            var messageResponse = raw.Send(new Uri(channelUri));

            var notificationResponse = GetNotificationResponse(messageResponse, null);

            return notificationResponse;
        }

        private NotificationResponse GetNotificationResponse(MessageSendResult messageResponse, INotificationItem notificationItem)
        {
            var notificationResponse = new NotificationResponse();
            notificationResponse.DeviceConnectionStatus = messageResponse.DeviceConnectionStatus.ToString();
            notificationResponse.NotificationStatus = messageResponse.NotificationStatus.ToString();
            notificationResponse.SubscriptionStatus = messageResponse.SubscriptionStatus.ToString();
            notificationResponse.NotificationItem = notificationItem;
            return notificationResponse;
        }
    }
}