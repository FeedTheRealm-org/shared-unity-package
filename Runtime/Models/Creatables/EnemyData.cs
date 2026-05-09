using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class EnemyData : ISerializationCallbackReceiver
    {
        public string id = "";
        public string name = "";
        public string description = "";
        public int healthPoints = 0;
        public string weaponId = "";
        public string lootTableId = "";
        public Dictionary<string, string> category_sprites = new();

        [SerializeField]
        private List<StringDictionaryEntry> category_sprites_serialized = new();

        public EnemyData(
            string id,
            string name,
            string description,
            int healthPoints,
            string weaponId,
            string lootTableId,
            Dictionary<string, string> category_sprites
        )
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.healthPoints = healthPoints;
            this.weaponId = weaponId;
            this.lootTableId = lootTableId;
            this.category_sprites =
                category_sprites != null
                    ? new Dictionary<string, string>(category_sprites)
                    : new Dictionary<string, string>();
        }

        public void OnBeforeSerialize()
        {
            category_sprites_serialized = StringDictionarySerialization.ToEntries(category_sprites);
        }

        public void OnAfterDeserialize()
        {
            category_sprites = StringDictionarySerialization.ToDictionary(
                category_sprites_serialized
            );
        }
    }
}
