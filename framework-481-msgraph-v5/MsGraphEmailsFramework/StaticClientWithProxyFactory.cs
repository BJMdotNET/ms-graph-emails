using Microsoft.Identity.Client;
using MsGraphEmailsFramework.Common;
using System.Net.Http;

namespace MsGraphEmailsFramework
{
    internal sealed class StaticClientWithProxyFactory : IMsalHttpClientFactory
    {
        private static readonly HttpClient _httpClient;

        static StaticClientWithProxyFactory()
        {
            var httpClientHandler = HttpClientHandlerRetriever.Execute(true, true);

            _httpClient = HttpClientRetriever.Execute(httpClientHandler);
        }

        public HttpClient GetHttpClient()
        {
            return _httpClient;
        }
    }
}
