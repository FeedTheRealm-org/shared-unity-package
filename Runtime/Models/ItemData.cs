using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class ItemData
    {
        public string id;
        public string name = "Item";
        public string description = ""; // TODO: this is not necesary and will be removed later
        public string spriteFilePath = "";

        public ItemData(string id, string name, string description, string spriteFilePath)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.spriteFilePath = spriteFilePath;
        }
    }
}
