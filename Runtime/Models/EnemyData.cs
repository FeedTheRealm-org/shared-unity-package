using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class EnemyData
    {
        [SerializeField]
        public string name;

        [SerializeField]
        public string description;

        // Max health points of the enemy
        [SerializeField]
        public int healthPoints;

        // Damage dealt by the enemy
        [SerializeField]
        public int damage;

        // Movement speed of the enemy
        public int speed;

        // Can the enemy move or is it stationary
        public bool canMove;

        // Attack range of the enemy
        public int range;

        // Optional sprite identifier/path
        [SerializeField]
        public string spriteId;

        // Loot items that this enemy can drop
        [SerializeField]
        public List<EnemyLootItem> lootItems = new List<EnemyLootItem>();

        // Gold dropped by this enemy
        [SerializeField]
        public float goldAmount;

        public EnemyData(
            string name,
            string description,
            int healthPoints,
            int damage,
            int speed,
            bool canMove,
            int range,
            string spriteId,
            List<EnemyLootItem> lootItems,
            float goldAmount
        )
        {
            this.name = name;
            this.description = description;
            this.healthPoints = healthPoints;
            this.damage = damage;
            this.speed = speed;
            this.canMove = canMove;
            this.range = range;
            this.spriteId = spriteId;
            this.lootItems = lootItems ?? new List<EnemyLootItem>();
            this.goldAmount = goldAmount;
        }
    }
}
