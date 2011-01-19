namespace Entile.Test.Tests
{
    class TestHttpNotificationChannelFactory : IHttpNotificationChannelFactory
    {
        public HttpNotificationChannelMock Mock = new HttpNotificationChannelMock();

        public IHttpNotificationChannel GetNotificationChannel(string channelName)
        {
            return Mock;
        }
    }
}