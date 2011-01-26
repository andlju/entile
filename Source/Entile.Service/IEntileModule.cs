using System;
using Entile.Common;

namespace Entile.Service
{
    public interface IEntileModule
    {
        void Initialize(INotificationQueue notificationQueue, IRegistrator registrator);

        string RemoteTileUriFormat { get; }
        string ModuleName { get; }
        Type ServiceType { get; }
    }
}