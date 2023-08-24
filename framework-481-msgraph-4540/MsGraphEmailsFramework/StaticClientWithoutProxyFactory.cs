using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MsGraphEmailsFramework
{
    internal sealed class StaticClientWithoutProxyFactory : IMsalHttpClientFactory
    {
        private static readonly HttpClient _httpClient;

        static StaticClientWithoutProxyFactory()
        {
            var httpClientHandler = HttpClientHandlerRetriever.Execute(false, true);

            _httpClient = new HttpClient(httpClientHandler);
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CustomUserAgent", "1.0"));
        }

        public HttpClient GetHttpClient()
        {
            return _httpClient;
        }
    }
}
