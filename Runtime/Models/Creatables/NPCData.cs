using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class NPCData : ISerializationCallbackReceiver
    {
        public string id = "";
        public string name = "";
        public string description = "";

        public List<NPCDialogData> dialogProgression = new List<NPCDialogData>();

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

        public NPCData(
            string id,
            string name,
            string description,
            List<NPCDialogData> dialogProgression,
            Dictionary<string, string> category_sprites
        )
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.dialogProgression =
                dialogProgression != null
                    ? new List<NPCDialogData>(dialogProgression)
                    : new List<NPCDialogData>();
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
