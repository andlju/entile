namespace Entile.Common
{
    public class ToastNotification : NotificationItemBase
    {
        public ToastNotification(string clientUniqueId)
            : base(clientUniqueId)
        {
        }

        public ToastNotification(string clientUniqueId, int numberOfAttempts)
            : base(clientUniqueId, numberOfAttempts)
        {
        }

        public string Title { get; set; }
        public string Body { get; set; }
    }
}