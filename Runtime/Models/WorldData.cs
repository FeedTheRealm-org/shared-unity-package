using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models {

    [Serializable]
    public class WorldData {
        [SerializeField]
        public string id; // TODO: check if this breaks when publishing world
        [SerializeField]
        public string worldName = "New World";
        [SerializeField]
        public List<PlacedAsset> objectPlacementData;
    }
}