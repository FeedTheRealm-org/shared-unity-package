using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models {
  [Serializable]
  public class EnemySpawnAreaData {
    [SerializeField]
    public Vector3 Position;

    public int Size = 1;

    [SerializeField]
    public int MaxEnemies = 3;

    [SerializeField]
    public float SpawnRate = 2f;

    [SerializeField]
    public int ResetAfterKills = 6;

    [SerializeField]
    public float ResetDelay = 10f;

    [SerializeField]
    public EnemyData EnemyForSpawn;

    public EnemySpawnAreaData(Vector3 position, int size, int maxEnemies = 3, float spawnRate = 2f, int resetAfterKills = 6, float resetDelay = 10f, EnemyData enemyForSpawn = null) {
      Position = position;
      Size = size;
      MaxEnemies = maxEnemies;
      SpawnRate = spawnRate;
      ResetAfterKills = resetAfterKills;
      ResetDelay = resetDelay;
      EnemyForSpawn = enemyForSpawn;
    }
  }
}
