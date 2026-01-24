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
        public List<ProductData> products = new List<ProductData>();

        public override string ToString()
        {
            return $"ShopData: {products.Count} products available.";
        }
    }
}
