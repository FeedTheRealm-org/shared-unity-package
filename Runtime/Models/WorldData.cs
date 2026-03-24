using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class WorldData
    {
        public string publishId = "";

        // The world id is now a unique identifier for the world,
        // and is used as the directory name for the world's data.
        // It is generated when saving if it doesn't exist, and is never changed after that.
        // This allows us to avoid having to update the world id in all zones when the world name changes.
        public string worldId = "";
        public string worldName = "New_FTR_World";
        public int startingZone = 1;
        public string description = "";
        public string created_by = "";
        public DateTime created_at;
        public DateTime last_edited_at;
        public DateTime published_at;
    }
}
