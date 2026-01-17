using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class MessageData
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string sender = "";

        [SerializeField]
        public string content = "";

        public MessageData(string id, string sender, string content)
        {
            this.id = id;
            this.sender = sender;
            this.content = content;
        }
    }
}
