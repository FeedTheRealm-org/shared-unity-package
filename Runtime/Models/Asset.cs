using System;
using UnityEngine;

namespace Models {

    /// <summary>
    /// This is the Asset Data type used to store Assets in the Database
    /// Assets are considered all kind of placeable objects in the world editor
    /// </summary>
    [Serializable]
    public class Asset {
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private Vector2Int size = Vector2Int.one;
        [SerializeField] private string modelPath;
        [SerializeField] private string materialPath;
        [NonSerialized] private GameObject assetModel = null;
        [NonSerialized] private bool isModelLoaded = false;


        public Asset(string id, string name, Vector2Int size, string modelPath, string materialPath) {
            this.id = id;
            this.name = name;
            this.size = size;
            this.modelPath = modelPath;
            this.materialPath = materialPath;
        }

        public Asset(string id, string name, Vector2Int size, GameObject assetModel) {
            this.id = id;
            this.name = name;
            this.size = size;
            this.assetModel = assetModel;
            isModelLoaded = true;
            ApplyCollisions(assetModel);
        }

        private void ApplyCollisions(GameObject gameObject) {
            // Add Rigidbody if it doesn't exist
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null) {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true;

            // Ensure collider exists
            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            if (colliders.Length == 0) {
                gameObject.AddComponent<BoxCollider>();
            }
        }


        private void LoadModel() {
            try {
                string pathWithoutExtension = System.IO.Path.ChangeExtension(modelPath, null);
                GameObject model = Resources.Load<GameObject>(pathWithoutExtension);
                if (model == null) {
                    Debug.LogError($"Asset {name} | Model not found at path: {modelPath}");
                    return;
                }
                assetModel = model;
                isModelLoaded = true;
            } catch (Exception e) {
                Debug.LogError($"Model could not be loaded for asset [{name}]: {e}");
            }
        }

        private GameObject GetPrefab() {
            if (!isModelLoaded) {
                LoadModel();
            }
            return assetModel;
        }


        public GameObject InstantiateModel() {
            try {
                GameObject prefab = GetPrefab();
                if (prefab == null) {
                    Debug.LogError($"Cannot instantiate model for {name}: prefab is null");
                    return null;
                }

                GameObject instance = UnityEngine.Object.Instantiate(prefab);
                //instance.transform.localScale = new Vector3(size.x, size.y, size.x);

                if (materialPath == null || materialPath == "") {
                    return instance;
                }

                string pathWithoutExtension = System.IO.Path.ChangeExtension(materialPath, null);
                // Load material by path (not by object name inside)
                Material material = Resources.Load<Material>(pathWithoutExtension);
                if (material != null) {
                    Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();
                    if (renderers.Length > 0) {
                        foreach (Renderer renderer in renderers) {
                            Material matInstance = new Material(material);
                            renderer.material = matInstance;
                        }
                        Debug.Log($"Applied material to {name} | material path: {materialPath}");
                    } else {
                        Debug.LogWarning($"No Renderer found on {name} or its children");
                    }
                } else {
                    Debug.LogWarning($"Material not found for {name} at path: {materialPath}");
                }

                return instance;
            } catch (Exception e) {
                Debug.LogError($"Error instantiating model for asset {name}: {e}");
                return null;
            }
        }

        public string Id => id;
        public string Name => name;
        public Vector2Int Size => size;
        public string ModelPath => modelPath;
        public string MaterialPath => materialPath;

        // TODO: remove this and use InstantiateModel directly (refactor in both repos)
        public GameObject AssetModelInstance => InstantiateModel();

        public GameObject GetModelInstance() {
            GameObject assetModel = GetPrefab();
            GameObject instance = UnityEngine.Object.Instantiate(assetModel);
            instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            instance.transform.localScale = Vector3.one;
            return instance;
        }
    }
}