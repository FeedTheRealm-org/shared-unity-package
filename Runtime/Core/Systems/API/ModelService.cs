using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FTRShared.Runtime.Models;
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

        private string GetBaseUrl() => $"{apiConfig.Hostname}:{apiConfig.Port}";

        /// <summary>
        /// Lists all asset models for a given world.
        /// Returns dictionary of model info with model_id as key.
        /// </summary>
        public async Task<Dictionary<string, ModelInfo>> ListWorldModels(
            string worldId,
            string accessToken
        )
        {
            string url = $"{GetBaseUrl()}/assets/models/world/{worldId}";
            var (responseText, result, statusCode) = await SendRequestAsync(
                url,
                "GET",
                accessToken,
                null,
                "ListWorldModels"
            );

            if (result == UnityWebRequest.Result.ConnectionError)
            {
                logger.Log(
                    $"ListWorldModels connection error: {responseText}",
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
                    $"ListWorldModels error ({statusCode}): {errorMessage}",
                    this,
                    Logging.LogType.Error
                );
                throw new System.Exception(errorMessage);
            }

            var response = JsonUtility.FromJson<WorldModelsResponse>(responseText);

            Dictionary<string, ModelInfo> models = new();
            foreach (var model in response.data.models)
                models[model.model_id] = model;
            return models;
        }

        /// <summary>
        /// Uploads asset model & material files for a world.
        /// </summary>
        public async Task<string> UploadModels(
            List<ModelRequest> structureModels,
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
                byte[] modelData = File.ReadAllBytes(structure.filePath);

                form.AddBinaryData(
                    $"{prefix}.model_file",
                    modelData,
                    Path.GetFileName(structure.filePath),
                    "application/octet-stream"
                );
            }

            var url = $"{GetBaseUrl()}/assets/models/world/{worldId}";
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
