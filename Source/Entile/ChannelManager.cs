using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Phone.Notification;

namespace Entile
{

    public class ChannelManager : IChannelManager
    {
        private readonly IHttpNotificationChannelFactory _channelFactory;

        private readonly Collection<Uri> _allowedRemoteUris;
        private readonly string _channelName;
        private readonly bool _requestToasts;
        private readonly bool _requestLiveTiles;

        private IHttpNotificationChannel _channel;

        public ChannelManager(string channelName, IEnumerable<Uri> allowedRemoteUris, bool requestToasts, bool requestLiveTiles) 
            : this(channelName, allowedRemoteUris, requestToasts, requestLiveTiles, new HttpNotificationChannelFactory())
        {
            
        }

        public ChannelManager(string channelName, IEnumerable<Uri> allowedRemoteUris, bool requestToasts, bool requestLiveTiles, IHttpNotificationChannelFactory channelFactory)
        {
            _channelFactory = channelFactory;
            _channelName = channelName;
            _requestToasts = requestToasts;
            _requestLiveTiles = requestLiveTiles;

            if (allowedRemoteUris == null)
                _allowedRemoteUris = null;
            else
                _allowedRemoteUris = new Collection<Uri>(allowedRemoteUris.ToList());
        }

        private void EnsureChannelExists()
        {
            if (_channel != null)
                return;

            // First, try to get the notification channel for this app
            _channel = _channelFactory.GetNotificationChannel(_channelName);

            // Ensure that we're listening to the correct event handlers
            SetupChannelEventHandlers();
        }

        public void OpenChannelAsync()
        {
            EnsureChannelExists();
            if (_channel.ChannelUri == null)
            {
                // No Uri set yet, that should be because we have a new, unopened channel
                _channel.Open();
            }
            else
            {
                // Tell everyone that we have a proper channel registered
                InvokeOpenChannelCompleted(_channel.ChannelUri);
            }
        }

        public void CloseChannel()
        {
            EnsureChannelExists();
            
            _channel.Close();
            _channel = null;
        }

        void SetupChannelEventHandlers()
        {
            _channel.ChannelUriUpdated += OnChannelUriUpdated;
            _channel.ErrorOccurred += OnErrorOccurred;
            _channel.HttpNotificationReceived += OnHttpNotificationReceived;
            _channel.ShellToastNotificationReceived += OnShellToastNotificationReceived;
        }

        public event EventHandler<HttpNotificationEventArgs> HttpNotificationReceived;
        public event EventHandler<NotificationEventArgs> ShellToastNotificationReceived;

        protected virtual void OnShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            if (ShellToastNotificationReceived != null)
                ShellToastNotificationReceived(sender, e);
        }

        protected virtual void OnHttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            if (HttpNotificationReceived != null)
                HttpNotificationReceived(sender, e);
        }

        void OnChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            if (!_channel.IsShellTileBound && _requestLiveTiles)
            {
                if (_allowedRemoteUris != null)
                    _channel.BindToShellTile(_allowedRemoteUris);
                else
                    _channel.BindToShellTile();
            }

            if (!_channel.IsShellToastBound && _requestToasts)
            {
                _channel.BindToShellToast();
            }

            // Tell everyone that we have a proper channel registered
            InvokeOpenChannelCompleted(_channel.ChannelUri);
        }

        void OnErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            switch (e.ErrorType)
            {
                case ChannelErrorType.ChannelOpenFailed:
                    InvokeOpenChannelCompleted(e.ErrorType);
                    break;
                case ChannelErrorType.PayloadFormatError:
                    // Channel will need to be rebound
                    break;
            }
        }

        public event EventHandler<OpenChannelCompletedEventArgs> OpenChannelCompleted;

        protected virtual void InvokeOpenChannelCompleted(Uri channelUri)
        {
            if (OpenChannelCompleted != null)
            {
                OpenChannelCompleted(this, new OpenChannelCompletedEventArgs(channelUri));
            }
        }

        protected virtual void InvokeOpenChannelCompleted(ChannelErrorType errorType)
        {
            if (OpenChannelCompleted != null)
            {
                OpenChannelCompleted(this, new OpenChannelCompletedEventArgs(errorType));
            }
        }
    }
}
