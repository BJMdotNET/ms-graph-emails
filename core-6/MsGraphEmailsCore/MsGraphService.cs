using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace MsGraphEmailsCore
{
    internal abstract class MsGraphService
    {
        private static readonly string[] _scopes = new[] { "https://graph.microsoft.com/.default" };

        protected GraphServiceClient GraphServiceClient;

        private DateTime? _validUntil;

        protected readonly MailConfiguration MailConfiguration;

        protected MsGraphService(MailConfiguration mailConfiguration)
        {
            MailConfiguration = mailConfiguration;
        }

        protected bool GraphServiceClientToBeInitiated()
        {
            return GraphServiceClient == null || !_validUntil.HasValue || DateTime.UtcNow > _validUntil;
        }
        
        protected async Task SetupGraphServiceClient()
        {
            Trace.TraceInformation($"{GetType().Name} -> SetupGraphClient");

            var authority = $"https://login.microsoftonline.com/{MailConfiguration.TenantId}/oauth2/v2.0/token";

            Trace.TraceInformation($"ClientId: {MailConfiguration.ClientId}");
            Trace.TraceInformation($"TenantId: {MailConfiguration.TenantId}");
            Trace.TraceInformation($"Authority: {authority}");

            var builder = ConfidentialClientApplicationBuilder
                .Create(MailConfiguration.ClientId);

            if (MailConfiguration.UseProxy)
            {
                builder = builder.WithHttpClientFactory(new StaticClientWithProxyFactory());
            }
            else
            {
                builder = builder.WithHttpClientFactory(new StaticClientWithoutProxyFactory());
            }

            builder = builder.WithClientSecret(MailConfiguration.Secret)
                .WithAuthority(new Uri(authority))
                ;

            var app = builder.Build();

            var authenticationResult = await app.AcquireTokenForClient(_scopes).ExecuteAsync();

            Trace.TraceInformation($"Authentication result: {authenticationResult.AccessToken} expires on {authenticationResult.ExpiresOn}");

            // Create GraphClient and attach auth header to all request (acquired on previous step)
            _validUntil = DateTime.UtcNow.AddMinutes(59);
            
            GraphServiceClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(requestMessage =>
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                    return Task.FromResult(0);
                }));

            //GraphClient = GetGraphServiceClient();
        }

        //private GraphServiceClient GetGraphServiceClient()
        //{
        //    var httpClientHandler = HttpClientHandlerRetriever.Execute(MailConfiguration.MsGraph.UseProxy);

        //    var httpProvider = new HttpProvider(httpClientHandler, true);

        //    return new GraphServiceClient(GetClientCredentialProvider(), httpProvider);
        //}

        //private IAuthenticationProvider GetClientCredentialProvider()
        //{
        //    var authority = $"https://login.microsoftonline.com/{MailConfiguration.MsGraph.TenantId}/v2.0";

        //    var builder = ConfidentialClientApplicationBuilder
        //        .Create(MailConfiguration.MsGraph.ClientId)
        //        .WithAuthority(authority)
        //        .WithClientSecret(MailConfiguration.MsGraph.Secret);

        //    if (MailConfiguration.MsGraph.UseProxy)
        //    {
        //        builder.WithHttpClientFactory(new StaticClientWithProxyFactory());
        //    }

        //    var confidentialClientApplication = builder.Build();

        //    return new DelegateAuthenticationProvider(
        //        async requestMessage =>
        //        {
        //            // Retrieve an access token for Microsoft Graph (gets a fresh token if needed).
        //            var authenticationResult = await confidentialClientApplication
        //                .AcquireTokenForClient(_scopes)
        //                .ExecuteAsync();

        //            // Add the access token in the Authorization header of the API request.
        //            requestMessage.Headers.Authorization =
        //                new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
        //        });
        //}
    }
}
