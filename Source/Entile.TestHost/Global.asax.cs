using System;
using System.ServiceModel.Activation;
using System.Web.Routing;
using Entile.Service;
using Entile.Service.Store;
using Entile.TestHost.ManualNotification;
using Entile.TestHost.WeekNumber;

namespace Entile.TestHost
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            EntileHost.Initialize();
            EntileHost.RegisterModule(new WeekNumberModule("http://entile.coding-insomnia.com/"), null);
            EntileHost.RegisterModule(new ManualNotificationModule(), null);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}