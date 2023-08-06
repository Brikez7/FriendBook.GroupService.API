using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace FriendBook.GroupService.Tests.TestHelpers
{
    internal static class JsonContentHelper
    {
        internal static HttpContent Create(object obj, string contentType = "application/json")
        {
            var myContent = JsonConvert.SerializeObject(obj);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return byteContent;
        }
    }
}
