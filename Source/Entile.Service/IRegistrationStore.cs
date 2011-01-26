using System.Collections.Generic;
using Entile.Common;

namespace Entile.Service
{
    public interface IRegistrationStore
    {
        void AddRegistration(Registration registration);
        void RemoveRegistration(string uniqueId);

        IEnumerable<Registration> ListAllRegistrations();
        Registration GetRegistration(string uniqueId);
        void UpdateRegistration(Registration registration);

        void UpdateExtraInfo(string uniqueId, IDictionary<string, string> extraInfo);
        void RemoveExtraInfo(string uniqueId);
        IDictionary<string, string> GetExtraInfo(string uniqueId);
    }
}