using FriendBook.GroupService.API;
using FriendBook.GroupService.API.Domain.Response;
using System.Text.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using NodaTime.Serialization.SystemTextJson;
using FriendBook.GroupService.API.Domain;

namespace FriendBook.GroupService.Tests.TestHelpers
{
    internal static class DeserializeHelper
    {
        internal static readonly JsonSerializerOptions _jsonSerializerSettings;
        static DeserializeHelper() 
        {
            _jsonSerializerSettings = new JsonSerializerOptions() 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            _jsonSerializerSettings = _jsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            _jsonSerializerSettings.Converters.Add(new BsonIdConverter());
        }
        internal static async Task<StandardResponse<T>> TryDeserializeStandardResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            var content = await httpResponseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<StandardResponse<T>>(content, _jsonSerializerSettings)
                ?? throw new JsonException($"Error deserialize JSON: StandardResponse<{typeof(T)}>");
        }
        internal static async Task<T> TryDeserialize<T>(HttpResponseMessage httpResponseMessage)
        {
            var content = await httpResponseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(content, _jsonSerializerSettings)
                ?? throw new JsonException($"Error deserialize JSON: {typeof(T)}");
        }
        internal static async Task<T> TryDeserialize<T>(Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerSettings)
                ?? throw new JsonException($"Error deserialize JSON: {typeof(T)}");
        }
    }
}
