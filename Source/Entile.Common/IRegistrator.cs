using System;
using System.Collections.Generic;

namespace Entile.Common
{
    public class ClientRegistrationEventArgs : EventArgs
    {
        public ClientRegistrationEventArgs(string uniqueId)
        {
            UniqueId = uniqueId;
        }

        public ClientRegistrationEventArgs(string uniqueId, IDictionary<string,string> extraInfo)
        {
            UniqueId = uniqueId;
            ExtraInfo = extraInfo;
        }

        public string UniqueId { get; private set; }
        public IDictionary<string,string> ExtraInfo { get; private set; }
    }

    public interface IRegistrator
    {
        string Register(string uniqueId, string notificationChannel, IDictionary<string, string> extraInfo);
        string UpdateExtraInfo(string uniqueId, IDictionary<string, string> extraInfo);
        void Unregister(string uniqueId);

        Registration GetRegistration(string uniqueId);
        IEnumerable<Registration> ListAllSubscribers();
        IDictionary<string, string> GetExtraInfo(string uniqueId);

        event EventHandler<ClientRegistrationEventArgs> ClientRegistered;
        event EventHandler<ClientRegistrationEventArgs> ClientExtraInfoUpdated;
    }
}