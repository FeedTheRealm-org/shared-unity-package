using System.Collections.Generic;
using FTRShared.Runtime.Models;

namespace API
{
    [System.Serializable]
    public class WorldRequest
    {
        public string file_name;
        public string description;
        public WorldData data;
    }

    [System.Serializable]
    public class CreatablesRequest
    {
        public CreatablesData createable_data;
    }

    [System.Serializable]
    public class CreatablesResponse
    {
        public string id;
        public string user_id;
        public string name;
        public string data;
        public string createable_data;
        public string created_at;
        public string updated_at;
    }

    [System.Serializable]
    public class WorldCreateResponse
    {
        public string id;
        public string user_id;
        public string name;
        public string data;
        public string description;
        public string created_at;
        public string updated_at;
    }

    [System.Serializable]
    public class WorldListItemResponse
    {
        public string id;
        public string user_id;
        public string name;
        public string data;
        public string description;
        public string created_at;
        public string updated_at;
    }

    [System.Serializable]
    public class WorldListResponse
    {
        public List<WorldListItemResponse> worlds;
        public int amount;
        public int limit;
        public int offset;
    }
}
