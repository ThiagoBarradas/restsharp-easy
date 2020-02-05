using Serilog;
using Serilog.Builder;
using Serilog.Builder.Models;
using System.Net.Http;
using System.Threading;

namespace RestSharp.Easy.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            LoggerBuilder builder = new LoggerBuilder();

            SeqOptions seqOptions = new SeqOptions
            {
                Enabled = true,
                Url = "http://localhost:5341"
            };

            Log.Logger = builder
                .UseSuggestedSetting("RestSharp", "RestSharp.Easy")
                .SetupSeq(seqOptions)
                .BuildLogger();

            Log.Logger.Error("TESTE");

            var client = new EasyRestClient("http://pruu.herokuapp.com/dump", requestKey: "12345");

            var body = new { test = "xxx" };

            var result = client.SendRequestAsync<dynamic>(HttpMethod.Post, "restsharp-easy", body)
                .GetAwaiter().GetResult();

            var result2 = client.SendRequest<dynamic>(HttpMethod.Post, "restsharp-easy", body);

            Thread.Sleep(5000);
        }
    }
}
