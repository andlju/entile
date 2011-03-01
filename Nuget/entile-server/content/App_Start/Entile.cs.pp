using $rootnamespace$.EntileModules;
using Entile.Server;

[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.EntileStartup), "Start")]

namespace $rootnamespace$.App_Start
{
    public class EntileStartup
    {
        public static void Start()
        {
            // TODO Change the host name (and probably register other modules)
            EntileHost.RegisterModule(new SampleModule("http://localhost:1234"));
        }
    }
}