using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;

namespace MsGraphEmailsFramework
{
    internal abstract class MsGraphService
    {
        protected GraphServiceClient GraphServiceClient;

        protected bool GraphServiceClientToBeInitiated()
            => GraphServiceClient == null;

        protected void SetupGraphServiceClient()
        {
            GraphServiceClient = new GraphServiceClient(
                new BaseBearerTokenAuthenticationProvider(
                    new TokenProvider()));
        }
    }
}
