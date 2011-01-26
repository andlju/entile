using System;

namespace Entile.Common
{

    public interface INotificationItem
    {
        string ClientUniqueId { get; }
        string NotificationId { get; }

        int MaxNumberOfAttempts { get; }
        int FailedAttempts { get; set; }
    }

    public abstract class NotificationItemBase : INotificationItem
    {
        private const int DefaultMaxNumberOfAttempts = 3;

        protected NotificationItemBase(string clientUniqueId) : this(clientUniqueId, DefaultMaxNumberOfAttempts)
        {
        }

        protected NotificationItemBase(string clientUniqueId, int maxNumberOfAttempts)
        {
            ClientUniqueId = clientUniqueId;
            NotificationId = Guid.NewGuid().ToString();
            MaxNumberOfAttempts = maxNumberOfAttempts;
            FailedAttempts = 0;
        }

        public string ClientUniqueId { get; private set; }
        public string NotificationId { get; private set; }
        public int MaxNumberOfAttempts { get; private set; }
        public int FailedAttempts { get; set; }
    }

    public class TileNotification : NotificationItemBase
    {
        public TileNotification(string clientUniqueId)
            : base(clientUniqueId)
        {
        }

        public TileNotification(string clientUniqueId, int numberOfAttempts)
            : base(clientUniqueId, numberOfAttempts)
        {
        }

        public int Counter { get; set; }
        public string Title { get; set; }
        public string BackgroundUri { get; set; }

    }
}