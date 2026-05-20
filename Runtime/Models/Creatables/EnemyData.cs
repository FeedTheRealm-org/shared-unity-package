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
        public int healthPoints = 100;
        public string weaponId = "";
        public string lootTableId = "";
        public Dictionary<string, string> category_sprites = new();

        // Character color fields (HSV)
        public API.CharacterColorHsv skin_color = new API.CharacterColorHsv
        {
            h = 0f,
            s = 0f,
            v = 100f,
        };
        public API.CharacterColorHsv hair_color = new API.CharacterColorHsv
        {
            h = 0f,
            s = 0f,
            v = 100f,
        };
        public API.CharacterColorHsv eye_color = new API.CharacterColorHsv
        {
            h = 0f,
            s = 0f,
            v = 100f,
        };

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
