using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class ChestData
    {
        public string id;
        public string name = "Chest";
        public string lootTableId;
        public Vector3 size = Vector3.one;
        public Vector3 rotation = Vector3.zero;
        public Vector3 position = Vector3.zero;
        public int chestCooldownMinutes = 1;
        public ChestModelData opendedChestModelData;
        public ChestModelData closedChestModelData;
    }

    [Serializable]
    public class ChestModelData
    {
        public string modelId;
        public bool isDefault;
        public Vector3 relativePosition = Vector3.zero;
        public Vector3 relativeRotation = Vector3.zero;
        public Vector3 relativeSize = Vector3.one;
    }
}
