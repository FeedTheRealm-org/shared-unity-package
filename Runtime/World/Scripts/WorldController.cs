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
        private bool showGridVisualization = false;

        // cached resolved material (shared, don't instantiate)
        private Material resolvedGridMaterial;

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
            // ensure runtime starts with visualization off
            showGridVisualization = false;
            ToggleGridVisualization(false);
        }

        private void UpdateWorld() {
            if (grid == null || worldPlane == null)
                return;

            // keep behavior: grid cell size relative to cellSize & gridSize
            grid.cellSize = new Vector3(cellSize / gridSize, 0, cellSize / gridSize);
            worldPlane.transform.localScale = new Vector3(gridSize, 1, gridSize);

            // refresh material reference (in case prefab/materials changed)
            resolvedGridMaterial = ResolveGridMaterial();
            UpdateGridMaterialProperties();
        }


        private Material ResolveGridMaterial() {

            if (worldPlane == null) return null;

            if (!worldPlane.TryGetComponent<Renderer>(out var renderer)) return null;

            var mats = renderer.sharedMaterials;
            // prefab uses material at element 1 per requirement
            if (mats != null && mats.Length > 1 && mats[1] != null) return mats[1];

            // fallback to first material if index 1 not present
            if (mats != null && mats.Length > 0) return mats[0];

            return null;
        }

        private void UpdateGridMaterialProperties() {
            var mat = resolvedGridMaterial ?? ResolveGridMaterial();
            if (mat == null) {
                // no material available — nothing to update
                return;
            }

            // size vector: how many cells per world unit (keep previous convention)
            Vector2 sizeVector = new Vector2(1f / cellSize, 1f / cellSize);

            // try several common property names (shader dependent)
            if (mat.HasProperty("_Size")) {
                mat.SetVector("_Size", sizeVector);
            } else if (mat.HasProperty("_GridSize")) {
                mat.SetVector("_GridSize", sizeVector);
            } else if (mat.HasProperty("_Tiling")) {
                mat.SetVector("_Tiling", sizeVector);
            } else {
                Debug.LogWarning($"Grid material on '{name}' doesn't expose expected size property (_Size/_GridSize/_Tiling).");
            }

            // ensure visibility property respects current flag if shader supports it
            if (mat.HasProperty("_Show")) {
                mat.SetFloat("_Show", showGridVisualization ? 1f : 0f);
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
            showGridVisualization = isVisible;
            var mat = resolvedGridMaterial ?? ResolveGridMaterial();
            if (mat != null) {
                if (mat.HasProperty("_Show")) {
                    mat.SetFloat("_Show", isVisible ? 1f : 0f);
                } else {
                    Debug.LogWarning("Grid material does not have a '_Show' property.");
                }
            }
        }


    }
}