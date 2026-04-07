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
        public NPCDialogData npcDialog = null;
        public Dictionary<string, string> category_sprites = new();

        [SerializeField]
        private List<StringDictionaryEntry> category_sprites_serialized = new();

        public NPCData(
            string id,
            string name,
            string description,
            NPCDialogData npcDialog,
            Dictionary<string, string> category_sprites
        )
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.npcDialog = npcDialog;
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
