using System;
using UnityEngine;

namespace Models {
  [Serializable]
  public class PlayerSpawnAreaData {
    [SerializeField]
    public Vector3 Position;
    public float Radius;

    public PlayerSpawnAreaData(Vector3 position, float radius) {
      Position = position;
      Radius = radius;
    }
  }
}
