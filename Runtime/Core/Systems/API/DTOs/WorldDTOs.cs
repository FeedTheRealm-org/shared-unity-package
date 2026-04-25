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
    public class WorldZoneMetadata
    {
        public int zone_id;
        public bool is_active;
    }

    [Serializable]
    public class WorldMetadata
    {
        public string id;
        public string user_id;
        public string name;
        public string description;
        public List<WorldZoneMetadata> zones;
        public string created_at;
        public string updated_at;
    }

    [Serializable]
    public class WorldListResponse
    {
        public List<WorldMetadata> worlds;
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
        public string description;
        public string data;
        public string createable_data;
        public List<WorldZoneMetadata> zones;
        public string created_at;
        public string updated_at;
    }

    [Serializable]
    public class WorldAddressResponse
    {
        public string ip;
        public int port;
    }

    [Serializable]
    public class WorldZonesResponse
    {
        public string world_id;
        public List<WorldZoneMetadata> zones;
    }

    [Serializable]
    public class WorldZoneResponse
    {
        public string world_id;
        public int zone_id;
        public string zone_data;
        public bool is_active;
    }

    [Serializable]
    public class ActiveWorldData
    {
        public WorldData worldData;
        public ZoneAddressResponse zoneAddress;
    }
}
