using System;
using Microsoft.Phone.Notification;

namespace Entile
{
    public class OpenChannelCompletedEventArgs : EventArgs
    {
        public OpenChannelCompletedEventArgs(Uri notificationUri)
        {
            NotificationUri = notificationUri;
        }

        public OpenChannelCompletedEventArgs(ChannelErrorType errorType)
        {
            ErrorType = errorType;
        }

        public Uri NotificationUri { get; private set; }
        public ChannelErrorType ErrorType { get; private set; }
    }

    public interface IChannelManager
    {
        void OpenChannelAsync();
        void CloseChannel();

        event EventHandler<OpenChannelCompletedEventArgs> OpenChannelCompleted;

        event EventHandler<HttpNotificationEventArgs> HttpNotificationReceived;
        event EventHandler<NotificationEventArgs> ShellToastNotificationReceived;
    }
}