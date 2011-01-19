using System;
using System.Collections.ObjectModel;
using Microsoft.Phone.Notification;

namespace Entile
{
    public interface IHttpNotificationChannel
    {
        Uri ChannelUri { get; }
        string ChannelName { get; }
        bool IsShellTileBound { get; }
        bool IsShellToastBound { get; }

        void Open();
        void Close();

        void BindToShellTile();
        void BindToShellTile(Collection<Uri> baseUri);

        void BindToShellToast();

        void UnbindToShellTile();
        void UnbindToShellToast();

        event EventHandler<NotificationChannelUriEventArgs> ChannelUriUpdated;
        event EventHandler<NotificationChannelErrorEventArgs> ErrorOccurred;
        event EventHandler<HttpNotificationEventArgs> HttpNotificationReceived;
        event EventHandler<NotificationEventArgs> ShellToastNotificationReceived;
    }
}