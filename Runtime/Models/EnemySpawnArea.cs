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

    public EnemySpawnAreaData(Vector3 position, int size) {
      Position = position;
      Size = size;
    }
  }
}
