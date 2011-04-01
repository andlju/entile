using System;
using System.Collections.Generic;

namespace Entile
{
    public interface IEntileClient
    {
        bool Enable { get; set; }
        void Refresh();
        void UpdateExtraInfo(IDictionary<string, string> extraInfo);

        IChannelManager ChannelManager { get; }
        IRegistrationManager RegistrationManager { get; }
        IRemoteTileManager RemoteTileManager { get; }

        bool Busy { get; }

        event EventHandler<EntileErrorEventArgs> ErrorOccured;
        event EventHandler<ToastMessageEventArgs> ToastMessageReceived;
        event EventHandler<RawMessageEventArgs> RawMessageReceived;
    }
}