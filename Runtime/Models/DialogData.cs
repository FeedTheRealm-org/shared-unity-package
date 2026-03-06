using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class DialogData
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string name = "Dialog";

        [SerializeField]
        public string npc;

        [SerializeField]
        public List<MessageData> messages = new();

        public DialogData(string id, string name, string npc)
        {
            this.id = id;
            this.name = name;
            this.npc = npc;
            this.messages = new List<MessageData>();
        }
    }
}
