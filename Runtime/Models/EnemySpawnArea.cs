using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
  [Serializable]
  public class EnemySpawnAreaData
  {
    public Vector3 Position;
    public float Radius = 3f;
    public int MaxEnemies = 3;
    public float SpawnRate = 2f;
    public int ResetAfterKills = 6;
    public float ResetDelay = 10f;

    public EnemySpawnAreaData(Vector3 position, float radius)
    {
      Position = position;
      Radius = radius;
    }
  }
}
