using System;
using System.Configuration;

namespace MsGraphEmailsFramework.Common
{
    public static class MailConfiguration
    {
        public static class Email
        {
            public static readonly string Sender = ConfigurationManager.AppSettings["Email.Sender"];

            public static readonly string Destination = ConfigurationManager.AppSettings["Email.Destination"];
        }

        public static class MsGraph
        {
            private static readonly Lazy<bool> _lazyUseProxy = new Lazy<bool>(() => ConfigurationHelper.GetBool("MSGraph.UseProxy"));

            public static readonly string ClientId = ConfigurationManager.AppSettings["MSGraph.ClientId"];
            public static readonly string TenantId = ConfigurationManager.AppSettings["MSGraph.TenantId"];
            public static readonly string Secret = ConfigurationManager.AppSettings["MSGraph.Secret"];
            public static readonly string ProxyAddress = ConfigurationManager.AppSettings["MSGraph.ProxyAddress"];
            public static readonly bool UseProxy = _lazyUseProxy.Value;
        }
    }
}
