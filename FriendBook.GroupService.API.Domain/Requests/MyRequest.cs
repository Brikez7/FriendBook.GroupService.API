using System.Net;

namespace FriendBook.GroupService.API.Domain.Requests
{
    public static class MyTypeRequest
    {
        public static readonly string GET = "GET";
        public static readonly string POST = "POST";
        public static readonly string DELETE = "DELETE";
    }
    public class MyRequest
    {
        public HttpWebRequest request;
        public string Adress;
        public string Response { get; set; }
        public string? Token { get; set; } = null;

        public MyRequest(string adress, string? token)
        {
            Adress = adress;
            Token = token;
        }
        public async Task SendRequest(string method)
        {
            request = (HttpWebRequest)WebRequest.Create(Adress);
            request.Method = method;
            if (Token != null)
            {
                request.Headers.Add("Authorization",Token);
            }
            try
            {
                HttpWebResponse response =  (HttpWebResponse)(await request.GetResponseAsync());
                var stream = response.GetResponseStream();
                if (stream != null)
                {
                    Response = await new StreamReader(stream).ReadToEndAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}