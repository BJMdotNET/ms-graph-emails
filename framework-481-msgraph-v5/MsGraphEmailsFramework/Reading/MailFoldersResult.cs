using System.Collections.Generic;
using Newtonsoft.Json;

namespace MsGraphEmailsFramework.Reading
{
    internal class MailFoldersResult
    {
        [JsonProperty(PropertyName = "value")]
        public List<Mailbox> MailFolders { get; set; }
    }

    public class Mailbox
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        //[JsonProperty(PropertyName = "parentFolderId")]
        //public string ParentFolderId { get; set; }

        //[JsonProperty(PropertyName = "childFolderCount")]
        //public int ChildFolderCount { get; set; }

        //[JsonProperty(PropertyName = "unreadItemCount")]
        //public int UnreadItemCount { get; set; }

        //[JsonProperty(PropertyName = "totalItemCount")]
        //public int TotalItemCount { get; set; }

        //[JsonProperty(PropertyName = "sizeInBytes")]
        //public int SizeInBytes { get; set; }

        //[JsonProperty(PropertyName = "isHidden")]
        //public bool IsHidden { get; set; }
    }
}
