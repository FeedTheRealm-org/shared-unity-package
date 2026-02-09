using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class NPCData
    {
        public string id = "";
        public string name = "";
        public string description = "";
        public string spriteFilepath = "";

        public NPCData(string id, string name, string description, string spriteFilepath)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.spriteFilepath = spriteFilepath;
        }
    }
}
