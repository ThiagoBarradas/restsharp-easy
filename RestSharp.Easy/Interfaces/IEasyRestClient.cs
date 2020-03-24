using RestSharp.Easy.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestSharp.Easy.Interfaces
{
    public interface IEasyRestClient
    {
        IRestClient RestClient { get; set; }
        
        void AddAuthorization(string authorization);
        
        void AddBearer(string bearer);
        
        void AddBasic(string basic);

        void AddBasic(string username, string password);

        BaseResponse<TSuccess, TError> SendRequest<TSuccess, TError>(
            HttpMethod method, 
            string endpoint, 
            object body = null, 
            IDictionary<string, string> query = null, 
            IDictionary<string, string> headers = null)
               where TSuccess : class, new() 
               where TError : class, new();

        Task<BaseResponse<TSuccess, TError>> SendRequestAsync<TSuccess, TError>(
            HttpMethod method,
            string endpoint,
            object body = null,
            IDictionary<string, string> query = null,
            IDictionary<string, string> headers = null)
               where TSuccess : class, new()
               where TError : class, new();

        BaseResponse<TSuccess> SendRequest<TSuccess>(
           HttpMethod method,
           string endpoint,
           object body = null,
           IDictionary<string, string> query = null,
           IDictionary<string, string> headers = null)
              where TSuccess : class, new();

        Task<BaseResponse<TSuccess>> SendRequestAsync<TSuccess>(
            HttpMethod method,
            string endpoint,
            object body = null,
            IDictionary<string, string> query = null,
            IDictionary<string, string> headers = null)
               where TSuccess : class, new();
    }
}
