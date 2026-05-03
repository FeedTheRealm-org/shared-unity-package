using System;
using Enums;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class ProductData
    {
        public string productId;
        public int price;
        public CurrencyType currency = CurrencyType.Gold;
        public string displayName;
        public string categoryName;
        public bool IsCosmetic => !string.IsNullOrEmpty(categoryName);

        public ProductData(
            string productId,
            int price,
            CurrencyType currency = CurrencyType.Gold,
            string displayName = "",
            string categoryName = ""
        )
        {
            this.productId = productId;
            this.price = price;
            this.currency = currency;
            this.displayName = displayName;
            this.categoryName = categoryName;
        }
    }
}
