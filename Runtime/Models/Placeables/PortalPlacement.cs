using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class PortalPlacementData
    {
        // references PortalData.id in creatables
        public string id;
        public string name = "Portal";
        public float radius = 3f;
        public Vector3 position = Vector3.zero;
    }
}
