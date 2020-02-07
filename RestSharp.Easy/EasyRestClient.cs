using Newtonsoft.Json;
using PackUtils;
using RestSharp.Easy.Interfaces;
using RestSharp.Easy.Models;
using RestSharp.Serilog.Auto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestSharp.Easy
{
    public class EasyRestClient : IEasyRestClient
    {
        private IRestClient RestClient;

        private NewtonsoftRestsharpJsonSerializer NewtonsoftRestsharpJsonSerializer;

        private JsonSerializer JsonSerializer;

        private JsonSerializerSettings JsonSerializerSettings;

        public EasyRestClient(
            string baseUrl,
            IDictionary<string, string> defaultHeaders = null,
            SerializeStrategyEnum serializeStrategy = SerializeStrategyEnum.SnakeCase,
            int timeoutInMs = 60000,
            string requestKey = null,
            IDictionary<string, string> additionalLogItems = null,
            string userAgent = "RestSharp Easy! https://github.com/ThiagoBarradas/restsharp-easy",
            string[] jsonBlackList = null)
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
                JsonLogBlacklist = jsonBlackList ?? EasyRestClientConfiguration.DefaultJsonBlacklist
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

        public BaseResponse<TSuccess, TError> SendRequest<TSuccess, TError>(HttpMethod method, string endpoint, object body = null, IDictionary<string, string> query = null, IDictionary<string, string> headers = null)
            where TSuccess : class, new()
            where TError : class, new()
        {
            return this.SendRequestAsync<TSuccess, TError>(method, endpoint, body, query, headers)
                .GetAwaiter().GetResult();
        }

        public async Task<BaseResponse<TSuccess, TError>> SendRequestAsync<TSuccess, TError>(HttpMethod method, string endpoint, object body = null, IDictionary<string, string> query = null, IDictionary<string, string> headers = null)
            where TSuccess : class, new()
            where TError : class, new()
        {
            BaseResponse<TSuccess, TError> response = new BaseResponse<TSuccess, TError>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var restSharpMethod = EnumUtility.ConvertToEnum<Method>(method.Method.ToUpper());
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

        public BaseResponse<TSuccess> SendRequest<TSuccess>(HttpMethod method, string endpoint, object body = null, IDictionary<string, string> query = null, IDictionary<string, string> headers = null) where TSuccess : class, new()
        {
            return this.SendRequest<TSuccess, dynamic>(method, endpoint, body, query, headers);
        }

        public async Task<BaseResponse<TSuccess>> SendRequestAsync<TSuccess>(HttpMethod method, string endpoint, object body = null, IDictionary<string, string> query = null, IDictionary<string, string> headers = null) where TSuccess : class, new()
        {
            return await this.SendRequestAsync<TSuccess, dynamic>(method, endpoint, body, query, headers);
        }

        private void Initialize(EasyRestClientConfiguration configuration)
        {
            var client = new RestClientAutolog(configuration.BaseUrl);

            client.Timeout = configuration.TimeoutInMs;
            
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

            client.Configuration.JsonBlacklist = configuration.JsonLogBlacklist;

            client.UserAgent = configuration.UserAgent;

            if (configuration.DefaultHeaders != null)
            {
                foreach (var header in configuration.DefaultHeaders)
                {
                    client.AddDefaultHeader(header.Key, header.Value);
                }
            }

            var strategy = configuration.SerializeStrategy.ToString();
            this.JsonSerializer = strategy.GetNewtonsoftJsonSerializer();
            this.JsonSerializerSettings = strategy.GetNewtonsoftJsonSerializerSettings();

            this.NewtonsoftRestsharpJsonSerializer = new NewtonsoftRestsharpJsonSerializer(this.JsonSerializer);
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
                catch(Exception e)
                {
                    response.Exception = e;
                }
            }
        }
    }
}