using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class CosmeticCategoryEntry
    {
        public string sprite_path = "";

        public string url_id = "";

        public int price = 1;

        public CosmeticCategoryEntry() { }

        public CosmeticCategoryEntry(string spritePath, string urlId = "", int price = 1)
        {
            this.sprite_path = spritePath ?? "";
            this.url_id = urlId ?? "";
            this.price = price;
        }
    }
}
