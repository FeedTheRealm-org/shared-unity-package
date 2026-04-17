using System;
using System.Collections.Generic;
using FTRShared.Runtime.Models;

namespace API
{
    [Serializable]
    public class WorldRequest
    {
        public string file_name;
        public string description;
        public WorldData data;
    }

    [Serializable]
    public class CreatablesRequest
    {
        public CreatablesData createable_data;
    }

    [Serializable]
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

    [Serializable]
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

    [Serializable]
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

    [Serializable]
    public class WorldListResponse
    {
        public List<WorldListItemResponse> worlds;
        public int amount;
        public int limit;
        public int offset;
    }

    [Serializable]
    public class WorldDetailResponse
    {
        public string id;
        public string user_id;
        public string name;
        public string data;
        public string createable_data;
        public string created_at;
        public string updated_at;
    }

    [Serializable]
    public class ActiveWorldData
    {
        public WorldData worldData;
        public ZoneAddressResponse zoneAddress;
        public int zoneId;
    }
}
