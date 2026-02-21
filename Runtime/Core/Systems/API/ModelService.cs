using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Models;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(fileName = "ModelService", menuName = "Scriptable Objects/API/ModelService")]
    public class ModelService : BaseApiService
    {
        [Header("API Config")]
        [SerializeField]
        private ApiConfig apiConfig;

        private string GetBaseUrl() =>
            $"http://{apiConfig.Hostname}:{apiConfig.Port}/assets/models/world";

        /// <summary>
        ///  Lists all asset models for a given world.
        /// </summary>
        public async Task<List<string>> ListWorldAssets(string worldId, string accessToken)
        {
            string url = $"{GetBaseUrl()}/{worldId}";
            var (responseText, result, statusCode) = await SendRequestAsync(
                url,
                "GET",
                accessToken,
                null,
                "ListWorldAssets"
            );

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"ListWorldAssets connection error: {responseText}",
                    this,
                    Logging.LogType.Error
                );
                throw new System.Exception(
                    "Unable to connect to server. Please check your internet connection."
                );
            }
            else if (result == UnityWebRequest.Result.ProtocolError)
            {
                string errorMessage = responseText;
                if (statusCode == 401)
                {
                    errorMessage = "Session expired. Please log in again.";
                }
                else if (statusCode >= 500)
                {
                    errorMessage = "Server error. Please try again later.";
                }
                logger.Log(
                    $"ListWorldAssets error ({statusCode}): {errorMessage}",
                    this,
                    Logging.LogType.Error
                );
                throw new System.Exception(errorMessage);
            }

            var response = JsonUtility.FromJson<AssetListResponse>(responseText);

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
        public async Task<string> UploadModels(
            List<StructureData> structureModels,
            string worldId,
            string accessToken
        )
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

            var url = $"{GetBaseUrl()}/{worldId}";
            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            uwr.method = "PUT";
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
