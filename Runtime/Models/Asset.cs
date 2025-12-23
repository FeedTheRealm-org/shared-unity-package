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
        [SerializeField] private Vector3 size = Vector3.one;
        [SerializeField] private string modelPath;
        [SerializeField] private string materialPath;
        [NonSerialized] private GameObject assetModel = null;
        [NonSerialized] private bool isModelLoaded = false;


        // This constructor is used in the World Creator
        public Asset(string id, string name, Vector2Int size, string modelPath, string materialPath) {
            this.id = id;
            this.name = name;
            this.size = new Vector3(size.x, 1, size.y);
            this.modelPath = modelPath;
            this.materialPath = materialPath;
        }

        // This constructor is used in the Client
        public Asset(string id, string name, Vector2Int size, GameObject assetModel) {
            this.id = id;
            this.name = name;
            this.size = new Vector3(size.x, 1, size.y);
            this.assetModel = assetModel;
            isModelLoaded = true;
            ApplyCollisions(assetModel);
        }

        // TODO: this needs to be refactored inmediately, we cant have multiple constructors like this
        public Asset(string id, string name, Vector3 size, GameObject assetModel) {
            this.id = id;
            this.name = name;
            this.size = size;
            this.assetModel = assetModel;
            isModelLoaded = true;
            ApplyCollisions(assetModel);
        }

        private void ApplyCollisions(GameObject gameObject) {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null) {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true;
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
                if (size.z == 0) size.z = 1;
                instance.transform.localScale = new Vector3(size.x, size.y, size.z);
                ApplyMaterial(instance);
                return instance;
            } catch (Exception e) {
                Debug.LogError($"Error instantiating model for asset {name}: {e}");
                return null;
            }
        }

        private void ApplyMaterial(GameObject modelInstance) {
            if (string.IsNullOrEmpty(materialPath)) {
                return;
            }
            try {
                string pathWithoutExtension = System.IO.Path.ChangeExtension(materialPath, null);
                Material material = Resources.Load<Material>(pathWithoutExtension);
                if (material == null) {
                    Debug.LogError($"Asset {name} | Material not found at path: {materialPath}");
                    return;
                }
                Renderer[] renderers = modelInstance.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers) {
                    renderer.material = material;
                }
            } catch (Exception e) {
                Debug.LogError($"Material could not be applied for asset [{name}]: {e}");
            }
        }

        public string Id => id;
        public string Name => name;
        public Vector3 Size => size;
        public string ModelPath => modelPath;
        public string MaterialPath => materialPath;

        public GameObject AssetModelInstance => InstantiateModel();

        // TODO: Check to remove this from ftr client
        public GameObject GetModelInstance() {
            GameObject assetModel = GetPrefab();
            GameObject instance = UnityEngine.Object.Instantiate(assetModel);
            instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            instance.transform.localScale = Vector3.one;
            return instance;
        }
    }
}