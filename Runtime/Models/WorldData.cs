using System;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class WorldData
    {
        public string worldId = "";
        public string _worldName = "New_FTR_World";
        public string worldName
        {
            get => _worldName;
            set => _worldName = value.Replace(" ", "_");
        }
        public int startingZone = 1;
        public string description = "";
        public string created_by = "";
        public DateTime created_at = DateTime.Now;
        public DateTime last_edited_at = DateTime.Now;
        public DateTime published_at;

        public override string ToString() =>
            $"WorldData(worldId: {worldId}, worldName: {worldName}, startingZone: {startingZone})";
    }
}
