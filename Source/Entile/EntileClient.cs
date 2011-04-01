using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Shell;

namespace Entile
{
    /// <summary>
    /// Main entry-point for most cases. Simply new this up and bind to the Enable-property.
    /// </summary>
    public class EntileClient : INotifyPropertyChanged, IEntileClient
    {
        private readonly IChannelManager _channelManager;
        private readonly IRegistrationManager _registrationManager;
        private readonly IRemoteTileManager _remoteTileManager;

        private readonly ISettingsProvider _settingsProvider;
        private IDictionary<string, string> _extraInfo;
        
        private bool _enable;
        private bool _busy;

        public EntileClient() 
            : this(
                new DefaultEntileConfig(),
                new DefaultSettingsProvider())
        {

        }

        public EntileClient(IEntileConfig entileConfig, ISettingsProvider settingsProvider)
            : this(
                new ChannelManager(
                    entileConfig.ChannelName,
                    entileConfig.AllowedTileUris,
                    entileConfig.RequestToasts,
                    entileConfig.RequestLiveTiles),

                new RegistrationManager(
                    entileConfig.RegistrationServiceUri,
                    settingsProvider.GetUniqueId()),

                new RemoteTileManager(),

                settingsProvider
            )
        {

        }

        public EntileClient(
            IChannelManager channelManager, 
            IRegistrationManager registrationManager, 
            IRemoteTileManager remoteTileManager, 
            ISettingsProvider settingsProvider
            )
        {
            _settingsProvider = settingsProvider;

            _channelManager = channelManager;
            _registrationManager = registrationManager;
            _remoteTileManager = remoteTileManager;

            ChannelManager.OpenChannelCompleted += OnOpenChannelCompleted;
            ChannelManager.HttpNotificationReceived += OnHttpNotificationReceived;
            ChannelManager.ShellToastNotificationReceived += OnShellToastNotificationReceived;
            RegistrationManager.RegisterCompleted += OnRegisterWithProviderCompleted;
            RegistrationManager.UpdateExtraInfoCompleted += OnUpdateExtraInfoCompleted;

            _extraInfo = _settingsProvider.GetExtraInfo();
            _enable = _settingsProvider.GetEnabled();
        }

        void OnShellToastNotificationReceived(object sender, Microsoft.Phone.Notification.NotificationEventArgs e)
        {
            InvokeToastMessageReceived(e.Collection["wp:Text1"], e.Collection["wp:Text2"]);
        }

        void OnHttpNotificationReceived(object sender, Microsoft.Phone.Notification.HttpNotificationEventArgs e)
        {
            var len = e.Notification.Body.Length;
            var body = new byte[len];
            e.Notification.Body.Read(body, 0, (int)len);

            InvokeRawMessageReceived(body);
        }

        void OnUpdateExtraInfoCompleted(object sender, UpdateExtraInfoCompletedEventArgs e)
        {
            Busy = false;
            if (e.Succeeded)
            {
                if (!string.IsNullOrEmpty(e.RemoteTileUri))
                {
                    RemoteTileManager.Start(e.RemoteTileUri, (UpdateInterval)Enum.Parse(typeof(UpdateInterval), e.RemoteTileInterval, true));
                }
            }
            else
            {
                InvokeErrorOccured("Error while updating extra info");
                _enable = false;
                InvokePropertyChangedEvent("Enable");
            }
        }

        void OnRegisterWithProviderCompleted(object sender, RegisterCompletedEventArgs e)
        {
            Busy = false;
            if (e.Succeeded)
            {
                if (!string.IsNullOrEmpty(e.RemoteTileUri))
                {
                    RemoteTileManager.Start(e.RemoteTileUri, (UpdateInterval)Enum.Parse(typeof(UpdateInterval), e.RemoteTileInterval, true));
                }
            }
            else
            {
                InvokeErrorOccured("Error while registering with provider");
                _enable = false;
                InvokePropertyChangedEvent("Enable");
            }
        }

        protected virtual void OpenAndRegister()
        {
            Busy = true;
            ChannelManager.OpenChannelAsync();
        }

        void OnOpenChannelCompleted(object sender, OpenChannelCompletedEventArgs e)
        {
            if (e.NotificationUri != null)
            {
                RegistrationManager.RegisterAsync(e.NotificationUri, _extraInfo);
            }
            else
            {
                Busy = false;
                InvokeErrorOccured("Error while opening channel");

                _enable = false;
                InvokePropertyChangedEvent("Enable");
            }
        }

        protected virtual void CloseAndUnregister()
        {
            // TODO Maybe we shouldn't just fire and forget this?
            RegistrationManager.UnregisterAsync();
            ChannelManager.CloseChannel();
            RemoteTileManager.Stop();
            Busy = false;
        }

        public void Refresh()
        {
            if (Enable)
            {
                OpenAndRegister();
            }
        }

        public void UpdateExtraInfo(IDictionary<string, string> extraInfo)
        {
            // TODO What happens if we are not yet properly registered? 
            _extraInfo = extraInfo;
            _settingsProvider.SetExtraInfo(extraInfo);

            if (Enable)
            {
                Busy = true;
                RegistrationManager.UpdateExtraInfoAsync(_extraInfo);
            }
        }

        public bool Enable
        {
            get { return _enable; }
            set
            {
                if (Busy)
                {
                    // HACK Not the nicest way of saying "don't even try to update status if I'm already working..
                    InvokePropertyChangedEvent("Enable");
                    return;
                }
                if (_enable != value)
                {
                    _enable = value;
                    InvokePropertyChangedEvent("Enable");

                    if (_enable)
                        OpenAndRegister();
                    else
                        CloseAndUnregister();

                    // Store the new setting
                    _settingsProvider.SetEnabled(_enable);
                }
            }
        }

        public IChannelManager ChannelManager
        {
            get { return _channelManager; }
        }

        public IRegistrationManager RegistrationManager
        {
            get { return _registrationManager; }
        }

        public IRemoteTileManager RemoteTileManager
        {
            get { return _remoteTileManager; }
        }

        public bool Busy
        {
            get { return _busy; }
            protected set
            {
                if (_busy != value)
                {
                    _busy = value;
                    InvokePropertyChangedEvent("Busy");
                }
            }
        }

        public event EventHandler<EntileErrorEventArgs> ErrorOccured;

        private void InvokeErrorOccured(string errorMessage)
        {
            EventHandler<EntileErrorEventArgs> handler = ErrorOccured;
            if (handler != null) handler(this, new EntileErrorEventArgs(errorMessage));
        }

        public event EventHandler<ToastMessageEventArgs> ToastMessageReceived;

        private void InvokeToastMessageReceived(string title, string body)
        {
            EventHandler<ToastMessageEventArgs> handler = ToastMessageReceived;
            if (handler != null) handler(this, new ToastMessageEventArgs(title, body));
        }

        public event EventHandler<RawMessageEventArgs> RawMessageReceived;

        private void InvokeRawMessageReceived(byte[] body)
        {
            EventHandler<RawMessageEventArgs> handler = RawMessageReceived;
            if (handler != null) handler(this, new RawMessageEventArgs(body));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void InvokePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                var dispatcher = Deployment.Current.Dispatcher;

                if (dispatcher.CheckAccess())
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                else
                {
                    dispatcher.BeginInvoke(PropertyChanged,
                                           this,
                                           new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}