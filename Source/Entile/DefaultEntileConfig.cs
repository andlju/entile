using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Entile
{
    /// <summary>
    /// The DefaultEntileConfig uses StaticResources to specify Uri:s and other configuration
    /// </summary>
    public class DefaultEntileConfig : IEntileConfig
    {
        private readonly string _channelName;
        private readonly Uri _registrationServiceUri;
        private readonly IEnumerable<Uri> _allowedTileUris;
        private readonly bool _requestLiveTiles;
        private readonly bool _requestToasts;

        public DefaultEntileConfig()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                _channelName = "TestChannel";
                _registrationServiceUri=new Uri("http://test.com/TestModule/Registration");
                _allowedTileUris = new Uri[0];
                _requestLiveTiles = true;
                _requestToasts = true;
                return;
            }

            // TODO Better error handling
            _channelName = (string) Application.Current.Resources["ChannelName"];
            if (_channelName == null)
                throw new InvalidOperationException("You must provide a ChannelName as a Static Resource in your app.");

            var regServiceUri = (string)Application.Current.Resources["RegistrationServiceUri"];
            if (regServiceUri == null)
                throw new InvalidOperationException("You must provide a RegistrationServiceUri as a Static Resource in your app.");
            _registrationServiceUri = new Uri(regServiceUri);

            var allowedTileUris = (string)Application.Current.Resources["AllowedTileUris"];
            if (allowedTileUris != null)
                _allowedTileUris = allowedTileUris.Split(';').Select(u => new Uri(u));
            else
                _allowedTileUris = new Uri[0];

            var requestToastsValue = (string)Application.Current.Resources["RequestToasts"];
            var requestLiveTilesValue = (string)Application.Current.Resources["RequestLiveTiles"];
            if (!bool.TryParse(requestToastsValue, out _requestToasts))
                _requestToasts = true; // Default to true
            if (!bool.TryParse(requestLiveTilesValue, out _requestLiveTiles))
                _requestLiveTiles = true; // Default to true
        }

        public string ChannelName { get { return _channelName; } }
        public Uri RegistrationServiceUri { get { return _registrationServiceUri; } }
        public IEnumerable<Uri> AllowedTileUris { get { return _allowedTileUris; } }

        public bool RequestToasts { get { return _requestToasts; } }
        public bool RequestLiveTiles { get { return _requestLiveTiles; } }
    }

}