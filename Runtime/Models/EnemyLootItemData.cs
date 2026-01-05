using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class EnemyLootItemData
    {
        // Name of the item dropped
        [SerializeField]
        public string itemName;

        // Item sprite identifier/path
        [SerializeField]
        public string spriteId;

        // Max Amount of this item dropped
        [SerializeField]
        public float maxAmount;

        // Drop probability [0-100]
        [SerializeField]
        public int dropChance;

        public EnemyLootItemData(string itemName, string spriteId, float maxAmount, int dropChance)
        {
            this.itemName = itemName;
            this.spriteId = spriteId;
            this.maxAmount = maxAmount;
            this.dropChance = dropChance;
        }
    }
}
