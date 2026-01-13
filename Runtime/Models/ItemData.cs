using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class ItemData
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string name = "Item";

        [SerializeField]
        public string description = "";

        [SerializeField]
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
