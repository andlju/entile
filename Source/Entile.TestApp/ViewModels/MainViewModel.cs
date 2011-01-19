using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Entile.Client.TestApp.ViewModels
{

    public class MainViewModel : INotifyPropertyChanged
    {
        private EntileClient _entile;
        public EntileClient Entile
        {
            get { return _entile; }
            set
            {
                if (_entile != value)
                {
                    _entile = value;
                    NotifyPropertyChanged("Entile");
                }
            }
        }

        public MainViewModel()
        {
            Entile = new EntileClient("EntileTest");
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