using System;

namespace Entile.Common
{
    public interface INotificationQueue
    {
        void EnqueueItem(INotificationItem notificationItem);
        void EnqueueItem(INotificationItem notificationItem, DateTime earliestSend);

        INotificationItem TakeItem();
    }
}