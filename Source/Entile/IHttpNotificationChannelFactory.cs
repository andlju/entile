namespace Entile
{
    public interface IHttpNotificationChannelFactory
    {
        IHttpNotificationChannel GetNotificationChannel(string channelName);
    }
}