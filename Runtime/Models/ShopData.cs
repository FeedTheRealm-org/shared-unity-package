using System;
using System.Collections.Generic;
using Enums;

namespace Models
{
    [Serializable]
    public class ProductData
    {
        public ItemData itemData;
        public int price;
        public CurrencyType currency = CurrencyType.Gold;

        public ProductData(ItemData itemData, int price, CurrencyType currency = CurrencyType.Gold)
        {
            this.itemData = itemData;
            this.price = price;
            this.currency = currency;
        }
    }

    [Serializable]
    public class ShopData
    {
        public string id;
        public string displayName;
        public List<ProductData> products = new();

        public override string ToString()
        {
            return $"ShopData: {products.Count} products available.";
        }
    }

    [Serializable]
    public class WorldShopsData
    {
        public List<ShopData> shops = new();
    }
}
