using UnityEngine;

namespace World {

    [ExecuteAlways]
    public class WorldController : MonoBehaviour {

        [Header("World Configuration")]

        [SerializeField, Min(0.1f)]
        private float cellSize = 1f;

        [SerializeField, Min(1)]
        private int gridSize = 5;

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

        private void OnValidate() {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                // Only update in edit mode, avoid runtime issues
                UnityEditor.EditorApplication.delayCall += () => {
                    if (this != null) {
                        UpdateWorld();
                        ToggleGridVisualization(showGridVisualization);
                    }
                };
            }
#endif
        }

        private void Start() {
            UpdateWorld();
            showGridVisualization = false;
            ToggleGridVisualization(showGridVisualization);
        }

        private void UpdateWorld() {
            if (grid == null || worldPlane == null)
                return;
            grid.cellSize = new Vector3(cellSize / gridSize, 0, cellSize / gridSize);
            worldPlane.transform.localScale = new Vector3(gridSize, 1, gridSize);
            UpdateGridMaterialProperties();
        }


        private void UpdateGridMaterialProperties() {
            if (placementGridMaterial == null) return;
            Vector2 sizeVector = new(1 / cellSize, 1 / cellSize);
            if (placementGridMaterial.HasProperty("_Size")) {
                placementGridMaterial.SetVector("_Size", sizeVector);
            } else {
                Debug.LogWarning("Grid material doesn't have _Size property");
            }
        }

        [ContextMenu("Reset World to Default")]
        private void ResetToDefault() {
            cellSize = 1f;
            gridSize = 10;
            showGridVisualization = false;
            UpdateWorld();
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


    }
}