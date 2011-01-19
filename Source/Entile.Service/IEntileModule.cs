using System;

namespace Entile.Service
{
    public interface IEntileModule
    {
        void Initialize(INotifier notifier, IRegistrator registrator);

        string RemoteTileUriFormat { get; }
        string ModuleName { get; }
        Type ServiceType { get; }
    }
}