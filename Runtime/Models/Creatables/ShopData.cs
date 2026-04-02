using System;
using System.Collections.Generic;
using Enums;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class ProductData
    {
        public string productId;
        public int price;
        public CurrencyType currency = CurrencyType.Gold;

        public ProductData(string productId, int price, CurrencyType currency = CurrencyType.Gold)
        {
            this.productId = productId;
            this.price = price;
            this.currency = currency;
        }
    }

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
