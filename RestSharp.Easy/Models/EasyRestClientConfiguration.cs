using Newtonsoft.Json;
using System.Collections.Generic;

namespace RestSharp.Easy.Models
{
    public class EasyRestClientConfiguration
    {
        public static string[] DefaultJsonBlacklist = new string[]
                {
                    "*card.token",
                    "*card.exp_year",
                    "*card.exp_month",
                    "*card.cvv",
                    "*card.number",
                    "token",
                    "exp_year",
                    "exp_month",
                    "cvv",
                    "number",
                    "*password"
                };

        public string BaseUrl { get; set; }

        public string[] JsonLogBlacklist { get; set; } = EasyRestClientConfiguration.DefaultJsonBlacklist;

        public IDictionary<string, string> DefaultHeaders { get; set; }

        public SerializeStrategyEnum SerializeStrategy { get; set; } = SerializeStrategyEnum.SnakeCase;

        public int TimeoutInMs { get; set; } = 60000;

        public string RequestKey { get; set; }

        public bool EnableLog { get; set; }

        public IDictionary<string, string> AdditionalLogItems { get; set; }

        public List<JsonConverter> Converters { get; set; }

        public string MerchantId { get; set; }

        public string UserAgent { get; set; } = "RestSharp Easy! https://github.com/ThiagoBarradas/restsharp-easy";
    }
}
