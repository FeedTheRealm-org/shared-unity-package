using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class CosmeticData : ISerializationCallbackReceiver
    {
        public string id = "";
        public string name = "";
        public string description = "";

        public Dictionary<string, CosmeticCategoryEntry> categories = new();

        [Serializable]
        private class CategoryEntryPair
        {
            public string key = "";
            public CosmeticCategoryEntry value = new();
        }

        [SerializeField]
        private List<CategoryEntryPair> categories_serialized = new();

        public CosmeticData(
            string id,
            string name,
            string description,
            Dictionary<string, string> category_sprites
        )
        {
            this.id = id;
            this.name = name;
            this.description = description;

            categories = new Dictionary<string, CosmeticCategoryEntry>();
            if (category_sprites != null)
            {
                foreach (var kvp in category_sprites)
                    categories[kvp.Key] = new CosmeticCategoryEntry(kvp.Value);
            }
        }

        public void OnBeforeSerialize()
        {
            categories_serialized = new List<CategoryEntryPair>(categories.Count);
            foreach (var kvp in categories)
            {
                if (kvp.Key == "EarringL")
                    continue;

                categories_serialized.Add(
                    new CategoryEntryPair
                    {
                        key = kvp.Key,
                        value = kvp.Value ?? new CosmeticCategoryEntry(),
                    }
                );
            }
        }

        public void OnAfterDeserialize()
        {
            categories = new Dictionary<string, CosmeticCategoryEntry>(
                categories_serialized?.Count ?? 0
            );
            if (categories_serialized == null)
                return;
            foreach (var pair in categories_serialized)
            {
                if (pair?.key != null)
                    categories[pair.key] = pair.value ?? new CosmeticCategoryEntry();
            }

            if (categories.ContainsKey("EarringR") && !categories.ContainsKey("EarringL"))
            {
                categories["EarringL"] = new CosmeticCategoryEntry(
                    categories["EarringR"].sprite_path,
                    categories["EarringR"].url_id,
                    categories["EarringR"].price
                );
            }
        }

        public string GetSpritePath(string categoryName) =>
            categories.TryGetValue(categoryName, out var e) ? e.sprite_path : "";

        public string GetUrlId(string categoryName) =>
            categories.TryGetValue(categoryName, out var e) ? e.url_id : "";
    }
}
