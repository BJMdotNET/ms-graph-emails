using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Graph.Users.Item.SendMail;
using MsGraphEmailsFramework.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MsGraphEmailsFramework.Sending
{
    internal class MsGraphMailService : MsGraphMailHandler
    {
        public async Task SendMail(MailMessage mailMessage)
        {
            //Trace.TraceInformation($"{GetType().Name} -> SendMail");

            try
            {
                if (GraphServiceClientToBeInitiated())
                {
                    Trace.TraceInformation("Calling SetupGraphClient");

                    SetupGraphServiceClient();
                }

                Trace.TraceInformation($"Sender: {MailConfiguration.Email.Sender}");

                var message = new Message
                {
                    From = StringToGraphRecipient(MailConfiguration.Email.Sender),
                    ToRecipients = StringsToGraphRecipients(mailMessage.ToRecipients) ?? new List<Recipient>(),
                    CcRecipients = StringsToGraphRecipients(mailMessage.CcRecipients) ?? new List<Recipient>(),
                    BccRecipients = StringsToGraphRecipients(mailMessage.BccRecipients) ?? new List<Recipient>(),
                    Subject = mailMessage.Subject,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = mailMessage.Body
                    },
                };

                var sendMailPostRequestBody = new SendMailPostRequestBody
                {
                    Message = message,
                    SaveToSentItems = true
                };

                var sendEmailRequestInformation = GraphServiceClient
                    .Users[MailConfiguration.Email.Sender]
                    .SendMail
                    .ToPostRequestInformation(sendMailPostRequestBody);

                var results = await GetJson(sendEmailRequestInformation).ConfigureAwait(false);

                //await GraphServiceClient.Users[MailConfiguration.Email.Sender]
                //    .SendMail
                //    .PostAsync(sendMailPostRequestBody)
                // .ConfigureAwait(false);
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

        private List<Recipient> StringsToGraphRecipients(IEnumerable<string> emailAddresses)
        {
            return emailAddresses?.Select(StringToGraphRecipient).ToList();
        }

        private Recipient StringToGraphRecipient(string emailAddress)
        {
            return string.IsNullOrWhiteSpace(emailAddress)
                ? null
                : new Recipient { EmailAddress = new EmailAddress { Address = emailAddress } };
        }
    }
}
