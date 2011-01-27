using Entile.ReferenceHost.EntileModules;
using Entile.Server;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Entile.ReferenceHost.EntileStartup), "Start")]

namespace Entile.ReferenceHost
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