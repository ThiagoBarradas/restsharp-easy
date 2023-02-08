using Newtonsoft.Json;
using RestSharp.Easy.Helper;
using RestSharp.Easy.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestSharp.Easy.Interfaces
{
    public interface IEasyRestClient
    {
        IRestClient RestClient { get; set; }
        JsonSerializerSettings JsonSerializerSettings { get; }
        NewtonsoftRestsharpJsonSerializer NewtonsoftRestsharpJsonSerializer { get; }

        void AddAuthorization(string authorization);

        void AddBearer(string bearer);

        void AddBasic(string basic);

        void AddBasic(string username, string password);

        BaseResponse<TSuccess, TError> SendRequest<TSuccess, TError>(RestRequest restRequest)
            where TSuccess : class, new()
            where TError : class, new();

        BaseResponse<TSuccess, TError> SendRequest<TSuccess, TError>(
            HttpMethod method,
            string endpoint,
            object body = null,
            ICollection<KeyValuePair<string, string>> query = null,
            ICollection<KeyValuePair<string, string>> headers = null)
               where TSuccess : class, new()
               where TError : class, new();

        BaseResponse<TSuccess> SendRequest<TSuccess>(RestRequest restRequest) 
            where TSuccess : class, new();

        BaseResponse<TSuccess> SendRequest<TSuccess>(
           HttpMethod method,
           string endpoint,
           object body = null,
           ICollection<KeyValuePair<string, string>> query = null,
           ICollection<KeyValuePair<string, string>> headers = null)
              where TSuccess : class, new();

        Task<BaseResponse<TSuccess, TError>> SendRequestAsync<TSuccess, TError>(RestRequest restRequest)
            where TSuccess : class, new()
            where TError : class, new();

        Task<BaseResponse<TSuccess, TError>> SendRequestAsync<TSuccess, TError>(
            HttpMethod method,
            string endpoint,
            object body = null,
            ICollection<KeyValuePair<string, string>> query = null,
            ICollection<KeyValuePair<string, string>> headers = null)
               where TSuccess : class, new()
               where TError : class, new();

        Task<BaseResponse<TSuccess>> SendRequestAsync<TSuccess>(RestRequest restRequest)
            where TSuccess : class, new();

        Task<BaseResponse<TSuccess>> SendRequestAsync<TSuccess>(
            HttpMethod method,
            string endpoint,
            object body = null,
            ICollection<KeyValuePair<string, string>> query = null,
            ICollection<KeyValuePair<string, string>> headers = null)
               where TSuccess : class, new();
    }
}
