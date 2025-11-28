using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace API {
    [CreateAssetMenu(fileName = "ModelService", menuName = "Scriptable Objects/API/ModelService")]
    public class ModelService : ScriptableObject {
        [Header("Server settings")]
        [SerializeField] public string Hostname;
        [SerializeField] public int Port;

        [Header("General settings")]
        [SerializeField] private Logging.Logger logger;

        private string GetBaseUrl() => $"http://{Hostname}:{Port}/assets/models";

        /// <summary>
        /// Uploads asset model & material files for a world.
        /// </summary>
        public IEnumerator UploadAssets(List<Models.Asset> assets, string worldId, string accessToken, System.Action<string> callback) {
            if (assets == null || assets.Count == 0) {
                logger.Log("No assets to upload.", this, Logging.LogType.Warning);
                callback?.Invoke("No assets to upload.");
                yield break;
            }

            logger.Log($"Uploading {assets.Count} assets for world ID: {worldId}", this);

            var form = new WWWForm();

            // Add world_id
            form.AddField("world_id", worldId);

            // Add assets as multipart fields
            for (int i = 0; i < assets.Count; i++) {
                var asset = assets[i];
                string prefix = $"models[{i}]";

                form.AddField($"{prefix}.model_id", asset.Id);
                form.AddField($"{prefix}.name", asset.Name);
                form.AddField($"{prefix}.model_file", asset.ModelPath);
                form.AddField($"{prefix}.model_file", asset.ModelPath);
                form.AddField($"{prefix}.material_file", asset.MaterialPath);

                // TODO: refactor this to store in the unity persistent data path

                byte[] modelData = File.ReadAllBytes(Path.Combine(Application.dataPath, "Resources", asset.ModelPath));

                form.AddBinaryData(
                    $"{prefix}.model_file",
                    modelData,
                    Path.GetFileName(asset.ModelPath),
                    "application/octet-stream"
                );

                byte[] materialData = File.ReadAllBytes(Path.Combine(Application.dataPath, "Resources", asset.MaterialPath));
                form.AddBinaryData(
                    $"{prefix}.material_file",
                    materialData,
                    Path.GetFileName(asset.MaterialPath),
                    "application/octet-stream"
                );
            }

            UnityWebRequest uwr = UnityWebRequest.Post(GetBaseUrl(), form);
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success) {
                logger.Log("Assets uploaded successfully", this);
                callback?.Invoke(null);
            } else {
                logger.Log($"Asset upload error: {uwr.error}", this, Logging.LogType.Error);
                callback?.Invoke(uwr.error);
            }
        }
    }
}
