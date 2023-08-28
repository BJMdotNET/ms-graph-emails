using Newtonsoft.Json;
using System.Collections.Generic;

namespace MsGraphEmailsFramework.Reading
{
    internal class MessagesResult
    {
        [JsonProperty(PropertyName = "value")]
        public List<MailFolderMessage> Messages { get; set; }
    }

    public class MailFolderMessage
    {
        //public string odataetag { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        //public DateTime? createdDateTime { get; set; }
        //public DateTime? lastModifiedDateTime { get; set; }
        [JsonProperty(PropertyName = "changeKey")]
        public string ChangeKey { get; set; }
        //public object[] categories { get; set; }
        //public DateTime? receivedDateTime { get; set; }
        //public DateTime? sentDateTime { get; set; }
        //public bool? hasAttachments { get; set; }
        //public string internetMessageId { get; set; }
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }
        //public string bodyPreview { get; set; }
        //public string importance { get; set; }
        //public string parentFolderId { get; set; }
        //public string conversationId { get; set; }
        //public string conversationIndex { get; set; }
        //public bool? isDeliveryReceiptRequested { get; set; }
        //public bool? isReadReceiptRequested { get; set; }
        //public bool? isRead { get; set; }
        //public bool? isDraft { get; set; }
        //public string webLink { get; set; }
        //public string inferenceClassification { get; set; }
        [JsonProperty(PropertyName = "body")]
        public Body Body { get; set; }
        //public Sender sender { get; set; }
        //public From from { get; set; }
        //public Torecipient[] toRecipients { get; set; }
        //public object[] ccRecipients { get; set; }
        //public object[] bccRecipients { get; set; }
        //public object[] replyTo { get; set; }
        //public Flag flag { get; set; }
    }

    public class Body
    {
        //public string contentType { get; set; }
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
    }

    //public class Sender
    //{
    //    public Emailaddress emailAddress { get; set; }
    //}

    //public class Emailaddress
    //{
    //    public string name { get; set; }
    //    public string address { get; set; }
    //}

    //public class From
    //{
    //    public Emailaddress1 emailAddress { get; set; }
    //}

    //public class Emailaddress1
    //{
    //    public string name { get; set; }
    //    public string address { get; set; }
    //}

    //public class Flag
    //{
    //    public string flagStatus { get; set; }
    //}

    //public class Torecipient
    //{
    //    public Emailaddress2 emailAddress { get; set; }
    //}

    //public class Emailaddress2
    //{
    //    public string name { get; set; }
    //    public string address { get; set; }
    //}
}
