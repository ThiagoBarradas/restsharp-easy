using Newtonsoft.Json;
using RestSharp.Easy.Helper;
using RestSharp.Easy.Interfaces;
using RestSharp.Easy.Models;
using RestSharp.Serilog.Auto;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestSharp.Easy
{
    public class EasyRestClient : IEasyRestClient
    {
        public IRestClient RestClient { get; set; }

        public JsonSerializerSettings JsonSerializerSettings { get; private set; }

        public NewtonsoftRestsharpJsonSerializer NewtonsoftRestsharpJsonSerializer { get; private set; }

        public EasyRestClient(
            string baseUrl = null,
            IDictionary<string, string> defaultHeaders = null,
            SerializeStrategyEnum serializeStrategy = SerializeStrategyEnum.SnakeCase,
            int timeoutInMs = 60000,
            string requestKey = null,
            IDictionary<string, string> additionalLogItems = null,
            string userAgent = "RestSharp Easy! https://github.com/ThiagoBarradas/restsharp-easy",
            string[] jsonBlackList = null,
            List<JsonConverter> converters = null,
            bool enableLog = true,
            Dictionary<HttpStatusCode, LogEventLevel> overrideLogLevelByStatusCode = null)
        {
            var config = new EasyRestClientConfiguration
            {
                BaseUrl = baseUrl,
                TimeoutInMs = timeoutInMs,
                DefaultHeaders = defaultHeaders,
                SerializeStrategy = serializeStrategy,
                RequestKey = requestKey,
                AdditionalLogItems = additionalLogItems,
                UserAgent = userAgent,
                RequestJsonLogBlacklist = jsonBlackList ?? EasyRestClientConfiguration.DefaultJsonBlacklist,
                Converters = converters,
                EnableLog = enableLog,
                OverrideLogLevelByStatusCode = overrideLogLevelByStatusCode
            };

            this.Initialize(config);
        }

        public EasyRestClient(EasyRestClientConfiguration config)
        {
            this.Initialize(config);
        }

        public void AddAuthorization(string authorization)
        {
            this.RestClient.AddDefaultHeader("Authorization", $"{authorization}");
        }

        public void AddBearer(string bearer)
        {
            this.AddAuthorization($"Bearer {bearer}");
        }

        public void AddBasic(string basic)
        {
            this.AddAuthorization($"Basic {basic}");
        }

        public void AddBasic(string username, string password)
        {
            var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            this.AddAuthorization($"Basic {basic}");
        }

        public BaseResponse<TSuccess, TError> SendRequest<TSuccess, TError>(RestRequest restRequest)
            where TSuccess : class, new()
            where TError : class, new()
        {
            return this.SendRequestAsync<TSuccess, TError>(restRequest)
                .GetAwaiter().GetResult();
        }

        public BaseResponse<TSuccess, TError> SendRequest<TSuccess, TError>(HttpMethod method, string endpoint, object body = null, ICollection<KeyValuePair<string, string>> query = null, ICollection<KeyValuePair<string, string>> headers = null)
            where TSuccess : class, new()
            where TError : class, new()
        {
            return this.SendRequestAsync<TSuccess, TError>(method, endpoint, body, query, headers)
                .GetAwaiter().GetResult();
        }

        public BaseResponse<TSuccess> SendRequest<TSuccess>(RestRequest restRequest) where TSuccess : class, new()
        {
            return this.SendRequest<TSuccess, dynamic>(restRequest);
        }

        public BaseResponse<TSuccess> SendRequest<TSuccess>(HttpMethod method, string endpoint, object body = null, ICollection<KeyValuePair<string, string>> query = null, ICollection<KeyValuePair<string, string>> headers = null) where TSuccess : class, new()
        {
            return this.SendRequest<TSuccess, dynamic>(method, endpoint, body, query, headers);
        }

        public async Task<BaseResponse<TSuccess, TError>> SendRequestAsync<TSuccess, TError>(RestRequest restRequest)
            where TSuccess : class, new()
            where TError : class, new()
        {
            BaseResponse<TSuccess, TError> response = new BaseResponse<TSuccess, TError>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var body = restRequest.GetRequestBody();

            if (body != null && restRequest.Method != Method.GET)
            {
                response.RawRequest = JsonConvert.SerializeObject(body, JsonSerializerSettings);
            }

            var restResponse = await this.RestClient.ExecuteAsync(restRequest);
            this.HandleResponse(response, restResponse);

            stopwatch.Stop();
            response.ElapsedTime = stopwatch.ElapsedMilliseconds;

            return response;
        }

        public async Task<BaseResponse<TSuccess, TError>> SendRequestAsync<TSuccess, TError>(HttpMethod method, string endpoint, object body = null, ICollection<KeyValuePair<string, string>> query = null, ICollection<KeyValuePair<string, string>> headers = null)
            where TSuccess : class, new()
            where TError : class, new()
        {
            BaseResponse<TSuccess, TError> response = new BaseResponse<TSuccess, TError>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var restSharpMethod = EnumHelper.ConvertToEnum<Method>(method.Method.ToUpper());
            var restRequest = new RestRequest(endpoint, restSharpMethod);
            restRequest.AddNewtonsoftRequestHandler(this.NewtonsoftRestsharpJsonSerializer);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    restRequest.AddHeader(header.Key, header.Value);
                }
            }

            if (query != null)
            {
                foreach (var queryItem in query)
                {
                    restRequest.AddQueryParameter(queryItem.Key, queryItem.Value);
                }
            }

            if (body != null && restSharpMethod != Method.GET)
            {
                restRequest.AddJsonBody(body);
                response.RawRequest = JsonConvert.SerializeObject(body, JsonSerializerSettings);
            }

            var restResponse = await this.RestClient.ExecuteAsync(restRequest, restRequest.Method);
            this.HandleResponse(response, restResponse);

            stopwatch.Stop();
            response.ElapsedTime = stopwatch.ElapsedMilliseconds;

            return response;
        }

        public async Task<BaseResponse<TSuccess>> SendRequestAsync<TSuccess>(RestRequest restRequest)
            where TSuccess : class, new()
        {
            return await this.SendRequestAsync<TSuccess, dynamic>(restRequest);
        }

        public async Task<BaseResponse<TSuccess>> SendRequestAsync<TSuccess>(HttpMethod method, string endpoint, object body = null, ICollection<KeyValuePair<string, string>> query = null, ICollection<KeyValuePair<string, string>> headers = null) where TSuccess : class, new()
        {
            return await this.SendRequestAsync<TSuccess, dynamic>(method, endpoint, body, query, headers);
        }

        private void Initialize(EasyRestClientConfiguration configuration)
        {
            var client = (string.IsNullOrEmpty(configuration.BaseUrl))
                ? new RestClientAutolog()
                : new RestClientAutolog(configuration.BaseUrl);

            client.Timeout = configuration.TimeoutInMs;
            client.EnableLog(configuration.EnableLog);

            client.Configuration.OverrideLogLevelByStatusCode = configuration.OverrideLogLevelByStatusCode;

            if (configuration.RequestKey != null)
            {
                client.AddLogAdditionalInfo("RequestKey", configuration.RequestKey);
            }

            if (configuration.AdditionalLogItems != null)
            {
                foreach (var logItem in configuration.AdditionalLogItems)
                {
                    client.AddLogAdditionalInfo(logItem.Key, logItem.Value);
                }
            }

            client.Configuration.RequestJsonBlacklist = configuration.RequestJsonLogBlacklist;
            client.Configuration.ResponseJsonBlacklist = configuration.ResponseJsonLogBlacklist;

            client.UserAgent = configuration.UserAgent;

            if (configuration.DefaultHeaders != null)
            {
                foreach (var header in configuration.DefaultHeaders)
                {
                    client.AddDefaultHeader(header.Key, header.Value);
                }
            }

            var strategy = configuration.SerializeStrategy.ToString();
            var jsonSerializer = strategy.GetNewtonsoftJsonSerializer();
            this.JsonSerializerSettings = strategy.GetNewtonsoftJsonSerializerSettings();

            if (configuration.Converters != null)
            {
                foreach (var converter in configuration.Converters)
                {
                    jsonSerializer.Converters.Add(converter);
                    this.JsonSerializerSettings.Converters.Add(converter);
                }
            }

            this.NewtonsoftRestsharpJsonSerializer = new NewtonsoftRestsharpJsonSerializer(jsonSerializer);
            client.AddNewtonsoftResponseHandler(this.NewtonsoftRestsharpJsonSerializer);

            this.RestClient = client;
        }

        private void HandleResponse<TSuccess, TError>(
            BaseResponse<TSuccess, TError> response,
            IRestResponse restResponse)
        {
            response.StatusCode = restResponse.StatusCode;
            response.RawResponse = restResponse.Content;

            if (restResponse.ErrorException != null)
            {
                response.Exception = restResponse.ErrorException;
            }

            if (restResponse.IsSuccessful == true &&
                string.IsNullOrWhiteSpace(response.RawResponse) == false)
            {
                try
                {
                    response.Data = JsonConvert.DeserializeObject<TSuccess>(response.RawResponse, this.JsonSerializerSettings);
                }
                catch (Exception e)
                {
                    response.Exception = e;
                }
            }

            if (restResponse.IsSuccessful == false &&
                string.IsNullOrWhiteSpace(response.RawResponse) == false)
            {
                try
                {
                    response.Error = JsonConvert.DeserializeObject<TError>(response.RawResponse, this.JsonSerializerSettings);
                }
                catch (Exception e)
                {
                    response.Exception = e;
                }
            }
        }
    }
}
