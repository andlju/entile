namespace Entile.Service
{
    public class NotificationResponse
    {
        public int StatusCode { get; set; }
        public string NotificationStatus { get; set; }
        public string SubscriptionStatus { get; set; }
        public string DeviceConnectionStatus { get; set; }
    }
}