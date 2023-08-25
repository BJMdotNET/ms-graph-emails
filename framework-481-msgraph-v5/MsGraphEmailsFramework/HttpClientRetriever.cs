using System;
using System.Net.Http;

namespace MsGraphEmailsFramework
{
    internal static class HttpClientRetriever
    {
        public static HttpClient Execute(HttpClientHandler httpClientHandler)
        {
            var httpClient = new HttpClient(httpClientHandler);
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            httpClient.DefaultRequestHeaders.UserAgent.Add(UserAgentHeader.Custom);

            return httpClient;
        }
    }
}
