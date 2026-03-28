using System;
using System.Collections.Generic;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class DialogData
    {
        public string id;

        public string name = "Dialog";

        public List<MessageData> messages = new();

        public DialogData(string id, string name)
        {
            this.id = id;
            this.name = name;
            this.messages = new List<MessageData>();
        }
    }
}
