using System;
using System.Collections.Generic;
using FTRShared.Runtime.Models;

namespace API
{
    [Serializable]
    public class ZoneRequest
    {
        public string worldId;
        public int zoneId;
        public ZoneData data;
    }

    [Serializable]
    public class ZoneResponse
    {
        public string world_id;
        public int zone_id;
        public string zone_data;
    }

    [Serializable]
    public class ZonesListResponse
    {
        public string world_id;
        public List<int> zones;
    }

    [Serializable]
    public class ZoneAddressResponse
    {
        public string ip;
        public int port;
    }
}
