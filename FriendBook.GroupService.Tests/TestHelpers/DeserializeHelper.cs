using FriendBook.GroupService.API.Domain.Response;
using MongoDB.Bson;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace FriendBook.GroupService.Tests.TestHelpers
{
    internal static class DeserializeHelper
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings;
        static DeserializeHelper() 
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateParseHandling = DateParseHandling.None,
                Converters = new List<JsonConverter> { new BsonObjectIdConvector() }
            };
            _jsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        }
        internal static async Task<StandardResponse<T>> TryDeserializeStandardResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StandardResponse<T>>(content, _jsonSerializerSettings)
                ?? throw new JsonException($"Error deserialize JSON: StandardResponse<{typeof(T)}>");
        }
        internal static async Task<T> TryDeserialize<T>(HttpResponseMessage httpResponseMessage)
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content, _jsonSerializerSettings)
                ?? throw new JsonException($"Error deserialize JSON: {typeof(T)}");
        }
        private class BsonObjectIdConvector : JsonConverter
        {
            public override bool CanConvert(Type objectType)
                => typeof(ObjectId).IsAssignableFrom(objectType);
            

            public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.String)
                    throw new Exception($"Unexpected token parsing ObjectId. Expected String, got {reader.TokenType}.");

                var value = (string?)reader?.Value;
                return string.IsNullOrEmpty(value) ? ObjectId.Empty : new ObjectId(value);
            }

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                if (value is ObjectId objectId)
                {
                    writer.WriteValue(objectId != ObjectId.Empty ? objectId.ToString() : string.Empty);
                }
                else
                {
                    throw new Exception("Expected ObjectId value.");
                }
            }
        }
    }
}
