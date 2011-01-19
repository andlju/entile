using System;
using System.ComponentModel;
using System.Windows;

namespace Entile
{
    
    public class EntileErrorEventArgs : EventArgs
    {
        public EntileErrorEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; private set; }
    }

}