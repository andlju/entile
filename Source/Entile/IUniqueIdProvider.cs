using System.Collections.Generic;

namespace Entile
{
    public interface ISettingsProvider
    {
        string GetUniqueId();

        bool GetEnabled();
        void SetEnabled(bool enabled);

        IDictionary<string, string> GetExtraInfo();
        void SetExtraInfo(IDictionary<string, string> extraInfo);
    }
}