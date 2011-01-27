using System;
using System.Collections.Generic;

namespace Entile
{
    public interface IEntileConfig
    {
        string ChannelName { get; }
        Uri RegistrationServiceUri { get; }
        IEnumerable<Uri> AllowedTileUris { get; }
        bool RequestLiveTiles { get; }
        bool RequestToasts { get; }
    }
}