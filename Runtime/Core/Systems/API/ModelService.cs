using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

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
        ///  Lists all asset models for a given world.
        /// </summary>
        /// <param name="worldId"></param>
        /// <param name="accessToken"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task<List<string>> ListWorldAssets(
            string worldId,
            string accessToken
        ) {
            string url = $"{GetBaseUrl().TrimEnd('/')}/{worldId}";

            UnityWebRequest uwr = UnityWebRequest.Get(url);
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success) {
                logger.Log($"ListWorldAssets error: {uwr.error}", this, Logging.LogType.Error);
                throw new System.Exception(uwr.error);
            }

            var response = JsonUtility.FromJson<AssetListResponse>(uwr.downloadHandler.text);

            List<string> modelIds = new();
            foreach (var item in response.data.models) {
                modelIds.Add(item.model_id);
            }

            return modelIds;
        }

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

                byte[] modelData = File.ReadAllBytes(Path.Combine(Application.dataPath, "Resources", asset.ModelPath));
                form.AddBinaryData(
                    $"{prefix}.model_file",
                    modelData,
                    Path.GetFileName(asset.ModelPath),
                    "application/octet-stream"
                );

                if (!string.IsNullOrEmpty(asset.MaterialPath)) {
                    form.AddField($"{prefix}.material_file", asset.MaterialPath);

                    byte[] materialData = File.ReadAllBytes(Path.Combine(Application.dataPath, "Resources", asset.MaterialPath));
                    form.AddBinaryData(
                        $"{prefix}.material_file",
                        materialData,
                        Path.GetFileName(asset.MaterialPath),
                        "application/octet-stream"
                    );
                }
            }

            var url = $"{GetBaseUrl().TrimEnd('/')}/{worldId}";
            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
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
