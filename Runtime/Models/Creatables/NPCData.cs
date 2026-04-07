using System;
using System.Collections.Generic;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class NPCData
    {
        public string id = "";
        public string name = "";
        public string description = "";
        public NPCDialogData npcDialog = null;
        public Dictionary<string, string> category_sprites;

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
            this.category_sprites = category_sprites;
        }
    }
}
