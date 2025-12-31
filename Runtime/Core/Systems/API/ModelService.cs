using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Models;

namespace API
{
    [CreateAssetMenu(fileName = "ModelService", menuName = "Scriptable Objects/API/ModelService")]
    public class ModelService : ScriptableObject
    {
        [Header("Server settings")]
        [SerializeField]
        public string Hostname;

        [SerializeField]
        public int Port;

        [Header("General settings")]
        [SerializeField]
        private Logging.Logger logger;

        private string GetBaseUrl() => $"http://{Hostname}:{Port}/assets/models";

        /// <summary>
        ///  Lists all asset models for a given world.
        /// </summary>
        public async Task<List<string>> ListWorldAssets(
            string worldId,
            string accessToken
        )
        {
            string url = $"{GetBaseUrl().TrimEnd('/')}/{worldId}";

            UnityWebRequest uwr = UnityWebRequest.Get(url);
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                logger.Log($"ListWorldAssets error: {uwr.error}", this, Logging.LogType.Error);
                throw new System.Exception(uwr.error);
            }

            var response = JsonUtility.FromJson<AssetListResponse>(uwr.downloadHandler.text);

            List<string> modelIds = new();
            foreach (var item in response.data.models)
            {
                modelIds.Add(item.model_id);
            }

            return modelIds;
        }

        /// <summary>
        /// Uploads asset model & material files for a world.
        /// </summary>
        public async Task<string> UploadModels(List<StructureData> structureModels, string worldId, string accessToken)
        {
            if (structureModels == null || structureModels.Count == 0)
            {
                logger.Log("No assets to upload.", this, Logging.LogType.Warning);
                return "No assets to upload.";
            }
            logger.Log($"Uploading {structureModels.Count} assets for world ID: {worldId}", this);

            var form = new WWWForm();

            for (int i = 0; i < structureModels.Count; i++)
            {
                var structure = structureModels[i];
                string prefix = $"models[{i}]";
                form.AddField($"{prefix}.model_id", structure.id);
                form.AddField($"{prefix}.name", structure.structureName);
                byte[] modelData = File.ReadAllBytes(structure.structureFilepath);

                form.AddBinaryData(
                    $"{prefix}.model_file",
                    modelData,
                    Path.GetFileName(structure.structureFilepath),
                    "application/octet-stream"
                );
            }

            var url = $"{GetBaseUrl().TrimEnd('/')}/{worldId}";
            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            await uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                logger.Log("Assets uploaded successfully", this);
                return string.Empty;
            }
            else
            {
                logger.Log($"Asset upload error: {uwr.error}", this, Logging.LogType.Error);
                return uwr.error;
            }
        }
    }
}
