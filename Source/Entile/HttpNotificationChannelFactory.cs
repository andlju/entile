using Microsoft.Phone.Notification;

namespace Entile
{
    public class HttpNotificationChannelFactory : IHttpNotificationChannelFactory
    {
        public IHttpNotificationChannel GetNotificationChannel(string channelName)
        {
            var channel = HttpNotificationChannel.Find(channelName);
            if (channel == null)
            {
                channel = new HttpNotificationChannel(channelName);
            }
            return new HttpNotificationChannelAdapter(channel);
        }
    }
}