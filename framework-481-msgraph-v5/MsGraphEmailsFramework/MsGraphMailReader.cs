using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Graph.Users.Item.Messages.Item.Move;
using MsGraphEmailsFramework.Common;
using MsGraphEmailsFramework.Reading;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MsGraphEmailsFramework
{
    internal class MsGraphMailReader : MsGraphMailHandler
    {
        private List<Mailbox> _mailFolders;

        private const string InboxName = "Inbox";
        private const string ProcessedName = "Processed";

        private string _inboxId = null;
        private string _processedId = null;

        private string requestId = "99999";

        // get Processed folder
        // get inbox -> get mails in inbox
        //    -> subject needs to be "AC Request Response"
        //       -> split body by \r \n -> look for line with RELID -> check if line contains requestId
        //          -> send success email
        //          -> move email to Processed
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

                await SetupMailFolders();
                SetupInbox();
                SetupProcessed();

                var inboxMessages = await GetInboxMessages();

                string emailId = null;

                foreach (var inboxMessage in inboxMessages)
                {
                    var relevantLine = inboxMessage.Body.Content
                        .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                        .ToList()
                        .SingleOrDefault(x => x.ContainsIgnoringCase("RELID"));

                    if (string.IsNullOrWhiteSpace(relevantLine) || !relevantLine.Contains(requestId))
                    {
                        continue;
                    }

                    emailId = inboxMessage.Id;
                }

                if(string.IsNullOrWhiteSpace(emailId))
                {
                    return;
                }

                var movePostRequestBody = new MovePostRequestBody
                {
                    DestinationId = _processedId,
                };

                var moveMessageRequestInformation = GraphServiceClient
                    .Users[MailConfiguration.Email.Sender]
                    .Messages[emailId]
                    .Move
                    .ToPostRequestInformation(movePostRequestBody);

                var results = await GetJson(moveMessageRequestInformation).ConfigureAwait(false);





                //-------------------
                // MOVE MESSAGES
                /*

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

        private async Task SetupMailFolders()
        {
            var getMailFoldersRequestInformation = GraphServiceClient
                .Users[MailConfiguration.Email.Sender]
                .MailFolders
                .ToGetRequestInformation();

            var jsonMailFolders = await GetJson(getMailFoldersRequestInformation).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(jsonMailFolders))
            {
                Trace.TraceError($"Failed to retrieve MailFolders");
                return;
            }

            var mailFolders = JsonConvert.DeserializeObject<MailFoldersResult>(jsonMailFolders);

            if (mailFolders == null)
            {
                Trace.TraceError($"Could not deserialize [{jsonMailFolders}]");
                return;
            }

            _mailFolders = mailFolders.MailFolders;
        }

        private void SetupInbox()
        {
            var inbox = GetMailFolder(InboxName);

            if (inbox == null)
            {
                Trace.TraceError($"Could not get MailFolder [{InboxName}]");
                return;
            }

            _inboxId = inbox.Id;
        }

        private async Task SetupProcessed()
        {
            var inboxChildFolders = await GetInboxChildFolders();

            var processed = inboxChildFolders?.SingleOrDefault(x => x.DisplayName.ContainsIgnoringCase(ProcessedName));

            if (processed == null)
            {
                Trace.TraceError($"Could not get MailFolder [{ProcessedName}]");
                return;
            }

            _processedId = processed.Id;
        }

        private async Task<List<Mailbox>> GetInboxChildFolders()
        {
            var getChildFoldersRequestInformation = GraphServiceClient
                .Users[MailConfiguration.Email.Sender]
                .MailFolders[_inboxId]
                .ChildFolders
                .ToGetRequestInformation();

            var jsonMailFolders = await GetJson(getChildFoldersRequestInformation).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(jsonMailFolders))
            {
                Trace.TraceError($"Failed to retrieve Inbox ChildFolders");
                return null;
            }

            var mailFolders = JsonConvert.DeserializeObject<MailFoldersResult>(jsonMailFolders);

            if (mailFolders == null)
            {
                Trace.TraceError($"Could not deserialize [{jsonMailFolders}]");
                return null;
            }

            return mailFolders.MailFolders;
        }

        private Mailbox GetMailFolder(string folderName)
        {
            return _mailFolders?.SingleOrDefault(x => x.DisplayName.ContainsIgnoringCase(folderName));
        }

        private async Task<List<MailFolderMessage>> GetInboxMessages()
        {
            var getInboxMessagesRequestInformation = GraphServiceClient
                .Users[MailConfiguration.Email.Sender]
                .MailFolders[_inboxId]
                .Messages
                .ToGetRequestInformation(x =>
                {
                    x.QueryParameters.Top = 50;
                    x.QueryParameters.Filter = "contains(subject,'AC Request Response')";
                });

            var jsonInboxMessages = await GetJson(getInboxMessagesRequestInformation).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(jsonInboxMessages))
            {
                Trace.TraceError($"Failed to retrieve Inbox Messages");
                return null;
            }

            var inboxMessages = JsonConvert.DeserializeObject<MessagesResult>(jsonInboxMessages);

            if (inboxMessages == null)
            {
                Trace.TraceError($"Could not deserialize [{jsonInboxMessages}]");
                return null;
            }

            return inboxMessages.Messages;
        }
    }
}
