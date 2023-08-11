using Microsoft.Extensions.Configuration;

namespace MsGraphEmailsCore
{
    public class MailConfiguration
    {
        public string Sender { get; }
        public string Destination { get; }

        public string ClientId { get; }
        public string TenantId { get; }
        public string Secret { get; }
        public string ProxyAddress { get; }
        public bool UseProxy { get; }

        public MailConfiguration()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            Sender = config.GetSection("Email").GetSection("Sender").Value;
            Destination = config.GetSection("Email").GetSection("Destination").Value;

            ClientId = config.GetSection("MSGraph").GetSection("ClientId").Value;
            TenantId = config.GetSection("MSGraph").GetSection("TenantId").Value;
            Secret = config.GetSection("MSGraph").GetSection("Secret").Value;
            ProxyAddress = config.GetSection("MSGraph").GetSection("ProxyAddress").Value;
            // ReSharper disable once AssignNullToNotNullAttribute
            UseProxy = bool.Parse(config.GetSection("MSGraph").GetSection("UseProxy").Value);
        }
    }
}
