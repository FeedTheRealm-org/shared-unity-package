namespace API {
    [System.Serializable]
    public class WorldRequest {
        public WorldDataWrapper data;
        public string file_name;
    }

    [System.Serializable]
    public class WorldDataWrapper {
        public string worldName;
        public ObjectPlacementData[] objectPlacementData;
    }

    [System.Serializable]
    public class ObjectPlacementData {
        public Position Position;
        public int AssetDataId;
    }

    [System.Serializable]
    public class Position {
        public float x;
        public float y;
        public float z;
    }
}