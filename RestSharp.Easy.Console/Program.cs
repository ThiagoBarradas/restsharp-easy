using System.Net.Http;

namespace RestSharp.Easy.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new EasyRestClient("https://xpto.com", requestKey: "12345");

            var result = client.SendRequestAsync<dynamic>(HttpMethod.Get, "").GetAwaiter().GetResult();
        }
    }
}
