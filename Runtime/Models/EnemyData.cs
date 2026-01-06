using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class EnemyData
    {
        [SerializeField]
        public string id;

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

        // Attack range of the enemy
        public int range;

        // Optional sprite identifier/path
        [SerializeField]
        public string spriteId;

        // Loot table that this enemy can drop
        [SerializeField]
        public LootTableData lootTable;

        public EnemyData(
            string id,
            string name,
            string description,
            int healthPoints,
            int damage,
            int speed,
            int range,
            string spriteId,
            LootTableData lootTable
        )
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.healthPoints = healthPoints;
            this.damage = damage;
            this.speed = speed;
            this.range = range;
            this.spriteId = spriteId;
            this.lootTable = lootTable;
        }
    }
}
