using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MsGraphEmailsFramework
{
    internal sealed class TokenProvider : IAccessTokenProvider
    {
        //private readonly MsGraphConfiguration _config;
        //private readonly ILogger _logger;

        //public TokenProvider(MsGraphConfiguration config)//, ILogger logger)
        //{
        //    _config = config;
        //}

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
            var authority = $"https://login.microsoftonline.com/{MailConfiguration.MsGraph.TenantId}/oauth2/v2.0/token";

            //_logger.LogInformation($"TenantId: {_config.TenantId}");
            //_logger.LogInformation($"Authority: {authority}");
            //_logger.LogInformation($"ClientId: {_config.ClientId}");
            //_logger.LogInformation($"Secret: {_config.Secret.First()}...{_config.Secret.Last()}");

            var builder = ConfidentialClientApplicationBuilder
                .Create(MailConfiguration.MsGraph.ClientId)
                .WithAuthority(new Uri(authority))
                .WithClientSecret(MailConfiguration.MsGraph.Secret);

            builder = MailConfiguration.MsGraph.UseProxy
                    ? builder.WithHttpClientFactory(new StaticClientWithProxyFactory())
                    : builder.WithHttpClientFactory(new StaticClientWithoutProxyFactory())
                ;

            var app = builder.Build();

            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var authenticationResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();

            //_logger.LogInformation($"Authentication result: {authenticationResult.AccessToken} expires on {authenticationResult.ExpiresOn}");

            return authenticationResult.AccessToken;
        }
    }
}
