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

        [SerializeField]
        private Session.Session session;

        private const string DefaultModelsWorldId = "00000000-0000-0000-0000-000000000000";

        private string GetBaseUrl() => $"{apiConfig.Hostname}:{apiConfig.Port}";

        public async Task<Dictionary<string, ModelInfo>> ListWorldModels(string worldId)
        {
            await session.EnsureValidSession();

            string url = $"{GetBaseUrl()}/assets/models/world/{worldId}";
            var (responseText, result, statusCode) = await SendRequestAsync(
                url,
                "GET",
                session.AccessToken,
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
                string errorMessage =
                    statusCode == 401 ? "Session expired. Please log in again."
                    : statusCode >= 500 ? "Server error. Please try again later."
                    : responseText;
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

        public async Task<Dictionary<string, ModelInfo>> ListDefaultModels()
        {
            return await ListWorldModels(DefaultModelsWorldId);
        }

        public async Task<string> UploadModels(List<ModelRequest> structureModels, string worldId)
        {
            await session.EnsureValidSession();

            if (structureModels == null || structureModels.Count == 0)
            {
                logger.Log("No assets to upload.", this, Logging.LogType.Warning);
                return "No assets to upload.";
            }

            foreach (var structure in structureModels)
            {
                string error = await UploadModel(structure, worldId);
                if (!string.IsNullOrEmpty(error))
                    return error;
            }

            return string.Empty;
        }

        private async Task<string> UploadModel(ModelRequest structure, string worldId)
        {
            var url = $"{GetBaseUrl()}/assets/models/world/{worldId}";

            byte[] modelData = File.ReadAllBytes(structure.filePath);
            float sizeInMB = modelData.Length / 1024f / 1024f;

            logger.Log(
                $"[ModelService] Uploading model: id={structure.id} | file={Path.GetFileName(structure.filePath)} | size={sizeInMB:F2} MB",
                this
            );

            if (sizeInMB > 10f)
                logger.Log(
                    $"[ModelService] WARNING: Model {structure.id} is large ({sizeInMB:F2} MB) and may be rejected by the server.",
                    this,
                    Logging.LogType.Warning
                );

            var form = new WWWForm();
            form.AddField("model_id", structure.id);
            form.AddBinaryData(
                "model_file",
                modelData,
                Path.GetFileName(structure.filePath),
                "application/octet-stream"
            );

            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            uwr.method = "PUT";

            await session.EnsureValidSession();
            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");
            await uwr.SendWebRequest();

            logger.Log(
                $"[ModelService] Response code: {uwr.responseCode} | id={structure.id}",
                this
            );
            logger.Log($"[ModelService] Response body: {uwr.downloadHandler?.text}", this);

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                logger.Log(
                    $"[ModelService] Model {structure.id} uploaded successfully ({sizeInMB:F2} MB).",
                    this
                );
                return string.Empty;
            }

            logger.Log(
                $"[ModelService] Upload error for {structure.id} ({sizeInMB:F2} MB): {uwr.error} | Response: {uwr.downloadHandler?.text}",
                this,
                Logging.LogType.Error
            );
            return uwr.error;
        }

        public async Task<string> DownloadModel(ModelInfo modelInfo)
        {
            await session.EnsureValidSession();

            string fileName = Path.GetFileName(modelInfo.url);
            string downloadUrl = $"{apiConfig.WorldsCDN}{modelInfo.url}";

            string tempPath = Path.Combine(Application.temporaryCachePath, fileName);

            logger.Log($"[ModelService] Downloading model: {fileName} from {downloadUrl}", this);

            UnityWebRequest uwr = UnityWebRequest.Get(downloadUrl);
            uwr.SetRequestHeader("Authorization", $"Bearer {session.AccessToken}");
            uwr.downloadHandler = new DownloadHandlerFile(tempPath);
            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                logger.Log(
                    $"[ModelService] Failed to download model {fileName}: {uwr.error}",
                    this,
                    Logging.LogType.Error
                );
                return null;
            }

            logger.Log($"[ModelService] Downloaded model: {fileName} to {tempPath}", this);
            return tempPath;
        }
    }
}
