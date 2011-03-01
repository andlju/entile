using System;
using Entile.Common;
using Entile.Service;

namespace Entile.ReferenceHost.EntileModules
{
    public class SampleModule : IEntileModule
    {
        private readonly string _hostUrl;

        public SampleModule(string hostUrl)
        {
            _hostUrl = hostUrl;
        }

        public void Initialize(INotificationQueue notificationQueue, IRegistrator registrator)
        {
            
        }

        public string RemoteTileUriFormat
        {
            // TODO Set tile format string
            get { return _hostUrl + "/Sample/GetTileImage?clientId={0}"; }
        }

        public string ModuleName
        {
            get { return "Sample"; }
        }

        public Type ServiceType
        {
            get { return typeof(SampleService); }
        }
    }
}