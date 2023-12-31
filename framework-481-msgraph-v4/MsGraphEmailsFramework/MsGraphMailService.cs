﻿using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MsGraphEmailsFramework
{
    internal class MsGraphMailService : MsGraphService
    {
        public async Task SendMail(MailMessage mailMessage)
        {
            Trace.TraceInformation($"{GetType().Name} -> SendMail");

            try
            {
                if (GraphServiceClientToBeInitiated())
                {
                    Trace.TraceInformation("Calling SetupGraphClient");

                    await SetupGraphServiceClient().ConfigureAwait(false);
                }

                Trace.TraceInformation($"Sender: {MailConfiguration.Email.Sender}");

                var message = new Message
                {
                    From = StringToGraphRecipient(MailConfiguration.Email.Sender),
                    ToRecipients = StringsToGraphRecipients(mailMessage.ToRecipients),
                    //CcRecipients = StringsToGraphRecipients(mailMessage.CcRecipients),
                    //BccRecipients = StringsToGraphRecipients(mailMessage.BccRecipients),
                    Subject = mailMessage.Subject,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = mailMessage.Body
                    },
                };

                await GraphServiceClient.Users[MailConfiguration.Email.Sender]
                        .SendMail(message, SaveToSentItems: true)
                        .Request()
                        .PostAsync()
                        .ConfigureAwait(false)
                    ;
            }
            catch (ServiceException exc)
            {
                Trace.TraceError($"MsGraphMailService: ServiceException: {nameof(exc.StatusCode)} = [{exc.StatusCode}]");
                Trace.TraceError($"MsGraphMailService: ServiceException: {nameof(exc.Message)} = [{exc.Message}]");
                Trace.TraceError($"MsGraphMailService: ServiceException: {nameof(exc.Error)} = [{exc.Error}]");
                Trace.TraceError($"MsGraphMailService: ServiceException: {nameof(exc.RawResponseBody)} = [{exc.RawResponseBody}]");
                Trace.TraceError($"MsGraphMailService: ServiceException: {exc}");

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

        private IEnumerable<Recipient> StringsToGraphRecipients(IEnumerable<string> emailAddresses)
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
