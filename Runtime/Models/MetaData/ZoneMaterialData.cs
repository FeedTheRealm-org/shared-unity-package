using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    /// <summary>
    /// This is a meta data class to hold all material-related info for a zone,
    /// For the world editor, we identify the current material by an id.
    /// In the game, there can only be one floor material per zone, so retrieval is via a get zone ground material that requires only the world and zone id.
    /// </summary>
    [Serializable]
    public class ZoneAreaData
    {
        public string zoneMaterialId;
        public float textureGranularity = 100.0f;
    }
}
