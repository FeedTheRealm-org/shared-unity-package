using System;
using System.Collections.Generic;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class ShopData
    {
        public string id;
        public string shopName;
        public List<ProductData> products = new();

        public override string ToString()
        {
            return $"ShopData: {products.Count} products available.";
        }
    }
}
