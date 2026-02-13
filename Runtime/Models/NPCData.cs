using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class NPCData
    {
        public string id = "";
        public string name = "";
        public string description = "";
        public string spriteFilepath = "";
        public NPCDialogData npcDialog = null;

        public NPCData(
            string id,
            string name,
            string description,
            string spriteFilepath,
            NPCDialogData npcDialog
        )
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.spriteFilepath = spriteFilepath;
            this.npcDialog = npcDialog;
        }
    }
}
