using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class ItemData
    {
        public string id;
        public string name = "Item";
        public string description = "";
        public string spriteFilePath = "";
        public int maxStack = 1;

        public ItemData(string id, string name, string description, string spriteFilePath)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.spriteFilePath = spriteFilePath;
        }
    }
}
