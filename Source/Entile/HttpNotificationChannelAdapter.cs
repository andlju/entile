using System;
using System.Collections.ObjectModel;
using Microsoft.Phone.Notification;

namespace Entile
{
    public class HttpNotificationChannelAdapter : IHttpNotificationChannel
    {
        private HttpNotificationChannel _wrappedNotificationChannel;

        public HttpNotificationChannelAdapter(HttpNotificationChannel wrappedNotificationChannel)
        {
            _wrappedNotificationChannel = wrappedNotificationChannel;
        }

        public Uri ChannelUri
        {
            get { return _wrappedNotificationChannel.ChannelUri; }
        }

        public string ChannelName
        {
            get { return _wrappedNotificationChannel.ChannelName; }
        }

        public bool IsShellTileBound
        {
            get { return _wrappedNotificationChannel.IsShellTileBound; }
        }

        public bool IsShellToastBound
        {
            get { return _wrappedNotificationChannel.IsShellToastBound; }
        }

        public void Open()
        {
            _wrappedNotificationChannel.Open();
        }

        public void Close()
        {
            _wrappedNotificationChannel.Close();
        }

        public void BindToShellTile()
        {
            _wrappedNotificationChannel.BindToShellTile();
        }

        public void BindToShellTile(Collection<Uri> baseUri)
        {
            _wrappedNotificationChannel.BindToShellTile(baseUri);
        }

        public void BindToShellToast()
        {
            _wrappedNotificationChannel.BindToShellToast();
        }

        public void UnbindToShellTile()
        {
            _wrappedNotificationChannel.UnbindToShellTile();
        }

        public void UnbindToShellToast()
        {
            _wrappedNotificationChannel.UnbindToShellToast();
        }

        public event EventHandler<NotificationChannelUriEventArgs> ChannelUriUpdated
        {
            add { _wrappedNotificationChannel.ChannelUriUpdated += value; }
            remove { _wrappedNotificationChannel.ChannelUriUpdated -= value; }
        }

        public event EventHandler<NotificationChannelErrorEventArgs> ErrorOccurred
        {
            add { _wrappedNotificationChannel.ErrorOccurred += value; }
            remove { _wrappedNotificationChannel.ErrorOccurred -= value; }
        }

        public event EventHandler<HttpNotificationEventArgs> HttpNotificationReceived
        {
            add { _wrappedNotificationChannel.HttpNotificationReceived += value; }
            remove { _wrappedNotificationChannel.HttpNotificationReceived -= value; }
        }

        public event EventHandler<NotificationEventArgs> ShellToastNotificationReceived
        {
            add { _wrappedNotificationChannel.ShellToastNotificationReceived += value; }
            remove { _wrappedNotificationChannel.ShellToastNotificationReceived -= value; }
        }
    }
}