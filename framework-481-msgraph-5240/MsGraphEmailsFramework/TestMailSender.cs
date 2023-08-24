using System;
using System.Diagnostics;

namespace MsGraphEmailsFramework
{
    public class TestMailSender : MailSender
    {
        public void Execute()
        {
            Trace.TraceInformation("-------------------------------------------------------------------------------------------------------------");

            var subject = $"MS Graph Test {DateTime.Now.ToDdMmYyyyDashHhMm()}";

            var body = $"<html><body><h3>MS Graph Test {DateTime.Now.ToDdMmYyyyDashHhMm()}</h3></body></html>";

            Send(subject, body);

            Trace.TraceInformation("-------------------------------------------------------------------------------------------------------------");
        }
    }
}
