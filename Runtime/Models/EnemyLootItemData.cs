using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class EnemyLootItemData
    {
        // Name of the item dropped
        [SerializeField]
        public string id;

        // Max Amount of this item dropped
        [SerializeField]
        public float maxAmount;

        // Drop probability [0-100]
        [SerializeField]
        public int dropChance;

        public EnemyLootItemData(string id, float maxAmount, int dropChance)
        {
            this.id = id;
            this.maxAmount = maxAmount;
            this.dropChance = dropChance;
        }
    }
}
