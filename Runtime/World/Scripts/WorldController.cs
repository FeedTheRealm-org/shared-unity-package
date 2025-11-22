using UnityEngine;

namespace World {

    [ExecuteAlways] // Runs in edit mode too
    public class WorldController : MonoBehaviour {

        [Header("World Configuration")]
        [SerializeField, Min(0.1f)]
        [Tooltip("Size of each grid cell in world units")]
        private float cellSize = 1f;

        [SerializeField, Min(1)]
        [Tooltip("Number of grid cells (creates gridSize x gridSize world)")]
        private int worldSize = 10;

        [Space(10)]
        [Header("World Components")]
        [SerializeField]
        [Tooltip("The visual ground plane that represents the world")]
        private GameObject worldPlane;

        [SerializeField]
        [Tooltip("Unity Grid component for object snapping")]
        private Grid grid;

        [Space(10)]
        [Header("Visual Settings")]
        [SerializeField]
        [Tooltip("Material used for grid visualization overlay")]
        private Material placementGridMaterial;

        [SerializeField]
        [Tooltip("Show/Hide grid visualization in Scene view")]
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
                worldSize * cellSize / 2f - cellSize / 2f,
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

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(WorldController))]
        public class WorldControllerEditor : UnityEditor.Editor {
            public override void OnInspectorGUI() {
                DrawDefaultInspector();

                WorldController controller = (WorldController)target;

                GUILayout.Space(10);

                UnityEditor.EditorGUILayout.HelpBox(
                    $"World Size: {controller.totalWorldSize:F1} x {controller.totalWorldSize:F1} units\n" +
                    $"Total Cells: {controller.totalCells}\n" +
                    $"Cell Size: {controller.cellSize:F1} units per cell",
                    UnityEditor.MessageType.Info
                );

                GUILayout.Label("Quick Actions", UnityEditor.EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Small World (50x50)")) {
                    controller.worldSize = 50;
                    controller.UpdateWorld();
                    UnityEditor.EditorUtility.SetDirty(controller);
                }
                if (GUILayout.Button("Medium World (100x100)")) {
                    controller.worldSize = 100;
                    controller.UpdateWorld();
                    UnityEditor.EditorUtility.SetDirty(controller);
                }
                if (GUILayout.Button("Large World (200x200)")) {
                    controller.worldSize = 200;
                    controller.UpdateWorld();
                    UnityEditor.EditorUtility.SetDirty(controller);
                }
                GUILayout.EndHorizontal();
            }
        }
#endif
    }
}