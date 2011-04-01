namespace Entile.Common
{
    public class RawNotification : NotificationItemBase
    {
        public RawNotification(string clientUniqueId) : base(clientUniqueId)
        {
        }

        public RawNotification(string clientUniqueId, int maxNumberOfAttempts) : base(clientUniqueId, maxNumberOfAttempts)
        {
        }

        public byte[] Body { get; set; }
    }
}