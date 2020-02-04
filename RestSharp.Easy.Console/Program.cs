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
                .UseSuggestedSetting("Lifecycle", "Billing.SDK")
                .SetupSeq(seqOptions)
                .BuildLogger();

            Log.Logger.Error("TESTE");

            var client = new EasyRestClient("https://stgapi.mundipagg.com/datadash/v1", requestKey: "12345");

            var result = client.SendRequestAsync<dynamic>(HttpMethod.Get, "").GetAwaiter().GetResult();

            Thread.Sleep(5000);
        }
    }
}
