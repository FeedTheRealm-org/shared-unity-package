using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class EnemyData
    {
        public string id = "";
        public string name = "";
        public string description = "";
        public int healthPoints = 0;
        public int damage = 0;
        public int speed = 0;
        public int range = 0;
        public string spriteFilepath = "";

        // Loot table that this enemy can drop
        public LootTableData lootTable;

        public EnemyData(
            string id,
            string name,
            string description,
            int healthPoints,
            int damage,
            int speed,
            int range,
            string spriteFilepath,
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
            this.spriteFilepath = spriteFilepath;
            this.lootTable = lootTable;
        }
    }
}
