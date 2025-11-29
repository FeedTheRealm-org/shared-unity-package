using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace API {
    [CreateAssetMenu(fileName = "WorldService", menuName = "Scriptable Objects/API/WorldService")]
    public class WorldService : ScriptableObject {
        [Header("Server settings")]
        [SerializeField] public string Hostname;
        [SerializeField] public int Port;

        [Header("General settings")]
        [SerializeField] private Logging.Logger logger;

        private string GetBaseUrl() => $"http://{Hostname}:{Port}/world";

        /// <summary>
        ///  Post a new world to the server.
        /// </summary>
        public IEnumerator CreateWorld(Models.WorldData worldData, string accessToken, System.Action<string, string> handler) {
            // Read file content

            logger.Log($"Uploading world data with these objects: {worldData.objectPlacementData}", this);

            // Wrap into final payload with file_name
            WorldRequest payload = new(worldData);

            string url = GetBaseUrl();
            string json = JsonUtility.ToJson(payload);

            var uwr = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            logger.Log($"Sending Request: {json}", this);

            yield return uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) {
                var res = string.IsNullOrEmpty(responseText) ? null : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log($"CreateWorld error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}", this, Logging.LogType.Error);
                handler?.Invoke("", res?.detail ?? responseText);
            } else {
                logger.Log($"CreateWorld response: {responseText}", this);
                var res = JsonUtility.FromJson<DataEnvelope<WorldCreateResponse>>(responseText);
                handler?.Invoke(res?.data?.id, "");
            }
        }

        /// <summary>
        /// Get a page of worlds from the server.
        /// </summary>
        public IEnumerator GetWorldPage(int offset, int limit, string filter, string accessToken, System.Action<int, List<Models.WorldData>, string> handler) {
            var url = $"{GetBaseUrl()}?offset={offset}&limit={limit}";
            if (!string.IsNullOrWhiteSpace(filter)) {
                var trimmed = filter.Trim();
                url = $"{url}&filter={UnityWebRequest.EscapeURL(trimmed)}";
            }
            logger.Log($"Fetching worlds from URL: {url}", this);

            var uwr = UnityWebRequest.Get(url);
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            logger.Log($"Using API Token: {accessToken}", this);

            yield return uwr.SendWebRequest();

            var responseText = uwr.downloadHandler?.text ?? uwr.error ?? string.Empty;
            logger.Log($"Worlds response text: {responseText}", this);

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) {
                var res = string.IsNullOrEmpty(responseText) ? null : JsonUtility.FromJson<ErrorResponse>(responseText);
                logger.Log($"GetWorldPage error: {(res != null ? $"{res.title}: {res.detail}" : responseText)}", this, Logging.LogType.Error);
                handler?.Invoke(0, null, res?.detail ?? responseText);
            } else {
                var envelope = string.IsNullOrEmpty(responseText) ? null : JsonUtility.FromJson<DataEnvelope<WorldListResponse>>(responseText);
                var worldListResponse = envelope?.data;

                if (worldListResponse == null) {
                    handler?.Invoke(0, null, "Failed to parse world list response");
                    yield break;
                }

                var worldDataList = new List<Models.WorldData>();

                foreach (var worldItem in worldListResponse.worlds) {
                    try {
                        var worldData = JsonUtility.FromJson<Models.WorldData>(worldItem.data);
                        worldData.id = worldItem.id;
                        worldDataList.Add(worldData);
                    } catch (System.Exception ex) {
                        logger.Log($"Failed to parse world data for {worldItem.id}: {ex.Message}", this, Logging.LogType.Error);
                    }
                }

                logger.Log($"GetWorldPage response: Loaded {worldDataList.Count} worlds", this);
                handler?.Invoke(worldListResponse.amount, worldDataList, "");
            }
        }

    }


}
