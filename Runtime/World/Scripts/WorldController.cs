using UnityEngine;

namespace World {

    [ExecuteAlways] // Runs in edit mode too
    public class WorldController : MonoBehaviour {

        [Header("World Configuration")]
        [SerializeField, Min(0.1f)]
        private float cellSize = 1f;

        [SerializeField, Min(1)]
        private int worldSize = 10;

        [Space(10)]
        [Header("World Components")]
        [SerializeField]
        private GameObject worldPlane;

        [SerializeField]
        private Grid grid;

        [Space(10)]
        [Header("Visual Settings")]
        [SerializeField]
        private Material placementGridMaterial;

        [SerializeField]
        private bool showGridVisualization = false;

        [Space(15)]
        [Header("World Info (Read Only)")]
        [SerializeField, HideInInspector] private float totalWorldSize;
        [SerializeField, HideInInspector] private int totalCells;

        private void OnValidate() {
            UpdateWorld();
            UpdateGridVisualization();
        }

        private void UpdateWorld() {
            if (grid == null || worldPlane == null)
                return;

            // Set the cell size for the Unity Grid
            grid.cellSize = new Vector3(cellSize, 0, cellSize);

            // Scale the world plane to match the grid
            // Default Unity plane is 10x10 units, so scale accordingly
            float scale = worldSize * cellSize / 10f;
            worldPlane.transform.localScale = new Vector3(scale, 1, scale);

            // Optional: position the world plane so its center aligns with the grid center
            worldPlane.transform.position = new Vector3(
                (worldSize * cellSize) / 2f - cellSize / 2f,
                worldPlane.transform.position.y,
                worldSize * cellSize / 2f - cellSize / 2f
            );
        }

        private void UpdateGridVisualization() {
            ToggleGridVisualization(showGridVisualization);
        }

        public Vector3Int GetSelectedPosition(Vector3 position) {
            return grid.WorldToCell(position);
        }

        public Vector3 GetCellCenterPosition(Vector3Int gridPosition) {
            return grid.GetCellCenterWorld(gridPosition);
        }

        public void PlaceObjectAt(Vector3Int gridPosition, GameObject obj) {
            Vector3 pos = grid.GetCellCenterWorld(gridPosition);
            obj.transform.position = pos;
        }

        public void RemoveObject(GameObject obj) {
            DestroyImmediate(obj, true);
        }

        public Vector3 GetCellPosition(Vector3Int gridPosition) {
            return grid.GetCellCenterWorld(gridPosition);
        }

        public void ToggleGridVisualization(bool isVisible) {
            if (placementGridMaterial != null) {
                placementGridMaterial.SetFloat("_Show", isVisible ? 1f : 0f);
            }
        }

        [ContextMenu("Reset World to Default")]
        private void ResetToDefault() {
            cellSize = 1f;
            worldSize = 10;
            showGridVisualization = false;
            UpdateWorld();
        }
    }
}