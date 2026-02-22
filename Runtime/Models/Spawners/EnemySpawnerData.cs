using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class EnemySpawnerData : SpawnerData
    {
        public int MaxEnemies = 3;
        public float SpawnRate = 2f;
        public int ResetAfterKills = 6;
        public float ResetDelay = 10f;

        public EnemySpawnerData(Vector3 position, float radius)
            : base(position, radius) { }
    }
}
