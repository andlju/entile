using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Entile
{
    public static class JsonExtension
    {
        public static string ToJson(this object obj)
        {
            if (obj != null)
            {
                var ser = new DataContractJsonSerializer(obj.GetType());

                using (var ms = new MemoryStream())
                {
                    ser.WriteObject(ms, obj);
                    var jsonBytes = ms.ToArray();
                    return Encoding.UTF8.GetString(jsonBytes, 0, jsonBytes.Length);
                }
            }
            return null;
        }

        public static T FromJson<T>(this string json)
        {
            if (json == null)
                return default(T);

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var ser = new DataContractJsonSerializer(typeof(T));
                return (T)ser.ReadObject(ms);
            }
        }
    }
}