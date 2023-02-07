using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
namespace Apeek.Common.Extensions
{
    public static class JsonHelper
    {
        #region Public Methods and Operators
        /// <summary>
        /// The deserialize.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <typeparam name="T">The type of the value to deserialize.</typeparam>
        /// <returns>
        /// The deserialized value.
        /// </returns>
        public static T Deserialize<T>(Stream stream) where T : class
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }
        public static T Deserialize<T>(string jsonStr) where T : class
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
        public static string Serialize<T>(T obj) where T : class
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
        #endregion
    }
}