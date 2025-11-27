namespace API {
    [System.Serializable]
    public class WorldCreateResponse {
        public string worldId;
    }

    [System.Serializable]
    public class WorldData {
        public string worldName;
        public ObjectPlacementData[] objectPlacementData;
        public string file_name;
    }
}