using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class CosmeticCategoryEntry
    {
        public string sprite_path = "";

        public string url_id = "";

        public float price = 1f;

        public CosmeticCategoryEntry() { }

        public CosmeticCategoryEntry(string spritePath, string urlId = "", float price = 1f)
        {
            this.sprite_path = spritePath ?? "";
            this.url_id = urlId ?? "";
            this.price = price;
        }
    }
}
