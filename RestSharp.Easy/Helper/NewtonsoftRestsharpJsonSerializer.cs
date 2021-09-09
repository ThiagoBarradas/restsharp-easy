using Newtonsoft.Json;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System.IO;

namespace RestSharp.Easy.Helper
{
    public class NewtonsoftRestsharpJsonSerializer : ISerializer, IDeserializer
    {
        private Newtonsoft.Json.JsonSerializer Serializer { get; set; }

        public NewtonsoftRestsharpJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            this.Serializer = serializer;
        }

        public string ContentType
        {
            get { return "application/json"; }
            set { }
        }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    Serializer.Serialize(jsonTextWriter, obj);

                    return stringWriter.ToString();
                }
            }
        }

        public T Deserialize<T>(RestSharp.IRestResponse response)
        {
            var content = response.Content;

            using (var stringReader = new StringReader(content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return Serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }
    }

    public static class NewtonsoftRestsharpJsonSerializerExtension
    {
        public static void AddNewtonsoftResponseHandler(this IRestClient restClient, NewtonsoftRestsharpJsonSerializer serializer)
        {
            string[] contentTypes = new string[]
            {
                "application/json",
                "text/json",
                "text/x-json",
                "text/javascript",
                "*+json"
            };

            foreach (var contentType in contentTypes)
            {
                restClient.AddHandler(contentType, serializer);
            }
        }

        public static void AddNewtonsoftRequestHandler(this IRestRequest restRequest, NewtonsoftRestsharpJsonSerializer serializer)
        {
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.JsonSerializer = serializer;
        }

        public static void AddNewtonsoftHandler(this IRestClient restClient, IRestRequest restRequest, NewtonsoftRestsharpJsonSerializer serializer)
        {
            restClient.AddNewtonsoftResponseHandler(serializer);
            restRequest.AddNewtonsoftRequestHandler(serializer);
        }
    }
}
