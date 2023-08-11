using Microsoft.Graph;
using System.Diagnostics;

namespace MsGraphEmailsCore
{
    internal class MsGraphMailService : MsGraphService
    {
        public MsGraphMailService(MailConfiguration mailConfiguration) : base(mailConfiguration)
        {
        }

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

                Trace.TraceInformation($"Sender: {MailConfiguration.Sender}");

                var message = new Message
                {
                    From = StringToGraphRecipient(MailConfiguration.Sender),
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

                await GraphServiceClient.Users[MailConfiguration.Sender]
                    .SendMail(message, SaveToSentItems: true)
                    .Request()
                    .PostAsync()
                    .ConfigureAwait(false)
                    ;
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
