using FriendBook.GroupService.API.Domain.Response;
using Newtonsoft.Json;

namespace FriendBook.GroupService.Tests.TestHelpers
{
    internal static class DeserializeHelper
    {
        internal static async Task<StandartResponse<T>> TryDeserializeStandartResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<StandartResponse<T>>(await httpResponseMessage.Content.ReadAsStringAsync())
                ?? throw new JsonException($"Error deserialize JSON: StandartResponse<{typeof(T)}>");
        }
    }
}
