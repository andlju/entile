using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace $rootnamespace$.EntileModules
{
    [ServiceContract]
    public interface ISampleService
    {
        [OperationContract]
        [WebInvoke(Method = "GET")]
        Stream GetTileImage(string clientId);

        [OperationContract]
        [WebInvoke(Method = "GET")]
        void SendSampleToastToAllClients();

        [OperationContract]
        [WebInvoke(Method = "GET")]
        void SendSampleTileToAllClients();
    }
}