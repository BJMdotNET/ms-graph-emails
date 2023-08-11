using System.Diagnostics;

namespace MsGraphEmailsCore
{
    public class TestMailSender : MailSender
    {
        public TestMailSender(MailConfiguration mailConfiguration) : base(mailConfiguration)
        {
        }

        public async Task Execute()
        {
            Trace.TraceInformation("-------------------------------------------------------------------------------------------------------------");
            Trace.TraceInformation("-------------------------------------------------------------------------------------------------------------");
            Trace.TraceInformation("-------------------------------------------------------------------------------------------------------------");

            var subject = $"MS Graph Test {DateTime.Now.ToDdMmYyyyDashHhMm()}";

            var body = $"<html><body><h3>MS Graph Test {DateTime.Now.ToDdMmYyyyDashHhMm()}</h3></body></html>";

            await Send(subject, body);
        }
    }
}
