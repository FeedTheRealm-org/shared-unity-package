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

        public string EnemyId = "";

        public EnemySpawnerData(Vector3 position, float radius, string enemyId = "")
            : base(position, radius)
        {
            EnemyId = enemyId;
        }
    }
}
