using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models {
  [Serializable]
  public class EnemySpawnAreaData {
    [SerializeField]
    private Vector3 position;

    [SerializeField]
    private int maxEnemies = 3;

    [SerializeField]
    private float spawnRate = 2f;

    [SerializeField]
    private int resetAfterKills = 6;

    [SerializeField]
    private float resetDelay = 10f;

    EnemySpawnAreaData(Vector3 position, int maxEnemies, float spawnRate, int resetAfterKills, float resetDelay) {
      this.position = position;
      this.maxEnemies = maxEnemies;
      this.spawnRate = spawnRate;
      this.resetAfterKills = resetAfterKills;
      this.resetDelay = resetDelay;
    }
  }
}
