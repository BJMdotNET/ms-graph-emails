using Microsoft.Graph;
using System.Diagnostics;

namespace MsGraphEmailsCore
{
    internal class MsGraphMailReader : MsGraphService
    {
        public MsGraphMailReader(MailConfiguration mailConfiguration) : base(mailConfiguration)
        {
        }

        public async Task Execute(string sender)
        {
            Trace.TraceInformation($"{GetType().Name} -> SendMail");

            try
            {
                if (GraphServiceClientToBeInitiated())
                {
                    Trace.TraceInformation("Calling SetupGraphClient");

                    await SetupGraphServiceClient().ConfigureAwait(false);
                }

                var messages = await GraphServiceClient.Users[sender]
                    // .MailFolders["inbox"]
                    .Messages
                    .Request()
                    // .Filter($"startswith(subject, '{subject}') and receivedDateTime gt {dt}")
                    // .Top(20)
                    .GetAsync();

                //List<QueryOption> options = new List<QueryOption>
                //{
                //    new QueryOption("$filter", "startswith(displayName,'the specific user's mail')")
                //};
                //var users = await graphClient.Users.Request(options).GetAsync();

                foreach (var message in messages)
                {

                }

                //var credentials = new ClientSecretCredential(
                //    config["GraphMail:TenantId"],
                //    config["GraphMail:ClientId"],
                //    config["GraphMail:ClientSecret"],
                //    new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });

                //// Define a new Microsoft Graph service client.            
                //GraphServiceClient _graphServiceClient = new GraphServiceClient(credentials);

                //// Get the e-mails for a specific user.
                //var messages = _graphServiceClient.Users["user@e-mail.com"].Messages.Request().GetAsync().Result;

                //-------------------
                // MOVE MESSAGES
                /*
                 IUserMailFoldersCollectionPage allMailFolders = await graphClient.Users[inbox].MailFolders.Request().GetAsync();
        foreach(MailFolder folder in allMailFolders)
        {
            if (folder.DisplayName == "01 Processed")
            {
                processedFolder = folder.Id;
            }
            if (folder.DisplayName == "02 Needs Attention")
            {
                needsAttentionFolder = folder.Id;
            }
        }

    if (needsAttention)
                {
                    needsAttentionCount++;
                    Message movedMsg = await graphClient.Users[inbox].Messages[message.Id].Move(needsAttentionFolder).Request().PostAsync(); 
                }
               else
                {
                     Message movedMsg = await graphClient.Users[inbox].Messages[message.Id].Move(processedFolder).Request().PostAsync();
                }
                 */
            }
            catch (ServiceException exc)
            {
                Trace.TraceError($"MsGraphMailService: ServiceException while sending mail: {nameof(exc.StatusCode)} = [{exc.StatusCode}]");
                Trace.TraceError($"MsGraphMailService: ServiceException while sending mail: {nameof(exc.Error)} = [{exc.Error}]");
                Trace.TraceError($"MsGraphMailService: ServiceException while sending mail: {nameof(exc.Message)} = [{exc.Message}]");
                Trace.TraceError($"MsGraphMailService: ServiceException while sending mail: {nameof(exc.RawResponseBody)} = [{exc.RawResponseBody}]");
                Trace.TraceError($"MsGraphMailService: ServiceException while sending mail: {exc}");

                throw;
            }
            catch (Exception exc)
            {
                var exceptionMessage = ExceptionMessageRetriever.Execute(exc);

                Trace.TraceError($"MsGraphMailService: Exception while sending mail: {exceptionMessage}");
                Trace.TraceError($"MsGraphMailService: Exception while sending mail: {exc}");

                throw;
            }
        }
    }
}
