using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Tasks;

namespace Entile.ReferenceApp
{
    public partial class EntileView : UserControl
    {
        public EntileView()
        {
            InitializeComponent();
        }

        private void EntileButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.URL = "http://coding-insomnia.com/entile";
            task.Show();
        }
    }
}
