using Entile.Common;

namespace Entile.Worker
{
    interface INotificationAgent
    {
        NotificationResponse SendNotification(string channelUri, INotificationItem notification);
    }
}