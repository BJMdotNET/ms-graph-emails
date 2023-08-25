using System.Collections.Generic;

namespace MsGraphEmailsFramework
{
    internal class MailMessage
    {
        public List<string> ToRecipients { get; set; }

        //public List<string> CcRecipients { get; set; }

        //public List<string> BccRecipients { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
