using MsGraphEmailsFramework.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MsGraphEmailsFramework.Sending
{
    public abstract class MailSender
    {
        private readonly List<string> _destinationAddresses;

        protected MailSender()
        {
            _destinationAddresses = new List<string> { MailConfiguration.Email.Destination };
        }

        protected void Send(string subject, string body)
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

                new MsGraphMailService().SendMail(mailMessage).GetAwaiter().GetResult();

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