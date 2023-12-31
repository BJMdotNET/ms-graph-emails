﻿using Microsoft.Identity.Client;
using System.Net.Http;

namespace MsGraphEmailsFramework.Network
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
