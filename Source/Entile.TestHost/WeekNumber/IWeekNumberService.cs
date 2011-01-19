using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Entile.TestHost.WeekNumber
{
    [ServiceContract]
    public interface IWeekNumberService
    {
        [OperationContract]
        [WebInvoke(Method = "GET")]
        Stream GetWeekNumberImage(string uniqueId);
    }
}