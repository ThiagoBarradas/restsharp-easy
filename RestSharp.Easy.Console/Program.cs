using Serilog;
using Serilog.Builder;
using Serilog.Builder.Models;
using System.Collections.Generic;
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

            var body = new { test = "xxx", PersonTest = PersonTest.FirstName };

            var result = client.SendRequestAsync<dynamic>(HttpMethod.Post, "restsharp-easy", body)
                .GetAwaiter().GetResult();

            var result2 = client.SendRequest<dynamic>(HttpMethod.Post, "restsharp-easy", body);

            var query3 = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("page", "1"),
                new KeyValuePair<string, string>("size", "10"),
                new KeyValuePair<string, string>("ids", "1"),
                new KeyValuePair<string, string>("ids", "2"),
            };

            var result3 = client.SendRequest<dynamic>(HttpMethod.Post, "restsharp-easy", body, query: query3);

            Thread.Sleep(5000);
        }
    }

    public enum PersonTest
    {
        FirstName
    }
}
