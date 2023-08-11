using System.Net;
using System.Security.Authentication;

namespace MsGraphEmailsCore
{
    public static class HttpClientHandlerRetriever
    {
        public static HttpClientHandler Execute(bool useProxy = true, bool ignoreSslValidation = false)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            if (useProxy)
            {
                httpClientHandler.UseProxy = true;
                httpClientHandler.Proxy = GetWebProxy();
                httpClientHandler.Credentials = CredentialCache.DefaultCredentials;
            }

            if (ignoreSslValidation)
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                httpClientHandler.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
            }

            return httpClientHandler;
        }

        private static WebProxy GetWebProxy()
        {
            return new WebProxy(new MailConfiguration().ProxyAddress)
            {
                Credentials = CredentialCache.DefaultCredentials
            };
        }
    }
}
