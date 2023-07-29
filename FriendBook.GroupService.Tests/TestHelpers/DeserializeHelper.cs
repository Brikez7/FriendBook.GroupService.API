using FriendBook.GroupService.API.Domain.Response;
using Newtonsoft.Json;

namespace FriendBook.GroupService.Tests.TestHelpers
{
    internal static class DeserializeHelper
    {
        internal static async Task<StandardResponse<T>> TryDeserializeStandardResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<StandardResponse<T>>(await httpResponseMessage.Content.ReadAsStringAsync())
                ?? throw new JsonException($"Error deserialize JSON: StandardResponse<{typeof(T)}>");
        }
        internal static async Task<T> TryDeserialize<T>(HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<T>(await httpResponseMessage.Content.ReadAsStringAsync())
                ?? throw new JsonException($"Error deserialize JSON: {typeof(T)}");
        }
    }
}
