using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestSharp.Easy.Helper
{
    public static class RestsharpHelper
    {
        public static void AddQueryString(this RestRequest request, IDictionary<string, List<string>> headers)
        {
            if (headers?.Any() != true)
            {
                return;
            }

            foreach (var queryItem in headers)
            {
                request.AddParameter(queryItem.Key, string.Join(",", queryItem.Value), ParameterType.QueryString);
            }
        }

        public static void AddQueryString(this RestRequest request, IDictionary<string, string> headers)
        {
            if (headers?.Any() != true)
            {
                return;
            }

            foreach (var queryItem in headers)
            {
                request.AddParameter(queryItem.Key, queryItem, ParameterType.QueryString);
            }
        }

        public static void AddJsonBodyAsString(this RestRequest request, string content)
        {
            if (request.Method != Method.GET && !string.IsNullOrWhiteSpace(content))
            {
                request.AddParameter("application/json", content, ParameterType.RequestBody);
            }
        }

        public static void AddBasicAuth(this RestRequest request, string user, string pass)
        {
            IRestRequest restRequest = request.AddHeader("Authorization", $"Basic " + $"{user}:{pass}".Base64Encode());
        }

        private static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static void AddHeaderNotEmpty(this RestRequest request, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value) == false)
            {
                request.AddHeader(key, value);
            }
        }

        public static object GetRequestBody(this RestRequest request)
        {
            if (request.Body != null) return request.Body;

            return request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody)?.Value;
        }
    }
}
