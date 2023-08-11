using System.Diagnostics;
using System.Reflection;

namespace MsGraphEmailsCore
{
    public abstract class MailSender
    {
        private readonly List<string> _destinationAddresses;
        private readonly MailConfiguration _mailConfiguration;

        protected MailSender(MailConfiguration mailConfiguration)
        {
            _mailConfiguration = mailConfiguration;
            _destinationAddresses = new List<string> { _mailConfiguration.Destination };
        }

        protected async Task Send(string subject, string body)
        {
            Trace.TraceInformation($"{GetType().Name} -> {MethodBase.GetCurrentMethod()?.Name}");

            if (!_destinationAddresses.Any())
            {
                Trace.TraceError("No DestinationAddresses passed");
                return;
            }

            try
            {
                var mailMessage = SetupMailMessage(subject, body);

                await new MsGraphMailService(_mailConfiguration).SendMail(mailMessage).ConfigureAwait(false);

                //Task.Run(() => new MsGraphMailService().SendMail(mailMessage)).ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                var exceptionMessage = ExceptionMessageRetriever.Execute(exc);

                Trace.TraceError($"MailSender: Error while sending mail: {exceptionMessage}");
                Trace.TraceError($"MailSender: Error while sending mail: {exc}");

                throw;
            }
        }

        private MailMessage SetupMailMessage(string subject, string body)
        {
            Trace.TraceInformation($"{GetType().Name} -> {MethodBase.GetCurrentMethod()?.Name}");

            return new MailMessage
            {
                ToRecipients = _destinationAddresses,
                Subject = subject,
                Body = body,
            };
        }
    }
}