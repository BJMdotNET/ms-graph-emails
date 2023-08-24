using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MsGraphEmailsFramework
{
    internal sealed class TokenProvider : IAccessTokenProvider
    {
        private readonly string[] _scopes = new[] { "https://graph.microsoft.com/.default" };
        private readonly string _authority = $"https://login.microsoftonline.com/{MailConfiguration.MsGraph.TenantId}/oauth2/v2.0/token";

        public Task<string> GetAuthorizationTokenAsync(
            Uri uri,
            Dictionary<string, object> additionalAuthenticationContext = default,
            CancellationToken cancellationToken = default)
        {
            var token = GetToken().GetAwaiter().GetResult();
            return Task.FromResult(token);
        }

        public AllowedHostsValidator AllowedHostsValidator => throw new NotImplementedException();

        private async Task<string> GetToken()
        {
            Trace.TraceInformation($"UseProxy: {MailConfiguration.MsGraph.UseProxy}");
            Trace.TraceInformation($"TenantId: {MailConfiguration.MsGraph.TenantId}");
            Trace.TraceInformation($"ClientId: {MailConfiguration.MsGraph.ClientId}");
            Trace.TraceInformation($"Secret: {MailConfiguration.MsGraph.Secret.First()}...{MailConfiguration.MsGraph.Secret.Last()}");
            Trace.TraceInformation($"Authority: {_authority}");

            var builder = ConfidentialClientApplicationBuilder
                .Create(MailConfiguration.MsGraph.ClientId)
                .WithAuthority(new Uri(_authority))
                .WithClientSecret(MailConfiguration.MsGraph.Secret);

            builder = MailConfiguration.MsGraph.UseProxy
                    ? builder.WithHttpClientFactory(new StaticClientWithProxyFactory())
                    : builder.WithHttpClientFactory(new StaticClientWithoutProxyFactory())
                ;

            var app = builder.Build();

            var authenticationResult = await app.AcquireTokenForClient(_scopes).ExecuteAsync();

            Trace.TraceInformation($"Authentication result: {authenticationResult.AccessToken} expires on {authenticationResult.ExpiresOn}");

            return authenticationResult.AccessToken;
        }
    }
}
