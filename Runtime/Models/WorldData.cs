using System;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class WorldData
    {
        public string worldId = "";

        // The world id is now a unique identifier for the world,
        // and is used as the directory name for the world's data.
        // It is generated when saving if it doesn't exist, and is never changed after that.
        // This allows us to avoid having to update the world id in all zones when the world name changes.
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
