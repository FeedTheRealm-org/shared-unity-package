using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class DialogData
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string name = "Item";

        [SerializeField]
        public string npc;

        public DialogData(string id, string name, string npc)
        {
            this.id = id;
            this.name = name;
            this.npc = npc;
        }
    }
}
