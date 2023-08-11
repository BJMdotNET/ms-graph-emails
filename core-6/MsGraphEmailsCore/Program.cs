namespace MsGraphEmailsCore
{
    internal static class Program
    {
        private static async Task Main()
        {
            await new TestMailSender(new MailConfiguration()).Execute(); //.GetAwaiter().GetResult();
        }
    }
}