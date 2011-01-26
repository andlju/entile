using System;
using System.Collections.Generic;
using Entile.Common;

namespace Entile.Service
{
    public class Registrator : IRegistrator
    {
        private readonly IRegistrationStore _registrationStore;
        private readonly string _remoteTileFormat;

        public Registrator(IRegistrationStore registrationStore, string remoteTileFormat)
        {
            _registrationStore = registrationStore;
            _remoteTileFormat = remoteTileFormat;
        }

        public string Register(string uniqueId, string notificationChannel, IDictionary<string, string> extraInfo)
        {
            _registrationStore.UpdateRegistration(new Registration() { UniqueId = uniqueId, NotificationChannel = notificationChannel });
            if (extraInfo != null)
            {
                _registrationStore.UpdateExtraInfo(uniqueId, extraInfo);
            }
            
            InvokeClientRegistered(new ClientRegistrationEventArgs(uniqueId, extraInfo));
            
            // If the module supports a remote tile, return its Uri
            if (_remoteTileFormat != null)
                return string.Format(_remoteTileFormat, uniqueId);
            
            return string.Empty;
        }

        public void Unregister(string uniqueId)
        {
            _registrationStore.RemoveRegistration(uniqueId);
            _registrationStore.RemoveExtraInfo(uniqueId);
        }

        public string UpdateExtraInfo(string uniqueId, IDictionary<string,string> extraInfo)
        {
            _registrationStore.UpdateExtraInfo(uniqueId, extraInfo);
            InvokeClientExtraInfoUpdated(new ClientRegistrationEventArgs(uniqueId, extraInfo));

            // If the module supports a remote tile, return its Uri
            if (_remoteTileFormat != null)
                return string.Format(_remoteTileFormat, uniqueId);
            return string.Empty;
        }

        public IEnumerable<Registration> ListAllSubscribers()
        {
            return _registrationStore.ListAllRegistrations();
        }

        public Registration GetRegistration(string uniqueId)
        {
            return _registrationStore.GetRegistration(uniqueId);
        }

        public IDictionary<string,string> GetExtraInfo(string uniqueId)
        {
            return _registrationStore.GetExtraInfo(uniqueId);
        }

        public event EventHandler<ClientRegistrationEventArgs> ClientRegistered;

        protected virtual void InvokeClientRegistered(ClientRegistrationEventArgs e)
        {
            EventHandler<ClientRegistrationEventArgs> handler = ClientRegistered;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<ClientRegistrationEventArgs> ClientExtraInfoUpdated;
        protected virtual void InvokeClientExtraInfoUpdated(ClientRegistrationEventArgs e)
        {
            EventHandler<ClientRegistrationEventArgs> handler = ClientExtraInfoUpdated;
            if (handler != null) handler(this, e);
        }
    }
}