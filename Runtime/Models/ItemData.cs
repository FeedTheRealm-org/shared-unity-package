using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class ItemData
    {
        public string id;
        public string name = "Item";
        public string description = ""; // TODO: this is not necesary and will be removed later
        public string spriteFilepath = "";

        public ItemData(string id, string name, string description, string spriteFilepath)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.spriteFilepath = spriteFilepath;
        }
    }
}
