using System.Collections.Generic;

namespace API {
    [System.Serializable]
    public class WorldRequest {
        public Models.WorldData data;
        public string file_name;

        public WorldRequest(Models.WorldData data) {
            this.data = data;
            file_name = $"{data.worldName}.world";
        }
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


    [System.Serializable]
    public class WorldListResponse {
        public List<Models.WorldData> worlds;
        public int amount;
        public int limit;
        public int offset;
    }

}