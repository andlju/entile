namespace Entile.Service
{
    public interface INotifier
    {
        NotificationResponse SendNotification(string uniqueId, TileNotification notification);
        NotificationResponse SendNotification(string uniqueId, ToastNotification notification);
        NotificationResponse SendNotification(string uniqueId, object notification);
    }
}