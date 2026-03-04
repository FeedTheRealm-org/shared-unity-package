using System;
using System.Collections.Generic;

namespace Models
{
    [Serializable]
    public class ProductData
    {
        public ItemData itemData;
        public int price;

        public ProductData(ItemData itemData, int price)
        {
            this.itemData = itemData;
            this.price = price;
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
