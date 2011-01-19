namespace Entile.Service
{
    interface INotificationAgent
    {
        NotificationResponse SendNotification(string channelUri, TileNotification notification);
        NotificationResponse SendNotification(string channelUri, ToastNotification notification);
        NotificationResponse SendNotification(string channelUri, object notification);
    }
}