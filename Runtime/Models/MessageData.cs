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
        private string _sender = "";

        [SerializeField]
        private string _content = "";

        public MessageData(string id, string sender, string content)
        {
            this.id = id;
            _sender = sender;
            _content = content;
        }

        public string Sender
        {
            get => _sender;
            set => _sender = value;
        }

        public string Content
        {
            get => _content;
        }
    }
}
