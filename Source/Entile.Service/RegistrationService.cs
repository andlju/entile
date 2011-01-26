using System.Collections.Generic;
using System.ServiceModel.Activation;
using Entile.Common;

namespace Entile.Service
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RegistrationService : IRegistrationService
    {
        public string EveryHourInterval = "EveryHour";
        public string EveryDayInterval = "EveryDay";
        public string EveryWeekInterval = "EveryWeek";
        public string EveryMonthInterval = "EveryMonth";

        private readonly IRegistrator _registrator;

        public RegistrationService(IRegistrator registrator)
        {
            _registrator = registrator;
        }

        public RemoteTileInfo Register(string uniqueId, string notificationChannel, IDictionary<string, string> extraInfo)
        {
            var remoteTileUri = _registrator.Register(uniqueId, notificationChannel, extraInfo);

            return new RemoteTileInfo()
            {
                Uri = remoteTileUri,
                Interval = EveryHourInterval // TODO Add config support for other intervals
            };
        }

        public void Unregister(string uniqueId)
        {
            _registrator.Unregister(uniqueId);
        }

        public RemoteTileInfo UpdateExtraInfo(string uniqueId, IDictionary<string, string> extraInfo)
        {
            var remoteTileUri = _registrator.UpdateExtraInfo(uniqueId, extraInfo);

            return new RemoteTileInfo()
                       {
                           Uri = remoteTileUri,
                           Interval = EveryHourInterval // TODO Add config support for other intervals
                       };
        }
    }
}