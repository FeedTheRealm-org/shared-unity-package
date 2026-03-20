using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class WorldData
    {
        public string publishId;
        public string worldDataDirectory;
        public string worldName = "New_FTR_World";
        public int startingZone = 1;
        public string description = "";
        public string created_by = "";
        public DateTime created_at;
        public DateTime last_edited_at;
        public DateTime published_at;
    }
}
