using System;
using System.Collections.Generic;

namespace FTRShared.Runtime.Models
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
        public string lootTableId = "";
        public Dictionary<string, string> category_sprites;

        public EnemyData(
            string id,
            string name,
            string description,
            int healthPoints,
            int damage,
            int speed,
            int range,
            string lootTableId,
            Dictionary<string, string> category_sprites
        )
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.healthPoints = healthPoints;
            this.damage = damage;
            this.speed = speed;
            this.range = range;
            this.lootTableId = lootTableId;
            this.category_sprites = category_sprites;
        }
    }
}
