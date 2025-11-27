using System.Collections.Generic;

namespace API {
    [System.Serializable]
    public class WorldRequest {
        public WorldData data;
        public string file_name;

        public WorldRequest(WorldData data, string worldName) {
            this.data = data;
            this.file_name = $"{worldName}.world";   // backend expects `<name>.world`
        }
    }

    [System.Serializable]
    public class WorldData {
        public string worldName;
        public List<PlacedAsset> objectPlacementData;
    }

    [System.Serializable]
    public class PlacedAsset {
        public Position Position;
        public int AssetDataId;
    }

    [System.Serializable]
    public class Position {
        public float x, y, z;
    }

    [System.Serializable]
    public class WorldCreateResponse {
        public string id;
        public string user_id;
        public string name;
        public string data;
        public string created_at;
        public string updated_at;
    }

}