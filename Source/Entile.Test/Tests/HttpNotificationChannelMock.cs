using System;
using System.Collections.ObjectModel;
using Microsoft.Phone.Notification;

namespace Entile.Test.Tests
{
    class HttpNotificationChannelMock : IHttpNotificationChannel
    {
        public Uri ChannelUri { get; set; }
        public string ChannelName { get; set; }
        public bool IsShellTileBound { get; set; }
        public bool IsShellToastBound { get; set; }

        public bool OpenCalled = false;
        public void Open()
        {
            OpenCalled = true;
        }

        public bool CloseCalled = false;
        public void Close()
        {
            CloseCalled = true;
        }

        public bool BindToShellTileCalled = false;
        public void BindToShellTile()
        {
            BindToShellTileCalled = true;
        }

        public void BindToShellTile(Collection<Uri> baseUri)
        {
            BindToShellTileCalled = true;
        }

        public bool BindToShellToastCalled = false;
        public void BindToShellToast()
        {
            BindToShellToastCalled = true;
        }

        public bool UnbindToShellTileCalled = false;
        public void UnbindToShellTile()
        {
            UnbindToShellTileCalled = true;
        }

        public bool UnbindToShellToastCalled = false;
        public void UnbindToShellToast()
        {
            UnbindToShellToastCalled = true;
        }

        public event EventHandler<NotificationChannelUriEventArgs> ChannelUriUpdated;

        public void InvokeChannelUriUpdated(NotificationChannelUriEventArgs e)
        {
            EventHandler<NotificationChannelUriEventArgs> handler = ChannelUriUpdated;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<NotificationChannelErrorEventArgs> ErrorOccurred;

        public void InvokeErrorOccurred(NotificationChannelErrorEventArgs e)
        {
            EventHandler<NotificationChannelErrorEventArgs> handler = ErrorOccurred;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HttpNotificationEventArgs> HttpNotificationReceived;

        public void InvokeHttpNotificationReceived(HttpNotificationEventArgs e)
        {
            EventHandler<HttpNotificationEventArgs> handler = HttpNotificationReceived;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<NotificationEventArgs> ShellToastNotificationReceived;

        public void InvokeShellToastNotificationReceived(NotificationEventArgs e)
        {
            EventHandler<NotificationEventArgs> handler = ShellToastNotificationReceived;
            if (handler != null) handler(this, e);
        }
    }
}