using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Windows;

namespace Entile
{
    /// <summary>
    /// The DefaultSettingsProvider stores everything in IsolatedStorageSettings. It also uses a new Guid as
    /// the uniqe client id.
    /// </summary>
    public class DefaultSettingsProvider : ISettingsProvider
    {
        private readonly string _uniqueIdKeyName;
        private readonly string _enableKeyName;
        private readonly string _extraInfoKeyName;

        public DefaultSettingsProvider()
            : this("EntileUniqueId", "EntileEnable", "EntileExtraInfo")
        {
        }

        public DefaultSettingsProvider(string uniqueIdKeyName, string enabledKeyName, string extraInfoKeyName)
        {
            _uniqueIdKeyName = uniqueIdKeyName;
            _enableKeyName = enabledKeyName;
            _extraInfoKeyName = extraInfoKeyName;
        }

        public string GetUniqueId()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                return Guid.NewGuid().ToString();
            }
            string uniqueId;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(_uniqueIdKeyName, out uniqueId))
                return uniqueId;

            uniqueId = Guid.NewGuid().ToString();
            IsolatedStorageSettings.ApplicationSettings[_uniqueIdKeyName] = uniqueId;
            return uniqueId;
        }

        public bool GetEnabled()
        {
            if (DesignerProperties.IsInDesignTool)
                return true;

            bool enabled;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(_enableKeyName, out enabled))
                return enabled;

            return false;
        }

        public void SetEnabled(bool enabled)
        {
            IsolatedStorageSettings.ApplicationSettings[_enableKeyName] = enabled;
        }

        public IDictionary<string, string> GetExtraInfo()
        {
            if (DesignerProperties.IsInDesignTool)
                return new Dictionary<string, string>();

            IDictionary<string, string> extraInfo;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(_extraInfoKeyName, out extraInfo))
                return extraInfo;
            
            return new Dictionary<string, string>();
        }

        public void SetExtraInfo(IDictionary<string,string> extraInfo)
        {
            IsolatedStorageSettings.ApplicationSettings[_extraInfoKeyName] = extraInfo;
        }
    }
}