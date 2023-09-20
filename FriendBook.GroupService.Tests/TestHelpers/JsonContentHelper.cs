using System.Net.Http.Json;

namespace FriendBook.GroupService.Tests.TestHelpers
{
    internal static class JsonContentHelper
    {
        internal static HttpContent Create<T>(T obj)
            => JsonContent.Create(obj, options: DeserializeHelper._jsonSerializerSettings);
    }
}
