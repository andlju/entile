using System;
using System.Collections.Generic;

namespace Entile
{
    public class RegisterCompletedEventArgs : EventArgs
    {
        // TODO Better error handling than "something failed"
        public RegisterCompletedEventArgs(bool succeeded, string remoteTileUri, string remoteTileInterval)
        {
            Succeeded = succeeded;
            RemoteTileUri = remoteTileUri;
            RemoteTileInterval = remoteTileInterval;
        }

        public bool Succeeded { get; private set; }

        public string RemoteTileUri { get; private set; }

        public string RemoteTileInterval { get; private set; }
    }

    public class UnregisterCompletedEventArgs : EventArgs
    {
        // TODO Better error handling than "something failed"
        public UnregisterCompletedEventArgs(bool succeeded)
        {
            Succeeded = succeeded;
        }

        public bool Succeeded { get; private set; }
    }

    public class UpdateExtraInfoCompletedEventArgs : EventArgs
    {
        // TODO Better error handling than "something failed"
        public UpdateExtraInfoCompletedEventArgs(bool succeeded, string remoteTileUri, string remoteTileInterval)
        {
            Succeeded = succeeded;
            RemoteTileUri = remoteTileUri;
            RemoteTileInterval = remoteTileInterval;
        }

        public bool Succeeded { get; private set; }

        public string RemoteTileUri { get; private set; }

        public string RemoteTileInterval { get; private set; }
    }

    public interface IRegistrationManager
    {
        void RegisterAsync(Uri notificationUri, IDictionary<string, string> extraInfo);
        void UpdateExtraInfoAsync(IDictionary<string, string> extraInfo);
        void UnregisterAsync();

        event EventHandler<RegisterCompletedEventArgs> RegisterCompleted;
        event EventHandler<UnregisterCompletedEventArgs> UnregisterCompleted;
        event EventHandler<UpdateExtraInfoCompletedEventArgs> UpdateExtraInfoCompleted;
    }
}