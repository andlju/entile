using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace Entile
{
    public class RegistrationManager : IRegistrationManager
    {
        private const string RegisterMethodPath = "/Register";
        private const string UnregisterMethodPath = "/Unregister";
        private const string UpdateExtraInfoMethodPath = "/UpdateExtraInfo";

        public const string JsonContentType = "application/json";

        private readonly IWebClientFactory _webClientFactory;

        private readonly string _uniqueClientId;
        private readonly Uri _registrationServiceUri;

        #region Request/response messages

        [DataContract]
        public class RegisterRequest
        {
            [DataMember]
            public string uniqueId { get; set; }

            [DataMember]
            public string notificationChannel { get; set; }

            [DataMember]
            public IDictionary<string, string> extraInfo { get; set; }
        }

        [DataContract]
        public class UnregisterRequest
        {
            [DataMember]
            public string uniqueId { get; set; }
        }

        [DataContract]
        public class UpdateExtraInfoRequest
        {
            [DataMember]
            public string uniqueId { get; set; }

            [DataMember]
            public IDictionary<string, string> extraInfo { get; set; }
        }
        
        [DataContract]
        public class RemoteTileInfo
        {
            [DataMember]
            public string Uri { get; set; }
            
            [DataMember]
            public string Interval { get; set; }
        }

        #endregion
        
        public RegistrationManager(Uri registrationServiceUri, string uniqueClientId)
            : this(registrationServiceUri, uniqueClientId, new WebClientFactory())
        {
        }

        public RegistrationManager(Uri registrationServiceUri, string uniqueClientId, IWebClientFactory webClientFactory)
        {
            if (registrationServiceUri == null)
                throw new ArgumentNullException("registrationServiceUri");
            _registrationServiceUri = registrationServiceUri;

            if (uniqueClientId == null)
                throw new ArgumentNullException("uniqueClientId");
            _uniqueClientId = uniqueClientId;

            if (webClientFactory == null)
                throw new ArgumentNullException("webClientFactory");
            _webClientFactory = webClientFactory;
        }

        public void RegisterAsync(Uri notificationUri, IDictionary<string, string> extraInfo)
        {
            Uri registerUri = new Uri(_registrationServiceUri + RegisterMethodPath);

            var client = _webClientFactory.CreateWebClient();
            client.SendCompleted += OnRegisterCompleted;
            var reg = new RegisterRequest()
                          {
                              uniqueId = UniqueNotificationId,
                              notificationChannel = notificationUri.ToString(),
                              extraInfo = extraInfo
                          }.ToJson();
            client.SendStringAsync(registerUri, reg, JsonContentType);
        }

        void OnRegisterCompleted(object sender, SendCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                InvokeRegisterCompleted(false, null, null);
                return;
            }

            var result = e.Result.FromJson<RemoteTileInfo>();
            InvokeRegisterCompleted(true, result.Uri, result.Interval);
        }

        public void UnregisterAsync()
        {
            Uri unregisterUri = new Uri(_registrationServiceUri + UnregisterMethodPath);
            var client = _webClientFactory.CreateWebClient();
            client.SendCompleted += OnUnregisterCompleted;
            client.SendStringAsync(unregisterUri,
                new UnregisterRequest()
                {
                    uniqueId = UniqueNotificationId
                }.ToJson(), JsonContentType);

        }

        void OnUnregisterCompleted(object sender, SendCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                InvokeUnregisterCompleted(false);
                return;
            }

            InvokeUnregisterCompleted(true);
        }

        public void UpdateExtraInfoAsync(IDictionary<string, string> extraInfo)
        {
            Uri unregisterUri = new Uri(_registrationServiceUri + UpdateExtraInfoMethodPath);
            var client = _webClientFactory.CreateWebClient();
            client.SendCompleted += OnUpdateExtraInfoCompleted;
            client.SendStringAsync(unregisterUri,
                new UpdateExtraInfoRequest()
                {
                    uniqueId = UniqueNotificationId,
                    extraInfo = extraInfo
                }.ToJson(), JsonContentType);

        }

        void OnUpdateExtraInfoCompleted(object sender, SendCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                InvokeUpdateExtraInfoCompleted(false, null, null);
                return;
            }

            var result = e.Result.FromJson<RemoteTileInfo>();
            InvokeUpdateExtraInfoCompleted(true, result.Uri, result.Interval);
        }

        public event EventHandler<RegisterCompletedEventArgs> RegisterCompleted;
        protected virtual void InvokeRegisterCompleted(bool succeeded, string remoteTileUri, string remoteTileInterval)
        {
            EventHandler<RegisterCompletedEventArgs> handler = RegisterCompleted;
            if (handler != null) handler(this, new RegisterCompletedEventArgs(succeeded, remoteTileUri, remoteTileInterval));
        }

        public event EventHandler<UnregisterCompletedEventArgs> UnregisterCompleted;
        protected virtual void InvokeUnregisterCompleted(bool succeeded)
        {
            EventHandler<UnregisterCompletedEventArgs> handler = UnregisterCompleted;
            if (handler != null) handler(this, new UnregisterCompletedEventArgs(succeeded));
        }

        public event EventHandler<UpdateExtraInfoCompletedEventArgs> UpdateExtraInfoCompleted;
        protected virtual void InvokeUpdateExtraInfoCompleted(bool succeeded, string remoteTileUri, string remoteTileInterval)
        {
            EventHandler<UpdateExtraInfoCompletedEventArgs> handler = UpdateExtraInfoCompleted;
            if (handler != null) handler(this, new UpdateExtraInfoCompletedEventArgs(succeeded, remoteTileUri, remoteTileInterval));
        }

        protected string UniqueNotificationId
        {
            get
            {
                return _uniqueClientId;
            }
        }
    }
}