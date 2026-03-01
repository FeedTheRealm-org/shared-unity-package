using System;

namespace Models
{
    [Serializable]
    public class NPCData
    {
        /// <summary>
        /// Unique identifier for this NPC within the game data set.
        /// </summary>
        public string id = "";
        public string name = "";
        public string description = "";

        /// <summary>
        /// Path or key used by the loading system to resolve this NPC's sprite
        /// (for example, a Resources path, Addressables key, or relative path,
        /// depending on the project's asset loading strategy).
        /// </summary>
        public string spriteFilepath = "";
        public NPCDialogData npcDialog = null;

        public NPCData(
            string id,
            string name,
            string description,
            string spriteFilePath,
            NPCDialogData npcDialog
        )
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.spriteFilepath = spriteFilePath;
            this.npcDialog = npcDialog;
        }
    }
}
