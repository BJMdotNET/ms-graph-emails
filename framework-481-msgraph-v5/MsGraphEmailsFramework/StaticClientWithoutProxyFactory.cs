using Microsoft.Identity.Client;
using MsGraphEmailsFramework.Common;
using System.Net.Http;

namespace MsGraphEmailsFramework
{
    internal sealed class StaticClientWithoutProxyFactory : IMsalHttpClientFactory
    {
        private static readonly HttpClient _httpClient;

        static StaticClientWithoutProxyFactory()
        {
            var httpClientHandler = HttpClientHandlerRetriever.Execute(false, true);

            _httpClient = HttpClientRetriever.Execute(httpClientHandler);
        }

        public HttpClient GetHttpClient()
        {
            return _httpClient;
        }
    }
}
