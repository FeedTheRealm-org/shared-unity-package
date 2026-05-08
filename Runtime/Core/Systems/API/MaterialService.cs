using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    [CreateAssetMenu(
        fileName = "MaterialService",
        menuName = "Scriptable Objects/API/MaterialService"
    )]
    public class MaterialService : BaseApiService
    {
        [Header("API Config")]
        [SerializeField]
        private ApiConfig apiConfig;

        [Header("Session settings")]
        [SerializeField]
        private Session.Session session;

        private string GetMaterialsUrl() =>
            $"{apiConfig.Hostname}:{apiConfig.Port}/assets/materials";

        public async Task<MaterialResponse[]> GetMaterialsListAsync(
            string worldId = null,
            int offset = 0,
            int limit = 24
        )
        {
            var safeOffset = Mathf.Max(0, offset);
            var safeLimit = Mathf.Max(1, limit);
            var url = $"{GetMaterialsUrl()}?offset={safeOffset}&limit={safeLimit}";

            if (!string.IsNullOrWhiteSpace(worldId))
                url += $"&world_id={worldId}";

            var uwr = new UnityWebRequest(url, "GET");
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Authorization", $"Bearer {session.APIToken}");

            await uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                if (uwr.responseCode == 404)
                    return Array.Empty<MaterialResponse>();

                logger.Log(
                    $"GetMaterialsListAsync error ({uwr.responseCode}): {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return null;
            }

            var wrapped = JsonUtility.FromJson<MaterialResponseList>(
                $"{{\"data\":{responseText}}}"
            );
            return wrapped?.data ?? Array.Empty<MaterialResponse>();
        }

        public async Task<MaterialResponse> GetMaterialByIdAsync(string materialId)
        {
            if (string.IsNullOrWhiteSpace(materialId))
            {
                logger.Log(
                    "GetMaterialByIdAsync called with empty materialId.",
                    this,
                    Logging.LogType.Warning
                );
                return null;
            }

            var url = $"{GetMaterialsUrl()}/{materialId}";
            var uwr = new UnityWebRequest(url, "GET");
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Authorization", $"Bearer {session.APIToken}");

            await uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                var err = string.IsNullOrEmpty(responseText)
                    ? null
                    : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log(
                    $"GetMaterialByIdAsync error: {(err != null ? $"{err.title}: {err.detail}" : responseText)}",
                    this,
                    Logging.LogType.Error
                );
                return null;
            }

            var envelope = JsonUtility.FromJson<DataEnvelope<MaterialResponse>>(responseText);
            return envelope?.data;
        }

        public async Task<MaterialResponse[]> UploadMaterialsAsync(
            string worldId,
            string[] ids,
            string[] names,
            string[] filePaths
        )
        {
            if (string.IsNullOrWhiteSpace(worldId))
            {
                logger.Log(
                    "UploadMaterialsAsync called without worldId.",
                    this,
                    Logging.LogType.Warning
                );
                return null;
            }

            if (
                ids == null
                || names == null
                || filePaths == null
                || ids.Length != filePaths.Length
                || ids.Length != names.Length
            )
            {
                logger.Log(
                    "UploadMaterialsAsync: ids, names and filePaths must be non-null and the same length.",
                    this,
                    Logging.LogType.Warning
                );
                return null;
            }

            var url = $"{GetMaterialsUrl()}/world/{worldId}";
            var formData = new List<IMultipartFormSection>();

            for (int i = 0; i < ids.Length; i++)
            {
                formData.Add(new MultipartFormDataSection($"ids[{i}]", ids[i]));
                formData.Add(new MultipartFormDataSection($"names[{i}]", names[i]));

                byte[] fileData = System.IO.File.ReadAllBytes(filePaths[i]);
                string fileName = System.IO.Path.GetFileName(filePaths[i]);
                formData.Add(
                    new MultipartFormFileSection($"materials[{i}]", fileData, fileName, "image/png")
                );
            }

            var uwr = UnityWebRequest.Post(url, formData);
            uwr.method = "PUT";
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Authorization", $"Bearer {session.APIToken}");

            await uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;
            logger.Log(
                $"[MaterialService] UploadMaterialsAsync response ({uwr.responseCode}): {responseText}",
                this
            );

            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
            {
                logger.Log(
                    $"UploadMaterialsAsync error (status {uwr.responseCode}): {responseText}",
                    this,
                    Logging.LogType.Error
                );
                return null;
            }

            var wrapped = JsonUtility.FromJson<MaterialResponseList>(responseText);
            return wrapped?.data;
        }
    }
}
