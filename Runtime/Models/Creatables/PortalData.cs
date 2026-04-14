using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class PortalData
    {
        public string id;
        public string name;
        public int zoneId;
        public string targetPortalId;

        // Here we save the name of the target portal and the position to teleport to,
        // so we don't have to do an extra lookup when accepting the portal request.
        public string targetPortalName;
        public Vector3 targetPosition;

        public PortalData(string id, string name, int zoneId)
        {
            this.id = id;
            this.name = name;
            this.zoneId = zoneId;
        }
    }
}
