using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Entile.Service
{

    [DataContract]
    public class RemoteTileInfo
    {
        [DataMember]
        public string Uri { get; set; }

        [DataMember]
        public string Interval { get; set; }
    }

    [ServiceContract]
    public interface IRegistrationService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        RemoteTileInfo Register(string uniqueId, string notificationChannel, IDictionary<string, string> extraInfo);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void Unregister(string uniqueId);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        RemoteTileInfo UpdateExtraInfo(string uniqueId, IDictionary<string, string> extraInfo);
    }

}