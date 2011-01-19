using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Entile.TestHost.ManualNotification
{
    [ServiceContract]
    public interface IManualNotificationService
    {
        [OperationContract]
        [WebInvoke(Method = "GET")]
        void SendNotification(string clientUniqueId, string title, string body);

        [OperationContract]
        [WebInvoke(Method = "GET")]
        IEnumerable<string> GetRegisteredClients();
    }
}