using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class PortalData
    {
        public string id;
        public string name;
        public string targetPortalId;
        public int zoneId;

        public PortalData(string id, string name, int zoneId)
        {
            this.id = id;
            this.name = name;
            this.zoneId = zoneId;
        }
    }
}
