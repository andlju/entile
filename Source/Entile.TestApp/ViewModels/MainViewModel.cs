using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Entile.Client.TestApp.ViewModels
{

    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly EntileClient _entile;

        public MainViewModel()
        {
            _entile = new EntileClient();
        }

        public EntileClient Entile
        {
            get { return _entile; }
        }

        private string _tileTitle;
        public string TileTitle
        {
            get { return _tileTitle; }
            set
            {
                _tileTitle = value;
                NotifyPropertyChanged("TileTitle");
                UpdateRegistrationInfo();
            }
        }

        private void UpdateRegistrationInfo()
        {
            var info = new Dictionary<string, string>();
            info["TileTitle"] = TileTitle;
            _entile.UpdateExtraInfo(info);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}