using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Graph.Users.Item.SendMail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Models.ODataErrors;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using MsGraphEmailsFramework.Common;
using Microsoft.Kiota.Abstractions;
using Newtonsoft.Json;
using MsGraphEmailsFramework.Reading;
using MailFolder = MsGraphEmailsFramework.Reading.MailFolder;

namespace MsGraphEmailsFramework
{
    internal class MsGraphMailReader : MsGraphMailHandler
    {
        public async Task Execute()
        {
            Trace.TraceInformation($"{GetType().Name} -> Execute");

            try
            {
                if (GraphServiceClientToBeInitiated())
                {
                    Trace.TraceInformation("Calling SetupGraphClient");

                    SetupGraphServiceClient();
                }

                var inbox = await GetInbox().ConfigureAwait(false);

                if (inbox == null)
                {
                    Trace.TraceError($"Could not get Inbox ID");
                    return;
                }

                var getInboxMessagesRequestInformation = GraphServiceClient
                    .Users[MailConfiguration.Email.Sender]
                    .MailFolders[inbox.Id]
                    .Messages
                    .ToGetRequestInformation();

                var jsonInboxMessages = await GetJson(getInboxMessagesRequestInformation).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(jsonInboxMessages))
                {
                    Trace.TraceError($"Failed to retrieve Inbox Messages");
                    return ;
                }





                //var messages = await GraphServiceClient
                //    .Users[MailConfiguration.Email.Sender]
                //    .MailFolders[]
                //    // .MailFolders["inbox"]
                //    .Messages
                //    .Request()
                //    // .Filter($"startswith(subject, '{subject}') and receivedDateTime gt {dt}")
                //    // .Top(20)
                //    .GetAsync();

                //List<QueryOption> options = new List<QueryOption>
                //{
                //    new QueryOption("$filter", "startswith(displayName,'the specific user's mail')")
                //};
                //var users = await graphClient.Users.Request(options).GetAsync();

                //foreach (var message in messages)
                //{

                //}

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
            catch (ODataError odataError)
            {
                if (odataError.Error != null)
                {
                    Trace.TraceError($"MsGraphMailService: odataError.Error.Code = {odataError.Error.Code}");
                    Trace.TraceError($"MsGraphMailService: odataError.Error.Message = {odataError.Error.Message}");
                    Trace.TraceError($"MsGraphMailService: odataError.Error = {odataError.Error}");
                }

                throw;
            }
            catch (Exception exc)
            {
                var exceptionMessage = ExceptionMessageRetriever.Execute(exc);

                Trace.TraceError($"MsGraphMailService: Exception: {exceptionMessage}");
                Trace.TraceError($"MsGraphMailService: Exception: {exc}");

                throw;
            }
        }

        private async Task<MailFolder> GetInbox()
        {
            var getMailFoldersRequestInformation = GraphServiceClient
                .Users[MailConfiguration.Email.Sender]
                .MailFolders
                .ToGetRequestInformation();

            var jsonMailFolders = await GetJson(getMailFoldersRequestInformation).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(jsonMailFolders))
            {
                Trace.TraceError($"Failed to retrieve MailFolders");
                return null;
            }

            var mailFolders = JsonConvert.DeserializeObject<MailFoldersResult>(jsonMailFolders);

            if (mailFolders == null)
            {
                Trace.TraceError($"Could not deserialize [{jsonMailFolders}]");
                return null;
            }

            return mailFolders.MailFolders.SingleOrDefault(x => x.DisplayName.ContainsIgnoringCase("Inbox"));
        }
    }
}
