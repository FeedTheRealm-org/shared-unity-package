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
        public float price = 0f;
        public Dictionary<string, string> category_sprites = new();
        public Dictionary<string, string> category_urls = new();
        public Dictionary<string, float> category_prices = new();

        [SerializeField]
        private List<StringFloatDictionaryEntry> category_prices_serialized = new();

        [SerializeField]
        private List<StringDictionaryEntry> category_sprites_serialized = new();

        [SerializeField]
        private List<StringDictionaryEntry> category_urls_serialized = new();

        public CosmeticData(
            string id,
            string name,
            string description,
            float price,
            Dictionary<string, string> category_sprites
        )
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.price = price;
            this.category_sprites =
                category_sprites != null
                    ? new Dictionary<string, string>(category_sprites)
                    : new Dictionary<string, string>();
        }

        public void OnBeforeSerialize()
        {
            category_sprites_serialized = StringDictionarySerialization.ToEntries(category_sprites);
            category_urls_serialized = StringDictionarySerialization.ToEntries(category_urls);
            category_prices_serialized = StringFloatDictionarySerialization.ToEntries(
                category_prices
            );
        }

        public void OnAfterDeserialize()
        {
            category_sprites = StringDictionarySerialization.ToDictionary(
                category_sprites_serialized
            );
            category_urls = StringDictionarySerialization.ToDictionary(category_urls_serialized);
            category_prices = StringFloatDictionarySerialization.ToDictionary(
                category_prices_serialized
            );
        }
    }
}
