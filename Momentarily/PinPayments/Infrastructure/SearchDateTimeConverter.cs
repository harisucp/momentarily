using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace PinPayments.Infrastructure
{
    public class SearchDateTimeConverter : DateTimeConverterBase 
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}