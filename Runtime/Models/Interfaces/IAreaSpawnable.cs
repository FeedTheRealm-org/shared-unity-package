using UnityEngine;

namespace Models {
    public interface IAreaSpawnable {
        Vector3 Position { get; set; }
        int MaxEnemies { get; set; }
        float SpawnRate { get; set; }
        int ResetAfterKills { get; set; }
        float ResetDelay { get; set; }
    }
}