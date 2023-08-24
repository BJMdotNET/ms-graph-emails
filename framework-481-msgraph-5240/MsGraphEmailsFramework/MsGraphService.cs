using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;

namespace MsGraphEmailsFramework
{
    internal abstract class MsGraphService
    {
        //protected readonly ILogger Logger;

        protected GraphServiceClient GraphServiceClient;

        protected MsGraphService()//, ILogger logger)
        {
            //Logger = logger;
        }

        protected bool GraphServiceClientToBeInitiated()
            => GraphServiceClient == null;

        protected void SetupGraphServiceClient()
        {
            GraphServiceClient = new GraphServiceClient(
                new BaseBearerTokenAuthenticationProvider(
                    new TokenProvider()));
        }

        //private static readonly string[] _scopes = new[] { "https://graph.microsoft.com/.default" };

        //protected GraphServiceClient GraphServiceClient;

        //private DateTime? _validUntil;

        //protected bool GraphServiceClientToBeInitiated()
        //{
        //    return GraphServiceClient == null || !_validUntil.HasValue || DateTime.UtcNow > _validUntil;
        //}

        //protected async Task SetupGraphServiceClient()
        //{
        //    Trace.TraceInformation($"{GetType().Name} -> SetupGraphClient");

        //    var authority = $"https://login.microsoftonline.com/{MailConfiguration.MsGraph.TenantId}/oauth2/v2.0/token";

        //    Trace.TraceInformation($"ClientId: {MailConfiguration.MsGraph.ClientId}");
        //    Trace.TraceInformation($"TenantId: {MailConfiguration.MsGraph.TenantId}");
        //    Trace.TraceInformation($"Authority: {authority}");

        //    var builder = ConfidentialClientApplicationBuilder
        //            .Create(MailConfiguration.MsGraph.ClientId)
        //        ;

        //    builder = MailConfiguration.MsGraph.UseProxy
        //            ? builder.WithHttpClientFactory(new StaticClientWithProxyFactory())
        //            : builder.WithHttpClientFactory(new StaticClientWithoutProxyFactory())
        //        ;

        //    builder = builder.WithClientSecret(MailConfiguration.MsGraph.Secret)
        //            .WithAuthority(new Uri(authority))
        //        ;

        //    var app = builder.Build();

        //    var authenticationResult = await app.AcquireTokenForClient(_scopes).ExecuteAsync();

        //    Trace.TraceInformation($"Authentication result: {authenticationResult.AccessToken} expires on {authenticationResult.ExpiresOn}");

        //    // Create GraphClient and attach auth header to all request (acquired on previous step)
        //    _validUntil = DateTime.UtcNow.AddMinutes(59);

        //    //var httpProvider = new HttpProvider(HttpClientHandlerRetriever.Execute(MailConfiguration.MsGraph.UseProxy, true), true);

        //    //var httpClient = MailConfiguration.MsGraph.UseProxy
        //    //    ? new StaticClientWithProxyFactory().GetHttpClient()
        //    //    : new StaticClientWithoutProxyFactory().GetHttpClient();

        //    //GraphServiceClient = new GraphServiceClient(httpClient)
        //    //{
        //    //    AuthenticationProvider = new DelegateAuthenticationProvider(requestMessage =>
        //    //    {
        //    //        requestMessage.Headers.Authorization =
        //    //            new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

        //    //        return Task.FromResult(0);
        //    //    })
        //    //};

        //    GraphServiceClient = new GraphServiceClient(
        //        new DelegateAuthenticationProvider(requestMessage =>
        //        {
        //            requestMessage.Headers.Authorization =
        //                new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

        //            return Task.FromResult(0);
        //        })
        //        //, httpProvider
        //    );

        //    //GraphClient = GetGraphServiceClient();
        //}

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
