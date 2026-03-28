using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class MessageData
    {
        public string id = "";
        public string sender = "";
        public string content = "";

        public MessageData(string id, string sender, string content)
        {
            this.id = id;
            this.sender = sender;
            this.content = content;
        }
    }
}
